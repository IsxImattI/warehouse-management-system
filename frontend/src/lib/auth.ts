import { browser } from '$app/environment';

export function getToken(): string | null {
    if (!browser) return null;
    return localStorage.getItem('wms_token');
}

export function setToken(token: string): void {
    localStorage.setItem('wms_token', token);
}

export function removeToken(): void {
    localStorage.removeItem('wms_token');
}

export function isLoggedIn(): boolean {
    return !!getToken();
}