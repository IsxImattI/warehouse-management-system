import { get } from '$lib/api';

export async function load() {
    const items = await get<any[]>('/api/items');
    return { items };
}