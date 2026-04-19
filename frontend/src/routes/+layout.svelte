<script lang="ts">
    import { browser } from '$app/environment';
    import { goto } from '$app/navigation';
    import { page } from '$app/stores';
    import '../app.css';
    import { isLoggedIn, removeToken, getToken } from '$lib/auth';

    let { children } = $props();

    function isAdmin(): boolean {
        const token = getToken();
        if (!token) return false;
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            return payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] === 'admin';
        } catch {
            return false;
        }
    }

    if (browser && !isLoggedIn() && $page.url.pathname !== '/login') {
        goto('/login');
    }

    function logout() {
        removeToken();
        goto('/login');
    }
</script>

{#if isLoggedIn()}
    <nav class="border-b px-6 py-3 flex gap-6 text-sm font-medium items-center">
        <a href="/" class="hover:underline">Inventory</a>
        <a href="/items" class="hover:underline">Items</a>
        <a href="/locations" class="hover:underline">Locations</a>
        <a href="/movement" class="hover:underline">Movement</a>
        <a href="/receive" class="hover:underline">Receive</a>
        <a href="/logs" class="hover:underline">Logs</a>
        {#if isAdmin()}
            <a href="/admin/users" class="hover:underline">Users</a>
        {/if}
        <button onclick={logout} class="ml-auto text-gray-400 hover:text-black">Logout</button>
    </nav>
{/if}

{@render children()}