using DesignPlatform.Geometry;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DesignPlatform.Core {

    /// <summary>
    /// The building class is responsible for creation of new building elements
    /// and functions as a general access point for all created elements.
    /// </summary>
    public partial class Building {


        private static Building instance;
        public static Building Instance {
            get { return instance ?? (instance = new Building()); }
        }
        public Building() {
            Spaces = new List<Space>();
            walls = new List<Wall>();
            slabs = new List<Slab>();
            roofs = new List<Roof>();
        }

        /// <summary>
        /// All spaces of this building
        /// </summary>
        public List<Space> Spaces { get; private set; }

        /// <summary>
        /// Builds a new space and adds it to the managed building list
        /// </summary>
        /// <param name="buildShape"></param>
        /// <param name="preview"></param>
        /// <param name="templateSpace"></param>
        /// <returns></returns>
        public Space BuildSpace(SpaceShape buildShape = SpaceShape.RECTANGLE, bool preview = false, Space templateSpace = null) {
            GameObject newSpaceGameObject = new GameObject("Space");
            Space newSpace = (Space)newSpaceGameObject.AddComponent(typeof(Space));

            if (preview) {
                newSpace.InitSpace(buildShape: buildShape, building: this, type: SpaceFunction.PREVIEW);
            }
            else {
                newSpace.InitSpace(buildShape: buildShape, building: this, type: SpaceFunction.DEFAULT);
                if (templateSpace != null) {
                    newSpaceGameObject.transform.position = templateSpace.transform.position;
                    newSpaceGameObject.transform.rotation = templateSpace.transform.rotation;
                }
            }

            if (preview == false) { Spaces.Add(newSpace); }

            return newSpace;
        }

        /// <summary>
        /// Removes a space from the managed building list
        /// </summary>
        /// <param name="space"></param>
        public void RemoveSpace(Space space) {
            if (Spaces.Contains(space)) { Spaces.Remove(space); }
        }

        /// <summary>
        /// All walls of this building
        /// </summary>
        private List<Wall> walls;
        public List<Wall> Walls {
            get {
                if (walls == null || walls.Count == 0) BuildAllWallsAsCLTElements();
                return walls;
            }
            private set {; }
        }

        /// <summary>
        /// All roof elements of this building
        /// </summary>
        private List<Roof> roofs;
        public List<Roof> Roofs{
            get {
                if (roofs == null || roofs.Count == 0) BuildAllRoofElements();
                return roofs;
            }
            private set {; }
        }

        public List<Wall> BuildAllWalls() {
            foreach (Opening opening in Openings) {
                opening.AttachClosestFaces();
            }
            foreach (Interface interFace in InterfacesVertical) {
                BuildWall(interFace);
            }
            return walls;
        }

        public List<Wall> BuildAllWallsAsCLTElements() {
            foreach (Opening opening in Openings) {
                opening.AttachClosestFaces();
            }
            CLTElementGenerator.IdentifyWallElementsAndJointTypes().ForEach(clt => BuildWall(clt));
            return walls;
        }

        public Roof BuildRoofElement(List<Vector3> outline) {
            GameObject newRoofGameObject = new GameObject("Roof");
            Roof newRoof = (Roof)newRoofGameObject.AddComponent<Roof>();

            newRoof.InitializeRoof(outline);

            roofs.Add(newRoof);

            return newRoof;
        }

        public List<Roof> BuildAllRoofElements() {
            List<List<Vector3>> roofOutlines = RoofGenerator.Instance.CreateRoofOutlines();
            foreach (List<Vector3> outline in roofOutlines) {
                BuildRoofElement(outline);
            }
            return roofs;
        }

        /// <summary>
        /// Build
        /// </summary>
        /// <param name="interFace"></param>
        /// <returns></returns>
        public Wall BuildWall(Interface interFace) {
            GameObject newWallGameObject = new GameObject("Wall");
            Wall newWall = (Wall)newWallGameObject.AddComponent(typeof(Wall));

            newWall.InitializeWall(interFace);

            walls.Add(newWall);

            return newWall;
        }

        /// <summary>
        /// Build 3D wall representation using CLT element information
        /// </summary>
        /// <param name="cltElement"></param>
        /// <returns></returns>
        public Wall BuildWall(CLTElement cltElement) {
            GameObject newWallGameObject = new GameObject("Wall");
            Wall newWall = (Wall)newWallGameObject.AddComponent(typeof(Wall));

            newWall.InitializeWall(cltElement);

            walls.Add(newWall);

            return newWall;
        }

        /// <summary>
        /// Removes a wall from the managed building list
        /// </summary>
        /// <param name="wall"></param>
        public void RemoveWall(Wall wall) {
            if (walls.Contains(wall)) walls.Remove(wall);
        }


        /// <summary>
        /// Removes a roof element from the managed building list
        /// </summary>
        /// <param name="roofElement">Roof element to remove</param>
        public void RemoveRoof(Roof roofElement) {
            if (roofs.Contains(roofElement)) roofs.Remove(roofElement);
        }

        /// <summary>
        /// Removes ALL walls
        /// </summary>
        public void DeleteAllWalls() {
            int amount = walls.Count;
            if (amount > 0) {
                for (int i = 0; i < amount; i++) {
                    walls[0].DeleteWall();
                }
            }
        }

        /// <summary>
        /// Removes ALL roof elements
        /// </summary>
        public void DeleteAllRoofElements() {
            int amount = roofs.Count;
            if (amount > 0) {
                for (int i = 0; i < amount; i++) {
                    roofs[0].DeleteRoof();
                }
            }
        }

        /// <summary>
        /// All slabs of this building
        /// </summary>
        private List<Slab> slabs;
        public List<Slab> Slabs {
            get {
                if (slabs.Count == 0) BuildAllSlabs();
                return slabs;
            }
            private set {; }
        }
        public List<Slab> BuildAllSlabs() {
            foreach (Interface interFace in InterfacesHorizontal) BuildSlab(interFace);
            return slabs;
        }
        /// <summary>
        /// Builds a new slab and adds it to the managed building list
        /// </summary>
        public Slab BuildSlab(Interface interFace) {
            GameObject newSlabGameObject = new GameObject("slab");
            Slab newSlab = (Slab)newSlabGameObject.AddComponent(typeof(Slab));
            newSlab.InitializeSlab(interFace);
            slabs.Add(newSlab);
            return newSlab;
        }
        /// <summary>
        /// Removes slab from the list of slabs
        /// </summary>
        public void RemoveSlab(Slab slab) {
            if (slabs.Contains(slab)) slabs.Remove(slab);
        }
        /// <summary>
        /// Removes ALL slabs
        /// </summary>
        public void DeleteAllSlabs() {
            int amount = slabs.Count;
            if (amount > 0) {
                for (int i = 0; i < amount; i++) {
                    slabs[0].DeleteSlab();
                }
            }
        }

        /// <summary>
        /// All the interfaces of this building
        /// </summary>
        public List<Interface> Interfaces {
            get { return Spaces.SelectMany(r => r.Interfaces).Distinct().ToList(); }
        }
        public List<Interface> InterfacesVertical {
            get { return Interfaces.Where(i => i.Orientation == Orientation.VERTICAL).ToList(); }
            private set {; }
        }
        public List<Interface> InterfacesHorizontal {
            get { return Interfaces.Where(i => i.Orientation == Orientation.HORIZONTAL).ToList(); }
            private set {; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void BuildAllInterfaces() {
            for (int r = 0; r < Spaces.Count; r++) {
                for (int f = 0; f < Spaces[r].Faces.Count; f++) {
                    Face face = Spaces[r].Faces[f];
                    BuildInterfaces(face);
                }
            }
        }


        public void BuildInterfaces(Face face) {
            // Slab interface
            if (face.Orientation == Orientation.HORIZONTAL) {
                new Interface(face);
            }
            // Wall interface
            if (face.Orientation == Orientation.VERTICAL) {
                BuildVerticalFaceInterfaces(face);
            }
        }


        private void BuildVerticalFaceInterfaces(Face face) {

            // Find points on face line from the controlpoints of all other spaces
            List<Vector3> splitPoints = new List<Vector3> { face.StartPoint, face.EndPoint };
            for (int r2 = 0; r2 < Spaces.Count; r2++) {
                if (face.Space == Spaces[r2]) continue;
                foreach (Vector3 point in Spaces[r2].GetControlPoints()) {
                    if (face.Line.IsOnLine(point)) {
                        splitPoints.Add(point);
                    }
                }
            }
            // Sort splitpoints between startpoint and endpoint
            splitPoints = splitPoints.OrderBy(p => face.Line.Parameter(p)).ToList();
            List<float> splitParameters = splitPoints.Select(p => face.Line.Parameter(p)).ToList();

            // Hvert interface-sted
            for (int i = 0; i < splitParameters.Count - 1; i++) {
                Line newInterfaceLine = new Line(splitPoints[i], splitPoints[i + 1]);

                // Check if an interface exists with the same points
                Interface duplicateInterface = null;
                foreach (Interface buildingInterface in Interfaces.Where(inte => inte.Orientation == Orientation.VERTICAL)) {
                    if (Vector3.Distance(buildingInterface.StartPoint, newInterfaceLine.StartPoint) < 0.001
                        && Vector3.Distance(buildingInterface.EndPoint, newInterfaceLine.EndPoint) < 0.001
                        || Vector3.Distance(buildingInterface.StartPoint, newInterfaceLine.EndPoint) < 0.001
                        && Vector3.Distance(buildingInterface.EndPoint, newInterfaceLine.StartPoint) < 0.001) {
                        duplicateInterface = buildingInterface;
                    }
                }

                // Attach to existing interface or create new
                if (duplicateInterface == null) {
                    new Interface(face, splitParameters[i], splitParameters[i + 1]);
                }
                else {
                    duplicateInterface.AttachFace(face, splitParameters[i], splitParameters[i + 1]);
                }
            }
        }
        /// <summary>
        /// Removes ALL interfaces
        /// </summary>
        public void DeleteAllInterfaces() {
            foreach (Interface interFace in Interfaces) {
                interFace.Delete();
            }
        }


        /// <summary>
        /// All the openings of this building
        /// </summary>
        public List<Opening> Openings {
            get { return Spaces.SelectMany(r => r.Openings).Distinct().ToList(); }
        }
        /// <summary>
        /// Builds a new opening and adds it to the managed building list
        /// </summary>
        /// <param name="openingShape"></param>
        /// <param name="preview"></param>
        /// <param name="templateOpening"></param>
        /// <param name="closestFaces"></param>
        /// <returns></returns>
        public Opening BuildOpening(OpeningFunction shape = OpeningFunction.WINDOW,
                                    bool preview = false,
                                    Opening templateOpening = null) {
            // Create game object
            GameObject newOpeningGameObject = new GameObject("Opening");
            if (templateOpening != null) {
                newOpeningGameObject.transform.position = templateOpening.transform.position;
                newOpeningGameObject.transform.rotation = templateOpening.transform.rotation;
            }

            // Create opening component
            Opening newOpening = (Opening)newOpeningGameObject.AddComponent(typeof(Opening));
            if (preview) newOpening.InitializeOpening(shape: shape, state: OpeningState.PREVIEW);
            else newOpening.InitializeOpening(shape: shape, state: OpeningState.PLACED);

            return newOpening;
        }        
        
        public Opening BuildOpening(OpeningFunction shape, Vector3 position, Quaternion rotation) {
            // Create game object
            GameObject newOpeningGameObject = new GameObject("Opening");
            newOpeningGameObject.transform.position = position;
            newOpeningGameObject.transform.rotation = rotation;

            // Create opening component
            Opening newOpening = (Opening)newOpeningGameObject.AddComponent(typeof(Opening));
            newOpening.InitializeOpening(shape: shape, state: OpeningState.PLACED);

            return newOpening;
        }


        /// <summary>
        /// Destroys and rebuilds all elements visible in POV mode.
        /// </summary>
        public void RebuildPOVElements() {
            // Delete preexisting	
            DeleteAllWalls();
            DeleteAllSlabs();
            DeleteAllInterfaces();
            DeleteAllRoofElements();

            // Build new
            BuildAllInterfaces();
            BuildAllWallsAsCLTElements();
            BuildAllRoofElements();
            BuildAllSlabs();
        }

        /// <summary>
        /// Finds the boundaries of the building in the X and Y axis.
        /// returns:  { minX, maxX, minY, maxY }
        /// </summary>
        public List<float> Bounds() {
            float minX = 0; float maxX = 0;
            float minY = 0; float maxY = 0;
            foreach (Space space in Spaces) {
                foreach (Vector3 controlPoint in space.GetControlPoints()) {
                    if (controlPoint[0] < minX) { minX = controlPoint[0]; }
                    if (controlPoint[0] > maxX) { minX = controlPoint[0]; }
                    if (controlPoint[2] < minY) { minX = controlPoint[2]; }
                    if (controlPoint[2] > maxY) { minX = controlPoint[2]; }
                }
            }
            return new List<float> { minX, maxX, minY, maxY };
        }
    }
}