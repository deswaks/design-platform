using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.IO;
using Xbim.Ifc4.ActorResource;
using Xbim.Ifc4.DateTimeResource;
using Xbim.Ifc4.ExternalReferenceResource;
using Xbim.Ifc4.PresentationOrganizationResource;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MaterialResource;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.QuantityResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.SharedBldgElements;

namespace Ifc {
    public static class Converter {

        public static IfcStore ifcModel;
        public static IfcBuilding ifcBuilding;
        public static List<IfcBuildingStorey> ifcBuildingStoreys = new List<IfcBuildingStorey>();
        public static Dictionary<Room, IfcSpace> ifcSpaces = new Dictionary<Room, IfcSpace>();
        public static Dictionary<Interface, IfcWall> ifcWalls = new Dictionary<Interface, IfcWall>();
        public static Dictionary<Interface, IfcSlab> ifcSlabs = new Dictionary<Interface, IfcSlab>();
        public static Dictionary<Opening, IfcOpeningElement> ifcOpenings = new Dictionary<Opening, IfcOpeningElement>();

        public static void CreateIfcBuilding(string name) {

            // Create building
            var building = ifcModel.Instances.New<IfcBuilding>();
            building.Name = name;
            building.CompositionType = IfcElementCompositionEnum.ELEMENT;
            var localPlacement = ifcModel.Instances.New<IfcLocalPlacement>();
            building.ObjectPlacement = localPlacement;
            var placement = ifcModel.Instances.New<IfcAxis2Placement3D>();
            localPlacement.RelativePlacement = placement;
            placement.Location = ifcModel.Instances.New<IfcCartesianPoint>(p => p.SetXYZ(0, 0, 0));
            var project = ifcModel.Instances.OfType<IfcProject>().FirstOrDefault();
            project?.AddBuilding(building);

            ifcBuilding = building;

            return;
        }

        public static void CreateIfcBuildingStorey(int index) {
            var storey = ifcModel.Instances.New<IfcBuildingStorey>();
            storey.Elevation = new IfcLengthMeasure(0.0f * index);

            storey.Name = "Storey " + index.ToString();
            storey.LongName = "Ground floor";

            // Relation to building
            var rel = ifcModel.Instances.New<IfcRelAggregates>();
            rel.RelatingObject = ifcBuilding;
            rel.RelatedObjects.Add(storey);

            //ifcBuilding.AddElement(storey);
            ifcBuildingStoreys.Add(storey);
            return;
        }

