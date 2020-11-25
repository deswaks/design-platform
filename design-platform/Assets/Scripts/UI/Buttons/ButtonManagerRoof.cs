using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DesignPlatform.Core {
    public class ButtonManagerRoof : MonoBehaviour {

        Slider pitchSlider; 
        Slider overhangSlider; 

        void Start()
        {
            List<Slider> sliders = gameObject.GetComponentsInChildren<Slider>().ToList();
            pitchSlider = sliders.First(s => s.gameObject.name.ToLower().Contains("pitch"));
            overhangSlider = sliders.First(s => s.gameObject.name.ToLower().Contains("overhang"));
        }


        public void ChangeRoofType(int i) {
            RoofGenerator.Instance.roofType = (ProceduralToolkit.Buildings.RoofType) i;
        }
        public void SetRoofPitch() {
            RoofGenerator.Instance.RoofPitch = pitchSlider.value;
        }
        public void SetRoofOverhang() {
            RoofGenerator.Instance.overhang = overhangSlider.value;
        }
    }
}