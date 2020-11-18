using DesignPlatform.Geometry;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DesignPlatform.Core {
    public partial class Building {


        private static Building instance;
        public static Building Instance {
            get { return instance ?? (instance = new Building()); }
        }
        public Building() {
            Rooms = new List<Room>();
            walls = new List<Wall>();
            slabs = new List<Slab>();
            interfaces = new List<Interface>();
            Openings = new List<Opening>();
        }

        /// <summary>
        /// All Rooms of this building
        /// </summary>
        public List<Room> Rooms { get; private set; }

        /// <summary>
        /// Builds a new room and adds it to the managed building list
        /// </summary>
        /// <param name="buildShape"></param>
        /// <param name="preview"></param>
        /// <param name="templateRoom"></param>
        /// <returns></returns>
        public Room BuildRoom(RoomShape buildShape = RoomShape.RECTANGLE, bool preview = false, Room templateRoom = null) {
            GameObject newRoomGameObject = new GameObject("Room");
            Room newRoom = (Room)newRoomGameObject.AddComponent(typeof(Room));

            if (preview) {
                newRoom.InitRoom(buildShape: buildShape, building: this, type: RoomType.PREVIEW);
                newRoomGameObject.name = "Preview room";
            }
            else {
                newRoom.InitRoom(buildShape: buildShape, building: this, type: RoomType.DEFAULT);
                if (templateRoom != null) {
                    newRoomGameObject.transform.position = templateRoom.transform.position;
                    newRoomGameObject.transform.rotation = templateRoom.transform.rotation;
                }
            }

            if (preview == false) { Rooms.Add(newRoom); }

            return newRoom;
        }
        /// <summary>
        /// Removes a room from the managed building list
        /// </summary>
        /// <param name="room"></param>
        public void RemoveRoom(Room room) {
            if (Rooms.Contains(room)) { Rooms.Remove(room); }
        }



        /// <summary>
        /// All walls of this building
        /// </summary>
        private List<Wall> walls;
        public List<Wall> Walls {
            get {
                if (walls == null || walls.Count == 0) BuildAllWalls();
                return walls;
            }
            private set {; }
        }
        public List<Wall> BuildAllWalls() {
            foreach (Opening opening in Openings) {
                opening.SetAttachedFaces();
                foreach (Face face in opening.Faces) {
                    face.AddOpening(opening.GetCoincidentInterface(), opening);
                }
            }
            foreach (Interface interFace in InterfacesVertical) {
                BuildWall(interFace);
            }
            return walls;
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
        /// Removes a wall from the managed building list
        /// </summary>
        /// <param name="wall"></param>
        public void RemoveWall(Wall wall) {
            if (walls.Contains(wall)) walls.Remove(wall);
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
        private List<Interface> interfaces;
        public List<Interface> Interfaces {
            get {
                if (interfaces == null || interfaces.Count == 0) return BuildAllInterfaces();
                else return interfaces;
            }
            private set {; }
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
        public List<Interface> BuildAllInterfaces() {
            for (int r = 0; r < Rooms.Count; r++) {
                for (int f = 0; f < Rooms[r].Faces.Count; f++) {
                    Face face = Rooms[r].Faces[f];
                    BuildInterfaces(face);
                }
            }
            return interfaces;
        }
        public List<Interface> BuildInterfaces(Face face) {
            // Slab interface
            if (face.Orientation == Orientation.HORIZONTAL) {
                return BuildHorizontalFaceInterfaces(face);
            }
            // Wall interface
            if (face.Orientation == Orientation.VERTICAL) {
                return BuildVerticalFaceInterfaces(face);
            }
            return null;
        }
        private List<Interface> BuildHorizontalFaceInterfaces(Face face) {
            List<Interface> faceInterfaces = new List<Interface>();
            Interface newInterface = new Interface(face);
            interfaces.Add(newInterface);              //Add interface to building
            face.AddInterface(newInterface);           //Add interface to face
            faceInterfaces.Add(newInterface);
            return faceInterfaces;
        }
        private List<Interface> BuildVerticalFaceInterfaces(Face face) {

            Line faceLine = new Line(face.Get2DEndPoints());

            // Find points on face line from the controlpoints of all other rooms
            List<Vector3> splitPoints = new List<Vector3> { faceLine.StartPoint, faceLine.EndPoint };
            for (int r2 = 0; r2 < Rooms.Count; r2++) {
                if (face.Room == Rooms[r2]) continue;
                foreach (Vector3 point in Rooms[r2].GetControlPoints()) {
                    if (faceLine.IsOnLine(point)) {
                        splitPoints.Add(point);
                    }
                }
            }
            // Sort splitpoints between startpoint and endpoint
            splitPoints = splitPoints.OrderBy(p => faceLine.Parameter(p)).ToList();
            List<float> splitParameters = splitPoints.Select(p => faceLine.Parameter(p)).ToList();

            // Hvert interface-sted
            List<Interface> faceInterfaces = new List<Interface>();
            for (int i = 0; i < splitParameters.Count - 1; i++) {
                Line interFaceLine = new Line(splitPoints[i], splitPoints[i + 1]);

                // Check if an interface exists with the same points
                Interface existingInterface = null;
                foreach (Interface interF in interfaces) {
                    if (interF.GetStartPoint() == interFaceLine.StartPoint
                        && interF.GetEndPoint() == interFaceLine.EndPoint
                        || interF.GetStartPoint() == interFaceLine.EndPoint
                        && interF.GetEndPoint() == interFaceLine.EndPoint) {
                        existingInterface = interF;
                    }
                }

                // Attach to existing interface
                if (existingInterface != null) {
                    existingInterface.Faces.Add(face);
                    face.AddInterface(existingInterface, splitParameters[i], splitParameters[i + 1]);
                    faceInterfaces.Add(existingInterface);
                }

                // Create new interface
                else {
                    Interface newInterface = new Interface(face);
                    interfaces.Add(newInterface);
                    face.AddInterface(newInterface, splitParameters[i], splitParameters[i + 1]);
                    faceInterfaces.Add(newInterface);
                }
            }
            return faceInterfaces;
        }
        /// <summary>
        /// Removes an interface from the managed building list
        /// </summary>
        /// <param name="interFace"></param>
        public void RemoveInterface(Interface interFace) {
            if (Interfaces.Contains(interFace)) { interfaces.Remove(interFace); }
        }
        /// <summary>
        /// Removes ALL interfaces
        /// </summary>
        public void DeleteAllInterfaces() {
            int amount = Interfaces.Count;
            if (amount > 0) {
                for (int i = 0; i < amount; i++) {
                    Interfaces[0].Delete();
                }
            }
        }


        /// <summary>
        /// All the openings of this building
        /// </summary>
        public List<Opening> Openings { get; private set; }
        /// <summary>
        /// Builds a new opening and adds it to the managed building list
        /// </summary>
        /// <param name="openingShape"></param>
        /// <param name="preview"></param>
        /// <param name="templateOpening"></param>
        /// <param name="closestFaces"></param>
        /// <returns></returns>
        public Opening BuildOpening(OpeningShape openingShape = OpeningShape.WINDOW,
                                    bool preview = false,
                                    Opening templateOpening = null,
                                    List<Face> attachedFaces = null) {

            GameObject newOpeningGameObject = new GameObject("Opening");
            Opening newOpening = (Opening)newOpeningGameObject.AddComponent(typeof(Opening));
            newOpening.InitializeOpening(attachedFaces: attachedFaces, openingShape: openingShape);
            if (preview) { newOpeningGameObject.name = "Preview opening"; }

            if (templateOpening != null) {
                newOpeningGameObject.transform.position = templateOpening.transform.position;
                newOpeningGameObject.transform.rotation = templateOpening.transform.rotation;
            }

            if (preview == false) {
                Openings.Add(newOpening);
                newOpening.SetOpeningState(Opening.OpeningStates.PLACED);
                //attachedFaces[0].AddOpening(newOpening.GetCoincidentInterface(), newOpening);
            }
            return newOpening;
        }
        /// <summary>
        /// Removes opening from the list of openings
        /// </summary>
        public void RemoveOpening(Opening opening) {
            if (Openings.Contains(opening)) Openings.Remove(opening);
        }


        /// <summary>
        /// Destroys and rebuilds all elements visible in POV mode.
        /// </summary>
        public void RebuildPOVElements() {
            // Delete preexisting	
            DeleteAllWalls();
            DeleteAllSlabs();
            DeleteAllInterfaces();
            // Build new
            BuildAllInterfaces();
            BuildAllWalls();
            BuildAllSlabs();
        }

        /// <summary>
        /// Finds the boundaries of the building in the X and Y axis.
        /// returns:  { minX, maxX, minY, maxY }
        /// </summary>
        public List<float> Bounds() {
            float minX = 0; float maxX = 0;
            float minY = 0; float maxY = 0;
            foreach (Room room in Rooms) {
                foreach (Vector3 controlPoint in room.GetControlPoints()) {
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