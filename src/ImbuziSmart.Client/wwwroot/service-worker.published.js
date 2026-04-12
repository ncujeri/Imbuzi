// iMbuzi Smart — Production Service Worker
// Offline-first: serves index.html for all navigation requests so the Blazor
// SPA loads from cache even when the network is unavailable.

importScripts('./service-worker-assets.js');

const CACHE_PREFIX = 'imbuzi-smart-';
const CACHE_NAME   = `${CACHE_PREFIX}${self.assetsManifest.version}`;

// ── Lifecycle ────────────────────────────────────────────────────────────────

self.addEventListener('install',  event => { event.waitUntil(onInstall()); });
// skipWaiting is NOT called automatically — the app banner triggers it via
// a 'SKIP_WAITING' message so the user controls when the update applies.
self.addEventListener('message', event => {
    if (event.data?.type === 'SKIP_WAITING') self.skipWaiting();
});
self.addEventListener('activate', event => { event.waitUntil(onActivate()); });
self.addEventListener('fetch',    event => {
    if (event.request.method === 'GET')
        event.respondWith(onFetch(event.request));
});

// ── Install: cache every asset listed in the build manifest ──────────────────

async function onInstall() {
    const cache    = await caches.open(CACHE_NAME);
    const requests = self.assetsManifest.assets
        .filter(a => a.hash)
        .map(a => new Request(a.url, { integrity: a.hash, cache: 'no-cache' }));
    await cache.addAll(requests);
}

// ── Activate: remove stale caches from previous versions ────────────────────

async function onActivate() {
    const keys = await caches.keys();
    await Promise.all(
        keys
            .filter(k => k.startsWith(CACHE_PREFIX) && k !== CACHE_NAME)
            .map(k  => caches.delete(k))
    );
    await self.clients.claim();
}

// ── Fetch: three-path strategy ───────────────────────────────────────────────

async function onFetch(request) {
    const cache = await caches.open(CACHE_NAME);

    // 1. Navigation requests (typing URL, refresh, link clicks that load a page)
    //    Always serve index.html from cache so the Blazor SPA boots offline.
    if (request.mode === 'navigate') {
        const indexHtml = await cache.match('index.html');
        if (indexHtml) return indexHtml;
        // If cache is cold (very first install not complete), fall through to network.
    }

    // 2. API calls — network-first so live data is preferred; fall back to cache.
    const url = new URL(request.url);
    if (url.pathname.startsWith('/api/')) {
        try {
            const networkResponse = await fetch(request);
            // Optionally update cache with fresh response (uncomment if desired):
            // cache.put(request, networkResponse.clone());
            return networkResponse;
        } catch {
            const cached = await cache.match(request);
            return cached ?? new Response(JSON.stringify({ offline: true }), {
                status: 503,
                headers: { 'Content-Type': 'application/json' }
            });
        }
    }

    // 3. Everything else (static assets, framework files, images, etc.)
    //    Cache-first: if cached serve immediately; otherwise fetch and cache.
    const cached = await cache.match(request);
    if (cached) return cached;

    try {
        const networkResponse = await fetch(request);
        // Only cache same-origin successful responses to avoid caching CDN errors.
        if (networkResponse.ok && url.origin === self.location.origin) {
            cache.put(request, networkResponse.clone());
        }
        return networkResponse;
    } catch {
        // Return a 503 so the browser doesn't show the offline dinosaur page for
        // sub-resources — the Blazor app itself handles the degraded state.
        return new Response('', { status: 503, statusText: 'Offline' });
    }
}
