import { Union, Record } from "../../.fable/fable-library.3.0.0-nagareyama-rc-008/Types.js";
import { union_type, bool_type, list_type, record_type, string_type } from "../../.fable/fable-library.3.0.0-nagareyama-rc-008/Reflection.js";
import { Response_LoggedInUser$reflection } from "../../../Shared/Users.fs.js";
import { ApplicationError$reflection } from "../../../Shared/Errors.fs.js";
import { FSharpResult$2 } from "../../.fable/fable-library.3.0.0-nagareyama-rc-008/Choice.js";

export class LoginForm extends Record {
    constructor(Username, Password) {
        super();
        this.Username = Username;
        this.Password = Password;
    }
}

export function LoginForm$reflection() {
    return record_type("Cookbook.Client.Pages.Login.Domain.LoginForm", [], LoginForm, () => [["Username", string_type], ["Password", string_type]]);
}

export class Model extends Record {
    constructor(Form, Errors, IsLoading) {
        super();
        this.Form = Form;
        this.Errors = Errors;
        this.IsLoading = IsLoading;
    }
}

export function Model$reflection() {
    return record_type("Cookbook.Client.Pages.Login.Domain.Model", [], Model, () => [["Form", LoginForm$reflection()], ["Errors", list_type(string_type)], ["IsLoading", bool_type]]);
}

export class Msg extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["UsernameChanged", "PasswordChanged", "Login", "LoggedIn"];
    }
}

export function Msg$reflection() {
    return union_type("Cookbook.Client.Pages.Login.Domain.Msg", [], Msg, () => [[["Item", string_type]], [["Item", string_type]], [], [["Item", union_type("Microsoft.FSharp.Core.FSharpResult`2", [Response_LoggedInUser$reflection(), ApplicationError$reflection()], FSharpResult$2, () => [[["ResultValue", Response_LoggedInUser$reflection()]], [["ErrorValue", ApplicationError$reflection()]]])]]]);
}

