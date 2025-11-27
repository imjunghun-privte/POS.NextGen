const PosBridge = (() => {
    let nextId = 1;
    const pending = new Map();

    const PROTOCOL_VERSION = "1.0";

    function buildEnvelope(type, payload, isResponse = false, success = true, errorCode = null, errorMessage = null, correlationId = null) {
        return {
            id: String(nextId++),
            type,
            source: "spa",
            target: "shell",
            version: PROTOCOL_VERSION,
            createdAt: new Date().toISOString(),
            correlationId,
            payload: payload ?? null,
            isResponse,
            success,
            errorCode,
            errorMessage
        };
    }

    function send(type, payload) {
        const env = buildEnvelope(type, payload);
        chrome.webview.postMessage(env);
    }

    function sendAndWait(type, payload, timeoutMs = 5000) {
        const env = buildEnvelope(type, payload);

        const { id } = env;

        const promise = new Promise((resolve, reject) => {
            const timeout = setTimeout(() => {
                pending.delete(id);
                reject(new Error("Timeout waiting for response from WPF"));
            }, timeoutMs);

            pending.set(id, { resolve, reject, timeout });
        });

        chrome.webview.postMessage(env);
        return promise;
    }

    function handleIncoming(e) {
        const msg = e.data;
        if (!msg || !msg.type) {
            console.warn("[PosBridge] invalid message from WPF", msg);
            return;
        }

        console.log("[PosBridge] WPF -> JS", msg);

        if (msg.isResponse && msg.id && pending.has(msg.id)) {
            const entry = pending.get(msg.id);
            clearTimeout(entry.timeout);
            pending.delete(msg.id);

            if (msg.success) {
                entry.resolve(msg);
            } else {
                entry.reject(new Error(msg.errorMessage || msg.errorCode || "Unknown error from WPF"));
            }
        }

        // TODO: push-only 메시지 (예: connection.changed 등)은 여기서 타입별로 처리
    }

    window.chrome.webview.addEventListener("message", handleIncoming);

    return {
        send,
        sendAndWait
    };
})();

// 버튼에서 사용하는 함수
function sendToWpf() {
    PosBridge.send("ping", { from: "SPA", sentAt: new Date().toISOString() });
}
