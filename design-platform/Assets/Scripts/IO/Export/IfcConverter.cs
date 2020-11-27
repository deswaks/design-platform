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
using DesignPlatform.Core;
using DesignPlatform.Geometry;

namespace DesignPlatform.Export {
    public static class IfcConverter {

        public static IfcStore ifcModel;
        public static IfcBuilding ifcBuilding;
        public static List<IfcBuildingStorey> ifcBuildingStoreys = new List<IfcBuildingStorey>();
        public static Dictionary<Core.Space, IfcSpace> ifcSpaces = new Dictionary<Core.Space, IfcSpace>();
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
        public static void CreateIfcSpace(Core.Space room) {

            // Get data from room
            int height = Mathf.RoundToInt(room.height * 1000);
            Vector3 roomOrigin = new Vector3(
                Mathf.RoundToInt(room.transform.position.x * 1000),
                Mathf.RoundToInt(room.transform.position.z * 1000),
                Mathf.RoundToInt(room.transform.position.y * 1000));
            Vector3 placementDirection = new Vector3(
                Mathf.RoundToInt(room.transform.right.normalized.x),
                Mathf.RoundToInt(room.transform.right.normalized.z),
                Mathf.RoundToInt(room.transform.right.normalized.y));
            List<Vector2> controlPoints = room.GetControlPoints(localCoordinates: true, closed: true)
                .Select(p => new Vector2(Mathf.RoundToInt(p.x * 1000),
                                          Mathf.RoundToInt(p.z * 1000))).ToList();
            string description = room.TypeName;

            // Create space
            var space = ifcModel.Instances.New<IfcSpace>();

            // Properties
            int roomIndex = Building.Instance.Spaces.FindIndex(r => r == room);
            space.Name = "Space " + ifcSpaces.Keys.Count();
            space.CompositionType = IfcElementCompositionEnum.ELEMENT;
            space.PredefinedType = IfcSpaceTypeEnum.INTERNAL;
            space.Description = new IfcText(description);

            // Create representation
            var productDefinition = ifcModel.Instances.New<IfcProductDefinitionShape>();
            var profile = IfcUtils.CreateProfile(controlPoints: controlPoints);
            var shape3D = IfcUtils.ShapeAsSweptProfile(profile: profile, depth: height,
                                              presentationLayer: "Conceptual Elements");
            productDefinition.Representations.Add(shape3D);
            space.Representation = productDefinition;

            // Model insertion
            space.ObjectPlacement = IfcUtils.CreateLocalPlacement(
                origin: roomOrigin, xDirection: placementDirection, zDirection: Vector3.forward);

            // Relation to building storey
            var rel = ifcModel.Instances.New<IfcRelAggregates>();
            rel.RelatingObject = ifcBuildingStoreys[0];
            rel.RelatedObjects.Add(space);

            ifcSpaces.Add(room, space);
            return;
        }

        /// <summary>
        /// Create an IfcSlab and insert it into the given model.
        /// </summary>
        /// <param name="model">Model to insert the space into.</param>
        /// <param name="room">Room to create the space geometry from.</param>
        /// <returns></returns>
        public static void CreateIfcSlab(Interface interFace) {

            if (interFace.Orientation != Orientation.HORIZONTAL) return;

            // Get data from slab
            int thickness = Mathf.RoundToInt(interFace.Thickness * 1000);
            int isCeiling = interFace.Faces[0].SpaceIndex - (interFace.Spaces[0].GetControlPoints().Count); //0 for floor, 1 for ceiling
            int elevation = Mathf.RoundToInt(isCeiling * interFace.Spaces[0].height * 1000);
            Vector3 extrusionOffset = new Vector3(0, 0, -thickness);
            Vector3 roomOrigin = new Vector3(
                Mathf.RoundToInt(interFace.Spaces[0].transform.position.x * 1000),
                Mathf.RoundToInt(interFace.Spaces[0].transform.position.z * 1000),
                Mathf.RoundToInt(interFace.Spaces[0].transform.position.y * 1000) + elevation);
            Vector3 placementDirection = new Vector3(
                Mathf.RoundToInt(interFace.Spaces[0].transform.right.normalized.x),
                Mathf.RoundToInt(interFace.Spaces[0].transform.right.normalized.z),
                Mathf.RoundToInt(interFace.Spaces[0].transform.right.normalized.y));
            List<Vector2> controlPoints = interFace.Spaces[0]
                .GetControlPoints(localCoordinates: true, closed: true)
                .Select(p => new Vector2(Mathf.RoundToInt(p.x * 1000),
                                          Mathf.RoundToInt(p.z * 1000))).ToList();

            // Create slab
            var slab = ifcModel.Instances.New<IfcSlab>();

            // Properties
            slab.Name = "Slab " + ifcSlabs.Keys.Count().ToString();
            if (isCeiling == 1) slab.PredefinedType = IfcSlabTypeEnum.ROOF;
            else slab.PredefinedType = IfcSlabTypeEnum.FLOOR;
            slab.Description = new IfcText(slab.ToString());

            // Create representation
            var productDefinition = ifcModel.Instances.New<IfcProductDefinitionShape>();
            var profile = IfcUtils.CreateProfile(controlPoints: controlPoints);
            var shape3D = IfcUtils.ShapeAsSweptProfile(profile: profile, depth: thickness,
                                              extrusionOffset: extrusionOffset,
                                              presentationLayer: "Building Elements");
            productDefinition.Representations.Add(shape3D);
            slab.Representation = productDefinition;

            // Model insertion
            slab.ObjectPlacement = IfcUtils.CreateLocalPlacement(
                origin: roomOrigin, xDirection: placementDirection, zDirection: Vector3.forward);

            // Relation to building storey
            var rel = ifcModel.Instances.New<IfcRelContainedInSpatialStructure>();
            rel.RelatingStructure = ifcBuildingStoreys[0];
            rel.RelatedElements.Add(slab);

            // Relation to rooms
            foreach (Core.Space room in interFace.Spaces) {
                var boundaryRel = ifcModel.Instances.New<IfcRelSpaceBoundary>();
                boundaryRel.RelatingSpace = ifcSpaces[room];
                boundaryRel.RelatedBuildingElement = slab;
            }

            ifcSlabs.Add(interFace, slab);
            return;
        }

