using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DesignPlatform.Core {
    public class ButtonManagerRoof : MonoBehaviour {

        Slider pitchSlider; 
        Slider overhangSlider;
        List<Button> buttons;

        void Start()
        {
            List<Slider> sliders = gameObject.GetComponentsInChildren<Slider>().ToList();
            buttons = gameObject.GetComponentsInChildren<RectTransform>().First(cm => cm.name == "RoofTypeButtons").GetComponentsInChildren<Button>().ToList();

            pitchSlider = sliders.First(s => s.gameObject.name.ToLower().Contains("pitch"));
            overhangSlider = sliders.First(s => s.gameObject.name.ToLower().Contains("overhang"));
        }

        public void ChangeRoofType(int i) {
            GameObject current = EventSystem.current.currentSelectedGameObject;
                
            current.GetComponentsInChildren<Image>().First(im => im.gameObject.name == "Icon").gameObject.transform.localScale = new Vector3(0, 0, 0);
            current.GetComponentsInChildren<Image>().First(im => im.gameObject.name == "Icon_sel").gameObject.transform.localScale = new Vector3(1,1,1);

            foreach(Button bt in buttons.Where(b => b.gameObject.name != current.name)) {
                bt.gameObject.GetComponentsInChildren<Image>().First(im => im.gameObject.name == "Icon_sel").gameObject.transform.localScale = new Vector3(0, 0, 0);
                bt.gameObject.GetComponentsInChildren<Image>().First(im => im.gameObject.name == "Icon").gameObject.transform.localScale = new Vector3(1, 1, 1);
            }


            RoofGenerator.Instance.roofType = (ProceduralToolkit.Buildings.RoofType) i;
        }
        public void SetRoofPitch() {
            RoofGenerator.Instance.RoofPitch = pitchSlider.value;
        }
        public void SetRoofOverhang() {
            RoofGenerator.Instance.overhang = overhangSlider.value/1000;
        }

    }
}