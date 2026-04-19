export const ssr = false;

import { get } from '$lib/api';

export async function load() {
    const users = await get<any[]>('/api/users');
    return { users };
}