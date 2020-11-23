import { Msg, Model, LoginForm } from "./Domain.fs.js";
import { cons, empty } from "../../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import { Cmd_ofSub, Cmd_none } from "../../.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { singleton } from "../../.fable/fable-library.3.0.0-nagareyama-rc-008/AsyncBuilder.js";
import { usersService } from "../../Server.fs.js";
import { Response_LoggedInUser$reflection, Request_Login } from "../../../Shared/Users.fs.js";
import { startImmediate } from "../../.fable/fable-library.3.0.0-nagareyama-rc-008/Async.js";
import { Record } from "../../.fable/fable-library.3.0.0-nagareyama-rc-008/Types.js";
import { record_type, lambda_type, unit_type, option_type } from "../../.fable/fable-library.3.0.0-nagareyama-rc-008/Reflection.js";
import { ApplicationErrorModule_explain } from "../../../Shared/Errors.fs.js";

export function stateInit() {
    return new Model(new LoginForm("", ""), empty(), false);
}

export function init() {
    return [stateInit(), Cmd_none()];
}

function handleLogin(state) {
    return Cmd_ofSub((dispatch) => {
        let arg00;
        const builder$0040 = singleton;
        arg00 = builder$0040.Delay(() => builder$0040.Bind(usersService.Login(new Request_Login(state.Username, state.Password)), (_arg1) => {
            dispatch(new Msg(3, _arg1));
            return builder$0040.Zero();
        }));
        startImmediate(arg00);
    });
}

export class LoginPageProps extends Record {
    constructor(handleNewToken) {
        super();
        this.handleNewToken = handleNewToken;
    }
}

export function LoginPageProps$reflection() {
    return record_type("Cookbook.Client.Pages.Login.State.LoginPageProps", [], LoginPageProps, () => [["handleNewToken", lambda_type(option_type(Response_LoggedInUser$reflection()), unit_type)]]);
}

export function update(props, msg, state) {
    let Errors_1;
    switch (msg.tag) {
        case 1: {
            return [new Model(new LoginForm(state.Form.Username, msg.fields[0]), state.Errors, state.IsLoading), Cmd_none()];
        }
        case 2: {
            const state$0027 = new Model(state.Form, empty(), true);
            return [state$0027, handleLogin(state.Form)];
        }
        case 3: {
            const res = msg.fields[0];
            const state$0027_1 = new Model(state.Form, state.Errors, false);
            if (res.tag === 1) {
                return [(Errors_1 = cons((ApplicationErrorModule_explain(res.fields[0])), state$0027_1.Errors), new Model(state$0027_1.Form, Errors_1, state$0027_1.IsLoading)), Cmd_none()];
            }
            else {
                return [stateInit(), Cmd_ofSub((_arg1) => {
                    props.handleNewToken((res.fields[0]));
                })];
            }
        }
        default: {
            return [new Model(new LoginForm(msg.fields[0], state.Form.Password), state.Errors, state.IsLoading), Cmd_none()];
        }
    }
}

