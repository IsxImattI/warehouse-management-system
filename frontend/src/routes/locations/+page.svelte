<script lang="ts">
    import { invalidateAll } from '$app/navigation';
    import { post, put, del } from '$lib/api';

    type Location = {
        id: number;
        code: string;
        name: string;
        qrCode: string | null;
        createdAt: string;
    };

    let { data } = $props();
    let locations: Location[] = $state(data.locations);

    let showForm = $state(false);
    let editingLocation = $state<Location | null>(null);

    let code = $state('');
    let name = $state('');
    let qrCode = $state('');
    let error = $state('');

    function openCreate() {
        editingLocation = null;
        code = ''; name = ''; qrCode = '';
        error = '';
        showForm = true;
    }

    function openEdit(location: Location) {
        editingLocation = location;
        code = location.code;
        name = location.name;
        qrCode = location.qrCode ?? '';
        error = '';
        showForm = true;
    }

    function closeForm() {
        showForm = false;
        editingLocation = null;
    }

    async function save() {
        try {
            const body = { code, name, qrCode: qrCode || null };
            if (editingLocation) {
                await put(`/api/locations/${editingLocation.id}`, body);
            } else {
                await post('/api/locations', body);
            }
            await invalidateAll();
            locations = data.locations;
            closeForm();
        } catch (e: any) {
            error = e.message;
        }
    }

    async function remove(id: number) {
        if (!confirm('Delete this location?')) return;
        try {
            await del(`/api/locations/${id}`);
            await invalidateAll();
            locations = data.locations;
        } catch (e: any) {
            error = e.message;
        }
    }
</script>

<div class="p-6">
    <div class="flex justify-between items-center mb-6">
        <h1 class="text-2xl font-bold">Locations</h1>
        <button
            onclick={openCreate}
            class="bg-black text-white px-4 py-2 rounded hover:bg-gray-800"
        >
            Add Location
        </button>
    </div>

    {#if error}
        <p class="text-red-500 mb-4">{error}</p>
    {/if}

    {#if showForm}
        <div class="border rounded p-4 mb-6 bg-gray-50">
            <h2 class="font-semibold mb-4">{editingLocation ? 'Edit Location' : 'New Location'}</h2>
            <div class="grid grid-cols-2 gap-4">
                <div>
                    <label class="block text-sm mb-1">Code *</label>
                    <input bind:value={code} class="border w-full p-2 rounded" placeholder="SHELF-A1" />
                </div>
                <div>
                    <label class="block text-sm mb-1">Name *</label>
                    <input bind:value={name} class="border w-full p-2 rounded" placeholder="Shelf A1" />
                </div>
                <div class="col-span-2">
                    <label class="block text-sm mb-1">QR Code value</label>
                    <input bind:value={qrCode} class="border w-full p-2 rounded" placeholder="Optional" />
                </div>
            </div>
            <div class="flex gap-2 mt-4">
                <button onclick={save} class="bg-black text-white px-4 py-2 rounded hover:bg-gray-800">
                    {editingLocation ? 'Update' : 'Create'}
                </button>
                <button onclick={closeForm} class="border px-4 py-2 rounded hover:bg-gray-100">
                    Cancel
                </button>
            </div>
        </div>
    {/if}

    {#if locations.length === 0}
        <p class="text-gray-500">No locations found.</p>
    {:else}
        <table class="w-full border-collapse text-sm">
            <thead>
                <tr class="bg-gray-100 text-left">
                    <th class="p-3 border">Code</th>
                    <th class="p-3 border">Name</th>
                    <th class="p-3 border">QR Code</th>
                    <th class="p-3 border">Actions</th>
                </tr>
            </thead>
            <tbody>
                {#each locations as location}
                    <tr class="hover:bg-gray-50">
                        <td class="p-3 border font-mono">{location.code}</td>
                        <td class="p-3 border">{location.name}</td>
                        <td class="p-3 border text-gray-400">{location.qrCode ?? '—'}</td>
                        <td class="p-3 border">
                            <div class="flex gap-2">
                                <button
                                    onclick={() => openEdit(location)}
                                    class="text-blue-600 hover:underline text-sm"
                                >
                                    Edit
                                </button>
                                <button
                                    onclick={() => remove(location.id)}
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