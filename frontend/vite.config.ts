import { sveltekit } from '@sveltejs/kit/vite';
import tailwindcss from '@tailwindcss/vite';
import { defineConfig } from 'vite';

export default defineConfig({
    plugins: [
        tailwindcss(),
        sveltekit()
    ],
    server: {
        headers: {
            'Content-Security-Policy': "script-src 'self' 'unsafe-eval'"
        }
    }
});