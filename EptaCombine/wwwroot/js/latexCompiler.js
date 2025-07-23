// Get elements
let monacoEditor;

const zipFileInput = document.getElementById('zipFile');
const saveBtn = document.getElementById('saveBtn');
const compileBtn = document.getElementById('compileBtn');
const togglePdfBtn = document.getElementById("togglePdfBtn");
const deleteZipBtn = document.getElementById('deleteZipBtn');
const pdfViewer = document.getElementById('pdfViewer');
const downloadPdfBtn = document.getElementById('downloadPdfBtn')
const toastBox = document.getElementById('toastBox');
const pdfContainer = document.getElementById("pdfContainer");
const editorColumn = document.getElementById("editorColumn");


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

window.addEventListener('load', () => {
    const toastBox = document.getElementById('toastBox');
    if (toastBox) {
        window.bootstrapToast = new bootstrap.Toast(toastBox);
    }
});

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

function setZipControlsEnabled(enabled) {
    zipFileInput.disabled = !enabled;
    document.getElementById('deleteZipBtn').disabled = enabled;
}

function setPdfVisibility(show) {
    if (show) {
        pdfContainer.classList.remove("d-none");
        editorColumn.style.width = "66.6667%";
    } else {
        pdfContainer.classList.add("d-none");
        editorColumn.style.width = "100%";
    }
}

zipFileInput.addEventListener('change', async function () {
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
        monacoEditor.setValue(result.content);
        setZipControlsEnabled(false);
    } catch (err) {
        console.error("Error loading main.tex:", err);
        // showToast(`Ошибка при скачивании .tex:`, false);
    }
}

// Save content
saveBtn.addEventListener('click', async function () {
    try {
        const res = await fetch(urls.saveMainTex, {
            method: "POST",
            headers: {
                "RequestVerificationToken": token,
                "Content-Type": "application/json"
            },
            body: JSON.stringify({content: monacoEditor.getValue()})
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
compileBtn.addEventListener('click', async function () {
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
        setPdfVisibility(true);
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
        monacoEditor.setValue('');
        pdfViewer.src = '';
        zipFileInput.value = '';
    } catch (err) {
        showToast(`Ошибка удаления ZIP: ${err.message}`, false);
    } finally {
        clearUIBusy();
    }
});


togglePdfBtn.addEventListener("click", () => {
    const isHidden = pdfContainer.classList.contains("d-none");
    setPdfVisibility(isHidden);
});


window.require.config({paths: {vs: 'https://cdn.jsdelivr.net/npm/monaco-editor@0.45.0/min/vs'}});

window.require(['vs/editor/editor.main'], async () => {
    // --- 1. Register a new language ---
    monaco.languages.register({id: 'latex'});

    // --- 2. Define the Monarch tokenizer for LaTeX syntax highlighting ---
    monaco.languages.setMonarchTokensProvider('latex', {
        // Set default token for anything not matched. 'source' is for normal text.
        defaultToken: 'source',

        // Language keywords
        keywords: [
            'documentclass', 'usepackage', 'begin', 'end', 'title', 'author', 'date',
            'maketitle', 'tableofcontents', 'section', 'subsection', 'subsubsection',
            'paragraph', 'subparagraph', 'chapter', 'appendix', 'let', 'newcommand',
            'renewcommand', 'def', 'label', 'ref', 'pageref', 'cite', 'footnote',
            'emph', 'textbf', 'textit', 'underline', 'texttt', 'item', 'caption',
            'includegraphics', 'include', 'input', 'bibliographystyle', 'bibliography'
        ],

        // Operators and delimiters
        operators: [
            '=', '+', '-', '*', '/', '^', '_', '&', '#', '@'
        ],

        // Brackets and parentheses
        brackets: [
            ['{', '}', 'delimiter.curly'],
            ['[', ']', 'delimiter.square'],
            ['(', ')', 'delimiter.parenthesis']
        ],

        // The main tokenizer
        tokenizer: {
            root: [
                // Comments: % followed by anything until the end of the line
                [/%.*$/, 'comment'],

                // Commands: backslash followed by letters
                [/\\([a-zA-Z]+)/, {
                    cases: {
                        '@keywords': 'keyword',
                        '@default': 'keyword.control'
                    }
                }],

                // Special characters and symbols
                [/\\./, 'keyword.control'],

                // Delimiters and operators
                [/[{}[\]()]/, '@brackets'],
                [/[=+\-*/^&_#@]/, 'operator'],

                // Numbers
                [/\d*\.\d+([eE][-+]?\d+)?/, 'number.float'],
                [/\d+/, 'number'],

                // Whitespace
                [/[ \t\r\n]+/, 'white'],
            ],

            math: [
                [/\\./, 'keyword.math'],
                [/[^{}\[\]()]+/, 'string.math'],
                [/[\{\}\[\]()]/, 'delimiter.math'],
            ],
        },
    });

    monaco.languages.setLanguageConfiguration('latex', {
        comments: {
            lineComment: '%'
        },
        brackets: [
            ['{', '}'],
            ['[', ']'],
            ['(', ')']
        ],
        autoClosingPairs: [
            {open: '{', close: '}'},
            {open: '[', close: ']'},
            {open: '(', close: ')'},
            {open: '`', close: '`'},
            {open: '“', close: '”'},
        ],
        surroundingPairs: [
            {open: '{', close: '}'},
            {open: '[', close: ']'},
            {open: '(', close: ')'}
        ],
        // Indentation rules
        onEnterRules: [
            {
                beforeText: /\\begin\{([a-zA-Z*]+)\}/,
                afterText: /\\end\{\1\}/,
                action: {indentAction: monaco.languages.IndentAction.IndentOutdent}
            },
            {
                beforeText: /^\s*\\item/,
                action: {indentAction: monaco.languages.IndentAction.None, appendText: '\\item '}
            }
        ]
    });

    monacoEditor = monaco.editor.create(document.getElementById('latexEditor'), {
        value: '',
        language: 'latex', // Use the newly registered language
        theme: 'vs-dark',
        automaticLayout: true,
        minimap: {enabled: true},
        fontSize: 14,
        scrollBeyondLastLine: false,
        wordWrap: 'on' // Enable word wrapping for better readability
    });

    await loadMainTexContent();
});