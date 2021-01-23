using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DesignPlatform.UI {

    /// <summary>
    /// Contains functions for manipulating the roof in the Roof UI menu
    /// </summary>
    public class ButtonManagerRoof : MonoBehaviour {

        Slider pitchSlider;
        Slider overhangSlider;
        List<Button> buttons;

        void Start() {
            List<Slider> sliders = gameObject.GetComponentsInChildren<Slider>().ToList();
            buttons = gameObject.GetComponentsInChildren<RectTransform>().First(cm => cm.name == "RoofTypeButtons").GetComponentsInChildren<Button>().ToList();

            pitchSlider = sliders.First(s => s.gameObject.name.ToLower().Contains("pitch"));
            overhangSlider = sliders.First(s => s.gameObject.name.ToLower().Contains("overhang"));
        }
        /// <summary>
        /// Changes current roof type (flat/hipped/gabled)
        /// </summary>
        /// <param name="i"></param>
        public void ChangeRoofType(int i) {
            GameObject current = EventSystem.current.currentSelectedGameObject;

            current.GetComponentsInChildren<Image>().First(im => im.gameObject.name == "Icon").gameObject.transform.localScale = new Vector3(0, 0, 0);
            current.GetComponentsInChildren<Image>().First(im => im.gameObject.name == "Icon_sel").gameObject.transform.localScale = new Vector3(1, 1, 1);

            foreach (Button bt in buttons.Where(b => b.gameObject.name != current.name)) {
                bt.gameObject.GetComponentsInChildren<Image>().First(im => im.gameObject.name == "Icon_sel").gameObject.transform.localScale = new Vector3(0, 0, 0);
                bt.gameObject.GetComponentsInChildren<Image>().First(im => im.gameObject.name == "Icon").gameObject.transform.localScale = new Vector3(1, 1, 1);
            }


            Core.Settings.RoofType = (ProceduralToolkit.Buildings.RoofType)i;
        }
        /// <summary>
        /// Sets roof pitch according to current slider value.
        /// </summary>
        public void SetRoofPitch() {
            Core.Settings.RoofPitch = pitchSlider.value;
        }

        /// <summary>
        /// Sets roof overhang according to current slider value.
        /// </summary>
        public void SetRoofOverhang() {
            Core.Settings.RoofOverhang = overhangSlider.value / 1000;
        }

    }
}