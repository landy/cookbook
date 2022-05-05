FROM mcr.microsoft.com/dotnet/sdk:6.0 as build

# Install node
RUN curl -fsSL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get update && apt-get install -y nodejs

WORKDIR /workspace
COPY . .
RUN dotnet tool restore

RUN dotnet run BundleClientApp


FROM nginx:1.21.6
COPY --from=build /workspace/deploy/public /app
COPY --from=build /workspace/deploy/spa.conf /etc/nginx/conf.d/default.conf