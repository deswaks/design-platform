using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
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
                UnityEngine.Debug.Log(e.Message);
            }
        }

        private static void CreateWalls() {
            foreach (Interface interFace in Building.Instance.interfaces) {

            }
            IfcWallStandardCase wall = Converter.CreateWall(model, 4000, 300, 2400);
            if (wall != null) Converter.AddPropertiesToWall(model, wall);

            using (var transaction = model.BeginTransaction("Add Wall")) {
                building.AddElement(wall);
                transaction.Commit();
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
        }

        /// <summary>
        /// Sets up the basic parameters any model must provide, units, ownership etc
        /// </summary>
        /// <param name="projectName">Name of the project</param>
        /// <returns></returns>
        private static IfcStore CreateandInitModel(string projectName) {
            //first we need to set up some credentials for ownership of data in the new model
            var credentials = new XbimEditorCredentials {
                ApplicationDevelopersName = "xbim developer",
                ApplicationFullName = "Hello Wall Application",
                ApplicationIdentifier = "HelloWall.exe",
                ApplicationVersion = "1.0",
                EditorsFamilyName = "Team",
                EditorsGivenName = "xbim",
                EditorsOrganisationName = "xbim developer"
            };
            //now we can create an IfcStore, it is in Ifc4 format and will be held in memory rather than in a database
            //database is normally better in performance terms if the model is large >50MB of Ifc or if robust transactions are required

            var model = IfcStore.Create(credentials, XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel);

            //Begin a transaction as all changes to a model are ACID
            using (var txn = model.BeginTransaction("Initialise Model")) {

                //create a project
                var project = model.Instances.New<IfcProject>();
                //set the units to SI (mm and metres)
                project.Initialize(ProjectUnits.SIUnitsUK);
                project.Name = projectName;
                //now commit the changes, else they will be rolled back at the end of the scope of the using statement
                txn.Commit();
            }
            return model;

        }


    }
}
