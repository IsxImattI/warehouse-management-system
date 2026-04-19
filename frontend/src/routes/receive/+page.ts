import { get } from '$lib/api';

export async function load() {
    const locations = await get<any[]>('/api/locations');
    return { locations };
}