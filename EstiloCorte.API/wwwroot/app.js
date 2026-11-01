const resultPanel = document.querySelector("#result-panel");
const resultTitle = document.querySelector("#result-title");
const resultBody = document.querySelector("#result-body");
const suggestionsPanel = document.querySelector("#suggestions-panel");
const suggestionsList = document.querySelector("#suggestions-list");
const lookupId = document.querySelector("#lookup-id");
const calendarGrid = document.querySelector("#calendar-grid");
const calendarRange = document.querySelector("#calendar-range");
let calendarWeekStart = getMonday(new Date());
const catalogDefinitions = {
    clientes: { endpoint: "/api/catalogos/clientes", emptyLabel: "Primero agrega un cliente" },
    barberos: { endpoint: "/api/catalogos/barberos", emptyLabel: "Primero agrega un barbero" },
    servicios: { endpoint: "/api/catalogos/servicios", emptyLabel: "Primero agrega un servicio" }
};

function showResult(title, body, isError = false, suggestions = [], suggestionFormId = null) {
    resultTitle.textContent = title;
    resultBody.textContent = typeof body === "string"
        ? body
        : JSON.stringify(body, null, 2);
    resultPanel.dataset.status = isError ? "error" : "success";
    suggestionsList.replaceChildren();
    suggestionsPanel.hidden = suggestions.length === 0;

    if (suggestions.length && suggestionFormId) {
        for (const suggestion of suggestions) {
            const button = document.createElement("button");
            const date = new Date(suggestion);
            button.className = "suggestion-option";
            button.type = "button";
            button.textContent = new Intl.DateTimeFormat("es", {
                weekday: "short",
                day: "numeric",
                month: "short",
                hour: "2-digit",
                minute: "2-digit"
            }).format(date);
            button.addEventListener("click", () => {
                const form = document.getElementById(suggestionFormId);
                form.elements.fechaHoraInicio.value = toDateTimeInputValue(date);
                form.elements.fechaHoraInicio.focus();
                form.scrollIntoView({ behavior: "smooth", block: "center" });
            });
            suggestionsList.append(button);
        }
    }

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
        const error = new Error(message);
        error.horariosSugeridos = body?.horariosSugeridos ?? [];
        throw error;
    }

    return body;
}

async function runAction(button, title, action, suggestionFormId = null) {
    button.disabled = true;

    try {
        const body = await action();
        showResult(title, body);
        return body;
    } catch (error) {
        showResult(
            "No se pudo completar la operación",
            error.message,
            true,
            error.horariosSugeridos ?? [],
            suggestionFormId
        );
        return null;
    } finally {
        button.disabled = false;
    }
}

function getMonday(date) {
    const monday = new Date(date.getFullYear(), date.getMonth(), date.getDate());
    const daysSinceMonday = (monday.getDay() + 6) % 7;
    monday.setDate(monday.getDate() - daysSinceMonday);
    return monday;
}

function toDateTimeInputValue(date) {
    const localDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
    return localDate.toISOString().slice(0, 16);
}

function toLocalDateTimeString(date) {
    return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, "0")}-${String(date.getDate()).padStart(2, "0")}T${String(date.getHours()).padStart(2, "0")}:${String(date.getMinutes()).padStart(2, "0")}:${String(date.getSeconds()).padStart(2, "0")}`;
}

function renderCalendar(citas) {
    const days = Array.from({ length: 7 }, (_, index) => {
        const date = new Date(calendarWeekStart);
        date.setDate(date.getDate() + index);
        return date;
    });

    calendarGrid.replaceChildren();
    calendarRange.textContent = `${new Intl.DateTimeFormat("es", {
        day: "numeric",
        month: "long"
    }).format(days[0])} – ${new Intl.DateTimeFormat("es", {
        day: "numeric",
        month: "long",
        year: "numeric"
    }).format(days[6])}`;

    for (const day of days) {
        const dayCitas = citas.filter((cita) => {
            const start = new Date(cita.fechaHoraInicio);
            return start.getFullYear() === day.getFullYear()
                && start.getMonth() === day.getMonth()
                && start.getDate() === day.getDate();
        });
        const column = document.createElement("section");
        const heading = document.createElement("div");
        const weekday = document.createElement("span");
        const dayNumber = document.createElement("strong");

        column.className = "calendar-day";
        heading.className = "calendar-day-heading";
        weekday.textContent = new Intl.DateTimeFormat("es", { weekday: "short" }).format(day);
        dayNumber.textContent = String(day.getDate());
        heading.append(weekday, dayNumber);
        column.append(heading);

        if (dayCitas.length === 0) {
            const empty = document.createElement("p");
            empty.className = "calendar-empty";
            empty.textContent = "Sin citas";
            column.append(empty);
        } else {
            for (const cita of dayCitas) {
                const item = document.createElement("button");
                const time = document.createElement("strong");
                const name = document.createElement("span");
                const service = document.createElement("span");
                const barber = document.createElement("span");
                item.type = "button";
                item.className = "calendar-appointment";
                if (cita.estado.toLowerCase() === "cancelada") {
                    item.classList.add("is-cancelled");
                }

                time.textContent = new Intl.DateTimeFormat("es", {
                    hour: "2-digit",
                    minute: "2-digit"
                }).format(new Date(cita.fechaHoraInicio));
                name.textContent = cita.clienteNombre ?? `Cita #${cita.id}`;
                service.textContent = cita.estado.toLowerCase() === "cancelada"
                    ? "Cancelada"
                    : cita.servicioNombre ?? "Servicio";
                barber.textContent = cita.barberoNombre ?? "Barbero";
                item.append(time, name, service, barber);
                item.addEventListener("click", () => showAppointmentDetails(cita.id));
                column.append(item);
            }
        }

        calendarGrid.append(column);
    }
}

