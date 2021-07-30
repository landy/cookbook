FROM mcr.microsoft.com/dotnet/sdk:5.0.302 as build

# Install node
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash
RUN apt-get update && apt-get install -y nodejs

WORKDIR /workspace
COPY . .
RUN dotnet tool restore

RUN dotnet fake build -t Bundle


FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
COPY --from=build /workspace/deploy /app
WORKDIR /app
EXPOSE 8085
ENTRYPOINT [ "dotnet", "Server.dll" ]