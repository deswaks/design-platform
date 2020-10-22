using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class NotificationHandler
{
    public static GameObject GenerateNotification(string text, string title, Vector3 location, GameObject parent, float timer = 0) {
        Object notificationPrefab = AssetDatabase.LoadAssetAtPath < GameObject >("Assets/Prefabs/notification.prefab");

        GameObject notificationObject = (GameObject)GameObject.Instantiate(notificationPrefab, parent.transform);

        notificationObject.transform.localPosition = location;// Vector3.zero;

        NotificationManager notificationManager = notificationObject.GetComponent<NotificationManager>();
        notificationManager.Initialize();

        notificationManager.description = text;
        notificationManager.title = title;

        
        Debug.Log(notificationObject.transform.localPosition);
        Debug.Log(notificationObject.transform.position);

        if(timer == 0) {
            notificationManager.OpenNotificationWithoutTimer();
        }
        else {
            notificationManager.timer = timer;
            notificationManager.OpenNotification();
        }
        
        return notificationObject;
    }

    public static void DestroyNotification(GameObject notificationGameObject) {
        SceneAsset.Destroy(notificationGameObject);
    }

}
