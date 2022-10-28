import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

/** @type {import('vite').UserConfig} */
export default defineConfig({
    plugins: [react()],
    root: "./src/Household.Api.Client",
    server: {
        port: 8080
    },
    build: {
        outDir:"../../deploy-fe"
    }

})