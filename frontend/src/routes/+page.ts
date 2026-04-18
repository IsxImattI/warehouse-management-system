import { get } from '$lib/api';

export async function load() {
    const inventory = await get<any[]>('/api/inventory');
    return { inventory };
}