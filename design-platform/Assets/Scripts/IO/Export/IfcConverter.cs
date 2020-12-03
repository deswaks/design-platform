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

    /// <summary>
    /// A converter to create IFC objects from the internal objects of the design platform.
    /// </summary>
    public static class IfcConverter {

        /// <summary>The Ifc building model.</summary>
        public static IfcStore ifcModel { get; set; }
        
        /// <summary>The IfcBuilding object.</summary>
        public static IfcBuilding ifcBuilding { get; private set; }
        
        /// <summary>Collection of the IfcBuilding storeys.</summary>
        public static List<IfcBuildingStorey> ifcBuildingStoreys { get; private set; } = new List<IfcBuildingStorey>();
        
        /// <summary>Collection of the IfcSpaces of the building, linked to the model objects.</summary>
        public static Dictionary<Core.Space, IfcSpace> ifcSpaces { get; private set; } = new Dictionary<Core.Space, IfcSpace>();
        
        /// <summary>Collection of the IfcWalls of the building, linked to the model interface objects.</summary>
        public static Dictionary<Interface, IfcWall> ifcWalls { get; private set; } = new Dictionary<Interface, IfcWall>();
        
        /// <summary>Collection of the IfcSlabs of the building, linked to the model interface objects.</summary>
        public static Dictionary<Interface, IfcSlab> ifcSlabs { get; private set; } = new Dictionary<Interface, IfcSlab>();
        
        /// <summary>Collection of the IfcOpenings of the building, linked to the model opening objects.</summary>
        public static Dictionary<Opening, IfcOpeningElement> ifcOpenings { get; private set; } = new Dictionary<Opening, IfcOpeningElement>();

        /// <summary>
        /// Create an IfcBuilding and insert it into the model.
        /// </summary>
        /// <param name="name">Name of the building.</param>
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

        /// <summary>
        /// Create an ifc building storey with its relations and insert it into the model.
        /// </summary>
        /// <param name="index">Index of the storey (eg. 1 for 1st floor).</param>
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
        /// Create an ifcSpace with its relations and insert it into the model.
        /// </summary>
        /// <param name="buildingSpace">Room to create the space geometry from.</param>
        public static void CreateIfcSpace(Core.Space buildingSpace) {

            // Get data from space
            int height = Mathf.RoundToInt(buildingSpace.Height * 1000);
            Vector3 roomOrigin = new Vector3(
                Mathf.RoundToInt(buildingSpace.transform.position.x * 1000),
                Mathf.RoundToInt(buildingSpace.transform.position.z * 1000),
                Mathf.RoundToInt(buildingSpace.transform.position.y * 1000));
            Vector3 placementDirection = new Vector3(
                Mathf.RoundToInt(buildingSpace.transform.right.normalized.x),
                Mathf.RoundToInt(buildingSpace.transform.right.normalized.z),
                Mathf.RoundToInt(buildingSpace.transform.right.normalized.y));
            List<Vector2> controlPoints = buildingSpace.GetControlPoints(localCoordinates: true, closed: true)
                .Select(p => new Vector2(Mathf.RoundToInt(p.x * 1000),
                                          Mathf.RoundToInt(p.z * 1000))).ToList();
            string description = Settings.SpaceTypeNames[buildingSpace.Function];

            // Create space
            var space = ifcModel.Instances.New<IfcSpace>();

            // Properties
            int spaceIndex = Building.Instance.Spaces.FindIndex(s => s == buildingSpace);
            space.Name = "Space " + ifcSpaces.Keys.Count();
            space.CompositionType = IfcElementCompositionEnum.ELEMENT;
            space.PredefinedType = IfcSpaceTypeEnum.INTERNAL;
            space.Description = new IfcText(description);

            // Create representation
            var productDefinition = ifcModel.Instances.New<IfcProductDefinitionShape>();
            var profile = IfcUtils.CreateProfile(controlPoints: controlPoints);
            var shape3D = IfcUtils.ShapeAsSweptProfile(profile: profile, extrusionDepth: height,
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

            ifcSpaces.Add(buildingSpace, space);
            return;
        }

        /// <summary>
        /// Create an IfcSlab with its relations and insert it into the model.
        /// </summary>
        /// <param name="interFace">Interface to create the slab geometry from.</param>
        public static void CreateIfcSlab(Interface interFace) {

            if (interFace.Orientation != Orientation.HORIZONTAL) return;

            // Get data from slab
            int thickness = Mathf.RoundToInt(interFace.Thickness * 1000);
            int isCeiling = interFace.Faces[0].SpaceIndex - (interFace.Spaces[0].GetControlPoints().Count); //0 for floor, 1 for ceiling
            int elevation = Mathf.RoundToInt(isCeiling * interFace.Spaces[0].Height * 1000);
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
            var shape3D = IfcUtils.ShapeAsSweptProfile(profile: profile, extrusionDepth: thickness,
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
        /// Create an IfcWall with its relations and insert it into the model.
        /// </summary>
        /// <param name="interFace">Interface to create the wall geometry from.</param>
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
                (int)(interFace.LocationLine.Midpoint.x * 1000),
                (int)(interFace.LocationLine.Midpoint.z * 1000),
                (int)(interFace.LocationLine.Midpoint.y * 1000));
            int length = (int)(endPoint - startPoint).magnitude;
            int height = (int)(interFace.Spaces[0].Height * 1000);
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
            var shape3D = IfcUtils.ShapeAsSweptProfile(profile: profile, extrusionDepth: height,
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
        /// Create an IfcOpening with its relations and insert it into the model
        /// (create it as a hole in its related walls).
        /// </summary>
        /// <param name="opening">Opening to create the opening geometry from.</param>
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
            var shape3D = IfcUtils.ShapeAsSweptProfile(profile: profile, extrusionDepth: depth,
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
        /// Create an IfcSlab for a roof element with its relations and insert it into the model.
        /// </summary>
        /// <param name="interFace">Interface to create the roof slab geometry from.</param>
        public static void CreateIfcRoofslab(Interface interFace) {

            if (interFace.Orientation != Orientation.HORIZONTAL) return;

            // Get data from wall
            int thickness = Mathf.RoundToInt(interFace.Thickness * 1000);
            int isCeiling = interFace.Faces[0].SpaceIndex - (interFace.Spaces[0].GetControlPoints().Count); //0 for floor, 1 for ceiling
            int elevation = Mathf.RoundToInt(isCeiling * interFace.Spaces[0].Height * 1000);
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
            var shape3D = IfcUtils.ShapeAsSweptProfile(profile: profile, extrusionDepth: thickness,
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

