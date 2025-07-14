# EptaCombine  

**ASP.NET Core Web Application (Razor Pages) с микросервисом FileConverter в Docker**  

Проект предоставляет контейнеризированное решение для конвертации файлов:  
- 📦 **Архивы** (ZIP, RAR, 7z) через SharpCompress  
- 🎵 **Аудио/видео** через FFMpegCore  
- 🖼️ **Изображения** через ImageSharp  

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
   - FileConverter API: http://localhost:6000

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
  - Порт: 6000:80
  - Поддерживает:
    - Конвертацию форматов
    - Изменение параметров медиа
    - Пакетную обработку

---

## Roadmap
- [ ] Микросервис выполнения кода (Sandbox API)
- [ ] Интеграция с облачными хранилищами

---
