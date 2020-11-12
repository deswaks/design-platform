﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.Csg;
using UnityEngine.ProBuilder.MeshOperations;
using System.Linq;
using DesignPlatform.Utils;

namespace DesignPlatform.Core {

    public enum OpeningShape {
        DOOR,
        WINDOW
    }

    public class Opening : MonoBehaviour {

        public Face parentFace { get; private set; }

        public float WindowWidth = 1.6f;
        public float WindowHeight = 1.2f;

        public float DoorWidth = 1f;
        public float DoorHeight = 2.4f;

        public float OpeningDepth = 0.2f;
        public float SillHeight;
        public float Width;
        public float Height;

        public Material previewMaterial;
        public Material windowMaterial;
        public Material doorMaterial;

        public List<Opening> openings { get; private set; }
        public Face[] attachedFaces = new Face[2];
        private OpeningShape shape;
        private Opening prefabOpening;
        public List<Vector3> controlPoints;

        public enum OpeningStates {
            PLACED,
            PREVIEW
        }
        private OpeningStates openingState;

        public void InitializeOpening(Face[] parentFaces = null,
                                      OpeningShape openingShape = OpeningShape.WINDOW) {

            gameObject.layer = 15; // Opening layer

            shape = openingShape;
            attachedFaces = parentFaces;

            openingState = OpeningStates.PREVIEW;

            GameObject prefabObject = AssetUtil.LoadAsset<GameObject>("prefabs", "OpeningPrefab");

            prefabOpening = (Opening)prefabObject.GetComponent(typeof(Opening));
            Material material = prefabOpening.previewMaterial;

            switch (shape) {
                case OpeningShape.WINDOW:
                    Width = WindowWidth;
                    Height = WindowHeight;
                    SillHeight = 1.1f;
                    controlPoints = new List<Vector3> {
                    -Vector3.right*Width/2 + Vector3.forward*OpeningDepth/2 + Vector3.up*SillHeight,
                    -Vector3.right*Width/2 + Vector3.forward*OpeningDepth/2 + Vector3.up*SillHeight + Vector3.up*Height,
                    Vector3.right*Width/2 + Vector3.forward*OpeningDepth/2 + Vector3.up*SillHeight + Vector3.up*Height,
                    Vector3.right*Width/2 + Vector3.forward*OpeningDepth/2 + Vector3.up*SillHeight };
                    material = prefabOpening.windowMaterial;
                    gameObject.name = "Window";


                    break;
                case OpeningShape.DOOR:
                    Width = DoorWidth;
                    Height = DoorHeight;
                    SillHeight = OpeningDepth/2;
                    controlPoints = new List<Vector3> {
                    -Vector3.right*Width/2 + Vector3.forward*OpeningDepth/2 + Vector3.up*SillHeight,
                    -Vector3.right*Width/2 + Vector3.forward*OpeningDepth/2 + Vector3.up*Height + Vector3.up*SillHeight,
                    Vector3.right*Width/2 + Vector3.forward*OpeningDepth/2 + Vector3.up*Height + Vector3.up*SillHeight,
                    Vector3.right*Width/2 + Vector3.forward*OpeningDepth/2 + Vector3.up*SillHeight };
                    material = prefabOpening.doorMaterial;
                    gameObject.name = "Door";
                    break;
            }

            //gameObject.AddComponent<MeshCollider>();
            gameObject.AddComponent<PolyShape>();
            gameObject.AddComponent<ProBuilderMesh>();

            PolyShape polyshape = gameObject.GetComponent<PolyShape>();
            polyshape.SetControlPoints(controlPoints);
            polyshape.extrude = OpeningDepth;
            polyshape.CreateShapeFromPolygon();

            gameObject.GetComponent<ProBuilderMesh>().Refresh();
            gameObject.GetComponent<MeshRenderer>().material = material;

            InitRender2D();
            InitRender3D();
        }

        private void InitRender3D() {
            UpdateRender3D();
        }
        public void UpdateRender3D() {
        }
        private void InitRender2D() {
            LineRenderer lr = gameObject.AddComponent<LineRenderer>();
            lr.materials = Enumerable.Repeat(AssetUtil.LoadAsset<Material>("materials", "plan_opening"), lr.positionCount).ToArray();
            lr.sortingOrder = 2;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;
            UpdateRender2D();
        }
        public void UpdateRender2D(float width = 0.2f) {
            LineRenderer lr = gameObject.GetComponent<LineRenderer>();

            // Reset line renderer and leave empty if wall visibility is false
            lr.positionCount = 0;
            if (!GlobalSettings.ShowOpeningLines) return;

            // Set controlpoints
            lr.useWorldSpace = false;
            float height = 3.001f;
            if (attachedFaces != null) height = attachedFaces[0].parentRoom.height + 0.001f;
            List<Vector3> points = GetControlPoints2D().Select(p =>
                p + Vector3.up * (height)).ToList();
            lr.positionCount = points.Count;
            lr.SetPositions(points.ToArray());

            // Style
            lr.startWidth = width; lr.endWidth = width;
            foreach (Material material in lr.materials) {
                if (shape == OpeningShape.DOOR) material.color = Color.gray;
                else material.color = Color.blue;
            }
        }

        public List<Vector3> GetControlPoints2D() {
            //List<Vector3> controlPoints2D = new List<Vector3> {
            //    new Vector3 ( -Width / 2, 0, -OpeningDepth/2 ),
            //    new Vector3 ( -Width / 2, 0,  OpeningDepth/2 ),
            //    new Vector3 (  Width / 2, 0,  OpeningDepth/2 ),
            //    new Vector3 (  Width / 2, 0, -OpeningDepth/2 ) };
            List<Vector3> controlPoints2D = new List<Vector3> {
                new Vector3 (  Width / 2, 0, 0 ),
                new Vector3 ( -Width / 2, 0, 0) };
            return controlPoints2D;
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

        // DeleteOpening

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
        public IList<Vector3> GetControlPoints(bool localCoordinates = false, bool closed = false, bool IList = false) {
            IList<Vector3> returnPoints = controlPoints;
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