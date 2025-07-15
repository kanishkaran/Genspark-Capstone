FROM node:alpine AS builder

WORKDIR /usr/src/app


COPY package.json package-lock.json ./

RUN npm install

COPY . .

RUN npm run build --prod


FROM nginx:alpine

COPY --from=builder /usr/src/app/dist/WarehouseFileArchiver/ /usr/share/nginx/html

EXPOSE 80

CMD [ "nginx", "-g", "daemon off;" ]