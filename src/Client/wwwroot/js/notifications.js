window.imbuziNotifications = {
    requestPermission: async () => {
        if (!('Notification' in window)) {
            return 'denied';
        }
        return await Notification.requestPermission();
    },

    showNotification: async (title, body) => {
        if (!('Notification' in window) || Notification.permission !== 'granted') {
            return;
        }

        if ('serviceWorker' in navigator) {
            const registration = await navigator.serviceWorker.ready;
            await registration.showNotification(title, {
                body: body,
                icon: 'icon-192.png',
                badge: 'icon-192.png',
                vibrate: [200, 100, 200]
            });
        } else {
            new Notification(title, { body: body, icon: 'icon-192.png' });
        }
    }
};
