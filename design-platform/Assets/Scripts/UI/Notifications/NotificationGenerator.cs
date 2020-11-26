using DesignPlatform.Utils;
using Michsky.UI.ModernUIPack;
using UnityEngine;

namespace DesignPlatform.UI {
    public static class NotificationHandler {
        public static GameObject GenerateNotification(string text, string title, Vector3 location, GameObject parent, float timer = 0) {
            Object notificationPrefab = AssetUtil.LoadAsset<GameObject>("prefabs", "notification");
            GameObject notificationObject = (GameObject)Object.Instantiate(notificationPrefab, parent.transform);

            notificationObject.transform.localPosition = location;// Vector3.zero;

            NotificationManager notificationManager = notificationObject.GetComponent<NotificationManager>();
            notificationManager.Initialize();

            notificationManager.description = text;
            notificationManager.title = title;

            if (timer == 0) {
                notificationManager.OpenNotificationWithoutTimer();
            }
            else {
                notificationManager.timer = timer;
                notificationManager.OpenNotification();
            }

            return notificationObject;
        }

        public static void DestroyNotification(GameObject notificationGameObject) {
            Object.Destroy(notificationGameObject);
        }

    }
}