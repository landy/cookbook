import { Union } from "../Client/.fable/fable-library.3.2.9/Types.js";
import { string_type, union_type } from "../Client/.fable/fable-library.3.2.9/Reflection.js";
import { printf, toText } from "../Client/.fable/fable-library.3.2.9/String.js";

export class DatabaseError extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Unspecified"];
    }
}

export function DatabaseError$reflection() {
    return union_type("Cookbook.Shared.Errors.DatabaseError", [], DatabaseError, () => [[]]);
}

export function DatabaseErrorModule_explain(_arg1) {
    return "Unspecified Database error occured";
}

export class UserError extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["UserAlreadyExists"];
    }
}

export function UserError$reflection() {
    return union_type("Cookbook.Shared.Errors.UserError", [], UserError, () => [[["username", string_type]]]);
}

export function UserErrorModule_explain(_arg1) {
    const username = _arg1.fields[0];
    return toText(printf("User with username \u0027%s\u0027 already exists"))(username);
}

export class AuthenticationError extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["InvalidUsernameOrPassword"];
    }
}

export function AuthenticationError$reflection() {
    return union_type("Cookbook.Shared.Errors.AuthenticationError", [], AuthenticationError, () => [[]]);
}

export function AuthenticationErrorModule_explain(_arg1) {
    return "Invalid username or password";
}

export class ApplicationError extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["AuthenticationError", "UserError", "DatabaseError"];
    }
}

export function ApplicationError$reflection() {
    return union_type("Cookbook.Shared.Errors.ApplicationError", [], ApplicationError, () => [[["Item", AuthenticationError$reflection()]], [["Item", UserError$reflection()]], [["Item", DatabaseError$reflection()]]]);
}

export function ApplicationErrorModule_explain(_arg1) {
    switch (_arg1.tag) {
        case 1: {
            const err_1 = _arg1.fields[0];
            return UserErrorModule_explain(err_1);
        }
        case 2: {
            const err_2 = _arg1.fields[0];
            return DatabaseErrorModule_explain(err_2);
        }
        default: {
            const err = _arg1.fields[0];
            return AuthenticationErrorModule_explain(err);
        }
    }
}

