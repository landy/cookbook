import { render } from "react-dom";
import { createElement } from "react";
import { Foo } from "./FelizApp.jsx";

render(createElement(Foo, null), document.getElementById("app-container"));

