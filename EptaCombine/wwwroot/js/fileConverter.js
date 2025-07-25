function showToast(message, isSuccess = true) {
    const toastBody = toastBox.querySelector('.toast-body');
    toastBody.textContent = message;
    toastBox.className = 'toast align-items-center text-white border-0';
    toastBox.classList.add(isSuccess ? 'bg-success' : 'bg-danger');
    window.bootstrapToast.show();
}

function setUIBusy(message) {
    const overlay = document.getElementById('overlay');
    const video = document.getElementById('overlay-video');

    overlay.classList.remove('d-none');
    document.getElementById('overlayMessage').textContent = message;

    video.currentTime = 0;
    video.play().catch(e => console.log("Video play blocked:", e));
}

function clearUIBusy() {
    const overlay = document.getElementById('overlay');
    overlay.classList.add('d-none');

    document.getElementById('overlay-video').pause();
}

window.addEventListener('load', () => {
    const toastBox = document.getElementById('toastBox');
    if (toastBox) {
        window.bootstrapToast = new bootstrap.Toast(toastBox);
    }
});

document.getElementById('uploadFile').addEventListener('change', async function () {
    const file = this.files[0];
    if (!file) return;

    const formData = new FormData();
    formData.append("uploadFile", file);

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    try {
        setUIBusy("Загрузка...");
        const res = await fetch("/FileConverter?handler=AnalyzeFile", {
            method: "POST",
            headers: {
                "RequestVerificationToken": token
            },
            body: formData
        });

        if (!res.ok) {
            clearUIBusy();
            const error = await res.text();
            console.error("Error from backend:", error);
            showToast("Ошибка загрузки", false)
            return;
        }

        const result = await res.json();
        const category = result.category;
        const formats = result.availableFormats;

        document.getElementById("categoryLabel").innerText = category;

        const select = document.getElementById("formatSelect");
        select.innerHTML = "";
        formats.forEach(f => {
            const opt = document.createElement("option");
            opt.value = f;
            opt.text = f;
            select.appendChild(opt);
        });

        clearUIBusy();
        document.getElementById("format-section").classList.remove("d-none");
        document.getElementById("result-section").classList.add("d-none");
        showToast("Файл загружен успешно")
    } catch (err) {
        clearUIBusy();
        console.error("JS exception:", err);
        showToast("Ошибка загрузки", false)
    }
});

document.getElementById('formatSelect').addEventListener('change', function() {
    document.getElementById("result-section").classList.add("d-none");
});

document.getElementById('convertBtn').addEventListener('click', async function () {
    const fileInput = document.getElementById("uploadFile");
    const file = fileInput.files[0];
    const outputFormat = document.getElementById("formatSelect").value;

    if (!file || !outputFormat) {
        return;
    }

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    setUIBusy("Конвертация...");
    document.getElementById("result-section").classList.add("d-none");

    const formData = new FormData();
    formData.append("uploadFile", file);
    formData.append("outputFormat", outputFormat);

    try {
        const response = await fetch("/FileConverter?handler=ConvertFile", {
            method: "POST",
            headers: {
                "RequestVerificationToken": token
            },
            body: formData
        });

        if (!response.ok) {
            clearUIBusy();
            const error = await response.text();
            console.error("Conversion failed:", error);
            showToast("Ошибка конвертации", false)
            return;
        }

        const blob = await response.blob();
        const disposition = response.headers.get("Content-Disposition") || "";
        let filename = "converted-file";

        const matchFilenameStar = disposition.match(/filename\*=UTF-8''([^;]+)/i);
        const matchFilename = disposition.match(/filename="?([^;"]+)"?/i);

        if (matchFilenameStar) {
            filename = decodeURIComponent(matchFilenameStar[1]);
        } else if (matchFilename) {
            filename = matchFilename[1];
        }

        const downloadUrl = URL.createObjectURL(blob);
        const downloadLink = document.getElementById("downloadLink");
        downloadLink.href = downloadUrl;
        downloadLink.download = filename;

        document.getElementById("result-section").classList.remove("d-none");
        clearUIBusy();
        showToast("Конвертация прошла успешно")

    } catch (err) {
        clearUIBusy();
        console.error("JS exception during convert:", err);
        showToast("Ошибка конвертации", false)
    }
});