        /// <summary>
        /// Create an IfcWall and insert it into the given model.
        /// </summary>
        /// <param name="model">Model to insert the wall into.</param>
        /// <param name="interFace">Vertical interface to create wall from.</param>
        /// <returns></returns>
        public static void CreateIfcWall(Interface interFace) {

            if (interFace.Orientation != Orientation.VERTICAL) return;

            // Get data from wall
            Vector3 startPoint = interFace.StartPoint * 1000;
            Vector3 endPoint = interFace.EndPoint * 1000;
            Vector3 wallDirection = (endPoint - startPoint).normalized;

            Vector3 localX = new Vector3(wallDirection.x,
                                         wallDirection.z,
                                         wallDirection.y);
            Vector3 localZ = new Vector3(0, 0, 1);
            Vector3 centerPoint = new Vector3(
                (int)(interFace.CenterPoint.x * 1000),
                (int)(interFace.CenterPoint.z * 1000),
                (int)(interFace.CenterPoint.y * 1000));
            int length = (int)(endPoint - startPoint).magnitude;
            int height = (int)(interFace.Spaces[0].height * 1000);
            int thickness = (int)(interFace.Thickness * 1000);
            string description = interFace.ToString();

            // Create wall
            var wall = ifcModel.Instances.New<IfcWall>();

            // Properties
            wall.Name = "Wall " + ifcWalls.Keys.Count().ToString();
            wall.PredefinedType = IfcWallTypeEnum.STANDARD;
            wall.Description = new IfcText(description);

            // Create representation
            var productDefinition = ifcModel.Instances.New<IfcProductDefinitionShape>();
            var profile = IfcUtils.CreateProfile(width: length, height: thickness);
            var shape3D = IfcUtils.ShapeAsSweptProfile(profile: profile, depth: height,
                                              presentationLayer: "Building Elements");
            var shape2D = IfcUtils.ShapeAsLinearCurve(length: length);
            productDefinition.Representations.Add(shape3D);
            productDefinition.Representations.Add(shape2D);
            wall.Representation = productDefinition;

            // Model insertion
            wall.ObjectPlacement = IfcUtils.CreateLocalPlacement(
                origin: centerPoint, xDirection: localX, zDirection: localZ);

            // Relation to building storey
            var rel = ifcModel.Instances.New<IfcRelContainedInSpatialStructure>();
            rel.RelatingStructure = ifcBuildingStoreys[0];
            rel.RelatedElements.Add(wall);

            // Relation to rooms
            foreach (Core.Space room in interFace.Spaces) {
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

            // Get properties from opening
            int height = Mathf.RoundToInt(opening.Height * 1000);
            int width = Mathf.RoundToInt(opening.Width * 1000);
            int depth = Mathf.RoundToInt(opening.Interface.Thickness * 1000);
            Vector3 ExtrusionOffset = new Vector3(
                0,
                Mathf.RoundToInt((opening.SillHeight + opening.Height / 2) * 1000),
                -depth / 2);
            Vector3 placementOrigin = new Vector3(
                Mathf.RoundToInt(opening.LocationPoint.x * 1000),
                Mathf.RoundToInt(opening.LocationPoint.z * 1000),
                Mathf.RoundToInt(opening.LocationPoint.y * 1000));
            Vector3 placementZVector = new Vector3(
                opening.Faces[0].Normal.x,
                opening.Faces[0].Normal.z,
                opening.Faces[0].Normal.y).normalized;
            Vector3 xVector = (opening.Interface.StartPoint - opening.Interface.EndPoint).normalized;
            Vector3 placementXVector = new Vector3(
                xVector.x,
                xVector.z,
                xVector.y).normalized;
            string description = opening.ToString();

            // Create opening
            var openingElement = ifcModel.Instances.New<IfcOpeningElement>();

            // Set properties
            openingElement.Name = "Opening " + ifcOpenings.Keys.Count().ToString();
            openingElement.PredefinedType = IfcOpeningElementTypeEnum.OPENING;
            openingElement.Description = new IfcText(opening.ToString());

            // Create representation
            var productDefinition = ifcModel.Instances.New<IfcProductDefinitionShape>();
            var profile = IfcUtils.CreateProfile(width: width, height: height);
            var shape3D = IfcUtils.ShapeAsSweptProfile(profile: profile, depth: depth,
                                              extrusionOffset: ExtrusionOffset);
            productDefinition.Representations.Add(shape3D);
            openingElement.Representation = productDefinition;

            // Model insertion
            openingElement.ObjectPlacement = IfcUtils.CreateLocalPlacement(
                origin: placementOrigin, xDirection: placementXVector, zDirection: placementZVector);

            // Set relations
            var ifcWall = ifcWalls[opening.Interface];
            var voidRelation = ifcModel.Instances.New<IfcRelVoidsElement>();
            voidRelation.RelatedOpeningElement = openingElement;
            voidRelation.RelatingBuildingElement = ifcWall;

            // Store opening
            ifcOpenings.Add(opening, openingElement);
            return;
        }


        /// <summary>
        /// Create an IfcRoof? and insert it into the given model.
        /// </summary>
        /// <param name="model">Model to insert the space into.</param>
        /// <param name="room">Room to create the space geometry from.</param>
        /// <returns></returns>
        public static void CreateIfcRoofslab(Interface interFace) {

            if (interFace.Orientation != Orientation.HORIZONTAL) return;

            // Get data from wall
            int thickness = Mathf.RoundToInt(interFace.Thickness * 1000);
            int isCeiling = interFace.Faces[0].SpaceIndex - (interFace.Spaces[0].GetControlPoints().Count); //0 for floor, 1 for ceiling
            int elevation = Mathf.RoundToInt(isCeiling * interFace.Spaces[0].height * 1000);
            Vector3 extrusionOffset = new Vector3(0, 0, -thickness);
            Vector3 roomOrigin = new Vector3(
                Mathf.RoundToInt(interFace.Spaces[0].transform.position.x * 1000),
                Mathf.RoundToInt(interFace.Spaces[0].transform.position.z * 1000),
                Mathf.RoundToInt(interFace.Spaces[0].transform.position.y * 1000) + elevation);
            Vector3 placementDirection = new Vector3(
                Mathf.RoundToInt(interFace.Spaces[0].transform.right.normalized.x),
                Mathf.RoundToInt(interFace.Spaces[0].transform.right.normalized.z),
                Mathf.RoundToInt(interFace.Spaces[0].transform.right.normalized.y));
            List<Vector2> controlPoints = interFace.Spaces[0]
                .GetControlPoints(localCoordinates: true, closed: true)
                .Select(p => new Vector2(Mathf.RoundToInt(p.x * 1000),
                                          Mathf.RoundToInt(p.z * 1000))).ToList();

            // Create space
            var slab = ifcModel.Instances.New<IfcSlab>();

            // Properties
            slab.Name = "Slab " + ifcSlabs.Keys.Count().ToString();
            if (isCeiling == 1) slab.PredefinedType = IfcSlabTypeEnum.ROOF;
            else slab.PredefinedType = IfcSlabTypeEnum.FLOOR;
            slab.Description = new IfcText(slab.ToString());

            // Create representation
            var productDefinition = ifcModel.Instances.New<IfcProductDefinitionShape>();
            var profile = IfcUtils.CreateProfile(controlPoints: controlPoints);
            var shape3D = IfcUtils.ShapeAsSweptProfile(profile: profile, depth: thickness,
                                              extrusionOffset: extrusionOffset,
                                              presentationLayer: "Building Elements");
            productDefinition.Representations.Add(shape3D);
            slab.Representation = productDefinition;

            // Model insertion
            slab.ObjectPlacement = IfcUtils.CreateLocalPlacement(
                origin: roomOrigin, xDirection: placementDirection, zDirection: Vector3.forward);

            // Relation to building storey
            var rel = ifcModel.Instances.New<IfcRelContainedInSpatialStructure>();
            rel.RelatingStructure = ifcBuildingStoreys[0];
            rel.RelatedElements.Add(slab);

            // Relation to rooms
            foreach (Core.Space room in interFace.Spaces) {
                var boundaryRel = ifcModel.Instances.New<IfcRelSpaceBoundary>();
                boundaryRel.RelatingSpace = ifcSpaces[room];
                boundaryRel.RelatedBuildingElement = slab;
            }

            ifcSlabs.Add(interFace, slab);
            return;
        }
    }
}

