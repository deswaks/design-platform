﻿using System;
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

        public static void IfcSpaceFromRoom(Room room) {

            return;
        }

        //public static IfcWall IfcWallFromWall(Wall wall) {
        //    return
        //}


        /// <summary>
        /// This creates a wall and it's geometry, many geometric representations are possible and extruded rectangular footprint is chosen as this is commonly used for standard case walls
        /// </summary>
        /// <param name="model"></param>
        /// <param name="length">Length of the rectangular footprint</param>
        /// <param name="width">Width of the rectangular footprint (width of the wall)</param>
        /// <param name="height">Height to extrude the wall, extrusion is vertical</param>
        /// <returns></returns>
        public static IfcWallStandardCase CreateWall(IfcStore model, Interface interFace) {

            // Get data from wall
            string wallName = interFace.ToString() ;
            Vector3 startPoint = interFace.GetStartPoint() * 1000;
            Vector3 endPoint = interFace.GetEndPoint() * 1000;
            Vector3 wallVector = (endPoint - startPoint).normalized;
            float length = (endPoint - startPoint).magnitude;
            float height = interFace.attachedFaces[0].parentRoom.height * 1000;
            float thickness = interFace.GetWallThickness() * 1000;

            //begin a transaction
            var transaction = model.BeginTransaction("Create Wall");
            var wall = model.Instances.New<IfcWallStandardCase>();
            wall.Name = wallName;

            //represent wall as a rectangular flat profile
            var rectProf = model.Instances.New<IfcRectangleProfileDef>();
            rectProf.ProfileType = IfcProfileTypeEnum.AREA;
            rectProf.YDim = thickness;
            rectProf.XDim = length;

            var insertPoint = model.Instances.New<IfcCartesianPoint>();
            insertPoint.SetXY(0, 0);
            rectProf.Position = model.Instances.New<IfcAxis2Placement2D>();
            rectProf.Position.Location = insertPoint;

            //model as a swept area solid
            var body = model.Instances.New<IfcExtrudedAreaSolid>();
            body.Depth = height;
            body.SweptArea = rectProf;
            body.ExtrudedDirection = model.Instances.New<IfcDirection>();
            body.ExtrudedDirection.SetXYZ(0, 0, 1);

            //parameters to insert the geometry in the model
            var origin = model.Instances.New<IfcCartesianPoint>();
            origin.SetXYZ(startPoint.x, startPoint.z, startPoint.y);
            body.Position = model.Instances.New<IfcAxis2Placement3D>();
            body.Position.Location = origin;

            //Create a Definition shape to hold the geometry
            var shape = model.Instances.New<IfcShapeRepresentation>();
            var modelContext = model.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
            shape.ContextOfItems = modelContext;
            shape.RepresentationType = "SweptSolid";
            shape.RepresentationIdentifier = "Body";
            shape.Items.Add(body);

            //Create a Product Definition and add the model geometry to the wall
            var rep = model.Instances.New<IfcProductDefinitionShape>();
            rep.Representations.Add(shape);
            wall.Representation = rep;

            //now place the wall into the model
            var lp = model.Instances.New<IfcLocalPlacement>();
            var ax3D = model.Instances.New<IfcAxis2Placement3D>();
            ax3D.Location = origin;
            ax3D.RefDirection = model.Instances.New<IfcDirection>();
            ax3D.RefDirection.SetXYZ(wallVector.x, wallVector.z, wallVector.y);
            ax3D.Axis = model.Instances.New<IfcDirection>();
            ax3D.Axis.SetXYZ(0, 0, 1);
            lp.RelativePlacement = ax3D;
            wall.ObjectPlacement = lp;

            // Where Clause: The IfcWallStandard relies on the provision of an IfcMaterialLayerSetUsage 
            var ifcMaterialLayerSetUsage = model.Instances.New<IfcMaterialLayerSetUsage>();
            var ifcMaterialLayerSet = model.Instances.New<IfcMaterialLayerSet>();
            var ifcMaterialLayer = model.Instances.New<IfcMaterialLayer>();
            ifcMaterialLayer.LayerThickness = 10;
            ifcMaterialLayerSet.MaterialLayers.Add(ifcMaterialLayer);
            ifcMaterialLayerSetUsage.ForLayerSet = ifcMaterialLayerSet;
            ifcMaterialLayerSetUsage.LayerSetDirection = IfcLayerSetDirectionEnum.AXIS2;
            ifcMaterialLayerSetUsage.DirectionSense = IfcDirectionSenseEnum.NEGATIVE;
            ifcMaterialLayerSetUsage.OffsetFromReferenceLine = 150;

            // Add material to wall
            var material = model.Instances.New<IfcMaterial>();
            material.Name = "some material";
            var ifcRelAssociatesMaterial = model.Instances.New<IfcRelAssociatesMaterial>();
            ifcRelAssociatesMaterial.RelatingMaterial = material;
            ifcRelAssociatesMaterial.RelatedObjects.Add(wall);
            ifcRelAssociatesMaterial.RelatingMaterial = ifcMaterialLayerSetUsage;

            // IfcPresentationLayerAssignment is required for CAD presentation in IfcWall or IfcWallStandardCase
            var ifcPresentationLayerAssignment = model.Instances.New<IfcPresentationLayerAssignment>();
            ifcPresentationLayerAssignment.Name = "some ifcPresentationLayerAssignment";
            ifcPresentationLayerAssignment.AssignedItems.Add(shape);

            // linear segment as IfcPolyline with two points is required for IfcWall
            var ifcPolyline = model.Instances.New<IfcPolyline>();
            var ifcStartPoint = model.Instances.New<IfcCartesianPoint>();
            ifcStartPoint.SetXY(startPoint.x, startPoint.z);
            var ifcEndPoint = model.Instances.New<IfcCartesianPoint>();
            ifcEndPoint.SetXY(endPoint.x, endPoint.z);
            ifcPolyline.Points.Add(ifcStartPoint);
            ifcPolyline.Points.Add(ifcEndPoint);

            var shape2D = model.Instances.New<IfcShapeRepresentation>();
            shape2D.ContextOfItems = modelContext;
            shape2D.RepresentationIdentifier = "Axis";
            shape2D.RepresentationType = "Curve2D";
            shape2D.Items.Add(ifcPolyline);
            rep.Representations.Add(shape2D);
            transaction.Commit();
            return wall;
        }

        /// <summary>
        /// Add some properties to the wall,
        /// </summary>
        /// <param name="model">XbimModel</param>
        /// <param name="wall"></param>
        public static void AddPropertiesToWall(IfcStore model, IfcWallStandardCase wall) {
            using (var txn = model.BeginTransaction("Create Wall")) {
                CreateElementQuantity(model, wall);
                CreateSimpleProperty(model, wall);
                txn.Commit();
            }
        }

        public static void CreateSimpleProperty(IfcStore model, IfcWallStandardCase wall) {
            var ifcPropertySingleValue = model.Instances.New<IfcPropertySingleValue>(psv => {
                psv.Name = "IfcPropertySingleValue:Time";
                psv.Description = "";
                psv.NominalValue = new IfcTimeMeasure(150.0);
                psv.Unit = model.Instances.New<IfcSIUnit>(siu => {
                    siu.UnitType = IfcUnitEnum.TIMEUNIT;
                    siu.Name = IfcSIUnitName.SECOND;
                });
            });
            var ifcPropertyEnumeratedValue = model.Instances.New<IfcPropertyEnumeratedValue>(pev => {
                pev.Name = "IfcPropertyEnumeratedValue:Music";
                pev.EnumerationReference = model.Instances.New<IfcPropertyEnumeration>(pe => {
                    pe.Name = "Notes";
                    pe.EnumerationValues.Add(new IfcLabel("Do"));
                    pe.EnumerationValues.Add(new IfcLabel("Re"));
                    pe.EnumerationValues.Add(new IfcLabel("Mi"));
                    pe.EnumerationValues.Add(new IfcLabel("Fa"));
                    pe.EnumerationValues.Add(new IfcLabel("So"));
                    pe.EnumerationValues.Add(new IfcLabel("La"));
                    pe.EnumerationValues.Add(new IfcLabel("Ti"));
                });
                pev.EnumerationValues.Add(new IfcLabel("Do"));
                pev.EnumerationValues.Add(new IfcLabel("Re"));
                pev.EnumerationValues.Add(new IfcLabel("Mi"));

            });
            var ifcPropertyBoundedValue = model.Instances.New<IfcPropertyBoundedValue>(pbv => {
                pbv.Name = "IfcPropertyBoundedValue:Mass";
                pbv.Description = "";
                pbv.UpperBoundValue = new IfcMassMeasure(5000.0);
                pbv.LowerBoundValue = new IfcMassMeasure(1000.0);
                pbv.Unit = model.Instances.New<IfcSIUnit>(siu => {
                    siu.UnitType = IfcUnitEnum.MASSUNIT;
                    siu.Name = IfcSIUnitName.GRAM;
                    siu.Prefix = IfcSIPrefix.KILO;
                });
            });

            var definingValues = new List<IfcReal> { new IfcReal(100.0), new IfcReal(200.0), new IfcReal(400.0), new IfcReal(800.0), new IfcReal(1600.0), new IfcReal(3200.0), };
            var definedValues = new List<IfcReal> { new IfcReal(20.0), new IfcReal(42.0), new IfcReal(46.0), new IfcReal(56.0), new IfcReal(60.0), new IfcReal(65.0), };
            var ifcPropertyTableValue = model.Instances.New<IfcPropertyTableValue>(ptv => {
                ptv.Name = "IfcPropertyTableValue:Sound";
                foreach (var item in definingValues) {
                    ptv.DefiningValues.Add(item);
                }
                foreach (var item in definedValues) {
                    ptv.DefinedValues.Add(item);
                }
                ptv.DefinedUnit = model.Instances.New<IfcContextDependentUnit>(cd => {
                    cd.Dimensions = model.Instances.New<IfcDimensionalExponents>(de => {
                        de.LengthExponent = 0;
                        de.MassExponent = 0;
                        de.TimeExponent = 0;
                        de.ElectricCurrentExponent = 0;
                        de.ThermodynamicTemperatureExponent = 0;
                        de.AmountOfSubstanceExponent = 0;
                        de.LuminousIntensityExponent = 0;
                    });
                    cd.UnitType = IfcUnitEnum.FREQUENCYUNIT;
                    cd.Name = "dB";
                });
            });

            var listValues = new List<IfcLabel> { new IfcLabel("Red"), new IfcLabel("Green"), new IfcLabel("Blue"), new IfcLabel("Pink"), new IfcLabel("White"), new IfcLabel("Black"), };
            var ifcPropertyListValue = model.Instances.New<IfcPropertyListValue>(plv => {
                plv.Name = "IfcPropertyListValue:Colours";
                foreach (var item in listValues) {
                    plv.ListValues.Add(item);
                }
            });

            var ifcMaterial = model.Instances.New<IfcMaterial>(m => {
                m.Name = "Brick";
            });
            var ifcPrValueMaterial = model.Instances.New<IfcPropertyReferenceValue>(prv => {
                prv.Name = "IfcPropertyReferenceValue:Material";
                prv.PropertyReference = ifcMaterial;
            });

            var ifcMaterialList = model.Instances.New<IfcMaterialList>(ml => {
                ml.Materials.Add(ifcMaterial);
                ml.Materials.Add(model.Instances.New<IfcMaterial>(m => { m.Name = "Cavity"; }));
                ml.Materials.Add(model.Instances.New<IfcMaterial>(m => { m.Name = "Block"; }));
            });

            var ifcMaterialLayer = model.Instances.New<IfcMaterialLayer>(ml => {
                ml.Material = ifcMaterial;
                ml.LayerThickness = 100.0;
            });
            var ifcPrValueMatLayer = model.Instances.New<IfcPropertyReferenceValue>(prv => {
                prv.Name = "IfcPropertyReferenceValue:MaterialLayer";
                prv.PropertyReference = ifcMaterialLayer;
            });

            var ifcDocumentReference = model.Instances.New<IfcDocumentReference>(dr => {
                dr.Name = "Document";
                dr.Location = "c://Documents//TheDoc.Txt";
            });
            var ifcPrValueRef = model.Instances.New<IfcPropertyReferenceValue>(prv => {
                prv.Name = "IfcPropertyReferenceValue:Document";
                prv.PropertyReference = ifcDocumentReference;
            });

            var ifcTimeSeries = model.Instances.New<IfcRegularTimeSeries>(ts => {
                ts.Name = "Regular Time Series";
                ts.Description = "Time series of events";
                ts.StartTime = new IfcDateTime("2015-02-14T12:01:01");
                ts.EndTime = new IfcDateTime("2015-05-15T12:01:01");
                ts.TimeSeriesDataType = IfcTimeSeriesDataTypeEnum.CONTINUOUS;
                ts.DataOrigin = IfcDataOriginEnum.MEASURED;
                ts.TimeStep = 604800; //7 days in secs
            });

            var ifcPrValueTimeSeries = model.Instances.New<IfcPropertyReferenceValue>(prv => {
                prv.Name = "IfcPropertyReferenceValue:TimeSeries";
                prv.PropertyReference = ifcTimeSeries;
            });

            var ifcAddress = model.Instances.New<IfcPostalAddress>(a => {
                a.InternalLocation = "Room 101";
                a.AddressLines.AddRange(new[] { new IfcLabel("12 New road"), new IfcLabel("DoxField") });
                a.Town = "Sunderland";
                a.PostalCode = "DL01 6SX";
            });
            var ifcPrValueAddress = model.Instances.New<IfcPropertyReferenceValue>(prv => {
                prv.Name = "IfcPropertyReferenceValue:Address";
                prv.PropertyReference = ifcAddress;
            });
            var ifcTelecomAddress = model.Instances.New<IfcTelecomAddress>(a => {
                a.TelephoneNumbers.Add(new IfcLabel("01325 6589965"));
                a.ElectronicMailAddresses.Add(new IfcLabel("bob@bobsworks.com"));
            });
            var ifcPrValueTelecom = model.Instances.New<IfcPropertyReferenceValue>(prv => {
                prv.Name = "IfcPropertyReferenceValue:Telecom";
                prv.PropertyReference = ifcTelecomAddress;
            });

            //lets create the IfcElementQuantity
            var ifcPropertySet = model.Instances.New<IfcPropertySet>(ps => {
                ps.Name = "Test:IfcPropertySet";
                ps.Description = "Property Set";
                ps.HasProperties.Add(ifcPropertySingleValue);
                ps.HasProperties.Add(ifcPropertyEnumeratedValue);
                ps.HasProperties.Add(ifcPropertyBoundedValue);
                ps.HasProperties.Add(ifcPropertyTableValue);
                ps.HasProperties.Add(ifcPropertyListValue);
                ps.HasProperties.Add(ifcPrValueMaterial);
                ps.HasProperties.Add(ifcPrValueMatLayer);
                ps.HasProperties.Add(ifcPrValueRef);
                ps.HasProperties.Add(ifcPrValueTimeSeries);
                ps.HasProperties.Add(ifcPrValueAddress);
                ps.HasProperties.Add(ifcPrValueTelecom);
            });

            //need to create the relationship
            model.Instances.New<IfcRelDefinesByProperties>(rdbp => {
                rdbp.Name = "Property Association";
                rdbp.Description = "IfcPropertySet associated to wall";
                rdbp.RelatedObjects.Add(wall);
                rdbp.RelatingPropertyDefinition = ifcPropertySet;
            });
        }

        public static void CreateElementQuantity(IfcStore model, IfcWallStandardCase wall) {
            //Create a IfcElementQuantity
            //first we need a IfcPhysicalSimpleQuantity,first will use IfcQuantityLength
            var ifcQuantityArea = model.Instances.New<IfcQuantityLength>(qa => {
                qa.Name = "IfcQuantityArea:Area";
                qa.Description = "";
                qa.Unit = model.Instances.New<IfcSIUnit>(siu => {
                    siu.UnitType = IfcUnitEnum.LENGTHUNIT;
                    siu.Prefix = IfcSIPrefix.MILLI;
                    siu.Name = IfcSIUnitName.METRE;
                });
                qa.LengthValue = 100.0;

            });
            //next quantity IfcQuantityCount using IfcContextDependentUnit
            var ifcContextDependentUnit = model.Instances.New<IfcContextDependentUnit>(cd => {
                cd.Dimensions = model.Instances.New<IfcDimensionalExponents>(de => {
                    de.LengthExponent = 1;
                    de.MassExponent = 0;
                    de.TimeExponent = 0;
                    de.ElectricCurrentExponent = 0;
                    de.ThermodynamicTemperatureExponent = 0;
                    de.AmountOfSubstanceExponent = 0;
                    de.LuminousIntensityExponent = 0;
                });
                cd.UnitType = IfcUnitEnum.LENGTHUNIT;
                cd.Name = "Elephants";
            });
            var ifcQuantityCount = model.Instances.New<IfcQuantityCount>(qc => {
                qc.Name = "IfcQuantityCount:Elephant";
                qc.CountValue = 12;
                qc.Unit = ifcContextDependentUnit;
            });


            //next quantity IfcQuantityLength using IfcConversionBasedUnit
            var ifcConversionBasedUnit = model.Instances.New<IfcConversionBasedUnit>(cbu => {
                cbu.ConversionFactor = model.Instances.New<IfcMeasureWithUnit>(mu => {
                    mu.ValueComponent = new IfcRatioMeasure(25.4);
                    mu.UnitComponent = model.Instances.New<IfcSIUnit>(siu => {
                        siu.UnitType = IfcUnitEnum.LENGTHUNIT;
                        siu.Prefix = IfcSIPrefix.MILLI;
                        siu.Name = IfcSIUnitName.METRE;
                    });

                });
                cbu.Dimensions = model.Instances.New<IfcDimensionalExponents>(de => {
                    de.LengthExponent = 1;
                    de.MassExponent = 0;
                    de.TimeExponent = 0;
                    de.ElectricCurrentExponent = 0;
                    de.ThermodynamicTemperatureExponent = 0;
                    de.AmountOfSubstanceExponent = 0;
                    de.LuminousIntensityExponent = 0;
                });
                cbu.UnitType = IfcUnitEnum.LENGTHUNIT;
                cbu.Name = "Inch";
            });
            var ifcQuantityLength = model.Instances.New<IfcQuantityLength>(qa => {
                qa.Name = "IfcQuantityLength:Length";
                qa.Description = "";
                qa.Unit = ifcConversionBasedUnit;
                qa.LengthValue = 24.0;
            });

            //lets create the IfcElementQuantity
            var ifcElementQuantity = model.Instances.New<IfcElementQuantity>(eq => {
                eq.Name = "Test:IfcElementQuantity";
                eq.Description = "Measurement quantity";
                eq.Quantities.Add(ifcQuantityArea);
                eq.Quantities.Add(ifcQuantityCount);
                eq.Quantities.Add(ifcQuantityLength);
            });

            //need to create the relationship
            model.Instances.New<IfcRelDefinesByProperties>(rdbp => {
                rdbp.Name = "Area Association";
                rdbp.Description = "IfcElementQuantity associated to wall";
                rdbp.RelatedObjects.Add(wall);
                rdbp.RelatingPropertyDefinition = ifcElementQuantity;
            });
        }
    }
}
