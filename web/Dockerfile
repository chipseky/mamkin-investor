FROM node:20.12.2 as builder-step
WORKDIR /app

COPY package.json package-lock.json ./

RUN npm install

COPY angular.json tsconfig.app.json tsconfig.json tsconfig.spec.json ./

COPY ./src ./src

RUN npm run build --configuration=production --output-path=/dist

FROM nginx:1.22
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=builder-step /app/dist/mamkin-investor/browser /usr/share/nginx/html
