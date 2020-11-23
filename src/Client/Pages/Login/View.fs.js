import styles$002Emodule from "./styles.module.scss";
import { useFeliz_React__React_useElmish_Static_645B1FB7 } from "../../.fable/Feliz.UseElmish.1.5.0/UseElmish.fs.js";
import { update, init } from "./State.fs.js";
import { join, printf, toConsole } from "../../.fable/fable-library.3.0.0-nagareyama-rc-008/String.js";
import { reactApi, reactElement, mkAttr } from "../../.fable/Feliz.1.17.0/Interop.fs.js";
import { ofSeq, cons, singleton, ofArray, map } from "../../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import { createObj } from "../../.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";
import { Helpers_combineClasses } from "../../.fable/Feliz.Bulma.2.4.0/ElementBuilders.fs.js";
import { Msg } from "./Domain.fs.js";
import { empty, singleton as singleton_1, append, delay } from "../../.fable/fable-library.3.0.0-nagareyama-rc-008/Seq.js";

const stylesheet = (null, styles$002Emodule);

export function Render(props) {
    let elems_7, props_6, names, elems_6, xs_3, elems_1, xs_16, elems_5, elms, xs_4, props_2, typ, xs_6, xs_7, elms_1, xs_8, props_3, typ_1, xs_10, xs_11, props_5, elems_4, props_4, typ_2, xs_13, xs_15, xs_18;
    const patternInput = useFeliz_React__React_useElmish_Static_645B1FB7(init, (msg, state) => update(props, msg, state), []);
    const state_1 = patternInput[0];
    const dispatch = patternInput[1];
    const arg10 = stylesheet["test"];
    const clo1 = toConsole(printf("class: %s"));
    clo1(arg10);
    const props_7 = singleton((elems_7 = [(props_6 = ofArray([(names = [stylesheet["test"]], mkAttr("className", join(" ", names))), (elems_6 = [(xs_3 = singleton((elems_1 = (map((err) => {
        let elems, xs;
        const props_1 = ofArray([mkAttr("className", "is-danger"), (elems = [(xs = ofArray([mkAttr("className", "message-body"), mkAttr("children", err)]), reactElement("div", createObj(xs)))], mkAttr("children", reactApi.Children.toArray(Array.from(elems))))]);
        const xs_2 = Helpers_combineClasses("message", props_1);
        return reactElement("article", createObj(xs_2));
    }, state_1.Errors)), mkAttr("children", reactApi.Children.toArray(Array.from(elems_1))))), reactElement("div", createObj(xs_3))), (xs_16 = ofArray([mkAttr("onSubmit", (e) => {
        e.preventDefault();
        dispatch(new Msg(2));
    }), (elems_5 = [(elms = ofArray([(xs_4 = ofArray([mkAttr("className", "label"), mkAttr("children", "Username")]), reactElement("label", createObj(xs_4))), (props_2 = ofArray([mkAttr("onChange", (ev) => {
        const arg = ev.target.value;
        dispatch((new Msg(0, arg)));
    }), mkAttr("value", state_1.Form.Username), mkAttr("autoFocus", true), mkAttr("name", "username"), mkAttr("required", true)]), (typ = mkAttr("type", "text"), (xs_6 = cons(typ, Helpers_combineClasses("input", props_2)), reactElement("input", createObj(xs_6)))))]), (xs_7 = ofArray([mkAttr("className", "field"), mkAttr("children", reactApi.Children.toArray(Array.from(elms)))]), reactElement("div", createObj(xs_7)))), (elms_1 = ofArray([(xs_8 = ofArray([mkAttr("className", "label"), mkAttr("children", "Password")]), reactElement("label", createObj(xs_8))), (props_3 = ofArray([mkAttr("onChange", (ev_1) => {
        const arg_1 = ev_1.target.value;
        dispatch((new Msg(1, arg_1)));
    }), mkAttr("name", "password"), mkAttr("required", true), mkAttr("value", state_1.Form.Password)]), (typ_1 = mkAttr("type", "password"), (xs_10 = cons(typ_1, Helpers_combineClasses("input", props_3)), reactElement("input", createObj(xs_10)))))]), (xs_11 = ofArray([mkAttr("className", "field"), mkAttr("children", reactApi.Children.toArray(Array.from(elms_1)))]), reactElement("div", createObj(xs_11)))), (props_5 = singleton((elems_4 = [(props_4 = ofSeq(delay(() => append(state_1.IsLoading ? singleton_1(mkAttr("className", "is-loading")) : empty(), delay(() => singleton_1(mkAttr("value", "Login")))))), (typ_2 = mkAttr("type", "submit"), (xs_13 = cons(typ_2, Helpers_combineClasses("button", props_4)), reactElement("input", createObj(xs_13)))))], mkAttr("children", reactApi.Children.toArray(Array.from(elems_4))))), (xs_15 = Helpers_combineClasses("field", props_5), reactElement("div", createObj(xs_15))))], mkAttr("children", reactApi.Children.toArray(Array.from(elems_5))))]), reactElement("form", createObj(xs_16)))], mkAttr("children", reactApi.Children.toArray(Array.from(elems_6))))]), (xs_18 = Helpers_combineClasses("box", props_6), reactElement("div", createObj(xs_18))))], mkAttr("children", reactApi.Children.toArray(Array.from(elems_7)))));
    const xs_20 = Helpers_combineClasses("container", props_7);
    return reactElement("div", createObj(xs_20));
}

