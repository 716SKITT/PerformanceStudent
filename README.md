# PerformanceStudent

Веб-приложение для учета учебного процесса: структура обучения, задания, оценки, посещаемость, ведомости и итоговые результаты.

## Стек

- Backend: ASP.NET Core 8, EF Core, PostgreSQL
- Frontend: Angular 19
- Оркестрация: Docker Compose
- Метрики: Prometheus endpoint на `/metrics`

## Основной функционал

- Аутентификация и ролевая модель (`Admin`, `Professor`, `Student`)
- Учебная структура:
  - учебные годы, семестры
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

## Быстрый старт

1. Скопировать переменные окружения:

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

Альтернатива через compose profile:

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

## Переменные окружения

См. `.env.example`:

- `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB`, `POSTGRES_PORT`
- `PGADMIN_DEFAULT_EMAIL`, `PGADMIN_DEFAULT_PASSWORD`, `PGADMIN_PORT`
- `API_PORT`, `ASPNETCORE_ENVIRONMENT`
- `FRONTEND_PORT`
- `DB_SERVICE`

## Примечания

- Dockerfile backend/frontend переведены на multi-stage build.
- Nginx-конфигурация удалена, т.к. не используется.
- Кэш/артефакты сборки (`node_modules`, `dist`, `.angular`, `bin`, `obj`) не должны попадать в git.
