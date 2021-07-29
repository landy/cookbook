import { Record } from "../Client/.fable/fable-library.3.2.9/Types.js";
import { list_type, unit_type, lambda_type, union_type, class_type, record_type, string_type } from "../Client/.fable/fable-library.3.2.9/Reflection.js";
import { printf, toText } from "../Client/.fable/fable-library.3.2.9/String.js";
import { ApplicationError$reflection } from "./Errors.fs.js";
import { FSharpResult$2 } from "../Client/.fable/fable-library.3.2.9/Choice.js";

export class Request_Login extends Record {
    constructor(Username, Password) {
        super();
        this.Username = Username;
        this.Password = Password;
    }
}

export function Request_Login$reflection() {
    return record_type("Cookbook.Shared.Users.Request.Login", [], Request_Login, () => [["Username", string_type], ["Password", string_type]]);
}

export class Request_AddUser extends Record {
    constructor(Username, Name, Password) {
        super();
        this.Username = Username;
        this.Name = Name;
        this.Password = Password;
    }
}

export function Request_AddUser$reflection() {
    return record_type("Cookbook.Shared.Users.Request.AddUser", [], Request_AddUser, () => [["Username", string_type], ["Name", string_type], ["Password", string_type]]);
}

export class Response_Token extends Record {
    constructor(Token, ExpiresUtc) {
        super();
        this.Token = Token;
        this.ExpiresUtc = ExpiresUtc;
    }
}

export function Response_Token$reflection() {
    return record_type("Cookbook.Shared.Users.Response.Token", [], Response_Token, () => [["Token", string_type], ["ExpiresUtc", class_type("System.DateTimeOffset")]]);
}

export class Response_UserSession extends Record {
    constructor(Username, Name, Token, RefreshToken) {
        super();
        this.Username = Username;
        this.Name = Name;
        this.Token = Token;
        this.RefreshToken = RefreshToken;
    }
}

export function Response_UserSession$reflection() {
    return record_type("Cookbook.Shared.Users.Response.UserSession", [], Response_UserSession, () => [["Username", string_type], ["Name", string_type], ["Token", Response_Token$reflection()], ["RefreshToken", Response_Token$reflection()]]);
}

export class Response_UserRow extends Record {
    constructor(Username, Name) {
        super();
        this.Username = Username;
        this.Name = Name;
    }
}

export function Response_UserRow$reflection() {
    return record_type("Cookbook.Shared.Users.Response.UserRow", [], Response_UserRow, () => [["Username", string_type], ["Name", string_type]]);
}

export function Route_builder(_arg1, m) {
    return toText(printf("/api/users/%s"))(m);
}

export class UsersService extends Record {
    constructor(Login, GetUsers, SaveUser) {
        super();
        this.Login = Login;
        this.GetUsers = GetUsers;
        this.SaveUser = SaveUser;
    }
}

export function UsersService$reflection() {
    return record_type("Cookbook.Shared.Users.UsersService", [], UsersService, () => [["Login", lambda_type(Request_Login$reflection(), class_type("Microsoft.FSharp.Control.FSharpAsync`1", [union_type("Microsoft.FSharp.Core.FSharpResult`2", [Response_UserSession$reflection(), ApplicationError$reflection()], FSharpResult$2, () => [[["ResultValue", Response_UserSession$reflection()]], [["ErrorValue", ApplicationError$reflection()]]])]))], ["GetUsers", lambda_type(unit_type, class_type("Microsoft.FSharp.Control.FSharpAsync`1", [union_type("Microsoft.FSharp.Core.FSharpResult`2", [list_type(Response_UserRow$reflection()), ApplicationError$reflection()], FSharpResult$2, () => [[["ResultValue", list_type(Response_UserRow$reflection())]], [["ErrorValue", ApplicationError$reflection()]]])]))], ["SaveUser", lambda_type(Request_AddUser$reflection(), class_type("Microsoft.FSharp.Control.FSharpAsync`1", [union_type("Microsoft.FSharp.Core.FSharpResult`2", [unit_type, ApplicationError$reflection()], FSharpResult$2, () => [[["ResultValue", unit_type]], [["ErrorValue", ApplicationError$reflection()]]])]))]]);
}

