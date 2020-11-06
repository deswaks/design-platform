using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.Csg;
using UnityEngine.ProBuilder.MeshOperations;
using System.Linq;
using DesignPlatform.Utils;

namespace DesignPlatform.Core {
    public class Opening : MonoBehaviour {
        //UnityEngine.ProBuilder.Csg.Model result;

        public Face parentFace { get; private set; }

        public float WindowWidth = 1.6f;
        public float WindowHeight = 1.2f;
        public float WindowSillHeight = 1.1f;
        public float DoorWidth = 0.8f;
        public float DoorHeight = 2.0f;
        public float OpeningDepth = 0.2f;

        public Material previewMaterial;
        public Material windowMaterial;
        public Material doorMaterial;
        //public Vector3 OpeningOrigin;

        public List<Opening> openings { get; private set; }
        public Face[] attachedFaces = new Face[2];
        private OpeningShape shape;
        private Opening prefabOpening;
        private List<Vector3> controlPoints;

        public enum OpeningStates {
            PLACED,
            PREVIEW
        }
        private OpeningStates openingState;

        // Generate preview-opening-object (2D-mode)
        // Find nearest wall 
        // Allign w. wall
        // Check if wall is wide enough
        // Place the opening min. length from wall-endpoints
        // Preview opening object in 2D mode

        public void InitializeOpening(Face[] parentFaces = null,
                                      OpeningShape openingShape = OpeningShape.WINDOW) {

            gameObject.layer = 15; // Opening layer

            shape = openingShape;
            attachedFaces = parentFaces;

            openingState = OpeningStates.PREVIEW;

            GameObject prefabObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/OpeningPrefab.prefab");

            prefabOpening = (Opening)prefabObject.GetComponent(typeof(Opening));
            Material material = prefabOpening.previewMaterial;

            switch (shape) {
                case OpeningShape.WINDOW:
                    controlPoints = new List<Vector3> {
                    -Vector3.right*WindowWidth/2 + Vector3.forward*OpeningDepth/2 + Vector3.up*WindowSillHeight,
                    -Vector3.right*WindowWidth/2 + Vector3.forward*OpeningDepth/2 + Vector3.up*WindowSillHeight + Vector3.up*WindowHeight,
                    Vector3.right*WindowWidth/2 + Vector3.forward*OpeningDepth/2 + Vector3.up*WindowSillHeight + Vector3.up*WindowHeight,
                    Vector3.right*WindowWidth/2 + Vector3.forward*OpeningDepth/2 + Vector3.up*WindowSillHeight
                };
                    material = prefabOpening.windowMaterial;
                    gameObject.name = "Window";


                    break;
                case OpeningShape.DOOR:
                    controlPoints = new List<Vector3> {
                    -Vector3.right*DoorWidth/2 + Vector3.forward*OpeningDepth/2,
                    -Vector3.right*DoorWidth/2 + Vector3.forward*OpeningDepth/2 + Vector3.up*DoorHeight,
                    Vector3.right*DoorWidth/2 + Vector3.forward*OpeningDepth/2 + Vector3.up*DoorHeight,
                    Vector3.right*DoorWidth/2 + Vector3.forward*OpeningDepth/2
                };
                    material = prefabOpening.doorMaterial;
                    gameObject.name = "Door";
                    break;
            }

            gameObject.AddComponent<MeshCollider>();
            gameObject.AddComponent<PolyShape>();
            gameObject.AddComponent<ProBuilderMesh>();

            PolyShape polyshape = gameObject.GetComponent<PolyShape>();
            polyshape.SetControlPoints(controlPoints);
            polyshape.extrude = OpeningDepth;
            polyshape.CreateShapeFromPolygon();

            gameObject.GetComponent<ProBuilderMesh>().Refresh();
            gameObject.GetComponent<MeshRenderer>().material = material;
        }

        /// <summary>
        /// Deletes the opening
        /// </summary>
        public void Delete() {
            if (Building.Instance.openings.Contains(this)) {
                Building.Instance.RemoveOpening(this);
            }
            Destroy(gameObject);
        }
        /// <summary>
        /// Moves the opening to the given position
        /// </summary>
        public void Move(Vector3 exactPosition) {
            Vector3 gridPosition = Grid.GetNearestGridpoint(exactPosition);
            gameObject.transform.position = gridPosition;
        }
        public void SubMove(Vector3 exactPosition) {
            Vector3 gridPosition = Grid.GetNearestSubGridpoint(exactPosition);
            gameObject.transform.position = gridPosition;
        }
        public void Rotate(Face closestFace) {
            Vector3 faceNormal = closestFace.parentRoom.GetWallNormals()[closestFace.faceIndex];
            Vector3 OpeningNormal = gameObject.transform.forward;
            float angle = Vector3.Angle(OpeningNormal, faceNormal);
            if (angle > 0f) {
                Vector3 centerPoint = gameObject.transform.TransformPoint(Vector3.zero);
                gameObject.transform.RotateAround(centerPoint, Vector3.up, angle);
            }

        }

        public void SetOpeningState(OpeningStates openingState) {
            this.openingState = openingState;
        }

        //public void EctractIntersection() {
        //    GameObject subtractWithGO = this.gameObject;
        //    GameObject subtractFromhGO = this.GetCoincidentInterface().wall.gameObject;
        //    // Returns a mesh
        //    // Træk Opening fra dens wall
        //    UnityEngine.ProBuilder.Csg.Model result;
        //    result = UnityEngine.ProBuilder.Csg.Boolean.Subtract(subtractFromhGO, subtractWithGO);
        //    ProBuilderMesh pb = ProBuilderMesh.Create();
        //    pb.GetComponent<MeshFilter>().sharedMesh = (Mesh)result;
        //    var materials = result.materials.ToArray();
        //    pb.GetComponent<MeshRenderer>().sharedMaterials = materials;
        //    MeshImporter importer = new MeshImporter(pb.gameObject);
        //    importer.Import(new MeshImportSettings() { quads = true, smoothing = true, smoothingAngle = 1f });
        //    pb.ToMesh();
        //    pb.Refresh();
        //    pb.CenterPivot(null);
        //}

        // DeleteOpening

        //Building class
        // Get opening

        // Get all openings

        // Remove opening

        // Delete all openings

        public Vector3 ClosestPoint(Vector3 mousePos, Face closestFace) {
            (Vector3 vA, Vector3 vB) = closestFace.Get2DEndPoints();
            Vector3 closestPoint = VectorFunctions.LineClosestPoint(vA, vB, mousePos);

            return closestPoint;
        }

        public Interface GetCoincidentInterface() {
            Vector3 openingPoint = gameObject.transform.position;
            float parameterOnFace = attachedFaces[0].GetPointParameter(openingPoint);

            return attachedFaces[0].GetInterfaceAtParameter(parameterOnFace);
        }
        /// <summary>
        /// Gets a list of controlpoints - in local coordinates. The controlpoints are the vertices of the underlying polyshape of the opening.
        /// </summary>
        public List<Vector3> GetControlPoints(bool localCoordinates = false, bool closed = false) {
            List<Vector3> returnPoints = controlPoints;
            if (closed) {
                returnPoints = controlPoints.Concat(new List<Vector3> { controlPoints[0] }).ToList();
            }
            if (!localCoordinates) {
                returnPoints = returnPoints.Select(p => gameObject.transform.TransformPoint(p)).ToList();
            }
            return returnPoints;
        }


    }
}
