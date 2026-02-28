FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build

FROM node:20-alpine AS runtime
WORKDIR /app

RUN npm install -g serve
COPY --from=build /app/dist/webstudents-ui ./dist/webstudents-ui

EXPOSE 4200

CMD ["serve", "-s", "dist/webstudents-ui", "-l", "4200"]
