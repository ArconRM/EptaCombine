document.getElementById('uploadFile').addEventListener('change', async function () {
    const file = this.files[0];
    if (!file) return;

    const formData = new FormData();
    formData.append("uploadFile", file);

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    try {
        const res = await fetch("/FileConverter?handler=AnalyzeFile", {
            method: "POST",
            headers: {
                "RequestVerificationToken": token
            },
            body: formData
        });

        if (!res.ok) {
            const error = await res.text();
            console.error("Error from backend:", error);
            document.getElementById("upload-error").innerText = error;
            return;
        }

        const result = await res.json();
        const category = result.category;
        const formats = result.availableFormats;

        document.getElementById("upload-error").innerText = "";
        document.getElementById("categoryLabel").innerText = category;

        const select = document.getElementById("formatSelect");
        select.innerHTML = "";
        formats.forEach(f => {
            const opt = document.createElement("option");
            opt.value = f;
            opt.text = f;
            select.appendChild(opt);
        });

        document.getElementById("format-section").classList.remove("d-none");
        document.getElementById("result-section").classList.add("d-none");
    } catch (err) {
        console.error("JS exception:", err);
        document.getElementById("upload-error").innerText = "Произошла ошибка.";
    }
});

document.getElementById('convertBtn').addEventListener('click', async function () {
    const fileInput = document.getElementById("uploadFile");
    const file = fileInput.files[0];
    const outputFormat = document.getElementById("formatSelect").value;

    if (!file || !outputFormat) {
        document.getElementById("upload-error").innerText = "Выберите файл и формат.";
        return;
    }

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    document.getElementById("progress-section").classList.remove("d-none");

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
            const error = await response.text();
            console.error("Conversion failed:", error);
            document.getElementById("upload-error").innerText = error;
            document.getElementById("progress-section").classList.add("d-none");
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

        document.getElementById("progress-section").classList.add("d-none");
        document.getElementById("result-section").classList.remove("d-none");

    } catch (err) {
        console.error("JS exception during convert:", err);
        document.getElementById("upload-error").innerText = "Произошла ошибка.";
        document.getElementById("progress-section").classList.add("d-none");
    }
});
