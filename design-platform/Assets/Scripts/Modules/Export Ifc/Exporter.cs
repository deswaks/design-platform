using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.Ifc;
<<<<<<< Updated upstream:design-platform/Assets/Scripts/Modules/Export Ifc/Exporter.cs
using Xbim.IO;
using Xbim.Ifc4.ActorResource;
using Xbim.Ifc4.DateTimeResource;
using Xbim.Ifc4.ExternalReferenceResource;
using Xbim.Ifc4.PresentationOrganizationResource;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
=======
>>>>>>> Stashed changes:design-platform/Assets/Scripts/IO/Export/IfcExporter.cs
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MaterialResource;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.QuantityResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.SharedBldgElements;

<<<<<<< Updated upstream:design-platform/Assets/Scripts/Modules/Export Ifc/Exporter.cs
namespace Ifc {
    public static class Exporter {
        private static IfcStore model;
        private static IfcBuilding building;

        public static void Export() {

            // Create model
            model = CreateandInitModel("CLT House");
            if (model == null) return;

            // Create building
            building = CreateBuilding(model, "Default Building");

            // Create all elements
            CreateWalls();

            // Save file
            try {
                model.SaveAs("Exports/Building.ifc", StorageType.Ifc);
                UnityEngine.Debug.Log("Building.ifc has been successfully written");
            }
            catch (Exception e) {
                UnityEngine.Debug.Log("Failed to save HelloWall.ifc");
=======
namespace DesignPlatform.Export {
    public static class IfcExporter {

        public static IfcStore ifcModel;

        /// <summary>
        /// 
        /// </summary>
        public static void Export() {

            BuildModel();
            //try {
            //    BuildModel();
            //}
            //catch (Exception e) {
            //    UnityEngine.Debug.Log("Failed to create ifc model, "+ e.Message);
            //}

            try {
                ifcModel.SaveAs("Exports/Design_platform_export.ifc", StorageType.Ifc);
                UnityEngine.Debug.Log("Successfully exported ifc to: ~Exports/Design_platform_export.ifc");
            }
            catch (Exception e) {
                UnityEngine.Debug.Log("Failed to export ifc model");
>>>>>>> Stashed changes:design-platform/Assets/Scripts/IO/Export/IfcExporter.cs
                UnityEngine.Debug.Log(e.Message);
            }

            // Open file
            Process.Start("Exports\\Design_platform_export.ifc");
        }

<<<<<<< Updated upstream:design-platform/Assets/Scripts/Modules/Export Ifc/Exporter.cs
        private static void CreateWalls() {

            // Create the interfaces that are the basis for walls
            Building.Instance.CreateInterfaces();

            foreach (Interface interFace in Building.Instance.interfaces) {
                IfcWallStandardCase wall = Converter.CreateWall(model, interFace);
                if (wall != null) Converter.AddPropertiesToWall(model, wall);

                // Add to model
                using (var transaction = model.BeginTransaction("Add Wall")) {
                    building.AddElement(wall);
                    transaction.Commit();
                }
            }
        }

        private static IfcBuilding CreateBuilding(IfcStore model, string name) {
            using (var txn = model.BeginTransaction("Create Building")) {
                var building = model.Instances.New<IfcBuilding>();
                building.Name = name;

                building.CompositionType = IfcElementCompositionEnum.ELEMENT;
                var localPlacement = model.Instances.New<IfcLocalPlacement>();
                building.ObjectPlacement = localPlacement;
                var placement = model.Instances.New<IfcAxis2Placement3D>();
                localPlacement.RelativePlacement = placement;
                placement.Location = model.Instances.New<IfcCartesianPoint>(p => p.SetXYZ(0, 0, 0));
                //get the project there should only be one and it should exist
                var project = model.Instances.OfType<IfcProject>().FirstOrDefault();
                project?.AddBuilding(building);
                txn.Commit();
                return building;
            }
=======
        /// <summary>
        /// 
        /// </summary>
        private static void BuildModel() {
            // Create model an building
            CreateandInitModel("Model");

            // Create all elements
            CreateBuilding("Building");
            CreateSpaces();
            Building.Instance.RebuildPOVElements();
            CreateWalls();
            CreateSlabs();
            CreateOpenings();
>>>>>>> Stashed changes:design-platform/Assets/Scripts/IO/Export/IfcExporter.cs
        }

        /// <summary>
        /// Sets up the basic parameters any model must provide, units, ownership etc
        /// </summary>
        /// <param name="projectName">Name of the project</param>
        /// <returns></returns>
        private static void CreateandInitModel(string projectName) {

            var credentials = new XbimEditorCredentials {
                ApplicationDevelopersName = "Deswaks",
                ApplicationFullName = "Design Platform",
                ApplicationIdentifier = "design-platform.exe",
                ApplicationVersion = "0.1",
                EditorsFamilyName = "Team",
                EditorsGivenName = "Deswaks",
                EditorsOrganisationName = "Deswaks"
            };
            ifcModel = IfcStore.Create(credentials, XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel);
            IfcConverter.ifcModel = ifcModel;

            using (var transaction = ifcModel.BeginTransaction("Initialise Model")) {
                var project = ifcModel.Instances.New<IfcProject>(); //create a project
                project.Initialize(ProjectUnits.SIUnitsUK); //set the units to SI (mm and metres)
                project.Name = projectName;
                transaction.Commit();
            }
        }

        private static void CreateBuilding(string buildingName) {
            using (var transaction = ifcModel.BeginTransaction("Create Building")) {
                IfcConverter.CreateIfcBuilding(buildingName);
                IfcConverter.CreateIfcBuildingStorey(index: 0);
                transaction.Commit();
            }
        }

        private static void CreateSpaces() {
            Building.Instance.RebuildPOVElements();
            foreach (Room room in Building.Instance.Rooms) {
                using (var transaction = ifcModel.BeginTransaction("Create Space")) {
                    IfcConverter.CreateIfcSpace(room);
                    transaction.Commit();
                }
            }
        }

        private static void CreateWalls() {
            foreach (Interface interFace in Building.Instance.InterfacesVertical) {
                using (var transaction = ifcModel.BeginTransaction("Create Wall")) {
                    IfcConverter.CreateIfcWall(interFace);
                    transaction.Commit();
                }
            }
        }

        private static void CreateOpenings() {
            foreach (Opening opening in Building.Instance.Openings) {
                using (var transaction = ifcModel.BeginTransaction("Create Opening")) {
                    IfcConverter.CreateIfcOpening(opening);
                    transaction.Commit();
                }
            }
        }

        private static void CreateSlabs() {
            foreach (Interface interFace in Building.Instance.InterfacesHorizontal) {
                using (var transaction = ifcModel.BeginTransaction("Create Slab")) {
                    IfcConverter.CreateIfcSlab(interFace);
                    transaction.Commit();
                }
            }
        }



    }
}