async function loadCalendar() {
    const start = new Date(calendarWeekStart);
    const end = new Date(start);
    end.setDate(end.getDate() + 7);
    calendarGrid.setAttribute("aria-busy", "true");

    try {
        const query = new URLSearchParams({
            desde: toLocalDateTimeString(start),
            hasta: toLocalDateTimeString(end)
        });
        const citas = await sendRequest(`/api/citas?${query}`);
        renderCalendar(citas);
    } catch (error) {
        calendarGrid.replaceChildren();
        const message = document.createElement("p");
        message.className = "calendar-empty calendar-error";
        message.textContent = `No se pudo cargar el calendario: ${error.message}`;
        calendarGrid.append(message);
    } finally {
        calendarGrid.setAttribute("aria-busy", "false");
    }
}

async function showAppointmentDetails(id) {
    try {
        const cita = await sendRequest(`/api/citas/${id}`);
        lookupId.value = cita.id;
        document.querySelector('#reschedule-form [name="barberoId"]').value = cita.barberoId;
        document.querySelector('#reschedule-form [name="servicioId"]').value = cita.servicioId;
        document.querySelector('#reschedule-form [name="fechaHoraInicio"]').value =
            toDateTimeInputValue(new Date(cita.fechaHoraInicio));
        showResult(`Cita #${cita.id}`, {
            Cliente: cita.clienteNombre,
            Barbero: cita.barberoNombre,
            Servicio: cita.servicioNombre,
            Inicio: new Intl.DateTimeFormat("es", {
                dateStyle: "medium",
                timeStyle: "short"
            }).format(new Date(cita.fechaHoraInicio)),
            Fin: new Intl.DateTimeFormat("es", {
                timeStyle: "short"
            }).format(new Date(cita.fechaHoraFin)),
            Estado: cita.estado
        });
    } catch (error) {
        showResult("No se pudo cargar la cita", error.message, true);
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
        }), "create-form");

    if (result?.id) {
        lookupId.value = result.id;
        await loadCalendar();
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

    const result = await runAction(button, "Cita reprogramada", () =>
        sendRequest(`/api/citas/${id}/reprogramar`, {
            method: "PUT",
            body: JSON.stringify(body)
        }), "reschedule-form");

    if (result) {
        await loadCalendar();
    }
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

    const result = await runAction(button, "Cita cancelada", () =>
        sendRequest(`/api/citas/${id}/cancelar`, { method: "PATCH" }));

    if (result) {
        await loadCalendar();
    }
});

document.querySelector("#clear-result").addEventListener("click", () => {
    resultPanel.hidden = true;
});

loadCatalogs().catch((error) => {
    showResult("No se pudieron cargar los catálogos", error.message, true);
});

document.querySelector("#calendar-prev").addEventListener("click", () => {
    calendarWeekStart.setDate(calendarWeekStart.getDate() - 7);
    loadCalendar();
});

document.querySelector("#calendar-next").addEventListener("click", () => {
    calendarWeekStart.setDate(calendarWeekStart.getDate() + 7);
    loadCalendar();
});

document.querySelector("#calendar-today").addEventListener("click", () => {
    calendarWeekStart = getMonday(new Date());
    loadCalendar();
});

loadCalendar();
