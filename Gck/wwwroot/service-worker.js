// Service Worker for Gck Gaming Hub PWA
// Version: 1.0.0

const CACHE_NAME = 'gck-gaming-v1';
const DATA_CACHE_NAME = 'gck-data-v1';

// Files to cache for offline use
const FILES_TO_CACHE = [
    '/',
    '/index.html',
    '/css/app.css',
    '/css/genz-gaming.css',
    '/css/loyalty.css',
    '/js/app.js',
    '/manifest.json',
    '/icon-192x192.png',
    '/icon-512x512.png',
    '/favicon-16x16.png',
    '/favicon-32x32.png'
];

// Install event - cache static resources
self.addEventListener('install', (event) => {
    console.log('[ServiceWorker] Install');
    event.waitUntil(
        caches.open(CACHE_NAME).then((cache) => {
            console.log('[ServiceWorker] Pre-caching offline page');
            return cache.addAll(FILES_TO_CACHE);
        })
    );
    self.skipWaiting();
});

// Activate event - clean up old caches
self.addEventListener('activate', (event) => {
    console.log('[ServiceWorker] Activate');
    event.waitUntil(
        caches.keys().then((keyList) => {
            return Promise.all(keyList.map((key) => {
                if (key !== CACHE_NAME && key !== DATA_CACHE_NAME) {
                    console.log('[ServiceWorker] Removing old cache', key);
                    return caches.delete(key);
                }
            }));
        })
    );
    self.clients.claim();
});

// Fetch event - serve from cache, fallback to network
self.addEventListener('fetch', (event) => {
    // Skip non-GET requests
    if (event.request.method !== 'GET') return;

    // Handle API calls differently
    if (event.request.url.includes('/api/')) {
        event.respondWith(
            caches.open(DATA_CACHE_NAME).then((cache) => {
                return fetch(event.request)
                    .then((response) => {
                        // Clone the response and cache it
                        if (response.status === 200) {
                            cache.put(event.request.url, response.clone());
                        }
                        return response;
                    })
                    .catch(() => {
                        // If fetch fails, try to return cached data
                        return cache.match(event.request);
                    });
            })
        );
        return;
    }

    // Handle static assets
    event.respondWith(
        caches.match(event.request).then((response) => {
            if (response) {
                return response;
            }

            return fetch(event.request).then((response) => {
                // Check if valid response
                if (!response || response.status !== 200 || response.type !== 'basic') {
                    return response;
                }

                // Clone the response
                const responseToCache = response.clone();

                caches.open(CACHE_NAME).then((cache) => {
                    cache.put(event.request, responseToCache);
                });

                return response;
            });
        })
    );
});

// Background sync for offline changes
self.addEventListener('sync', (event) => {
    console.log('[ServiceWorker] Background sync', event.tag);
    
    if (event.tag === 'sync-offline-changes') {
        event.waitUntil(syncOfflineChanges());
    }
});

// Function to sync offline changes
// Note: Actual sync is handled by C# SyncService using LocalStorage
// This is a placeholder for future background sync enhancement
async function syncOfflineChanges() {
    console.log('[ServiceWorker] Background sync triggered - handled by C# SyncService');
    return Promise.resolve();
}

// Push notification handler
self.addEventListener('push', (event) => {
    const options = {
        body: event.data ? event.data.text() : 'رویداد جدید در گیم سنتر کوثر',
        icon: '/icon-192x192.png',
        badge: '/icon-192x192.png',
        vibrate: [100, 50, 100],
        data: {
            dateOfArrival: Date.now(),
            primaryKey: 1
        }
    };

    event.waitUntil(
        self.registration.showNotification('گیم سنتر کوثر', options)
    );
});

// Notification click handler
self.addEventListener('notificationclick', (event) => {
    event.notification.close();
    event.waitUntil(
        clients.openWindow('/')
    );
});
