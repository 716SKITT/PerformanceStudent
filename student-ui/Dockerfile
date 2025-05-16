FROM node:20-alpine
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .

EXPOSE 4200

ENV CHOKIDAR_USEPOLLING=true

CMD ["npm", "run", "start"]
