/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./index.html",
        "./Household.Api.Client/.fable-build/**/*.{js,ts,jsx,tsx}",
    ],
    theme: {
        extend: {},
    },
    plugins: [
        require("daisyui")
    ],
};
