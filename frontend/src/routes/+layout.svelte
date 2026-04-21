<script lang="ts">
    import { browser } from '$app/environment';
    import { goto } from '$app/navigation';
    import { page } from '$app/stores';
    import '../app.css';
    import { loggedIn, removeToken, getToken, initAuth } from '$lib/auth';

    let { children } = $props();

    if (browser) initAuth();

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

    $effect(() => {
        if (browser && !$loggedIn && $page.url.pathname !== '/login') {
            goto('/login');
        }
    });

    function logout() {
        removeToken();
        goto('/login');
    }

    const navLinks = [
        { href: '/', label: 'Inventory' },
        { href: '/items', label: 'Items' },
        { href: '/locations', label: 'Locations' },
        { href: '/movement', label: 'Movement' },
        { href: '/receive', label: 'Receive' },
        { href: '/logs', label: 'Logs' },
    ];
</script>

{#if $loggedIn}
    <header style="border-bottom: 3px solid #1a1a1a; background: #ffffff;">
        <div style="display: flex; align-items: stretch; gap: 0;">
            <div style="padding: 0 24px; display: flex; align-items: center; border-right: 3px solid #1a1a1a; background: #1a1a1a;">
                <span style="font-family: 'Barlow Condensed', sans-serif; font-weight: 900; font-size: 20px; text-transform: uppercase; letter-spacing: 0.05em; color: #f5f4f0;">WMS</span>
            </div>
            <nav style="display: flex; align-items: stretch; flex: 1;">
                {#each navLinks as link}
                    <a href={link.href} style="font-family: 'Barlow Condensed', sans-serif; font-weight: 700; font-size: 13px; text-transform: uppercase; letter-spacing: 0.05em; padding: 0 20px; display: flex; align-items: center; text-decoration: none; border-right: 1px solid #e0dfd9; transition: background 0.1s; color: {$page.url.pathname === link.href ? 'white' : '#1a1a1a'}; background: {$page.url.pathname === link.href ? '#ff4500' : 'transparent'};">{link.label}</a>
                {/each}
                {#if isAdmin()}
                    <a href="/admin/users" style="font-family: 'Barlow Condensed', sans-serif; font-weight: 700; font-size: 13px; text-transform: uppercase; letter-spacing: 0.05em; padding: 0 20px; display: flex; align-items: center; text-decoration: none; border-right: 1px solid #e0dfd9; color: {$page.url.pathname === '/admin/users' ? 'white' : '#1a1a1a'}; background: {$page.url.pathname === '/admin/users' ? '#ff4500' : 'transparent'};">Users</a>
                {/if}
            </nav>
            <button
                onclick={logout}
                style="font-family: 'Barlow Condensed', sans-serif; font-weight: 700; font-size: 13px; text-transform: uppercase; letter-spacing: 0.05em; padding: 0 20px; border: none; border-left: 3px solid #1a1a1a; background: transparent; cursor: pointer; color: #666;"
            >Logout</button>
        </div>
    </header>
{/if}

{@render children()}