﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace DesignPlatform.Core {
    public class UISwitch : MonoBehaviour {
        // Events
        public UnityEvent OnEvents;
        public UnityEvent OffEvents;

        // Saving
        //public bool saveValue = true;
        //public string switchTag = "Switch";

        // Settings
        public bool isOn = true;
        public bool invokeAtStart = true;

        Animator switchAnimator;
        Button switchButton;

        void Start() {
            try {
                switchAnimator = gameObject.GetComponent<Animator>();
                switchButton = gameObject.GetComponent<Button>();
                switchButton.onClick.AddListener(AnimateSwitch);
            }

            catch {
                Debug.LogError("Switch - Cannot initalize the switch due to missing variables.", this);
            }



            if (isOn == true) {
                switchAnimator.Play("Switch On");
                isOn = true;
            }

            else {
                switchAnimator.Play("Switch Off");
                isOn = false;
            }


            if (invokeAtStart == true && isOn == true)
                OnEvents.Invoke();
            if (invokeAtStart == true && isOn == false)
                OffEvents.Invoke();
        }

        void OnEnable() {
            if (switchAnimator == null)
                switchAnimator = gameObject.GetComponent<Animator>();


            if (isOn == true) {
                switchAnimator.Play("Switch On");
                isOn = true;
            }

            else {
                switchAnimator.Play("Switch Off");
                isOn = false;
            }

        }

        public void AnimateSwitch() {
            if (isOn == true) {
                switchAnimator.Play("Switch Off");
                isOn = false;
                OffEvents.Invoke();

            }

            else {
                switchAnimator.Play("Switch On");
                isOn = true;
                OnEvents.Invoke();
            }
        }
    }

}