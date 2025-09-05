// Get elements
let codeEditor;

const languageSelect = document.getElementById('languageSelect');
const runBtn = document.getElementById('runBtn');
const clearBtn = document.getElementById('clearBtn');
const clearOutputBtn = document.getElementById('clearOutputBtn');
const outputContent = document.getElementById('outputContent');
const editorLanguage = document.getElementById('editorLanguage');
const executionTime = document.getElementById('executionTime');
const toastBox = document.getElementById('toastBox');

// Get URLs from data attributes
const container = document.getElementById('codeRunnerContainer');
const urls = {
    runCode: container.dataset.runCodeUrl
};

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

// Language mappings for Monaco Editor
const languageMap = {
    'CSharp': 'csharp',
    'Python': 'python',
    'JavaScript': 'javascript',
    'java': 'java',
    'cpp': 'cpp',
    'c': 'c',
    'go': 'go',
    'rust': 'rust',
    'php': 'php',
    'ruby': 'ruby'
};

const ProgramLanguage = {
    "CSharp": 0,
    "Python": 1,
    "JavaScript": 2
};

// Default code templates
const codeTemplates = {
    'javascript': `console.log("Hello, World!");

// Example: Basic arithmetic
let a = 10;
let b = 5;
console.log(\`Sum: \${a + b}\`);
console.log(\`Product: \${a * b}\`);

// Example: Array operations
let numbers = [1, 2, 3, 4, 5];
let doubled = numbers.map(n => n * 2);
console.log("Doubled:", doubled);`,

    'python': `print("Hello, World!")

# Example: Basic arithmetic
a = 10
b = 5
print(f"Sum: {a + b}")
print(f"Product: {a * b}")

# Example: List operations
numbers = [1, 2, 3, 4, 5]
doubled = [n * 2 for n in numbers]
print("Doubled:", doubled)`,

    'csharp': `using System;
using System.Linq;

class Program 
{
    static void Main() 
    {
        Console.WriteLine("Hello, World!");
        
        // Example: Basic arithmetic
        int a = 10;
        int b = 5;
        Console.WriteLine($"Sum: {a + b}");
        Console.WriteLine($"Product: {a * b}");
        
        // Example: Array operations
        int[] numbers = {1, 2, 3, 4, 5};
        var doubled = numbers.Select(n => n * 2).ToArray();
        Console.WriteLine($"Doubled: [{string.Join(", ", doubled)}]");
    }
}`,

    'java': `public class Main {
    public static void main(String[] args) {
        System.out.println("Hello, World!");
        
        // Example: Basic arithmetic
        int a = 10;
        int b = 5;
        System.out.println("Sum: " + (a + b));
        System.out.println("Product: " + (a * b));
        
        // Example: Array operations
        int[] numbers = {1, 2, 3, 4, 5};
        System.out.print("Doubled: [");
        for (int i = 0; i < numbers.length; i++) {
            System.out.print(numbers[i] * 2);
            if (i < numbers.length - 1) System.out.print(", ");
        }
        System.out.println("]");
    }
}`,

    'cpp': `#include <iostream>
#include <vector>

int main() {
    std::cout << "Hello, World!" << std::endl;
    
    // Example: Basic arithmetic
    int a = 10;
    int b = 5;
    std::cout << "Sum: " << (a + b) << std::endl;
    std::cout << "Product: " << (a * b) << std::endl;
    
    // Example: Vector operations
    std::vector<int> numbers = {1, 2, 3, 4, 5};
    std::cout << "Doubled: [";
    for (size_t i = 0; i < numbers.size(); i++) {
        std::cout << numbers[i] * 2;
        if (i < numbers.size() - 1) std::cout << ", ";
    }
    std::cout << "]" << std::endl;
    
    return 0;
}`,

    'c': `#include <stdio.h>

int main() {
    printf("Hello, World!\\n");
    
    // Example: Basic arithmetic
    int a = 10;
    int b = 5;
    printf("Sum: %d\\n", a + b);
    printf("Product: %d\\n", a * b);
    
    // Example: Array operations
    int numbers[] = {1, 2, 3, 4, 5};
    int size = sizeof(numbers) / sizeof(numbers[0]);
    printf("Doubled: [");
    for (int i = 0; i < size; i++) {
        printf("%d", numbers[i] * 2);
        if (i < size - 1) printf(", ");
    }
    printf("]\\n");
    
    return 0;
}`,

    'go': `package main

import "fmt"

func main() {
    fmt.Println("Hello, World!")
    
    // Example: Basic arithmetic
    a := 10
    b := 5
    fmt.Printf("Sum: %d\\n", a+b)
    fmt.Printf("Product: %d\\n", a*b)
    
    // Example: Slice operations
    numbers := []int{1, 2, 3, 4, 5}
    fmt.Print("Doubled: [")
    for i, n := range numbers {
        fmt.Print(n * 2)
        if i < len(numbers)-1 {
            fmt.Print(", ")
        }
    }
    fmt.Println("]")
}`,

    'rust': `fn main() {
    println!("Hello, World!");
    
    // Example: Basic arithmetic
    let a = 10;
    let b = 5;
    println!("Sum: {}", a + b);
    println!("Product: {}", a * b);
    
    // Example: Vector operations
    let numbers = vec![1, 2, 3, 4, 5];
    let doubled: Vec<i32> = numbers.iter().map(|&n| n * 2).collect();
    println!("Doubled: {:?}", doubled);
}`,

    'php': `<?php
echo "Hello, World!\\n";

// Example: Basic arithmetic
$a = 10;
$b = 5;
echo "Sum: " . ($a + $b) . "\\n";
echo "Product: " . ($a * $b) . "\\n";

// Example: Array operations
$numbers = [1, 2, 3, 4, 5];
$doubled = array_map(function($n) { return $n * 2; }, $numbers);
echo "Doubled: [" . implode(", ", $doubled) . "]\\n";
?>`,

    'ruby': `puts "Hello, World!"

# Example: Basic arithmetic
a = 10
b = 5
puts "Sum: #{a + b}"
puts "Product: #{a * b}"

# Example: Array operations
numbers = [1, 2, 3, 4, 5]
doubled = numbers.map { |n| n * 2 }
puts "Doubled: #{doubled}"`
};

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

