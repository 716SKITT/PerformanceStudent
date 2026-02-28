#!/usr/bin/env sh
set -eu

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"

if [ -f "$ROOT_DIR/.env" ]; then
  # shellcheck disable=SC1091
  . "$ROOT_DIR/.env"
fi

SQL_FILE="${SQL_FILE:-$ROOT_DIR/db/datainstall.sql}"
DB_SERVICE="${DB_SERVICE:-postgres}"
DB_USER="${DB_USER:-${POSTGRES_USER:-student_admin}}"
DB_NAME="${DB_NAME:-${POSTGRES_DB:-student_db}}"

if [ ! -f "$SQL_FILE" ]; then
  echo "SQL file not found: $SQL_FILE" >&2
  exit 1
fi

if ! docker compose ps "$DB_SERVICE" >/dev/null 2>&1; then
  echo "Service '$DB_SERVICE' is not available in docker compose." >&2
  exit 1
fi

echo "Applying SQL from $SQL_FILE to $DB_SERVICE/$DB_NAME ..."
docker compose exec -T "$DB_SERVICE" psql -v ON_ERROR_STOP=1 -U "$DB_USER" -d "$DB_NAME" -f - < "$SQL_FILE"
echo "datainstall completed."