        /// <summary>
        /// Create an IfcSpace and insert it into the given model.
        /// </summary>
        /// <param name="model">Model to insert the space into.</param>
        /// <param name="room">Room to create the space geometry from.</param>
        /// <returns></returns>
        public static void CreateIfcSpace(Room room) {

            // Create space
            var space = ifcModel.Instances.New<IfcSpace>();

            // Properties
            int roomIndex = Building.Instance.Rooms.FindIndex(r => r == room);
            space.Name = "Space " + ifcSpaces.Keys.Count();
            space.LongName = room.TypeName;
            space.CompositionType = IfcElementCompositionEnum.ELEMENT;

            // Relation to building storey
            var rel = ifcModel.Instances.New<IfcRelAggregates>();
            rel.RelatingObject = ifcBuildingStoreys[0];
            rel.RelatedObjects.Add(space);

            // Geometry representation 1 (AND 2D CURVE): profile as a closed polyline
            var profileCurve = ifcModel.Instances.New<IfcPolyline>();
            foreach (Vector3 point in room.GetControlPoints(localCoordinates: true, closed: true)) {
                var ifcPoint = ifcModel.Instances.New<IfcCartesianPoint>();
                ifcPoint.SetXY(point.x * 1000, point.z * 1000);
                profileCurve.Points.Add(ifcPoint);
            }
            var profile = ifcModel.Instances.New<IfcArbitraryClosedProfileDef>();
            profile.OuterCurve = profileCurve;
            profile.ProfileType = IfcProfileTypeEnum.AREA;

            // Geometry representation 2: Sweep profile
            var body = ifcModel.Instances.New<IfcExtrudedAreaSolid>();
            body.Depth = room.height * 1000;
            body.SweptArea = profile;
            body.ExtrudedDirection = ifcModel.Instances.New<IfcDirection>();
            body.ExtrudedDirection.SetXYZ(0, 0, 1);

            // Geometry representation 3: Insert the geometry in the model (at correct place)
            var bodyOrigin = ifcModel.Instances.New<IfcCartesianPoint>();
            Vector3 roomOrigin = room.gameObject.transform.position;
            bodyOrigin.SetXYZ(roomOrigin.x * 1000, roomOrigin.z * 1000, roomOrigin.y * 1000);
            body.Position = ifcModel.Instances.New<IfcAxis2Placement3D>();
            body.Position.Location = bodyOrigin;

            // Create a Definition shape to hold the geometry
            var modelContext = ifcModel.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();

            var shape3D = ifcModel.Instances.New<IfcShapeRepresentation>();
            shape3D.ContextOfItems = modelContext;
            shape3D.RepresentationType = "SweptSolid";
            shape3D.RepresentationIdentifier = "Body";
            shape3D.Items.Add(body);

            var shape2D = ifcModel.Instances.New<IfcShapeRepresentation>();
            shape2D.ContextOfItems = modelContext;
            shape2D.RepresentationIdentifier = "Axis";
            shape2D.RepresentationType = "Curve2D";
            shape2D.Items.Add(profileCurve);

            var rep = ifcModel.Instances.New<IfcProductDefinitionShape>();
            rep.Representations.Add(shape3D);
            rep.Representations.Add(shape2D);
            space.Representation = rep;

            // Presentation Layer Assignment is required for CAD presentation in IfcWall or IfcWallStandardCase
            var ifcPresentationLayerAssignment = ifcModel.Instances.New<IfcPresentationLayerAssignment>();
            ifcPresentationLayerAssignment.Name = "Conceptual elements";
            ifcPresentationLayerAssignment.AssignedItems.Add(shape3D);

            ifcSpaces.Add(room, space);
            return;
        }

