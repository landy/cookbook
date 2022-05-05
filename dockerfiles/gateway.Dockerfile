FROM nginx:1.21.6
COPY nginx/gateway.conf /etc/nginx/conf.d/default.conf