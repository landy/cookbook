import { initStyles } from "./AppStyles.fs.js";
import { LoginPageProps } from "./Pages/Login/State.fs.js";
import { render } from "react-dom";
import { createElement } from "react";
import { Render } from "./Pages/Login/View.fs.js";

initStyles();

export const props = new LoginPageProps((value) => {
    void value;
});

render(createElement(Render, props), document.getElementById("elmish-app"));

