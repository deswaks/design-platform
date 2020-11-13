using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System.Linq;
using DesignPlatform.Utils;
using DesignPlatform.Database;

namespace DesignPlatform.Core {
    public partial class Building {
        
        public List<Room> Rooms { get; private set; }
        public List<Wall> Walls { get; private set; }
        public List<Slab> Slabs { get; private set; }
        public List<Interface> Interfaces { get; private set; }
        public List<Opening> Openings { get; private set; }

        private static Building instance;
        public static Building Instance {
            // Use the ?? operator, to return 'instance' if 'instance' does not equal null
            // otherwise we assign instance to a new component and return that
            get { return instance ?? (instance = new Building()); }
        }

        public Building() {
            Rooms = new List<Room>();
            Walls = new List<Wall>();
            Slabs = new List<Slab>();
            Interfaces = new List<Interface>();
            Openings = new List<Opening>();
        }

        /// <summary>
        /// Removes room from the list of rooms
        /// </summary>
        /// <param name="room"></param>
        public void RemoveRoom(Room room) {
            if (Rooms.Contains(room)) { Rooms.Remove(room); }
        }

        /// <summary>
        /// Removes interface from the list of interfaces
        /// </summary>
        /// <param name="interFace"></param>
        public void RemoveInterface(Interface interFace) {
            if (Interfaces.Contains(interFace)) { Interfaces.Remove(interFace); }
        }

