// Get elements
const zipFileInput = document.getElementById('zipFile');
const latexEditor = document.getElementById('latexEditor');
const saveBtn = document.getElementById('saveBtn');
const compileBtn = document.getElementById('compileBtn');
const deleteZipBtn = document.getElementById('deleteZipBtn');
const pdfViewer = document.getElementById('pdfViewer');
const downloadPdfBtn = document.getElementById('downloadPdfBtn')
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

function setUIBusy(message) {
    document.getElementById('overlay').classList.remove('d-none');
    document.getElementById('overlayMessage').textContent = message;
    document.getElementById('latexCompilerContainer').classList.add('disabled-overlay');
}

function clearUIBusy() {
    document.getElementById('overlay').classList.add('d-none');
    document.getElementById('latexCompilerContainer').classList.remove('disabled-overlay');
}

function setZipControlsEnabled(enabled) {
    zipFileInput.disabled = !enabled;
    document.getElementById('deleteZipBtn').disabled = enabled;
}


zipFileInput.addEventListener('change', async function() {
    setUIBusy("Загрузка файла...");
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
            showToast(`Ошибка загрузки`, false);
            return;
        }

        const result = await res.json();
        if (result.success) {
            showToast('ZIP файл успешно загружен');
            await loadMainTexContent();
        }
    } catch (err) {
        console.error("JS exception:", err);
        showToast(`Ошибка загрузки`, false);
    } finally {
        clearUIBusy();
        setZipControlsEnabled(false);
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
        setZipControlsEnabled(false);
    } catch (err) {
        console.error("Error loading main.tex:", err);
        // showToast(`Ошибка при скачивании .tex:`, false);
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
            showToast('Файл успешно сохранен');
        }
    } catch (err) {
        console.error("Save error:", err);
        showToast(`Ошибка при сохранении`, false);
    }
});

// Compile to PDF
compileBtn.addEventListener('click', async function() {
    setUIBusy("Компиляция...");
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
        showToast('PDF скомпилирован успешно');

        downloadPdfBtn.href = url;
    } catch (err) {
        console.error("Compile error:", err);
        showToast(`Ошибка при компиляции`, false);
    } finally {
        clearUIBusy();
    }
});

deleteZipBtn.addEventListener('click', async () => {
    setUIBusy("Удаление ZIP...");
    try {
        const res = await fetch(urls.cleanup, {
            method: "POST",
            headers: {
                "RequestVerificationToken": token
            }
        });

        if (!res.ok) throw new Error(await res.text());

        showToast("ZIP удален");
        setZipControlsEnabled(true);
        latexEditor.value = '';
        pdfViewer.src = '';
        zipFileInput.value = '';
    } catch (err) {
        showToast(`Ошибка удаления ZIP: ${err.message}`, false);
    } finally {
        clearUIBusy();
    }
});

window.addEventListener('load', async () => {
    await loadMainTexContent();
})