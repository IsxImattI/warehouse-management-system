<script>
    import '../app.css';
    let { children } = $props();
</script>

<nav class="border-b px-6 py-3 flex gap-6 text-sm font-medium">
    <a href="/" class="hover:underline">Inventory</a>
    <a href="/items" class="hover:underline">Items</a>
    <a href="/locations" class="hover:underline">Locations</a>
    <a href="/movement" class="hover:underline">Movement</a>
	<a href="/receive" class="hover:underline">Receive</a>
</nav>

{@render children()}