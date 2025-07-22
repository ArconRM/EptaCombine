# EptaCombine  

**ASP.NET Core Web Application (Razor Pages) с микросервисами в Docker**  

- Сервис для конвертации файлов:  
   -  📦 **Архивы** (ZIP, RAR, 7z) через SharpCompress  
   - 🎵 **Аудио/видео** через FFMpegCore  
   - 🖼️ **Изображения** через ImageSharp

- Сервис для редактирования и pdf-компиляции шаблонов LaTeX.

---

## 🐳 Запуск через Docker Compose

### Инструкция
1. Клонируйте репозиторий:
   ```bash
   git clone https://github.com/ArconRM/EptaCombine.git
   cd EptaCombine
   ```

2. Соберите и запустите контейнеры:
   ```bash
   docker-compose up -d --build
   ```

3. Система будет доступна:
   - Web UI: http://localhost:5000
   - FileConverter API: http://localhost:5001
   - LaTeX Compiler API: http://localhost:5002

4. Для остановки:
   ```bash
   docker-compose down
   ```

---

### Контейнеры:
- **web** - ASP.NET Razor Pages приложение
  - Порт: 5000:80
  - Зависит от fileconverter

- **fileconverter** - Микросервис обработки файлов
  - Порт: 5001:80
  - Поддерживает:
    - Конвертацию форматов
    - Изменение параметров медиа
    - Пакетную обработку
   
- **latexcompiler** - Микросервис компиляции LaTeX
  - Порт: 5002:80
  - Поддерживает:
     -   Загрузку шаблона LaTeX
     -   Редактирование main.tex
     -   Компиляцию в pdf

---

## Roadmap
- [ ] Микросервис выполнения кода (Sandbox API)
- [ ] Авторизация и аутентификация
- [ ] Хранение истории
---
