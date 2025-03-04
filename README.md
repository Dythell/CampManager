# CampManager

**CampManager** – это веб-приложение для управления детским лагерем. Приложение позволяет:

- Управлять сменами (Sessions)
- Создавать отряды (Groups) и добавлять в них детей (Children)
- Назначать вожатых (Counselors) на отряды
- Организовывать мероприятия (Events) с использованием шаблонов (EventTemplates)
- Вести комментарии (Comments) к мероприятиям (с использованием SignalR для реализации real-time взаимодействия)

---

## Содержание

- [Технологии](#технологии)
- [Установка и запуск](#установка-и-запуск)
  - [Настройка базы данных PostgreSQL](#настройка-базы-данных-postgresql)
  - [Запуск бекенда](#запуск-бекенда)
  - [Запуск фронтенда](#запуск-фронтенда)
- [Структура проекта](#структура-проекта)
- [Контакты](#контакты)

---

## Технологии

- **.NET 8** (C#) + **ASP.NET Core** – создание RESTful API
- **Entity Framework Core** с **Npgsql** – ORM для работы с **PostgreSQL**
- **JWT (Json Web Token)** – аутентификация и авторизация
- **SignalR** – реализация чата/комментариев в реальном времени (при необходимости)
- **HTML/CSS/JavaScript** – фронтенд (без фреймворков), взаимодействие с API через fetch-запросы
- **Repository Pattern** и **DTO** – разделение логики доступа к данным и передачи данных между слоями
- **BCrypt** 	- Хеширование паролей

---

## Установка и запуск

### Настройка базы данных PostgreSQL

1. **Установите PostgreSQL** (версия 13+).
2. **Создайте базу данных** (например, `camp_db`).
3. **Настройте строку подключения** в файле `appsettings.json` или `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=camp_db;Username=postgres;Password=YOUR_PASSWORD"
     }
   }
   
  ```bash
  dotnet ef database update
```
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```
---

## Запуск бекенда
  1.Клонируйте репозиторий:
```bash
git clone https://github.com/username/CampManager.git
```
  2.Перейдите в папку проекта:
```bash
cd CampManager
```
3. Восстановите зависимости
```bash
dotnet restore
```
4. Откройте проект в Visual Studio, VS Code или другом IDE.
5. Проверьте настройки DI и строку подключения в файле Program.cs (или Startup.cs).
6. Запустите проект (F5 в Visual Studio или через командную строку с командой dotnet run).
7. Бекенд будет доступен по адресу: https://localhost:7060.

---

## Запуск фронтенда
  Используя Live Server (VS Code):
  1. Установите расширение Live Server.
  2. Перейдите в нужную папку фронтенда (например, Pages/MainPage) и откройте файл main.html.
  3. Нажмите «Go Live».
Пример URL: http://127.0.0.1:5500/Pages/MainPage/main.html.
- Альтернативно:
Используйте любой другой локальный сервер (например, http-server для Node.js) для раздачи статичных файлов из папки Pages.
  
## Структура проекта
```plaintext
CampManager
├── Controllers
│   ├── AuthController.cs
│   ├── ChildrenController.cs
│   ├── CommentsController.cs
│   ├── CounselorsController.cs
│   ├── EventsController.cs
│   ├── EventTemplatesController.cs
│   ├── GroupsController.cs
│   ├── ProfileController.cs
│   └── SessionsController.cs
├── DTOs
│   ├── CreateChildDTO.cs
│   ├── CreateEventDTO.cs
│   ├── CreateEventTemplateDTO.cs
│   ├── CreateGroupDTO.cs
│   ├── CreateSessionDTO.cs
│   ├── LoginRequestDTO.cs
│   ├── RegisterRequestDTO.cs
│   └── UserDTO.cs
├── Hubs
│   └── CommentHub.cs
├── Models
│   ├── Child.cs
│   ├── Comment.cs
│   ├── Counselor.cs
│   ├── Event.cs
│   ├── EventTemplate.cs
│   ├── Group.cs
│   ├── Notification.cs
│   ├── Session.cs
│   ├── SessionChild.cs
│   ├── SessionCounselor.cs
│   └── User.cs
├── Pages
│   ├── Auth
│   ├── CreateChild
│   ├── Event
│   ├── EventTemplates
│   ├── Groups
│   ├── MainPage
│   ├── ProfilePage
│   ├── SessionDetails
│   └── Sessions
├── Repositories
│   ├── ChildRepository.cs
│   ├── CounselorRepository.cs
│   ├── EventRepository.cs
│   ├── EventTemplateRepository.cs
│   ├── GroupRepository.cs
│   ├── SessionChildRepository.cs
│   ├── SessionRepository.cs
│   └── UserRepository.cs
├── Services
│   └── JwtService.cs
├── ApplicationDbContext.cs
├── Program.cs
└── appsettings.json
```
---
## Контакты
  Автор: Дмитрий Мишланов
  GitHub: https://github.com/dythell/CampManager
