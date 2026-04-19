import { get } from '$lib/api';

export async function load() {
    const logs = await get<any[]>('/api/movementlogs');
    return { logs };
}