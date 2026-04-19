<script lang="ts">
    type Log = {
        id: number;
        item: { id: number; sku: string; name: string };
        fromLocation: { id: number; code: string; name: string } | null;
        toLocation: { id: number; code: string; name: string } | null;
        quantity: number;
        note: string | null;
        createdAt: string;
    };

    let { data } = $props();
    let logs: Log[] = data.logs;
</script>

<div class="p-6">
    <h1 class="text-2xl font-bold mb-6">Movement History</h1>

    {#if logs.length === 0}
        <p class="text-gray-500">No movements recorded yet.</p>
    {:else}
        <div class="overflow-x-auto">
            <table class="w-full border-collapse text-sm">
                <thead>
                    <tr class="bg-gray-100 text-left">
                        <th class="p-3 border">Date</th>
                        <th class="p-3 border">Item</th>
                        <th class="p-3 border">From</th>
                        <th class="p-3 border">To</th>
                        <th class="p-3 border">Quantity</th>
                        <th class="p-3 border">Note</th>
                    </tr>
                </thead>
                <tbody>
                    {#each logs as log}
                        <tr class="hover:bg-gray-50">
                            <td class="p-3 border text-gray-400 whitespace-nowrap">
                                {new Date(log.createdAt).toLocaleString()}
                            </td>
                            <td class="p-3 border">
                                <span class="font-medium">{log.item.name}</span>
                                <span class="block text-xs text-gray-400 font-mono">{log.item.sku}</span>
                            </td>
                            <td class="p-3 border font-mono text-sm">
                                {log.fromLocation?.code ?? '—'}
                            </td>
                            <td class="p-3 border font-mono text-sm">
                                {log.toLocation?.code ?? '—'}
                            </td>
                            <td class="p-3 border">{log.quantity}</td>
                            <td class="p-3 border text-gray-400">{log.note ?? '—'}</td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>
    {/if}
</div>