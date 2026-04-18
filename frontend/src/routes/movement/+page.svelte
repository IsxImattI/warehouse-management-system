<script lang="ts">
    import { post } from '$lib/api';

    type Item = { id: number; sku: string; name: string };
    type Location = { id: number; code: string; name: string };

    let { data } = $props();
    let items: Item[] = data.items;
    let locations: Location[] = data.locations;

    let itemId = $state('');
    let fromLocationId = $state('');
    let toLocationId = $state('');
    let quantity = $state('');
    let note = $state('');

    let success = $state('');
    let error = $state('');
    let loading = $state(false);

    async function submit() {
        if (!itemId || !quantity) {
            error = 'Item and quantity are required.';
            return;
        }
        if (!fromLocationId && !toLocationId) {
            error = 'At least one location (from or to) is required.';
            return;
        }

        error = '';
        success = '';
        loading = true;

        try {
            await post('/api/movement', {
                itemId: parseInt(itemId),
                fromLocationId: fromLocationId ? parseInt(fromLocationId) : null,
                toLocationId: toLocationId ? parseInt(toLocationId) : null,
                quantity: parseFloat(quantity),
                note: note || null
            });
            success = 'Movement executed successfully.';
            itemId = ''; fromLocationId = ''; toLocationId = ''; quantity = ''; note = '';
        } catch (e: any) {
            error = e.message;
        } finally {
            loading = false;
        }
    }
</script>

<div class="p-6 max-w-lg">
    <h1 class="text-2xl font-bold mb-6">Stock Movement</h1>

    {#if success}
        <p class="text-green-600 mb-4 border border-green-300 bg-green-50 p-3 rounded">{success}</p>
    {/if}
    {#if error}
        <p class="text-red-500 mb-4 border border-red-300 bg-red-50 p-3 rounded">{error}</p>
    {/if}

    <div class="flex flex-col gap-4">
        <div>
            <label class="block text-sm font-medium mb-1">Item *</label>
            <select bind:value={itemId} class="border w-full p-2 rounded">
                <option value="">Select item...</option>
                {#each items as item}
                    <option value={item.id}>{item.sku} — {item.name}</option>
                {/each}
            </select>
        </div>

        <div>
            <label class="block text-sm font-medium mb-1">From Location</label>
            <select bind:value={fromLocationId} class="border w-full p-2 rounded">
                <option value="">— Incoming stock (no source) —</option>
                {#each locations as loc}
                    <option value={loc.id}>{loc.code} — {loc.name}</option>
                {/each}
            </select>
        </div>

        <div>
            <label class="block text-sm font-medium mb-1">To Location</label>
            <select bind:value={toLocationId} class="border w-full p-2 rounded">
                <option value="">— Outgoing stock (no destination) —</option>
                {#each locations as loc}
                    <option value={loc.id}>{loc.code} — {loc.name}</option>
                {/each}
            </select>
        </div>

        <div>
            <label class="block text-sm font-medium mb-1">Quantity *</label>
            <input
                bind:value={quantity}
                type="number"
                min="0.001"
                step="0.001"
                class="border w-full p-2 rounded"
                placeholder="0.000"
            />
        </div>

        <div>
            <label class="block text-sm font-medium mb-1">Note</label>
            <input
                bind:value={note}
                class="border w-full p-2 rounded"
                placeholder="Optional"
            />
        </div>

        <button
            onclick={submit}
            disabled={loading}
            class="bg-black text-white px-4 py-2 rounded hover:bg-gray-800 disabled:opacity-50"
        >
            {loading ? 'Processing...' : 'Execute Movement'}
        </button>
    </div>
</div>