        /// <summary>
        /// Create an IfcSpace and insert it into the given model.
        /// </summary>
        /// <param name="model">Model to insert the space into.</param>
        /// <param name="room">Room to create the space geometry from.</param>
        /// <returns></returns>
        public static void CreateIfcSlab(Interface interFace) {

            if (interFace.Orientation != Orientation.HORIZONTAL) return;

            // Get data from wall
            int thickness = (int)(interFace.Thickness * 1000);
            int isCeiling = interFace.Faces[0].FaceIndex - (interFace.GetControlPoints().Count); //0 for floor, 1 for ceiling
            int elevation = (int)(isCeiling * interFace.Rooms[0].height * 1000);
            Vector3 roomOrigin = interFace.Rooms[0].transform.position * 1000;
            Vector3 placementDirection = interFace.Rooms[0].transform.right.normalized;

            // Create space
            var slab = ifcModel.Instances.New<IfcSlab>();

            // Properties
            slab.Name = "Slab " + ifcSlabs.Keys.Count().ToString();

            // Geometry representation 1 (AND 2D CURVE): profile as a closed polyline
            var profileCurve = ifcModel.Instances.New<IfcPolyline>();
            foreach (Vector3 point in interFace.Rooms[0].GetControlPoints(localCoordinates: true, closed: true)) {
                var ifcPoint = ifcModel.Instances.New<IfcCartesianPoint>();
                ifcPoint.SetXY(point.x * 1000, point.z * 1000);
                profileCurve.Points.Add(ifcPoint);
            }
            var profile = ifcModel.Instances.New<IfcArbitraryClosedProfileDef>();
            profile.OuterCurve = profileCurve;
            profile.ProfileType = IfcProfileTypeEnum.AREA;

            // Geometry representation 2: Sweep profile
            var body = ifcModel.Instances.New<IfcExtrudedAreaSolid>();
            body.Depth = thickness;
            body.SweptArea = profile;
            body.ExtrudedDirection = ifcModel.Instances.New<IfcDirection>();
            body.ExtrudedDirection.SetXYZ(0, 0, -1);

            // Geometry representation 3: Insert the geometry in the model (at correct place)
            var bodyOrigin = ifcModel.Instances.New<IfcCartesianPoint>();
            bodyOrigin.SetXYZ(0, 0, 0);
            body.Position = ifcModel.Instances.New<IfcAxis2Placement3D>();
            body.Position.Location = bodyOrigin;
            var placementLocation = ifcModel.Instances.New<IfcCartesianPoint>();
            placementLocation.SetXYZ(roomOrigin.x, roomOrigin.y, roomOrigin.z + elevation);
            var placementAxis = ifcModel.Instances.New<IfcAxis2Placement3D>();
            placementAxis.Location = placementLocation;
            var localPlacement = ifcModel.Instances.New<IfcLocalPlacement>();
            placementAxis.RefDirection = ifcModel.Instances.New<IfcDirection>();
            placementAxis.RefDirection.SetXYZ(placementDirection.x, placementDirection.z, placementDirection.y);
            placementAxis.Axis = ifcModel.Instances.New<IfcDirection>();
            placementAxis.Axis.SetXYZ(0, 0, 1);
            localPlacement.RelativePlacement = placementAxis;
            slab.ObjectPlacement = localPlacement;

            // Create a Definition shape to hold the geometry
            var modelContext = ifcModel.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();

            var shape3D = ifcModel.Instances.New<IfcShapeRepresentation>();
            shape3D.ContextOfItems = modelContext;
            shape3D.RepresentationType = "SweptSolid";
            shape3D.RepresentationIdentifier = "Body";
            shape3D.Items.Add(body);

            var shape2D = ifcModel.Instances.New<IfcShapeRepresentation>();
            shape2D.ContextOfItems = modelContext;
            shape2D.RepresentationIdentifier = "Axis";
            shape2D.RepresentationType = "Curve2D";
            shape2D.Items.Add(profileCurve);

            var rep = ifcModel.Instances.New<IfcProductDefinitionShape>();
            rep.Representations.Add(shape3D);
            rep.Representations.Add(shape2D);
            slab.Representation = rep;

            // Presentation Layer Assignment is required for CAD presentation in IfcWall or IfcWallStandardCase
            var presentationLayer = ifcModel.Instances.New<IfcPresentationLayerAssignment>();
            presentationLayer.Name = "Building elements";
            presentationLayer.AssignedItems.Add(shape3D);

            // Relation to building storey
            var rel = ifcModel.Instances.New<IfcRelContainedInSpatialStructure>();
            rel.RelatingStructure = ifcBuildingStoreys[0];
            rel.RelatedElements.Add(slab);

            // Relation to rooms
            foreach (Room room in interFace.Rooms) {
                var boundaryRel = ifcModel.Instances.New<IfcRelSpaceBoundary>();
                boundaryRel.RelatingSpace = ifcSpaces[room];
                boundaryRel.RelatedBuildingElement = slab;
            }

            ifcSlabs.Add(interFace, slab);
            return;
        }

