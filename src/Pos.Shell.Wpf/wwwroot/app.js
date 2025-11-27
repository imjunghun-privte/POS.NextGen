function sendToWpf() {
    const msg = { type: "ping", time: new Date().toISOString() };
    chrome.webview.postMessage(msg);
}

window.chrome.webview.addEventListener("message", e => {
    console.log("WPF → JS 메시지:", e.data);
});
