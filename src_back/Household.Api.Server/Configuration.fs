module Household.Api.Server.Configuration

type DatabaseConfiguration = {
    DatabaseName : string
    RefreshTokensContainerName : string
    UsersContainerName : string
    RecipesContainerName : string
}