const resultPanel = document.querySelector("#result-panel");
const resultTitle = document.querySelector("#result-title");
const resultBody = document.querySelector("#result-body");
const lookupId = document.querySelector("#lookup-id");
const catalogDefinitions = {
    clientes: { endpoint: "/api/catalogos/clientes", emptyLabel: "Primero agrega un cliente" },
    barberos: { endpoint: "/api/catalogos/barberos", emptyLabel: "Primero agrega un barbero" },
    servicios: { endpoint: "/api/catalogos/servicios", emptyLabel: "Primero agrega un servicio" }
};

function showResult(title, body, isError = false) {
    resultTitle.textContent = title;
    resultBody.textContent = typeof body === "string"
        ? body
        : JSON.stringify(body, null, 2);
    resultPanel.dataset.status = isError ? "error" : "success";
    resultPanel.hidden = false;
    resultPanel.scrollIntoView({ behavior: "smooth", block: "nearest" });
}

async function sendRequest(path, options = {}) {
    const response = await fetch(path, {
        ...options,
        headers: {
            Accept: "application/json",
            ...(options.body ? { "Content-Type": "application/json" } : {}),
            ...options.headers
        }
    });

    const text = await response.text();
    let body = text;

    if (text) {
        try {
            body = JSON.parse(text);
        } catch {
            body = text;
        }
    }

    if (!response.ok) {
        const message = body && typeof body === "object"
            ? body.mensaje ?? body.title ?? JSON.stringify(body)
            : body || `La solicitud falló (${response.status}).`;
        throw new Error(message);
    }

    return body;
}

async function runAction(button, title, action) {
    button.disabled = true;

    try {
        const body = await action();
        showResult(title, body);
        return body;
    } catch (error) {
        showResult("No se pudo completar la operación", error.message, true);
        return null;
    } finally {
        button.disabled = false;
    }
}

function catalogLabel(type, item) {
    if (type === "clientes") {
        return item.telefono ? `${item.nombre} · ${item.telefono}` : item.nombre;
    }

    if (type === "servicios") {
        return `${item.nombre} · ${item.duracionMinutos} min · $${Number(item.precio).toFixed(2)}`;
    }

    return item.nombre;
}

async function loadCatalogs(preferred = {}) {
    const catalogs = await Promise.all(
        Object.entries(catalogDefinitions).map(async ([type, definition]) => [
            type,
            await sendRequest(definition.endpoint)
        ])
    );

    for (const [type, items] of catalogs) {
        document.querySelectorAll(`select[data-catalog="${type}"]`).forEach((select) => {
            const previousValue = select.value;
            const placeholder = new Option(
                items.length ? "Selecciona una opción" : catalogDefinitions[type].emptyLabel,
                ""
            );
            select.replaceChildren(placeholder);

            for (const item of items) {
                select.add(new Option(catalogLabel(type, item), item.id));
            }

            select.value = String(preferred[type] ?? previousValue);
            select.disabled = items.length === 0;
        });
    }

    return Object.fromEntries(catalogs.map(([type, items]) => [type, items.length]));
}

document.querySelector("#create-form").addEventListener("submit", async (event) => {
    event.preventDefault();
    const form = event.currentTarget;
    const button = form.querySelector('button[type="submit"]');
    const values = Object.fromEntries(new FormData(form));
    const body = {
        clienteId: Number(values.clienteId),
        barberoId: Number(values.barberoId),
        servicioId: Number(values.servicioId),
        fechaHoraInicio: values.fechaHoraInicio
    };

    const result = await runAction(button, "Cita creada", () =>
        sendRequest("/api/citas", {
            method: "POST",
            body: JSON.stringify(body)
        }));

    if (result?.id) {
        lookupId.value = result.id;
    }
});

document.querySelectorAll("[data-create-catalog]").forEach((form) => {
    form.addEventListener("submit", async (event) => {
        event.preventDefault();
        const currentForm = event.currentTarget;
        const button = currentForm.querySelector('button[type="submit"]');
        const type = currentForm.dataset.createCatalog;
        const values = Object.fromEntries(new FormData(currentForm));
        let body;

        if (type === "clientes" || type === "barberos") {
            body = {
                nombre: values.nombre,
                telefono: values.telefono || null,
                correo: values.correo || null
            };
        } else {
            body = {
                nombre: values.nombre,
                descripcion: values.descripcion || null,
                precio: Number(values.precio),
                duracionMinutos: Number(values.duracionMinutos)
            };
        }

        const result = await runAction(button, currentForm.dataset.success, () =>
            sendRequest(catalogDefinitions[type].endpoint, {
                method: "POST",
                body: JSON.stringify(body)
            }));

        if (!result) {
            return;
        }

        currentForm.reset();
        try {
            await loadCatalogs({ [type]: result.id });
        } catch (error) {
            showResult("Registro guardado, pero no se pudieron actualizar las listas", error.message, true);
        }
    });
});

document.querySelector("#lookup-form").addEventListener("submit", async (event) => {
    event.preventDefault();
    const button = event.currentTarget.querySelector('button[type="submit"]');
    const id = Number(lookupId.value);

    await runAction(button, "Detalle de la cita", () =>
        sendRequest(`/api/citas/${id}`));
});

document.querySelector("#reschedule-form").addEventListener("submit", async (event) => {
    event.preventDefault();
    const form = event.currentTarget;
    const button = form.querySelector('button[type="submit"]');
    const id = Number(lookupId.value);

    if (!id) {
        showResult("Falta el ID de la cita", "Consulta primero una cita o ingresa su ID.", true);
        return;
    }

    const values = Object.fromEntries(new FormData(form));
    const body = {
        barberoId: Number(values.barberoId),
        servicioId: Number(values.servicioId),
        fechaHoraInicio: values.fechaHoraInicio
    };

    await runAction(button, "Cita reprogramada", () =>
        sendRequest(`/api/citas/${id}/reprogramar`, {
            method: "PUT",
            body: JSON.stringify(body)
        }));
});

document.querySelector("#cancel-button").addEventListener("click", async (event) => {
    const button = event.currentTarget;
    const id = Number(lookupId.value);

    if (!id) {
        showResult("Falta el ID de la cita", "Consulta primero una cita o ingresa su ID.", true);
        return;
    }

    if (!window.confirm(`¿Quieres cancelar la cita #${id}?`)) {
        return;
    }

    await runAction(button, "Cita cancelada", () =>
        sendRequest(`/api/citas/${id}/cancelar`, { method: "PATCH" }));
});

document.querySelector("#clear-result").addEventListener("click", () => {
    resultPanel.hidden = true;
});

loadCatalogs().catch((error) => {
    showResult("No se pudieron cargar los catálogos", error.message, true);
});
