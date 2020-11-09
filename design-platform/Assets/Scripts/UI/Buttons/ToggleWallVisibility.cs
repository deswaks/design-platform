using UnityEngine;
using UnityEngine.UI;

namespace DesignPlatform.Core {

    public class ToggleWallVisibility : MonoBehaviour {
        Toggle m_Toggle;

        void Start() {
            //Fetch the Toggle GameObject
            m_Toggle = GetComponent<Toggle>();
            //Add listener for when the state of the Toggle changes, to take action
            m_Toggle.onValueChanged.AddListener(delegate {
                ToggleValueChanged(m_Toggle);
            });
        }

        // Toggle wall visibility
        void ToggleValueChanged(Toggle change) {
            GlobalSettings.ShowWallLines = GetComponent<Toggle>().isOn;
            foreach (Room room in Building.Instance.rooms) {
                room.UpdateRender2D();
            }
        }
    }
}