<script lang="ts">
    import { goto } from '$app/navigation';
    import { post } from '$lib/api';
    import { setToken } from '$lib/auth';

    let username = $state('');
    let password = $state('');
    let error = $state('');
    let loading = $state(false);

    async function login() {
        if (!username || !password) {
            error = 'Username and password are required.';
            return;
        }
        loading = true;
        error = '';
        try {
            const res = await post<{ token: string }>('/api/auth/login', { username, password });
            setToken(res.token);
            goto('/');
        } catch (e: any) {
            error = 'Invalid credentials.';
        } finally {
            loading = false;
        }
    }
</script>

<div class="min-h-screen flex items-center justify-center bg-gray-50">
    <div class="border rounded p-8 w-full max-w-sm bg-white">
        <h1 class="text-2xl font-bold mb-6">WMS Login</h1>

        {#if error}
            <p class="text-red-500 mb-4 text-sm">{error}</p>
        {/if}

        <div class="flex flex-col gap-4">
            <div>
                <label class="block text-sm mb-1" for="username">Username</label>
                <input
                    id="username"
                    bind:value={username}
                    class="border w-full p-2 rounded"
                    placeholder="username"
                />
            </div>
            <div>
                <label class="block text-sm mb-1" for="password">Password</label>
                <input
                    id="password"
                    bind:value={password}
                    type="password"
                    class="border w-full p-2 rounded"
                    placeholder="••••••••"
                />
            </div>
            <button
                onclick={login}
                disabled={loading}
                class="bg-black text-white px-4 py-2 rounded hover:bg-gray-800 disabled:opacity-50"
            >
                {loading ? 'Logging in...' : 'Login'}
            </button>
        </div>
    </div>
</div>