        /// <summary>
        /// Builds a new room
        /// </summary>
        public Room BuildRoom(RoomShape buildShape = RoomShape.RECTANGLE, bool preview = false, Room templateRoom = null) {
            GameObject newRoomGameObject = new GameObject("Room");
            Room newRoom = (Room)newRoomGameObject.AddComponent(typeof(Room));

            if (preview) {
                newRoom.InitRoom(buildShape: buildShape, building: this, type: RoomType.PREVIEW);
                newRoomGameObject.name = "Preview room";
            }

            if (templateRoom != null) {
                newRoomGameObject.transform.position = templateRoom.transform.position;
                newRoomGameObject.transform.rotation = templateRoom.transform.rotation;
                newRoom.InitRoom(buildShape: buildShape, building: this, type: RoomType.DEFAULT);
            }

            if (preview == false) { Rooms.Add(newRoom); }

            return newRoom;
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

        /// <summary>
        /// Builds a new wall
        /// </summary>
        public Wall BuildWall(Interface interFace) {
            GameObject newWallGameObject = new GameObject("Wall");
            Wall newWall = (Wall)newWallGameObject.AddComponent(typeof(Wall));

            newWall.InitializeWall(interFace);
            Walls.Add(newWall);

            return newWall;
        }

        /// <summary>
        /// Removes wall from the list of walls
        /// </summary>
        public void RemoveWall(Wall wall) {
            if (Walls.Contains(wall)) Walls.Remove(wall);
        }

        /// <summary>
        /// Removes ALL walls
        /// </summary>
        public void DeleteAllWalls() {
            int amount = Walls.Count;
            for (int i = 0; i < amount; i++) {
                Walls[0].DeleteWall();
            }
        }
        /// <summary>
        /// Builds a new salb
        /// </summary>
        public Slab BuildSlab(Interface interFace) {
            GameObject newSlabGameObject = new GameObject("slab");
            Slab newSlab = (Slab)newSlabGameObject.AddComponent(typeof(Slab));

            newSlab.InitializeSlab(interFace);

            Slabs.Add(newSlab);

            return newSlab;
        }

        /// <summary>
        /// Get a list of builded slabs
        /// </summary>
        public List<Slab> GetSlabs() {
            return Slabs;
        }

        /// <summary>
        /// Removes slab from the list of slabs
        /// </summary>
        public void RemoveSlab(Slab slab) {
            if (Slabs.Contains(slab)) Slabs.Remove(slab);
        }

        /// <summary>
        /// Removes ALL slabs
        /// </summary>
        public void DeleteAllSlabs() {
            int amount = Slabs.Count;
            for (int i = 0; i < amount; i++) {
                Slabs[0].DeleteSlab();
            }
        }

        /// <summary>
        /// Builds a new opening
        /// </summary>
        public Opening BuildOpening(OpeningShape openingShape = OpeningShape.WINDOW,
                                    bool preview = false,
                                    Opening templateOpening = null,
                                    Face[] closestFaces = null) {

            GameObject newOpeningGameObject = new GameObject("Opening");
            Opening newOpening = (Opening)newOpeningGameObject.AddComponent(typeof(Opening));
            newOpening.InitializeOpening(parentFaces: closestFaces, openingShape: openingShape);
            if (preview) { newOpeningGameObject.name = "Preview opening"; }

            if (templateOpening != null) {
                newOpeningGameObject.transform.position = templateOpening.transform.position;
                newOpeningGameObject.transform.rotation = templateOpening.transform.rotation;
            }

            if (preview == false) {
                Openings.Add(newOpening);
                newOpening.SetOpeningState(Opening.OpeningStates.PLACED);
                closestFaces[0].AddOpening(newOpening);
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
        /// Removes ALL interfaces
        /// </summary>
        public void DeleteAllInterfaces() {
            int amount = Interfaces.Count;
            for (int i = 0; i < amount; i++) {
                Interfaces[0].Delete();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CreateVerticalInterfaces() {
            // For all faces
            for (int r = 0; r < Rooms.Count; r++) {
                for (int f = 0; f < Rooms[r].Faces.Count; f++) {
                    Face face = Rooms[r].Faces[f];
                    CreateVertivalInterfacesOnFace(face);
                }
            }
            foreach (Interface interFace in Interfaces) {
                Debug.Log(interFace);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="face"></param>
        public void CreateVertivalInterfacesOnFace(Face face) {

            // Skip if face is not a wall
            if (face.orientation != Orientation.VERTICAL) return;

            // Find points on face line from the controlpoints of all other rooms
            List<Vector3> splitPoints = new List<Vector3>();
            for (int r = 0; r < Rooms.Count; r++) {
                if (face.parentRoom == Rooms[r]) continue;
                foreach (Vector3 point in Rooms[r].GetControlPoints(localCoordinates: false)) {
                    if (face.IsPointOnFace(point)) {
                        splitPoints.Add(point);
                    }
                }
            }
            Debug.Log(splitPoints);

            // Sort splitpoints between startpoint and endpoint
            List<Vector3> endPoints = face.GetControlPoints(localCoordinates: false);
            Vector3 startPoint = endPoints[0]; Vector3 endPoint = endPoints[1];
            splitPoints.Add(startPoint); splitPoints.Add(endPoint);
            splitPoints = splitPoints.OrderBy(p => (p - startPoint).magnitude).ToList();
            List<float> splitParameters = splitPoints.Select(p => (p - startPoint).magnitude).ToList();
            splitParameters = RangeUtils.Reparametrize(splitParameters, splitParameters[0], splitParameters[splitParameters.Count - 1]);

            // Hvert interface-sted
            for (int i = 0; i < splitParameters.Count - 1; i++) {

                // Check if an interface exists with the same points
                Vector3 ifStartPoint = splitPoints[i];
                Vector3 ifEndPoint = splitPoints[i + 1];
                Interface existingInterface = null;
                foreach (Interface interFace in Interfaces) {
                    if (interFace.GetStartPoint() == ifStartPoint && interFace.GetEndPoint() == ifEndPoint
                     || interFace.GetStartPoint() == ifEndPoint && interFace.GetEndPoint() == ifStartPoint) {
                        existingInterface = interFace;
                    }
                }

                // Attach to existing interface
                if (existingInterface != null) {
                    existingInterface.attachedFaces[1] = face;
                    face.AddInterface(existingInterface, splitParameters[i], splitParameters[i + 1]);
                }

                // Create new interface
                else {
                    Interface interFace = new Interface();
                    interFace.attachedFaces[0] = face;
                    Interfaces.Add(interFace);
                    face.AddInterface(interFace, splitParameters[i], splitParameters[i + 1]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CreateHorizontalInterfaces() {
            // For all faces
            for (int r = 0; r < Rooms.Count; r++) {
                for (int f = 0; f < Rooms[r].Faces.Count; f++) {

                    Face face = Rooms[r].Faces[f];

                    // Skip if face is not a slab
                    if (face.orientation != Orientation.HORIZONTAL) continue;

                    // Create new interface
                    Interface interFace = new Interface();
                    interFace.attachedFaces[0] = face;      //Add face to interface
                    Interfaces.Add(interFace);              //Add interface to building
                    face.AddInterface(interFace);           //Add interface to face
                }
            }
        }
        public void UpdatePOVElements() {
            // Delete preexisting	
            if (Walls.Count > 0) DeleteAllWalls();
            if (Slabs.Count > 0) DeleteAllSlabs();
            if (Interfaces.Count > 0) DeleteAllInterfaces();

            CreateVerticalInterfaces();
            CreateHorizontalInterfaces();
            
            foreach (Interface interFace in Interfaces) {
                if (interFace.GetOrientation() == Orientation.VERTICAL) {
                    BuildWall(interFace);
                }
                if (interFace.GetOrientation() == Orientation.HORIZONTAL)
                    BuildSlab(interFace);
            }
        }

        public List<List<Vector3>> GetInterfacesEndpoints() {
            List<List<Vector3>> InterfacesEndpoints = new List<List<Vector3>>();
            for (int i = 0; i < Interfaces.Count; i++) {
                if (Interfaces[i].GetOrientation() == Orientation.VERTICAL) {
                    InterfacesEndpoints.Add(new List<Vector3>());
                    InterfacesEndpoints[i].Add(Interfaces[i].GetStartPoint());
                    InterfacesEndpoints[i].Add(Interfaces[i].GetEndPoint());
                }
            }
            return InterfacesEndpoints;
        }
    }
}