        /// <summary>
        /// Create an IfcWall and inset it into the given model.
        /// </summary>
        /// <param name="model">Model to insert the wall into.</param>
        /// <param name="interFace">Vertical interface to create wall from.</param>
        /// <returns></returns>
        public static void CreateIfcWall(Interface interFace) {

            if (interFace.Orientation != Orientation.VERTICAL) return;

            // Get data from wall
<<<<<<< Updated upstream:design-platform/Assets/Scripts/Modules/Export Ifc/Converter.cs
            string wallName = interFace.ToString() ;
            Vector3 startPoint = interFace.GetStartPoint() * 1000;
            Vector3 endPoint = interFace.GetEndPoint() * 1000;
            Vector3 midPoint = (endPoint - startPoint)/2;
            Vector3 wallVector = (endPoint - startPoint).normalized;
            float length = (endPoint - startPoint).magnitude;
            float height = interFace.attachedFaces[0].parentRoom.height * 1000;
            float thickness = interFace.GetWallThickness() * 1000;
=======
            Vector3 startPoint = interFace.StartPoint * 1000;
            Vector3 endPoint = interFace.EndPoint * 1000;
            Vector3 wallVector = (endPoint - startPoint).normalized;
            float length = (endPoint - startPoint).magnitude;
            float height = interFace.Rooms[0].height * 1000;
            float thickness = interFace.Thickness * 1000;
            string description = interFace.ToString();

            // Create wall
            var wall = ifcModel.Instances.New<IfcWall>();
>>>>>>> Stashed changes:design-platform/Assets/Scripts/IO/Export/IfcConverter.cs

            // Properties
            wall.Name = "Wall " + ifcWalls.Keys.Count().ToString();
            wall.PredefinedType = IfcWallTypeEnum.STANDARD;
            wall.Description = new IfcText(description);

            // Geometry 3D Profile
            var rectProf = ifcModel.Instances.New<IfcRectangleProfileDef>();
            rectProf.ProfileType = IfcProfileTypeEnum.AREA;
            rectProf.YDim = thickness;
            rectProf.XDim = length;
            var insertPoint = ifcModel.Instances.New<IfcCartesianPoint>();
            insertPoint.SetXY(length / 2, 0);
            rectProf.Position = ifcModel.Instances.New<IfcAxis2Placement2D>();
            rectProf.Position.Location = insertPoint;

            // Geometry 3D Extrusion
            var body = ifcModel.Instances.New<IfcExtrudedAreaSolid>();
            body.Depth = height;
            body.SweptArea = rectProf;
            body.ExtrudedDirection = ifcModel.Instances.New<IfcDirection>();
            body.ExtrudedDirection.SetXYZ(0, 0, 1);

            // Geometry 3D Insertion
            var bodyOrigin = ifcModel.Instances.New<IfcCartesianPoint>();
            bodyOrigin.SetXYZ(0, 0, 0);
            body.Position = ifcModel.Instances.New<IfcAxis2Placement3D>();
            body.Position.Location = bodyOrigin;
            var placementLocation = ifcModel.Instances.New<IfcCartesianPoint>();
            placementLocation.SetXYZ(startPoint.x, startPoint.z, startPoint.y);
            var placementAxis = ifcModel.Instances.New<IfcAxis2Placement3D>();
            placementAxis.Location = placementLocation;
            var localPlacement = ifcModel.Instances.New<IfcLocalPlacement>();
            placementAxis.RefDirection = ifcModel.Instances.New<IfcDirection>();
            placementAxis.RefDirection.SetXYZ(wallVector.x, wallVector.z, wallVector.y);
            placementAxis.Axis = ifcModel.Instances.New<IfcDirection>();
            placementAxis.Axis.SetXYZ(0, 0, 1);
            localPlacement.RelativePlacement = placementAxis;
<<<<<<< Updated upstream:design-platform/Assets/Scripts/Modules/Export Ifc/Converter.cs
            
=======
>>>>>>> Stashed changes:design-platform/Assets/Scripts/IO/Export/IfcConverter.cs
            wall.ObjectPlacement = localPlacement;

            // Geometry 2D as linear segment (required for IfcWall)
            var ifcPolyline = ifcModel.Instances.New<IfcPolyline>();
            var ifcStartPoint = ifcModel.Instances.New<IfcCartesianPoint>();
            ifcStartPoint.SetXY(startPoint.x, startPoint.z);
            var ifcEndPoint = ifcModel.Instances.New<IfcCartesianPoint>();
            ifcEndPoint.SetXY(endPoint.x, endPoint.z);
            ifcPolyline.Points.Add(ifcStartPoint);
            ifcPolyline.Points.Add(ifcEndPoint);

            // Create a Product Definition and add the model geometry to the wall
            var rep = ifcModel.Instances.New<IfcProductDefinitionShape>();

            var shape3D = ifcModel.Instances.New<IfcShapeRepresentation>();
            var modelContext = ifcModel.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
            shape3D.ContextOfItems = modelContext;
            shape3D.RepresentationType = "SweptSolid";
            shape3D.RepresentationIdentifier = "Body";
            shape3D.Items.Add(body);

            var shape2D = ifcModel.Instances.New<IfcShapeRepresentation>();
            shape2D.ContextOfItems = modelContext;
            shape2D.RepresentationIdentifier = "Axis";
            shape2D.RepresentationType = "Curve2D";
            shape2D.Items.Add(ifcPolyline);

            rep.Representations.Add(shape3D);
            rep.Representations.Add(shape2D);
            wall.Representation = rep;

            // Presentation Layer Assignment is required for CAD presentation in IfcWall or IfcWallStandardCase
            var ifcPresentationLayerAssignment = ifcModel.Instances.New<IfcPresentationLayerAssignment>();
            ifcPresentationLayerAssignment.Name = "Building Elements";
            ifcPresentationLayerAssignment.AssignedItems.Add(shape3D);

            // Relation to building storey
            var rel = ifcModel.Instances.New<IfcRelContainedInSpatialStructure>();
            rel.RelatingStructure = ifcBuildingStoreys[0];
            rel.RelatedElements.Add(wall);

            // Relation to rooms
            foreach (Room room in interFace.Rooms) {
                var boundaryRel = ifcModel.Instances.New<IfcRelSpaceBoundary>();
                boundaryRel.RelatingSpace = ifcSpaces[room];
                boundaryRel.RelatedBuildingElement = wall;
            }

            ifcWalls.Add(interFace, wall);
            return;
        }

