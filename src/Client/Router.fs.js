import { Union } from "./.fable/fable-library.3.0.0-nagareyama-rc-008/Types.js";
import { union_type } from "./.fable/fable-library.3.0.0-nagareyama-rc-008/Reflection.js";
import { empty, tryFind, ofArray, singleton } from "./.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import { equalsSafe, equals } from "./.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";
import { defaultArg, map } from "./.fable/fable-library.3.0.0-nagareyama-rc-008/Option.js";
import { RouterModule_nav } from "./.fable/Feliz.Router.3.2.0/Router.fs.js";

export class Page extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Main", "Login", "UsersList", "UsersAdd", "UsersEdit"];
    }
}

export function Page$reflection() {
    return union_type("Cookbook.Client.Router.Page", [], Page, () => [[], [], [], [], []]);
}

const basicMapping = ofArray([[singleton("login"), new Page(1)], [singleton("users"), new Page(2)], [ofArray(["users", "add"]), new Page(3)], [ofArray(["users", "edit"]), new Page(4)]]);

export function PageModule_parseFromUrlSegments(_arg1) {
    let option_1;
    let option;
    option = tryFind((tupledArg) => equals(tupledArg[0], _arg1), basicMapping);
    option_1 = map((tuple) => tuple[1], option);
    return defaultArg(option_1, new Page(0));
}

export function PageModule_toUrlSegments(_arg1) {
    let option_1;
    let option;
    option = tryFind((tupledArg) => equalsSafe(tupledArg[1], _arg1), basicMapping);
    option_1 = map((tuple) => tuple[0], option);
    return defaultArg(option_1, empty());
}

export function Router_goToUrl(e) {
    e.preventDefault();
    const href = e.currentTarget.attributes.href.value;
    RouterModule_nav(singleton(href), 1, 2);
}

export function Router_navigatePage(p) {
    let arg00;
    let list;
    list = PageModule_toUrlSegments(p);
    arg00 = Array.from(list);
    return singleton((_arg76) => {
        RouterModule_nav(ofArray(arg00), 1, 2);
    });
}

