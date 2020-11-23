import { Union } from "../Client/.fable/fable-library.3.0.0-nagareyama-rc-008/Types.js";
import { string_type, union_type } from "../Client/.fable/fable-library.3.0.0-nagareyama-rc-008/Reflection.js";
import { printf, toText } from "../Client/.fable/fable-library.3.0.0-nagareyama-rc-008/String.js";

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
    const clo1 = toText(printf("User with username \u0027%s\u0027 already exists"));
    return clo1(_arg1.fields[0]);
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
            return UserErrorModule_explain(_arg1.fields[0]);
        }
        case 2: {
            return DatabaseErrorModule_explain(_arg1.fields[0]);
        }
        default: {
            return AuthenticationErrorModule_explain(_arg1.fields[0]);
        }
    }
}

