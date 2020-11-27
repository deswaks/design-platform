using ProceduralToolkit.Buildings;

namespace DesignPlatform.Core {

    /// <summary>
    /// Contains settings for the Building Elements.
    /// </summary>
    public static class Settings {

        public static float RoofThickness = 0.2f;
        public static float RoofPitch = 15;
        public static float RoofOverhang = 0.15f;
        public static RoofType RoofType = RoofType.Gabled;

        public static float WindowWidth = 1.6f;
        public static float WindowHeight = 1.05f;
        public static float WindowSillHeight = 1.1f;
        public static float WindowThickness = 0.051f;

        public static float DoorWidth = 0.9f;
        public static float DoorHeight = 2.1f;
        public static float DoorSillHeight = 0.05f;
        public static float DoorThickness = 0.051f;

        public static float OpeningDepth = 0.051f;

    }
}