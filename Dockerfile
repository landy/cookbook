FROM mcr.microsoft.com/dotnet/sdk:6.0 as build

# Install node
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash
RUN apt-get update && apt-get install -y nodejs

WORKDIR /workspace
COPY . .
RUN dotnet tool restore

RUN dotnet run Bundle


FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.15
COPY --from=build /workspace/deploy /app
WORKDIR /app
EXPOSE 80
ENTRYPOINT [ "dotnet", "Server.dll" ]