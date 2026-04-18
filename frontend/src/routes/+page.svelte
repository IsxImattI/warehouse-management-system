<script lang="ts">
    type InventoryItem = {
        id: number;
        item: { id: number; sku: string; name: string };
        location: { id: number; code: string; name: string };
        quantity: number;
        updatedAt: string;
    };

    let { data } = $props();
    let inventory: InventoryItem[] = data.inventory;
</script>

<div class="p-6">
    <h1 class="text-2xl font-bold mb-6">Inventory</h1>

    {#if inventory.length === 0}
        <p class="text-gray-500">No inventory records found.</p>
    {:else}
        <div class="overflow-x-auto">
            <table class="w-full border-collapse text-sm">
                <thead>
                    <tr class="bg-gray-100 text-left">
                        <th class="p-3 border">SKU</th>
                        <th class="p-3 border">Item</th>
                        <th class="p-3 border">Location</th>
                        <th class="p-3 border">Quantity</th>
                        <th class="p-3 border">Last Updated</th>
                    </tr>
                </thead>
                <tbody>
                    {#each inventory as row}
                        <tr class="hover:bg-gray-50">
                            <td class="p-3 border font-mono">{row.item.sku}</td>
                            <td class="p-3 border">{row.item.name}</td>
                            <td class="p-3 border">{row.location.code}</td>
                            <td class="p-3 border">{row.quantity}</td>
                            <td class="p-3 border text-gray-400">
                                {new Date(row.updatedAt).toLocaleString()}
                            </td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>
    {/if}
</div>