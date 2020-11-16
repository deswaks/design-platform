﻿using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.ModernUIPack {
    public class NotificationManager : MonoBehaviour {
        // CONTENT
        public Sprite icon;
        public string title = "Notification Title";
        [TextArea] public string description = "Notification description";
        public float timer = 3.0f;

        // Resources
        public Animator notificationAnimator;
        public Image iconObj;
        public TextMeshProUGUI titleObj;
        public TextMeshProUGUI descriptionObj;

        // Settings
        public bool enableTimer = true;
        public bool useCustomContent = false;
        public bool useStacking = false;
        public NotificationStyle notificationStyle;

        public enum NotificationStyle {
            FADING,
            POPUP,
            SLIDING
        }

        void Start() {
            try {
                if (notificationAnimator == null)
                    notificationAnimator = gameObject.GetComponent<Animator>();

                if (useCustomContent == false) {
                    iconObj.sprite = icon;
                    titleObj.text = title;
                    descriptionObj.text = description;
                }
            }

            catch {
                Debug.LogError("Notification - Cannot initalize the object due to missing components.", this);
            }

            if (useStacking == true) {
                try {
                    var stacking = (NotificationStacking)GameObject.FindObjectsOfType(typeof(NotificationStacking))[0];
                    stacking.notifications.Add(this);
                    stacking.enableUpdating = true;
                    gameObject.SetActive(false);
                }

                catch { }
            }
        }
        public void Initialize() {
            try {
                if (notificationAnimator == null)
                    notificationAnimator = gameObject.GetComponent<Animator>();

                if (useCustomContent == false) {
                    iconObj.sprite = icon;
                    titleObj.text = title;
                    descriptionObj.text = description;
                }
            }

            catch {
                Debug.LogError("Notification - Cannot initalize the object due to missing components.", this);
            }

            if (useStacking == true) {
                try {
                    var stacking = (NotificationStacking)GameObject.FindObjectsOfType(typeof(NotificationStacking))[0];
                    stacking.notifications.Add(this);
                    stacking.enableUpdating = true;
                    gameObject.SetActive(false);
                }

                catch { }
            }
        }

        IEnumerator StartTimer() {
            //Dette tidspunkt er lige når den dukker op
            yield return new WaitForSeconds(timer);
            notificationAnimator.Play("Out");
            StopCoroutine("StartTimer");
            //Dette tidspunkt er lige når den lukker igen
            Destroy(gameObject); // <- indsat af Niklas
        }

        public void OpenNotification() {
            notificationAnimator.Play("In");

            if (enableTimer == true)
                StartCoroutine("StartTimer");
        }

        public void OpenNotificationWithoutTimer() {
            notificationAnimator.Play("In");
        }

        public void CloseNotification() {
            notificationAnimator.Play("Out");

        }

        public void UpdateUI() {
            try {
                iconObj.sprite = icon;
                titleObj.text = title;
                descriptionObj.text = description;
            }

            catch {
                Debug.LogError("Notification - Cannot update the object due to missing components.", this);
            }
        }
    }

}