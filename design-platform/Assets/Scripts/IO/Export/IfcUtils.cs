using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Xbim.Ifc;
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

namespace DesignPlatform.Export {
    public static class IfcUtils {

        public static IfcStore ifcModel;

        /// <summary>
        /// Sets up a local placement element
        /// </summary>
        /// <param name="origin">Origin of the local placement coordinate system</param>
        /// <param name="xDirection">X direction of the local placement coordinate system</param>
        /// <param name="zDirection">Z direction of the local placement coordinate system</param>
        /// <returns></returns>
        public static IfcLocalPlacement CreateLocalPlacement(Vector3 origin,
                                                               Vector3 xDirection,
                                                               Vector3 zDirection) {
            var placementAxis = ifcModel.Instances.New<IfcAxis2Placement3D>();
            placementAxis.Location = ifcModel.Instances.New<IfcCartesianPoint>();
            placementAxis.Location.SetXYZ(origin.x, origin.y, origin.z);
            placementAxis.RefDirection = ifcModel.Instances.New<IfcDirection>();
            placementAxis.RefDirection.SetXYZ(xDirection.x, xDirection.y, xDirection.z);
            placementAxis.Axis = ifcModel.Instances.New<IfcDirection>();
            placementAxis.Axis.SetXYZ(zDirection.x, zDirection.y, zDirection.z);

            var localPlacement = ifcModel.Instances.New<IfcLocalPlacement>();
            localPlacement.RelativePlacement = placementAxis;
            return localPlacement;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlPoints"></param>
        /// <returns></returns>
        public static IfcProfileDef CreateProfile(List<Vector2> controlPoints) {
            var profileCurve = ifcModel.Instances.New<IfcPolyline>();
            foreach (Vector3 point in controlPoints) {
                var ifcPoint = ifcModel.Instances.New<IfcCartesianPoint>();
                ifcPoint.SetXY(point.x, point.y);
                profileCurve.Points.Add(ifcPoint);
            }
            var profile = ifcModel.Instances.New<IfcArbitraryClosedProfileDef>();
            profile.OuterCurve = profileCurve;
            profile.ProfileType = IfcProfileTypeEnum.AREA;
            return profile;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static IfcProfileDef CreateProfile(int width, int height) {
            var profile = ifcModel.Instances.New<IfcRectangleProfileDef>();
            profile.ProfileType = IfcProfileTypeEnum.AREA;
            profile.YDim = height;
            profile.XDim = width;
            profile.Position = ifcModel.Instances.New<IfcAxis2Placement2D>();
            profile.Position.Location = ifcModel.Instances.New<IfcCartesianPoint>();
            profile.Position.Location.SetXY(0, 0);
            return profile;
        }

        /// <summary>
        /// Sets up a full product definition with 3D and 2D body.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="depth"></param>
        /// <param name="extrusionOffset"></param>
        /// <param name="presentationLayer"></param>
        /// <param name="add2D"></param>
        /// <returns></returns>
        public static IfcShapeRepresentation ShapeAsSweptProfile(
                                    IfcProfileDef profile,
                                    int depth,
                                    Vector3 extrusionOffset = new Vector3(),
                                    string presentationLayer = null) {

            // Solid body as extrusion
            var body = ifcModel.Instances.New<IfcExtrudedAreaSolid>();
            body.Depth = depth;
            body.SweptArea = profile;
            body.ExtrudedDirection = ifcModel.Instances.New<IfcDirection>();
            body.ExtrudedDirection.SetXYZ(0, 0, 1);
            body.Position = ifcModel.Instances.New<IfcAxis2Placement3D>();
            body.Position.Location = ifcModel.Instances.New<IfcCartesianPoint>();
            body.Position.Location.SetXYZ(extrusionOffset.x, extrusionOffset.y, extrusionOffset.z);

            // Shape representation (3D)
            var shapeRep = ifcModel.Instances.New<IfcShapeRepresentation>();
            shapeRep.ContextOfItems = ifcModel.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
            shapeRep.RepresentationType = "SweptSolid";
            shapeRep.RepresentationIdentifier = "body";
            shapeRep.Items.Add(body);

            // Presentation Layer Assignment
            if (presentationLayer != null) {
                var ifcPresentationLayerAssignment = ifcModel.Instances.New<IfcPresentationLayerAssignment>();
                ifcPresentationLayerAssignment.Name = presentationLayer;
                ifcPresentationLayerAssignment.AssignedItems.Add(shapeRep);
            }

            return shapeRep;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static IfcShapeRepresentation ShapeAsLinearCurve(int length) {
            // Linear Axis as Poly line
            var curve2D = ifcModel.Instances.New<IfcPolyline>();
            var ifcStartPoint = ifcModel.Instances.New<IfcCartesianPoint>();
            ifcStartPoint.SetXY(-length / 2, 0);
            var ifcEndPoint = ifcModel.Instances.New<IfcCartesianPoint>();
            ifcEndPoint.SetXY(length / 2, 0);
            curve2D.Points.Add(ifcStartPoint);
            curve2D.Points.Add(ifcEndPoint);

            // Shape representation
            var shapeRep = ifcModel.Instances.New<IfcShapeRepresentation>();
            shapeRep.ContextOfItems = ifcModel.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
            shapeRep.RepresentationType = "Curve2D";
            shapeRep.RepresentationIdentifier = "Axis";
            shapeRep.Items.Add(curve2D);

            return shapeRep;
        }
    }
}

