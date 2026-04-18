<script lang="ts">
    import { invalidateAll } from '$app/navigation';
    import { post, put, del } from '$lib/api';

    type Item = {
        id: number;
        sku: string;
        ean: string | null;
        name: string;
        description: string | null;
        createdAt: string;
    };

    let { data } = $props();
    let items: Item[] = $state(data.items);

    let showForm = $state(false);
    let editingItem = $state<Item | null>(null);

    let sku = $state('');
    let ean = $state('');
    let name = $state('');
    let description = $state('');
    let error = $state('');

    function openCreate() {
        editingItem = null;
        sku = ''; ean = ''; name = ''; description = '';
        error = '';
        showForm = true;
    }

    function openEdit(item: Item) {
        editingItem = item;
        sku = item.sku;
        ean = item.ean ?? '';
        name = item.name;
        description = item.description ?? '';
        error = '';
        showForm = true;
    }

    function closeForm() {
        showForm = false;
        editingItem = null;
    }

    async function save() {
        try {
            const body = { sku, ean: ean || null, name, description: description || null };
            if (editingItem) {
                await put(`/api/items/${editingItem.id}`, body);
            } else {
                await post('/api/items', body);
            }
            await invalidateAll();
            items = data.items;
            closeForm();
        } catch (e: any) {
            error = e.message;
        }
    }

    async function remove(id: number) {
        if (!confirm('Delete this item?')) return;
        try {
            await del(`/api/items/${id}`);
            await invalidateAll();
            items = data.items;
        } catch (e: any) {
            error = e.message;
        }
    }
</script>

<div class="p-6">
    <div class="flex justify-between items-center mb-6">
        <h1 class="text-2xl font-bold">Items</h1>
        <button
            onclick={openCreate}
            class="bg-black text-white px-4 py-2 rounded hover:bg-gray-800"
        >
            Add Item
        </button>
    </div>

    {#if error}
        <p class="text-red-500 mb-4">{error}</p>
    {/if}

    {#if showForm}
        <div class="border rounded p-4 mb-6 bg-gray-50">
            <h2 class="font-semibold mb-4">{editingItem ? 'Edit Item' : 'New Item'}</h2>
            <div class="grid grid-cols-2 gap-4">
                <div>
                    <label class="block text-sm mb-1">SKU *</label>
                    <input bind:value={sku} class="border w-full p-2 rounded" placeholder="SKU-001" />
                </div>
                <div>
                    <label class="block text-sm mb-1">EAN</label>
                    <input bind:value={ean} class="border w-full p-2 rounded" placeholder="1234567890123" />
                </div>
                <div>
                    <label class="block text-sm mb-1">Name *</label>
                    <input bind:value={name} class="border w-full p-2 rounded" placeholder="Item name" />
                </div>
                <div>
                    <label class="block text-sm mb-1">Description</label>
                    <input bind:value={description} class="border w-full p-2 rounded" placeholder="Optional" />
                </div>
            </div>
            <div class="flex gap-2 mt-4">
                <button onclick={save} class="bg-black text-white px-4 py-2 rounded hover:bg-gray-800">
                    {editingItem ? 'Update' : 'Create'}
                </button>
                <button onclick={closeForm} class="border px-4 py-2 rounded hover:bg-gray-100">
                    Cancel
                </button>
            </div>
        </div>
    {/if}

    {#if items.length === 0}
        <p class="text-gray-500">No items found.</p>
    {:else}
        <table class="w-full border-collapse text-sm">
            <thead>
                <tr class="bg-gray-100 text-left">
                    <th class="p-3 border">SKU</th>
                    <th class="p-3 border">EAN</th>
                    <th class="p-3 border">Name</th>
                    <th class="p-3 border">Description</th>
                    <th class="p-3 border">Actions</th>
                </tr>
            </thead>
            <tbody>
                {#each items as item}
                    <tr class="hover:bg-gray-50">
                        <td class="p-3 border font-mono">{item.sku}</td>
                        <td class="p-3 border text-gray-400">{item.ean ?? '—'}</td>
                        <td class="p-3 border">{item.name}</td>
                        <td class="p-3 border text-gray-400">{item.description ?? '—'}</td>
                        <td class="p-3 border">
                            <div class="flex gap-2">
                                <button
                                    onclick={() => openEdit(item)}
                                    class="text-blue-600 hover:underline text-sm"
                                >
                                    Edit
                                </button>
                                <button
                                    onclick={() => remove(item.id)}
                                    class="text-red-600 hover:underline text-sm"
                                >
                                    Delete
                                </button>
                            </div>
                        </td>
                    </tr>
                {/each}
            </tbody>
        </table>
    {/if}
</div>