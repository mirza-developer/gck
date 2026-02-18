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
async function syncOfflineChanges() {
    try {
        // Get offline changes from IndexedDB
        const db = await openDatabase();
        const changes = await getAllChanges(db);
        
        if (changes.length === 0) {
            console.log('[ServiceWorker] No offline changes to sync');
            return;
        }

        console.log('[ServiceWorker] Syncing', changes.length, 'changes');
        
        // Send each change to the server
        for (const change of changes) {
            try {
                const response = await fetch(change.url, {
                    method: change.method,
                    headers: change.headers,
                    body: change.body
                });

                if (response.ok) {
                    // Remove synced change from IndexedDB
                    await deleteChange(db, change.id);
                    console.log('[ServiceWorker] Synced change', change.id);
                }
            } catch (error) {
                console.error('[ServiceWorker] Failed to sync change', change.id, error);
            }
        }
    } catch (error) {
        console.error('[ServiceWorker] Sync failed', error);
    }
}

// IndexedDB helper functions
function openDatabase() {
    return new Promise((resolve, reject) => {
        const request = indexedDB.open('GckOfflineDB', 1);
        
        request.onerror = () => reject(request.error);
        request.onsuccess = () => resolve(request.result);
        
        request.onupgradeneeded = (event) => {
            const db = event.target.result;
            if (!db.objectStoreNames.contains('offlineChanges')) {
                db.createObjectStore('offlineChanges', { keyPath: 'id', autoIncrement: true });
            }
        };
    });
}

function getAllChanges(db) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['offlineChanges'], 'readonly');
        const store = transaction.objectStore('offlineChanges');
        const request = store.getAll();
        
        request.onerror = () => reject(request.error);
        request.onsuccess = () => resolve(request.result);
    });
}

function deleteChange(db, id) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['offlineChanges'], 'readwrite');
        const store = transaction.objectStore('offlineChanges');
        const request = store.delete(id);
        
        request.onerror = () => reject(request.error);
        request.onsuccess = () => resolve();
    });
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
