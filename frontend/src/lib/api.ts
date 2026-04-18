const BASE = import.meta.env.VITE_API_URL ?? 'http://localhost:5079';

export async function get<T>(path: string): Promise<T> {
    const r = await fetch(`${BASE}${path}`);
    if (!r.ok) throw new Error(await r.text());
    return r.json();
}

export async function post<T>(path: string, body: unknown): Promise<T> {
    const r = await fetch(`${BASE}${path}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    });
    if (!r.ok) throw new Error(await r.text());
    return r.json();
}

export async function put<T>(path: string, body: unknown): Promise<T> {
    const r = await fetch(`${BASE}${path}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    });
    if (!r.ok) throw new Error(await r.text());
    return r.json();
}

export async function del(path: string): Promise<void> {
    const r = await fetch(`${BASE}${path}`, { method: 'DELETE' });
    if (!r.ok) throw new Error(await r.text());
}