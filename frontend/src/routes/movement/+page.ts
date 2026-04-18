import { get } from '$lib/api';

export async function load() {
    const [items, locations] = await Promise.all([
        get<any[]>('/api/items'),
        get<any[]>('/api/locations')
    ]);
    return { items, locations };
}