function updateEditorLanguage() {
    const selectedLanguage = languageMap[languageSelect.value] || languageSelect.value;

    // Update Monaco editor language
    monaco.editor.setModelLanguage(codeEditor.getModel(), selectedLanguage);

    // Update language display
    editorLanguage.textContent = selectedLanguage.charAt(0).toUpperCase() + selectedLanguage.slice(1);

    // Load template code if editor is empty or contains template
    const currentCode = codeEditor.getValue().trim();
    if (!currentCode || Object.values(codeTemplates).some(template => currentCode === template.trim())) {
        if (codeTemplates[selectedLanguage]) {
            codeEditor.setValue(codeTemplates[selectedLanguage]);
        }
    }
}

function formatExecutionTime(ms) {
    if (ms < 1000) {
        return `${ms}ms`;
    } else {
        return `${(ms / 1000).toFixed(2)}s`;
    }
}

// Event listeners
languageSelect.addEventListener('change', updateEditorLanguage);

runBtn.addEventListener('click', async function() {
    const code = codeEditor.getValue().trim();
    if (!code) {
        showToast("Введите код для выполнения", false);
        return;
    }

    setUIBusy("Выполняется код...");
    const startTime = Date.now();

    try {
        const response = await fetch(urls.runCode, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify({
                code: code,
                programLanguage: ProgramLanguage[languageSelect.value]
            })
        });

        if (!response.ok) {
            clearUIBusy();
            const error = await response.text();
            console.error("Running code failed:", error);
            showToast("Ошибка при запуске кода", false)
            return;
        }

        const result = await response.json();
        const executionTimeMs = Date.now() - startTime;
    
        // Display output
        console.log(result.output);
        outputContent.innerHTML = result.output
            .replace(/&/g, '&amp;')      // Escape & first
            .replace(/</g, '&lt;')       // Escape < 
            .replace(/>/g, '&gt;')       // Escape >
            .replace(/"/g, '&quot;')     // Escape "
            .replace(/'/g, '&#39;')      // Escape '
            .replace(/\n/g, '<br>');     // Convert \n to <br>

        // Show execution time
        executionTime.textContent = `⏱️ ${formatExecutionTime(executionTimeMs)}`;
        executionTime.classList.remove('d-none');

        showToast(result.isSuccess ? "Код успешно выполнен" : "Код выполнен, но с ошибками");

    } catch (error) {
        console.error('Execution error:', error);
        outputContent.textContent = `❌ Ошибка выполнения: ${error.message}`;
        executionTime.classList.add('d-none');
        showToast("Ошибка при выполнении кода", false);
    } finally {
        clearUIBusy();
    }
});

clearBtn.addEventListener('click', function() {
    const selectedLanguage = languageMap[languageSelect.value] || languageSelect.value;
    
    if (codeTemplates[selectedLanguage]) {
        codeEditor.setValue(codeTemplates[selectedLanguage]);
    } else {
        codeEditor.setValue('');
    }
    showToast("Редактор очищен");
});

clearOutputBtn.addEventListener('click', function() {
    outputContent.textContent = "Нажмите \"Запустить код\" чтобы увидеть результат выполнения...";
    executionTime.classList.add('d-none');
});

// Initialize Monaco Editor
window.addEventListener('load', function() {
    const toastBox = document.getElementById('toastBox');
    if (toastBox) {
        window.bootstrapToast = new bootstrap.Toast(toastBox);
    }
});

// Setup Monaco Editor
window.require.config({paths: {vs: 'https://cdn.jsdelivr.net/npm/monaco-editor@0.45.0/min/vs'}});

window.require(['vs/editor/editor.main'], function () {
    codeEditor = monaco.editor.create(document.getElementById('codeEditor'), {
        value: codeTemplates.csharp, // Default to Python template
        language: 'csharp',
        theme: 'vs-dark',
        automaticLayout: true,
        minimap: { enabled: true },
        fontSize: 14,
        scrollBeyondLastLine: false,
        wordWrap: 'on',
        lineNumbers: 'on',
        folding: true,
        bracketMatching: 'always',
        autoIndent: 'advanced',
        formatOnPaste: true,
        formatOnType: true
    });

    // Set initial language display
    updateEditorLanguage();
});