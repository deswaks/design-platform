using ProceduralToolkit.Buildings;
using System.Collections.Generic;
using UnityEngine;


namespace DesignPlatform.Core {

    /// <summary>
    /// Contains settings for the Building Elements.
    /// </summary>
    public static class Settings {

        // Logic settings
        /// <summary>The distance for which something is determined to be indentical</summary>
        public static readonly float nearThreshold = 0.001f;


        //Space settings
        /// <summary>The name given to each of the space functions.</summary>
        public static readonly Dictionary<SpaceFunction, string> SpaceTypeNames = new Dictionary<SpaceFunction, string> {
            { SpaceFunction.PREVIEW,  "Preview"},
            { SpaceFunction.DEFAULT,  ""},
            { SpaceFunction.SINGLEROOM,  "Single Bed\nRoom"},
            { SpaceFunction.DOUBLEROOM,  "Double Bed\nRoom"},
            { SpaceFunction.LIVINGROOM,  "Living\nRoom"},
            { SpaceFunction.KITCHEN,  "Kitchen"},
            { SpaceFunction.BATHROOM,  "Bathroom"},
            { SpaceFunction.CORRIDOR,  "Corridor"}
        };
        /// <summary>The name given to each of the space functions.</summary>
        public static readonly Dictionary<SpaceFunction, string> SpaceMaterialAssets = new Dictionary<SpaceFunction, string> {
            { SpaceFunction.PREVIEW,  "plan_space_default"},
            { SpaceFunction.DEFAULT,  "plan_space_default"},
            { SpaceFunction.SINGLEROOM,  "plan_space_singleroom"},
            { SpaceFunction.DOUBLEROOM,  "plan_space_doubleroom"},
            { SpaceFunction.LIVINGROOM,  "plan_space_livingroom"},
            { SpaceFunction.KITCHEN,  "plan_space_kitchen"},
            { SpaceFunction.BATHROOM,  "plan_space_bathroom"},
            { SpaceFunction.CORRIDOR,  "plan_space_corridor"}
        };
        /// <summary>The name given to each space game object.</summary>
        public static readonly Dictionary<SpaceShape, string> SpaceGameObjectNames = new Dictionary<SpaceShape, string> {
            { SpaceShape.RECTANGLE, "Space(Rectangle)"},
            { SpaceShape.LSHAPE,    "Space(L-Shape)"},
            { SpaceShape.USHAPE,    "Space(U-Shape)"},
            { SpaceShape.SSHAPE,    "Space(S-Shape)"},
            { SpaceShape.TSHAPE,    "Space(T-Shape)"}
        };
        /// <summary>The Controlpoints of each default shape</summary>
        public static readonly Dictionary<SpaceShape, List<Vector3>> SpaceControlPoints = new Dictionary<SpaceShape, List<Vector3>> {
            { SpaceShape.RECTANGLE, new List<Vector3> {
                new Vector3(0, 0, 0), new Vector3(0, 0, 3),
                new Vector3(3, 0, 3), new Vector3(3, 0, 0)} },
            { SpaceShape.LSHAPE,    new List<Vector3> {
                new Vector3(0, 0, 0), new Vector3(0, 0, 5),
                new Vector3(3, 0, 5), new Vector3(3, 0, 3),
                new Vector3(5, 0, 3), new Vector3(5, 0, 0) } },
            { SpaceShape.USHAPE,    new List<Vector3> {
                new Vector3(0, 0, 0), new Vector3(0, 0, 5),
                new Vector3(3, 0, 5), new Vector3(3, 0, 3),
                new Vector3(5, 0, 3), new Vector3(5, 0, 5),
                new Vector3(8, 0, 5), new Vector3(8, 0, 0) }},
            { SpaceShape.SSHAPE,    new List<Vector3> {
                new Vector3(0, 0, 0), new Vector3(0, 0, 5),
                new Vector3(3, 0, 5), new Vector3(3, 0, 3),
                new Vector3(6, 0, 3), new Vector3(6, 0, -2),
                new Vector3(3, 0, -2), new Vector3(3, 0, 0) }},
            { SpaceShape.TSHAPE,    new List<Vector3> {
                new Vector3(0, 0, 0), new Vector3(-2, 0, 0),
                new Vector3(-2, 0, 3), new Vector3(5, 0, 3),
                new Vector3(5, 0, 0), new Vector3(3, 0, 0),
                new Vector3(3, 0, -3), new Vector3(0, 0, -3) }}
        };



        // Wall settings
        /// <summary>Determines how the building should build the walls. They can be based upon interfaces or CLT elements.</summary>
        public static readonly WallSource WallSource = WallSource.CLT_ELEMENT;


        // Roof settings
        /// <summary>Thickness of the roof. Thickness is measured perpendicular to the surface of the roof.</summary>
        public static float RoofThickness = 0.2f;
        /// <summary>Pitch of the roof. Determines its slope.</summary>
        public static float RoofPitch = 15;
        /// <summary>Overhang of the roof. Determines the offset of the roof edge from the wall edges.</summary>
        public static float RoofOverhang = 0.15f;
        /// <summary>Type of the roof. Can be set to Flat, Hipped or Gabled.</summary>
        public static RoofType RoofType = RoofType.Gabled;



        // Opening settings
        /// <summary>The horizontal dimension of the window.</summary>
        public static float WindowWidth = 1.6f;
        /// <summary>The vertical dimension of the window.</summary>
        public static float WindowHeight = 1.05f;
        /// <summary>The height of the wall below the window.</summary>
        public static float WindowSillHeight = 1.1f;
        /// <summary>The thickness of the window.</summary>
        public static float WindowThickness = 0.051f;

        /// <summary>The horizontal dimension of the window.</summary>
        public static float DoorWidth = 0.9f;
        /// <summary>The vertical dimension of the door.</summary>
        public static float DoorHeight = 2.1f;
        /// <summary>The height of the wall below the door.</summary>
        public static float DoorSillHeight = 0.05f;
        /// <summary>The thickness of the door.</summary>
        public static float DoorThickness = 0.051f;

        /// <summary>Depth of the default opening 3D extrusion</summary>
        public static float OpeningDepth = 0.051f;



        // CLT element settings
        /// <summary>Visual Quality of the CLT element. Can be set to DVQ, IVQ or NVQ</summary>
        public static CLTQuality CLTQuality = CLTQuality.DVQ;
    }

}