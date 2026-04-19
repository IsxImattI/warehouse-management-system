<script lang="ts">
    import { onDestroy, tick } from 'svelte';
    import { Html5Qrcode } from 'html5-qrcode';
    import { get, post } from '$lib/api';

    type Location = { id: number; code: string; name: string; qrCode: string | null };
    type Item = { id: number; sku: string; ean: string | null; name: string };

    let { data } = $props();
    let locations: Location[] = data.locations;

    // Steps: 'scan_item' | 'create_item' | 'scan_location' | 'confirm'
    let step = $state<'scan_item' | 'create_item' | 'scan_location' | 'confirm'>('scan_item');

    let scanner: Html5Qrcode | null = null;
    let scannerActive = $state(false);

    let scannedItem = $state<Item | null>(null);
    let scannedLocation = $state<Location | null>(null);
    let quantity = $state('1');

    // New item form
    let newSku = $state('');
    let newEan = $state('');
    let newName = $state('');
    let newDescription = $state('');

    let error = $state('');
    let success = $state('');
    let loading = $state(false);

    async function startScanner() {
        scannerActive = true;
        error = '';
        await tick();

        try {
            scanner = new Html5Qrcode('qr-reader');
            await scanner.start(
                { facingMode: 'environment' },
                { fps: 10, qrbox: 250 },
                (decoded) => handleScan(decoded),
                () => {}
            );
        } catch (e: any) {
            error = `Camera error: ${e.message ?? e}`;
            scannerActive = false;
        }
    }

    async function stopScanner() {
        if (scanner) {
            await scanner.stop();
            scanner = null;
        }
        scannerActive = false;
    }

    async function handleScan(value: string) {
        await stopScanner();

        if (step === 'scan_item') {
            // Try to find by EAN or SKU
            try {
                const items = await get<Item[]>('/api/items');
                const match = items.find(i => i.ean === value || i.sku === value);
                if (match) {
                    scannedItem = match;
                    step = 'scan_location';
                } else {
                    // Item not found — go to create form
                    newEan = value;
                    newSku = '';
                    newName = '';
                    newDescription = '';
                    step = 'create_item';
                }
            } catch (e: any) {
                error = e.message;
            }
        } else if (step === 'scan_location') {
            const match = locations.find(l => l.qrCode === value || l.code === value);
            if (match) {
                scannedLocation = match;
                step = 'confirm';
            } else {
                error = `No location found for: ${value}`;
            }
        }
    }

    async function createItem() {
        if (!newSku || !newName) {
            error = 'SKU and Name are required.';
            return;
        }
        loading = true;
        error = '';
        try {
            const created = await post<Item>('/api/items', {
                sku: newSku,
                ean: newEan || null,
                name: newName,
                description: newDescription || null
            });
            scannedItem = created;
            step = 'scan_location';
        } catch (e: any) {
            error = e.message;
        } finally {
            loading = false;
        }
    }

    async function confirm() {
        if (!scannedItem || !scannedLocation || !quantity) return;
        loading = true;
        error = '';
        try {
            await post('/api/movement', {
                itemId: scannedItem.id,
                fromLocationId: null,
                toLocationId: scannedLocation.id,
                quantity: parseFloat(quantity),
                note: 'Received via scan'
            });
            success = `${scannedItem.name} (${parseFloat(quantity)} units) received at ${scannedLocation.code}.`;
            reset();
        } catch (e: any) {
            error = e.message;
        } finally {
            loading = false;
        }
    }

    function reset() {
        step = 'scan_item';
        scannedItem = null;
        scannedLocation = null;
        quantity = '1';
        error = '';
    }

    onDestroy(() => {
        if (scanner) scanner.stop();
    });
</script>

