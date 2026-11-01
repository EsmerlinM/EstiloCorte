const resultPanel = document.querySelector("#result-panel");
const resultTitle = document.querySelector("#result-title");
const resultBody = document.querySelector("#result-body");
const lookupId = document.querySelector("#lookup-id");

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
