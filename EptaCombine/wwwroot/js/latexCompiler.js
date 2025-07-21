// Get elements
const zipFileInput = document.getElementById('zipFile');
const latexEditor = document.getElementById('latexEditor');
const saveBtn = document.getElementById('saveBtn');
const compileBtn = document.getElementById('compileBtn');
const pdfViewer = document.getElementById('pdfViewer');
const toastBox = document.getElementById('toastBox');

// Get URLs from data attributes
const container = document.getElementById('latexCompilerContainer');
const urls = {
    upload: container.dataset.uploadUrl,
    getMainTex: container.dataset.getMainTexUrl,
    saveMainTex: container.dataset.saveMainTexUrl,
    compile: container.dataset.compileUrl,
    cleanup: container.dataset.cleanupUrl
};

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

const bootstrapToast = new bootstrap.Toast(toastBox);

function showToast(message, isSuccess = true) {
    const toastBody = toastBox.querySelector('.toast-body');
    toastBody.textContent = message;
    toastBox.className = 'toast align-items-center text-white border-0';
    toastBox.classList.add(isSuccess ? 'bg-success' : 'bg-danger');
    bootstrapToast.show();
}

zipFileInput.addEventListener('change', async function() {
    const file = this.files[0];
    if (!file) return;

    const formData = new FormData();
    formData.append("zipFile", file);

    try {
        const res = await fetch(urls.upload, {
            method: "POST",
            headers: {
                "RequestVerificationToken": token
            },
            body: formData
        });

        if (!res.ok) {
            const error = await res.text();
            console.error("Error from backend:", error);
            showToast(`Upload error: ${error}`, false);
            return;
        }

        const result = await res.json();
        if (result.success) {
            showToast('ZIP file uploaded successfully');
            await loadMainTexContent();
        }
    } catch (err) {
        console.error("JS exception:", err);
        showToast(`Upload error: ${err.message}`, false);
    }
});

// Load main.tex content
async function loadMainTexContent() {
    try {
        const res = await fetch(urls.getMainTex, {
            method: "POST",
            headers: {
                "RequestVerificationToken": token,
                "Content-Type": "application/json"
            }
        });

        if (!res.ok) {
            throw new Error(await res.text());
        }

        const result = await res.json();
        latexEditor.value = result.content;
    } catch (err) {
        console.error("Error loading main.tex:", err);
        showToast(`Error loading content: ${err.message}`, false);
    }
}

// Save content
saveBtn.addEventListener('click', async function() {
    try {
        const res = await fetch(urls.saveMainTex, {
            method: "POST",
            headers: {
                "RequestVerificationToken": token,
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ content: latexEditor.value })
        });

        if (!res.ok) {
            throw new Error(await res.text());
        }

        const result = await res.json();
        if (result.success) {
            showToast('File saved successfully');
        }
    } catch (err) {
        console.error("Save error:", err);
        showToast(`Save error: ${err.message}`, false);
    }
});

// Compile to PDF
compileBtn.addEventListener('click', async function() {
    try {
        // First save the content
        await saveBtn.click();

        // Then compile
        const res = await fetch(urls.compile, {
            method: "POST",
            headers: {
                "RequestVerificationToken": token
            }
        });

        if (!res.ok) {
            throw new Error(await res.text());
        }

        const blob = await res.blob();
        const url = URL.createObjectURL(blob);
        pdfViewer.src = url;
        showToast('PDF compiled successfully');
    } catch (err) {
        console.error("Compile error:", err);
        showToast(`Compile error: ${err.message}`, false);
    }
});

window.addEventListener('beforeunload', function() {
    try {
        const formData = new FormData();
        formData.append('__RequestVerificationToken', token);
        navigator.sendBeacon(urls.cleanup, formData);
    } catch (err) {
        console.error("Cleanup error:", err);
    }
});