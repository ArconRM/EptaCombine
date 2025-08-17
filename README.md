# EptaCombine  

**ASP.NET Core Web Application (Razor Pages) с микросервисами в Docker**  

- Сервис для конвертации файлов:  
   -  📦 **Архивы** (ZIP, RAR, 7z) через SharpCompress  
   - 🎵 **Аудио/видео** через FFMpegCore  
   - 🖼️ **Изображения** через ImageSharp

- Сервис для редактирования и pdf-компиляции шаблонов LaTeX.

- Аутентификация пользователя и сохранение шаблонов LaTeX

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
   docker compose up -d --build
   ```

3. Система будет доступна:
   - Web UI: http://localhost:5100
   - FileConverter API: http://localhost:5101
   - LaTeX Compiler API: http://localhost:5102
   - База pg: http://localhost:5432

4. Для остановки:
   ```bash
   docker compose down
   ```

---

### Контейнеры:
- **web** - ASP.NET Razor Pages приложение
  - Порт: 5100:80
  - Зависит от fileconverter

- **fileconverter** - Микросервис обработки файлов
  - Порт: 5101:80
  - Поддерживает:
    - Конвертацию форматов
    - Изменение параметров медиа
    - Пакетную обработку
   
- **latexcompiler** - Микросервис компиляции LaTeX
  - Порт: 5102:80
  - Поддерживает:
     -   Загрузку шаблона LaTeX
     -   Редактирование main.tex и thesis.bib
     -   Компиляцию в pdf

---

## Roadmap
- [ ] Микросервис выполнения кода (Sandbox API)
- [x] Авторизация и аутентификация
- [ ] Хранение истории
---