        /// <summary>
        /// Create an IfcWall and inset it into the given model.
        /// </summary>
        /// <param name="model">Model to insert the wall into.</param>
        /// <param name="interFace">Vertical interface to create wall from.</param>
        /// <returns></returns>
        public static void CreateIfcOpening(Opening opening) {

            int height = (int)(opening.Height * 1000);
            int width = (int)(opening.Width * 1000);
            int depth = (int)(opening.OpeningDepth * 1000);
            int sillHeight = (int)(opening.SillHeight * 1000);
            Vector3 localPlacementOrigin = new Vector3(
                (int)(opening.CenterPoint.x * 1000),
                (int)(opening.CenterPoint.z * 1000),
                (int)(opening.CenterPoint.y * 1000));
            Vector3 localPlacementAxis = new Vector3(
                (int)(opening.Faces[0].Normal.x * 1000),
                (int)(opening.Faces[0].Normal.z * 1000),
                (int)(opening.Faces[0].Normal.y * 1000));
            Vector3 xVector = (opening.Interface.EndPoint - opening.Interface.StartPoint).normalized;
            Vector3 localPlacementX = new Vector3(
                (int)(xVector.x * 1000),
                (int)(xVector.z * 1000),
                (int)(xVector.y * 1000));
            string description = opening.ToString();

            // Create wall
            var openingElement = ifcModel.Instances.New<IfcOpeningElement>();

            // Properties
            openingElement.Name = "Opening " + ifcOpenings.Keys.Count().ToString();
            openingElement.PredefinedType = IfcOpeningElementTypeEnum.OPENING;
            openingElement.Description = new IfcText(description);

            // Geometry representation 1 (AND 2D CURVE): profile as a closed polyline
            var profile = ifcModel.Instances.New<IfcRectangleProfileDef>();
            profile.ProfileType = IfcProfileTypeEnum.AREA;
            profile.YDim = height;
            profile.XDim = width;
            var insertPoint = ifcModel.Instances.New<IfcCartesianPoint>();
            insertPoint.SetXY(0, -height / 2);
            profile.Position = ifcModel.Instances.New<IfcAxis2Placement2D>();
            profile.Position.Location = insertPoint;

            // Geometry representation 2: Sweep profile
            var body = ifcModel.Instances.New<IfcExtrudedAreaSolid>();
            body.Depth = depth;
            body.SweptArea = profile;
            body.ExtrudedDirection = ifcModel.Instances.New<IfcDirection>();
            body.ExtrudedDirection.SetXYZ(0, 0, 1);

            // Geometry 3D Insertion
            var bodyOrigin = ifcModel.Instances.New<IfcCartesianPoint>();
            bodyOrigin.SetXYZ(0, sillHeight, depth / 2);
            body.Position = ifcModel.Instances.New<IfcAxis2Placement3D>();
            body.Position.Location = bodyOrigin;
            var placementLocation = ifcModel.Instances.New<IfcCartesianPoint>();
            placementLocation.SetXYZ(localPlacementOrigin.x, localPlacementOrigin.y, localPlacementOrigin.z);
            var placementAxis = ifcModel.Instances.New<IfcAxis2Placement3D>();
            placementAxis.Location = placementLocation;
            var localPlacement = ifcModel.Instances.New<IfcLocalPlacement>();
            placementAxis.RefDirection = ifcModel.Instances.New<IfcDirection>();
            placementAxis.RefDirection.SetXYZ(localPlacementX.x, localPlacementX.y, localPlacementX.z);
            placementAxis.Axis = ifcModel.Instances.New<IfcDirection>();
            placementAxis.Axis.SetXYZ(localPlacementAxis.x, localPlacementAxis.y, localPlacementAxis.z);
            localPlacement.RelativePlacement = placementAxis;
            openingElement.ObjectPlacement = localPlacement;

            // Geometry 2D as linear segment (required for IfcWall)
            //var ifcPolyline = ifcModel.Instances.New<IfcPolyline>();
            //var ifcStartPoint = ifcModel.Instances.New<IfcCartesianPoint>();
            //ifcStartPoint.SetXY(startPoint2D.x, startPoint2D.y);
            //var ifcEndPoint = ifcModel.Instances.New<IfcCartesianPoint>();
            //ifcEndPoint.SetXY(endPoint2D.x, endPoint2D.y);
            //ifcPolyline.Points.Add(ifcStartPoint);
            //ifcPolyline.Points.Add(ifcEndPoint);

            // Create a Product Definition and add the model geometry to the wall
            var rep = ifcModel.Instances.New<IfcProductDefinitionShape>();

            var shape3D = ifcModel.Instances.New<IfcShapeRepresentation>();
            var modelContext = ifcModel.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
            shape3D.ContextOfItems = modelContext;
            shape3D.RepresentationType = "SweptSolid";
            shape3D.RepresentationIdentifier = "Reference";
            shape3D.Items.Add(body);

            rep.Representations.Add(shape3D);
            openingElement.Representation = rep;

            // Presentation Layer Assignment is required for CAD presentation in IfcWall or IfcWallStandardCase
            var ifcPresentationLayerAssignment = ifcModel.Instances.New<IfcPresentationLayerAssignment>();
            ifcPresentationLayerAssignment.Name = "Void Elements";
            ifcPresentationLayerAssignment.AssignedItems.Add(shape3D);

            // Relation to wall
            // They are attached to the wall using the inverse relationship HasOpenings pointing to IfcRelVoidsElement.
            var voidRelation = ifcModel.Instances.New<IfcRelVoidsElement>();
            voidRelation.RelatedOpeningElement = openingElement;
            voidRelation.RelatingBuildingElement = ifcWalls[opening.Interface];

            ifcOpenings.Add(opening, openingElement);
            return;
        }

