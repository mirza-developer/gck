// Network Status Monitoring with API Health Check
let dotNetReference = null;
let apiUrl = '';
let checkInterval = null;
let isCurrentlyOnline = true;

export function initialize(dotNetRef, apiBaseUrl) {
    dotNetReference = dotNetRef;
    apiUrl = apiBaseUrl;
    
    // Listen for online/offline events as immediate indicators
    window.addEventListener('online', handleBrowserOnline);
    window.addEventListener('offline', handleBrowserOffline);
    
    // Start periodic API health check (every 30 seconds)
    startApiHealthCheck();
    
    console.log('[NetworkStatus] Initialized with API URL:', apiUrl);
}

function handleBrowserOnline() {
    console.log('[NetworkStatus] Browser online event');
    // Immediately check API accessibility
    checkApiHealth();
}

function handleBrowserOffline() {
    console.log('[NetworkStatus] Browser offline event');
    updateStatus(false);
}

function startApiHealthCheck() {
    // Initial check
    checkApiHealth();
    
    // Check every 30 seconds
    checkInterval = setInterval(() => {
        checkApiHealth();
    }, 30000);
}

async function checkApiHealth() {
    // First check if browser thinks we're online
    if (!navigator.onLine) {
        updateStatus(false);
        return;
    }
    
    try {
        // Try to reach the API with a simple HEAD request to avoid CORS issues
        // Add a cache-busting query parameter to prevent cached responses
        const controller = new AbortController();
        const timeoutId = setTimeout(() => controller.abort(), 5000); // 5 second timeout
        
        const response = await fetch(`${apiUrl}/health?t=${Date.now()}`, {
            method: 'HEAD',
            mode: 'no-cors', // no-cors mode to avoid CORS preflight
            cache: 'no-cache',
            signal: controller.signal
        });
        
        clearTimeout(timeoutId);
        
        // In no-cors mode, we can't read the response, but if fetch succeeds, API is reachable
        updateStatus(true);
        console.log('[NetworkStatus] API health check: Online');
    } catch (error) {
        console.log('[NetworkStatus] API health check failed:', error.message);
        updateStatus(false);
    }
}

function updateStatus(isOnline) {
    if (isCurrentlyOnline !== isOnline) {
        isCurrentlyOnline = isOnline;
        console.log('[NetworkStatus] Status changed to:', isOnline ? 'Online' : 'Offline');
        if (dotNetReference) {
            dotNetReference.invokeMethodAsync('UpdateNetworkStatus', isOnline);
        }
    }
}

export function isOnline() {
    return isCurrentlyOnline;
}

export function checkNow() {
    checkApiHealth();
}

export function dispose() {
    window.removeEventListener('online', handleBrowserOnline);
    window.removeEventListener('offline', handleBrowserOffline);
    
    if (checkInterval) {
        clearInterval(checkInterval);
        checkInterval = null;
    }
    
    dotNetReference = null;
}
