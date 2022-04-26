FROM mcr.microsoft.com/dotnet/sdk:6.0 as build

WORKDIR /workspace
COPY . .
RUN dotnet tool restore

RUN dotnet run BundleRecipesApi


FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.15
COPY --from=build /workspace/deployRecipes /app
WORKDIR /app
EXPOSE 80
ENTRYPOINT [ "dotnet", "Household.Recipes.dll" ]