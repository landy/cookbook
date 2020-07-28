module Cookbook.Client.Pages.UsersList.Domain

type UserRow = {
    Username : string
    Name : string
}

type Model = {
    Users : UserRow list
    IsLoading : bool
}

type Msg =
    | LoadUsers
    | UsersLoaded of UserRow list
