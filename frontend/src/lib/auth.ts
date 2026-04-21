import { browser } from '$app/environment';
import { writable } from 'svelte/store';

export const loggedIn = writable(false);

export function getToken(): string | null {
    if (!browser) return null;
    return localStorage.getItem('wms_token');
}

export function setToken(token: string): void {
    localStorage.setItem('wms_token', token);
    loggedIn.set(true);
}

export function removeToken(): void {
    localStorage.removeItem('wms_token');
    loggedIn.set(false);
}

export function isLoggedIn(): boolean {
    return !!getToken();
}

export function initAuth(): void {
    if (browser) {
        loggedIn.set(!!localStorage.getItem('wms_token'));
    }
}