        public static void CreateIfcWallFAILED(Interface interFace) {

            if (interFace.Orientation != Orientation.VERTICAL) return;

            // Get information about wall
            Vector3 origin = interFace.Origin;
            Vector3 wallAxis = (interFace.EndPoint - interFace.StartPoint).normalized;
            Vector3 wallNormal = interFace.Faces[0].Normal.normalized;
            int thickness = (int)(interFace.Thickness * 1000);
            List<Vector2> wallOuterProfile = interFace.GetLocalNormalOuterVertices(closed: true);
            List<List<Vector2>> holesOuterProfiles = interFace.GetLocalNormalHoleVertices(closed: true);
            Vector2 startPoint2D = wallOuterProfile[0];
            Vector2 endPoint2D = wallOuterProfile[3];


            // Create wall
            var wall = ifcModel.Instances.New<IfcWall>();

            // Properties
            wall.Name = "Wall " + ifcWalls.Keys.Count().ToString();

            // Geometry representation 1 (AND 2D CURVE): profile as a closed polyline
            var profile = ifcModel.Instances.New<IfcArbitraryProfileDefWithVoids>();
            profile.ProfileType = IfcProfileTypeEnum.AREA;

            var wallProfile = ifcModel.Instances.New<IfcPolyline>();
            foreach (Vector2 point in wallOuterProfile) {
                var ifcPoint = ifcModel.Instances.New<IfcCartesianPoint>();
                ifcPoint.SetXY((int)(point.x * 1000), (int)(point.y * 1000));
                wallProfile.Points.Add(ifcPoint);
            }
            profile.OuterCurve = wallProfile;

            foreach (List<Vector2> openingSet in holesOuterProfiles) {
                var holeProfile = ifcModel.Instances.New<IfcPolyline>();
                foreach (Vector2 point in openingSet) {
                    var ifcPoint = ifcModel.Instances.New<IfcCartesianPoint>();
                    ifcPoint.SetXY((int)(point.x * 1000), (int)(point.y * 1000));
                    holeProfile.Points.Add(ifcPoint);
                }
                profile.InnerCurves.Add(holeProfile);
            }

            // Geometry representation 2: Sweep profile
            var body = ifcModel.Instances.New<IfcExtrudedAreaSolid>();
            body.Depth = thickness;
            body.SweptArea = profile;
            body.ExtrudedDirection = ifcModel.Instances.New<IfcDirection>();
            body.ExtrudedDirection.SetXYZ(0, 0, 1);

            // Geometry 3D Insertion
            var bodyOrigin = ifcModel.Instances.New<IfcCartesianPoint>();
            bodyOrigin.SetXYZ(0, 0, 0);
            body.Position = ifcModel.Instances.New<IfcAxis2Placement3D>();
            body.Position.Location = bodyOrigin;
            var placementLocation = ifcModel.Instances.New<IfcCartesianPoint>();
            placementLocation.SetXYZ((int)(origin.x * 1000), (int)(origin.z * 1000), (int)(origin.y * 1000));
            var placementAxis = ifcModel.Instances.New<IfcAxis2Placement3D>();
            placementAxis.Location = placementLocation;
            var localPlacement = ifcModel.Instances.New<IfcLocalPlacement>();
            placementAxis.RefDirection = ifcModel.Instances.New<IfcDirection>();
            placementAxis.RefDirection.SetXYZ(wallAxis.x, wallAxis.z, wallAxis.y);
            placementAxis.Axis = ifcModel.Instances.New<IfcDirection>();
            placementAxis.Axis.SetXYZ(wallNormal.x, wallNormal.z, wallNormal.y);
            localPlacement.RelativePlacement = placementAxis;
            wall.ObjectPlacement = localPlacement;

            // Geometry 2D as linear segment (required for IfcWall)
            var ifcPolyline = ifcModel.Instances.New<IfcPolyline>();
            var ifcStartPoint = ifcModel.Instances.New<IfcCartesianPoint>();
            ifcStartPoint.SetXY(startPoint2D.x, startPoint2D.y);
            var ifcEndPoint = ifcModel.Instances.New<IfcCartesianPoint>();
            ifcEndPoint.SetXY(endPoint2D.x, endPoint2D.y);
            ifcPolyline.Points.Add(ifcStartPoint);
            ifcPolyline.Points.Add(ifcEndPoint);

            // Create a Product Definition and add the model geometry to the wall
            var rep = ifcModel.Instances.New<IfcProductDefinitionShape>();

            var shape3D = ifcModel.Instances.New<IfcShapeRepresentation>();
            var modelContext = ifcModel.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
            shape3D.ContextOfItems = modelContext;
            shape3D.RepresentationType = "SweptSolid";
            shape3D.RepresentationIdentifier = "Body";
            shape3D.Items.Add(body);

            var shape2D = ifcModel.Instances.New<IfcShapeRepresentation>();
            shape2D.ContextOfItems = modelContext;
            shape2D.RepresentationIdentifier = "Axis";
            shape2D.RepresentationType = "Curve2D";
            shape2D.Items.Add(ifcPolyline);

            rep.Representations.Add(shape3D);
            rep.Representations.Add(shape2D);
            wall.Representation = rep;

            // Presentation Layer Assignment is required for CAD presentation in IfcWall or IfcWallStandardCase
            var ifcPresentationLayerAssignment = ifcModel.Instances.New<IfcPresentationLayerAssignment>();
            ifcPresentationLayerAssignment.Name = "Building Elements";
            ifcPresentationLayerAssignment.AssignedItems.Add(shape3D);

            // Relation to building storey
            var rel = ifcModel.Instances.New<IfcRelContainedInSpatialStructure>();
            rel.RelatingStructure = ifcBuildingStoreys[0];
            rel.RelatedElements.Add(wall);

            // Relation to rooms
            foreach (Room room in interFace.Rooms) {
                var boundaryRel = ifcModel.Instances.New<IfcRelSpaceBoundary>();
                boundaryRel.RelatingSpace = ifcSpaces[room];
                boundaryRel.RelatedBuildingElement = wall;
            }

            ifcWalls.Add(interFace, wall);
            return;
        }
    }
}