<div class="p-6 max-w-md mx-auto">
    <h1 class="text-2xl font-bold mb-2">Receive Stock</h1>

    <!-- Progress indicator -->
    <div class="flex gap-2 mb-6 text-xs">
        {#each ['scan_item', 'create_item', 'scan_location', 'confirm'] as s}
            <span class="px-2 py-1 rounded {step === s ? 'bg-black text-white' : 'bg-gray-100 text-gray-400'}">
                {s === 'scan_item' ? 'Scan Item' : s === 'create_item' ? 'New Item' : s === 'scan_location' ? 'Scan Location' : 'Confirm'}
            </span>
        {/each}
    </div>

    {#if success}
        <div class="border border-green-300 bg-green-50 p-4 rounded mb-4">
            <p class="text-green-700 font-medium">{success}</p>
            <button onclick={() => { success = ''; }} class="mt-2 text-sm underline text-green-700">
                Receive another
            </button>
        </div>
    {/if}

    {#if error}
        <p class="text-red-500 mb-4 border border-red-300 bg-red-50 p-3 rounded">{error}</p>
    {/if}

    <!-- QR Reader -->
    {#if scannerActive}
        <div class="mb-4">
            <div id="qr-reader" class="w-full rounded border"></div>
            <button onclick={stopScanner} class="mt-2 border px-4 py-2 rounded w-full hover:bg-gray-100">
                Cancel
            </button>
        </div>
    {/if}

    <!-- Step: Scan Item -->
    {#if step === 'scan_item' && !scannerActive}
        <div class="border rounded p-4">
            <p class="text-gray-600 mb-4">Scan the barcode or QR code on the item.</p>
            <button onclick={startScanner} class="bg-black text-white px-4 py-3 rounded w-full text-lg">
                Scan Item Barcode
            </button>
        </div>
    {/if}

    <!-- Step: Create Item -->
    {#if step === 'create_item'}
        <div class="border rounded p-4">
            <p class="text-gray-600 mb-4">Item not found. Fill in the details to create it.</p>
            <div class="flex flex-col gap-3">
                <div>
                    <label class="block text-sm mb-1" for="new-ean">EAN (scanned)</label>
                    <input id="new-ean" bind:value={newEan} class="border w-full p-2 rounded bg-gray-50" readonly />
                </div>
                <div>
                    <label class="block text-sm mb-1" for="new-sku">SKU *</label>
                    <input id="new-sku" bind:value={newSku} class="border w-full p-2 rounded" placeholder="SKU-001" />
                </div>
                <div>
                    <label class="block text-sm mb-1" for="new-name">Name *</label>
                    <input id="new-name" bind:value={newName} class="border w-full p-2 rounded" placeholder="Item name" />
                </div>
                <div>
                    <label class="block text-sm mb-1" for="new-desc">Description</label>
                    <input id="new-desc" bind:value={newDescription} class="border w-full p-2 rounded" placeholder="Optional" />
                </div>
                <button onclick={createItem} disabled={loading} class="bg-black text-white px-4 py-2 rounded hover:bg-gray-800 disabled:opacity-50">
                    {loading ? 'Creating...' : 'Create & Continue'}
                </button>
                <button onclick={reset} class="border px-4 py-2 rounded hover:bg-gray-100">
                    Cancel
                </button>
            </div>
        </div>
    {/if}

    <!-- Step: Scan Location -->
    {#if step === 'scan_location' && !scannerActive}
        <div class="border rounded p-4">
            <div class="mb-4 p-3 bg-gray-50 rounded">
                <p class="text-xs text-gray-500">Item</p>
                <p class="font-medium">{scannedItem?.name}</p>
                <p class="text-sm text-gray-400 font-mono">{scannedItem?.sku}</p>
            </div>
            <p class="text-gray-600 mb-4">Now scan the location QR code.</p>
            <button onclick={startScanner} class="bg-black text-white px-4 py-3 rounded w-full text-lg">
                Scan Location QR
            </button>
            <button onclick={reset} class="mt-2 border px-4 py-2 rounded w-full hover:bg-gray-100">
                Cancel
            </button>
        </div>
    {/if}

    <!-- Step: Confirm -->
    {#if step === 'confirm'}
        <div class="border rounded p-4">
            <h2 class="font-semibold mb-4">Confirm Receipt</h2>
            <div class="flex flex-col gap-2 mb-4">
                <div class="p-3 bg-gray-50 rounded">
                    <p class="text-xs text-gray-500">Item</p>
                    <p class="font-medium">{scannedItem?.name}</p>
                    <p class="text-sm text-gray-400 font-mono">{scannedItem?.sku}</p>
                </div>
                <div class="p-3 bg-gray-50 rounded">
                    <p class="text-xs text-gray-500">Location</p>
                    <p class="font-medium">{scannedLocation?.name}</p>
                    <p class="text-sm text-gray-400 font-mono">{scannedLocation?.code}</p>
                </div>
                <div>
                    <label class="block text-sm mb-1" for="qty">Quantity</label>
                    <input id="qty" bind:value={quantity} type="number" min="0.001" step="0.001" class="border w-full p-2 rounded text-lg" />
                </div>
            </div>
            <button onclick={confirm} disabled={loading} class="bg-black text-white px-4 py-3 rounded w-full text-lg disabled:opacity-50">
                {loading ? 'Processing...' : 'Confirm Receipt'}
            </button>
            <button onclick={reset} class="mt-2 border px-4 py-2 rounded w-full hover:bg-gray-100">
                Cancel
            </button>
        </div>
    {/if}
</div>