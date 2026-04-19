<script lang="ts">
    import { invalidateAll } from '$app/navigation';
    import { post, del } from '$lib/api';

    type User = {
        id: number;
        username: string;
        role: string;
        createdAt: string;
    };

    let { data } = $props();
    let users: User[] = $state(data.users);

    let showForm = $state(false);
    let username = $state('');
    let password = $state('');
    let role = $state('operator');
    let error = $state('');
    let loading = $state(false);

    async function create() {
        if (!username || !password) {
            error = 'Username and password are required.';
            return;
        }
        loading = true;
        error = '';
        try {
            await post('/api/users', { username, password, role });
            await invalidateAll();
            users = data.users;
            username = ''; password = ''; role = 'operator';
            showForm = false;
        } catch (e: any) {
            error = e.message;
        } finally {
            loading = false;
        }
    }

    async function remove(id: number, name: string) {
        if (!confirm(`Delete user "${name}"?`)) return;
        try {
            await del(`/api/users/${id}`);
            await invalidateAll();
            users = data.users;
        } catch (e: any) {
            error = e.message;
        }
    }
</script>

<div class="p-6">
    <div class="flex justify-between items-center mb-6">
        <h1 class="text-2xl font-bold">User Management</h1>
        <button
            onclick={() => showForm = !showForm}
            class="bg-black text-white px-4 py-2 rounded hover:bg-gray-800"
        >
            Add User
        </button>
    </div>

    {#if error}
        <p class="text-red-500 mb-4 border border-red-300 bg-red-50 p-3 rounded">{error}</p>
    {/if}

    {#if showForm}
        <div class="border rounded p-4 mb-6 bg-gray-50">
            <h2 class="font-semibold mb-4">New User</h2>
            <div class="grid grid-cols-2 gap-4">
                <div>
                    <label class="block text-sm mb-1" for="username">Username *</label>
                    <input id="username" bind:value={username} class="border w-full p-2 rounded" placeholder="username" />
                </div>
                <div>
                    <label class="block text-sm mb-1" for="password">Password *</label>
                    <input id="password" bind:value={password} type="password" class="border w-full p-2 rounded" placeholder="••••••••" />
                </div>
                <div>
                    <label class="block text-sm mb-1" for="role">Role</label>
                    <select id="role" bind:value={role} class="border w-full p-2 rounded">
                        <option value="operator">Operator</option>
                        <option value="admin">Admin</option>
                    </select>
                </div>
            </div>
            <div class="flex gap-2 mt-4">
                <button onclick={create} disabled={loading} class="bg-black text-white px-4 py-2 rounded hover:bg-gray-800 disabled:opacity-50">
                    {loading ? 'Creating...' : 'Create'}
                </button>
                <button onclick={() => showForm = false} class="border px-4 py-2 rounded hover:bg-gray-100">
                    Cancel
                </button>
            </div>
        </div>
    {/if}

    {#if users.length === 0}
        <p class="text-gray-500">No users found.</p>
    {:else}
        <table class="w-full border-collapse text-sm">
            <thead>
                <tr class="bg-gray-100 text-left">
                    <th class="p-3 border">Username</th>
                    <th class="p-3 border">Role</th>
                    <th class="p-3 border">Created</th>
                    <th class="p-3 border">Actions</th>
                </tr>
            </thead>
            <tbody>
                {#each users as user}
                    <tr class="hover:bg-gray-50">
                        <td class="p-3 border font-medium">{user.username}</td>
                        <td class="p-3 border">
                            <span class="px-2 py-1 rounded text-xs {user.role === 'admin' ? 'bg-black text-white' : 'bg-gray-100'}">
                                {user.role}
                            </span>
                        </td>
                        <td class="p-3 border text-gray-400">{new Date(user.createdAt).toLocaleString()}</td>
                        <td class="p-3 border">
                            <button
                                onclick={() => remove(user.id, user.username)}
                                class="text-red-600 hover:underline text-sm"
                            >
                                Delete
                            </button>
                        </td>
                    </tr>
                {/each}
            </tbody>
        </table>
    {/if}
</div>