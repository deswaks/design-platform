using DesignPlatform.Geometry;
using DesignPlatform.Utils;
using Michsky.UI.ModernUIPack;
using UnityEngine;

namespace DesignPlatform.UI {

    /// <summary>
    /// Handles notifications (pop-up messages)
    /// </summary>
    public static class NotificationHandler {
        /// <summary>
        /// Generates a notification (pop-up message) with desired content at specified location for a specified duration.
        /// </summary>
        /// <param name="text">Message to be shown.</param>
        /// <param name="title">Title of message shown.</param>
        /// <param name="location">Transform coordinates (XYZ) of notification box.</param>
        /// <param name="parent">Parent GameObject of notification box.</param>
        /// <param name="timer">Duration (in seconds) to show message. 0 will show notification indefinitely.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Destroys notification object.
        /// </summary>
        /// <param name="notificationGameObject">Object to destroy.</param>
        public static void DestroyNotification(GameObject notificationGameObject) {
            Object.Destroy(notificationGameObject);
        }

    }
}