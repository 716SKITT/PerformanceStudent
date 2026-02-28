# PerformanceStudent

Веб-приложение для учета учебного процесса СПО: структура обучения, задания, оценки, посещаемость, ведомости и итоговые результаты.

## CI и качество

- GitHub Actions workflow: `.github/workflows/ci.yml`
- Проверки в CI:
  - backend lint: `dotnet format --verify-no-changes`
  - backend unit tests: `dotnet test`

## Отчет по релизу

Блок ниже обновляется автоматически при публикации GitHub Release workflow-ом `release-report`.

<!-- RELEASE_REPORT_START -->

# Release Report

- Repository: 716SKITT/PerformanceStudent
- Tag: 1.0.0
- Published at: 2026-02-28T14:51:21Z
- Commit: 2c2ad713e6d378045725d270188bca43ba1923f1
- Workflow run: https://github.com/716SKITT/PerformanceStudent/actions/runs/22522961044

## Included commits
- 2c2ad71 Merge pull request #8 from 716SKITT/hotfix/actions-permissions (716Skitt)
- fecda4f refactoring (linter) (PavlenkoEA)
- 2a9a3c6 Merge pull request #6 from 716SKITT/hotfix/actions-permissions (716Skitt)
- 0c56418 fix actions permissions n2 (PavlenkoEA)
- 23848b9 Merge pull request #4 from 716SKITT/hotfix/actions-permissions (716Skitt)
- 505ce59 fix actions permissions (PavlenkoEA)
- 5eb7fd4 Merge pull request #1 from 716SKITT/feature/rethinking-entities (716Skitt)
- 1fcfdd5 unit test & github actions (PavlenkoEA)
- c8ed31c added business entities, changed ui-ux (PavlenkoEA)
- b600cd7 Delete WebStudents/wwwroot directory (716Skitt)
- d620dda Delete WebStudents/obj directory (716Skitt)
- 0493ee4 Delete WebStudents/bin/Debug/net8.0 directory (716Skitt)
- 42bd5c3 Delete WebStudents/Pages directory (716Skitt)
- 26232f0 fix: rebase env & add: CORS (Egor Pavlenko)
- 24dd706 swagger (Egor Pavlenko)
- f7d0dc5 review html (Egor Pavlenko)
- e65e50e first commit (Egor Pavlenko)

<!-- RELEASE_REPORT_END -->

## Стек

- Backend: ASP.NET Core 8, EF Core, PostgreSQL
- Frontend: Angular 19 (`WebStudentsUI`)
- Оркестрация: Docker Compose
- Метрики: Prometheus endpoint на `GET /metrics`

## Структура репозитория

- `WebStudents/` — backend API
  - solution: `WebStudents/WebStudents.sln`
  - project: `WebStudents/WebStudents.csproj`
- `WebStudentsUI/` — frontend Angular
- `db/datainstall.sql` — полный reset + демо-данные
- `scripts/datainstall.sh` — запуск datainstall в контейнере PostgreSQL
- `docker-compose.yml` — инфраструктура проекта

## Основной функционал

- Аутентификация и ролевая модель (`Admin`, `Professor`, `Student`)
- Учебная структура:
  - учебные годы и семестры
  - группы
  - дисциплины
  - назначения дисциплин на группу/семестр/преподавателя
- Учебный процесс:
  - задания
  - оценки
  - посещаемость
- Аттестация:
  - ведомости
  - итоговые оценки
- Профили пользователей

## Сущности домена

- `UserAccount` — учетная запись, роль, связь с персоной (`LinkedPersonId`)
- `Student` — студент, привязка к группе
- `Proffessor` — преподаватель
- `AcademicYear` — учебный год
- `Semester` — семестр учебного года
- `StudentGroup` — учебная группа
- `Discipline` — дисциплина и тип контроля
- `DisciplineOffering` — назначение дисциплины на группу/семестр/преподавателя
- `Assignment` — задание по назначению
- `Grade` — оценка студента за задание
- `Attendance` — отметка посещаемости
- `GradeSheet` — ведомость по назначению
- `FinalGrade` — итоговая оценка студента в ведомости

## Быстрый старт (Docker)

1. Создать `.env` из шаблона:

```bash
cp .env.example .env
```

2. Запустить проект:

```bash
docker compose up -d --build
```

3. Доступные сервисы:

- Frontend: `http://localhost:${FRONTEND_PORT}` (по умолчанию `4200`)
- API: `http://localhost:${API_PORT}` (по умолчанию `5006`)
- Swagger: `http://localhost:${API_PORT}/swagger`
- Metrics: `http://localhost:${API_PORT}/metrics`
- pgAdmin: `http://localhost:${PGADMIN_PORT}` (по умолчанию `5050`)

## Заполнение БД демо-данными

Скрипт `datainstall` полностью очищает таблицы и заполняет реалистичным русскоязычным набором данных.

```bash
./scripts/datainstall.sh
```

Альтернатива:

```bash
docker compose --profile tools run --rm datainstall
```

## Демо-аккаунты (после datainstall)

- Admin: `admin / admin123`
- Professors:
  - `prof_igor / prof123`
  - `prof_elena / prof123`
  - `prof_oleg / prof123`
  - `prof_marina / prof123`
- Students: `stud_* / stud123`

## Локальная сборка без Docker (опционально)

Backend:

```bash
cd WebStudents
dotnet build WebStudents.sln
```

Frontend:

```bash
cd WebStudentsUI
npm ci
npm run build
```

## Переменные окружения

См. `.env.example`:

- `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB`, `POSTGRES_PORT`
- `PGADMIN_DEFAULT_EMAIL`, `PGADMIN_DEFAULT_PASSWORD`, `PGADMIN_PORT`
- `API_PORT`, `ASPNETCORE_ENVIRONMENT`
- `FRONTEND_PORT`
- `DB_SERVICE`
