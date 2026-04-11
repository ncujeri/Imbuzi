// iMbuzi Smart — IndexedDB wrapper for offline-first data storage

const DB_NAME = 'ImbuziSmartDb';
const DB_VERSION = 2;

const STORES = {
    animals: { keyPath: 'id', indexes: ['tenantId', 'tag', 'status'] },
    matingRecords: { keyPath: 'id', indexes: ['tenantId', 'buckId', 'doeId'] },
    heatRecords: { keyPath: 'id', indexes: ['tenantId', 'animalId'] },
    medicalLogs: { keyPath: 'id', indexes: ['tenantId', 'animalId'] },
    weightRecords: { keyPath: 'id', indexes: ['tenantId', 'animalId'] },
    costEntries: { keyPath: 'id', indexes: ['tenantId', 'animalId', 'category'] },
    farmSettings: { keyPath: 'id', indexes: ['tenantId'] },
    syncQueue: { keyPath: 'id', indexes: ['entityType', 'action'] }
};

function openDb() {
    return new Promise((resolve, reject) => {
        const request = indexedDB.open(DB_NAME, DB_VERSION);

        request.onupgradeneeded = (event) => {
            const db = event.target.result;
            for (const [storeName, config] of Object.entries(STORES)) {
                if (!db.objectStoreNames.contains(storeName)) {
                    const store = db.createObjectStore(storeName, { keyPath: config.keyPath });
                    for (const index of config.indexes) {
                        store.createIndex(index, index, { unique: false });
                    }
                }
            }
        };

        request.onsuccess = (event) => resolve(event.target.result);
        request.onerror = (event) => reject(event.target.error);
    });
}

window.imbuziDb = {
    getItem: async (storeName, id) => {
        const db = await openDb();
        return new Promise((resolve, reject) => {
            const tx = db.transaction(storeName, 'readonly');
            const store = tx.objectStore(storeName);
            const request = store.get(id);
            request.onsuccess = () => resolve(request.result ? JSON.stringify(request.result) : null);
            request.onerror = () => reject(request.error);
        });
    },

    getAllItems: async (storeName) => {
        const db = await openDb();
        return new Promise((resolve, reject) => {
            const tx = db.transaction(storeName, 'readonly');
            const store = tx.objectStore(storeName);
            const request = store.getAll();
            request.onsuccess = () => resolve(JSON.stringify(request.result || []));
            request.onerror = () => reject(request.error);
        });
    },

    getByIndex: async (storeName, indexName, key) => {
        const db = await openDb();
        return new Promise((resolve, reject) => {
            const tx = db.transaction(storeName, 'readonly');
            const store = tx.objectStore(storeName);
            const index = store.index(indexName);
            const request = index.getAll(key);
            request.onsuccess = () => resolve(JSON.stringify(request.result || []));
            request.onerror = () => reject(request.error);
        });
    },

    putItem: async (storeName, jsonData) => {
        const db = await openDb();
        const item = typeof jsonData === 'string' ? JSON.parse(jsonData) : jsonData;
        return new Promise((resolve, reject) => {
            const tx = db.transaction(storeName, 'readwrite');
            const store = tx.objectStore(storeName);
            const request = store.put(item);
            request.onsuccess = () => resolve();
            request.onerror = () => reject(request.error);
        });
    },

    deleteItem: async (storeName, id) => {
        const db = await openDb();
        return new Promise((resolve, reject) => {
            const tx = db.transaction(storeName, 'readwrite');
            const store = tx.objectStore(storeName);
            const request = store.delete(id);
            request.onsuccess = () => resolve();
            request.onerror = () => reject(request.error);
        });
    },

    addToSyncQueue: async (entityType, entityId, action) => {
        const db = await openDb();
        const entry = {
            id: crypto.randomUUID(),
            entityType,
            entityId,
            action,
            timestamp: new Date().toISOString()
        };
        return new Promise((resolve, reject) => {
            const tx = db.transaction('syncQueue', 'readwrite');
            const store = tx.objectStore('syncQueue');
            const request = store.put(entry);
            request.onsuccess = () => resolve();
            request.onerror = () => reject(request.error);
        });
    },

    getSyncQueue: async () => {
        const db = await openDb();
        return new Promise((resolve, reject) => {
            const tx = db.transaction('syncQueue', 'readonly');
            const store = tx.objectStore('syncQueue');
            const request = store.getAll();
            request.onsuccess = () => resolve(JSON.stringify(request.result || []));
            request.onerror = () => reject(request.error);
        });
    },

    clearSyncQueue: async () => {
        const db = await openDb();
        return new Promise((resolve, reject) => {
            const tx = db.transaction('syncQueue', 'readwrite');
            const store = tx.objectStore('syncQueue');
            const request = store.clear();
            request.onsuccess = () => resolve();
            request.onerror = () => reject(request.error);
        });
    },

    isOnline: () => navigator.onLine
};
