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

<div style="min-height: 100vh; display: flex; align-items: center; justify-content: center; background: #f5f4f0;">
    <div style="width: 100%; max-width: 400px;">
        <div style="border-bottom: 3px solid #1a1a1a; background: #1a1a1a; padding: 20px 28px; margin-bottom: 0;">
            <h1 style="margin: 0; font-family: 'Barlow Condensed', sans-serif; font-weight: 900; font-size: 28px; text-transform: uppercase; letter-spacing: 0.05em; color: #f5f4f0;">
                WMS
            </h1>
            <p style="margin: 4px 0 0; font-family: 'Space Mono', monospace; font-size: 11px; color: #888; text-transform: uppercase; letter-spacing: 0.1em;">
                Warehouse Management System
            </p>
        </div>

        <div style="background: white; border: 3px solid #1a1a1a; border-top: none; padding: 28px;">
            {#if error}
                <div class="error-msg" style="margin-bottom: 20px;">{error}</div>
            {/if}

            <div style="display: flex; flex-direction: column; gap: 16px;">
                <div>
                    <label style="display: block; font-family: 'Barlow Condensed', sans-serif; font-weight: 700; font-size: 11px; text-transform: uppercase; letter-spacing: 0.1em; margin-bottom: 6px;" for="username">
                        Username
                    </label>
                    <input id="username" bind:value={username} placeholder="username" />
                </div>
                <div>
                    <label style="display: block; font-family: 'Barlow Condensed', sans-serif; font-weight: 700; font-size: 11px; text-transform: uppercase; letter-spacing: 0.1em; margin-bottom: 6px;" for="password">
                        Password
                    </label>
                    <input id="password" bind:value={password} type="password" placeholder="••••••••" />
                </div>
                <button class="primary" onclick={login} disabled={loading} style="margin-top: 8px; width: 100%; padding: 12px;">
                    {loading ? 'Authenticating...' : 'Login'}
                </button>
            </div>
        </div>
    </div>
</div>