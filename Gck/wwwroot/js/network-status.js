// Network Status Monitoring
let dotNetReference = null;

export function initialize(dotNetRef) {
    dotNetReference = dotNetRef;
    
    // Listen for online/offline events
    window.addEventListener('online', handleOnline);
    window.addEventListener('offline', handleOffline);
    
    console.log('[NetworkStatus] Initialized');
}

function handleOnline() {
    console.log('[NetworkStatus] Online');
    if (dotNetReference) {
        dotNetReference.invokeMethodAsync('UpdateNetworkStatus', true);
    }
}

function handleOffline() {
    console.log('[NetworkStatus] Offline');
    if (dotNetReference) {
        dotNetReference.invokeMethodAsync('UpdateNetworkStatus', false);
    }
}

export function isOnline() {
    return navigator.onLine;
}

export function dispose() {
    window.removeEventListener('online', handleOnline);
    window.removeEventListener('offline', handleOffline);
    dotNetReference = null;
}
