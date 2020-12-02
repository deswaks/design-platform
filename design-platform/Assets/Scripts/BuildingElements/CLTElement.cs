using DesignPlatform.Geometry;
using DesignPlatform.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace DesignPlatform.Core {

    /// <summary>
    /// Classifies the quality of a CLT element
    /// </summary>
    public enum CLTQuality {
        /// <summary>Domestic Visual Quality</summary>
        DVQ,
        /// <summary>Industrial Visual Quality</summary>
        IVQ,
        /// <summary>Non-visual Quality</summary>
        NVQ
    }

    /// <summary>
    /// The CLT element represents a structural panel made of CLT (cross laminated timber)
    /// </summary>
    public class CLTElement {

        /// <summary>The start point of the wall and its joint type</summary>
        public (Vector3 point, WallJointType jointType) StartJoint { get; set; } = (new Vector3(), WallJointType.None);

        /// <summary>The end point of the wall and its joint type</summary>
        public (Vector3 point, WallJointType jointType) EndJoint { get; set; } = (new Vector3(), WallJointType.None);

        /// <summary>A list of all the joints along the wall (not start and end) represented by their point and joint type.</summary>
        public List<(Vector3 point, WallJointType jointType)> MidJoints { get; set; } = new List<(Vector3 point, WallJointType jointType)>();

        public List<(Vector3 point, WallJointType jointType)> AllJoints {
            get {
                List<(Vector3 point, WallJointType jointType)> joints = new List<(Vector3 point, WallJointType jointType)>();
                joints.Add(StartJoint);
                joints.AddRange(MidJoints);
                joints.Add(EndJoint);
                return joints;
            }
        }

        /// <summary>The interfaces coincident with this element.</summary>
        public List<Interface> Interfaces { get; private set; } = new List<Interface>();

        /// <summary>The openings on this element.</summary>
        public List<Opening> Openings {
            get { return Interfaces.SelectMany(interFace => interFace.Openings).ToList(); }
        }

        /// <summary>The two dimensional line at the base of this element.</summary>
        public Line Line {
            get {
                return new Line(StartJoint.point, EndJoint.point);
            }
        }

        /// <summary>Total length of the element.</summary>
        public float Length {
            get { return Vector3.Distance(StartJoint.point, EndJoint.point); }
        }

        /// <summary>The height of the heighest adjacent space along the element.</summary>
        public float Height { get; private set; } = 3.0f; // SKAL SUGES FRA NOGET

        /// <summary>The maximum preferred thickness of all the connected faces</summary>
        public float Thickness {
            get {
                float t = Interfaces.Max(i => i.Thickness);
                List<Interface> ifen = Interfaces;
                return Interfaces.Max(i => i.Thickness);
            }
        }

        /// <summary>Defines the visual quality of the CLT element</summary>
        public CLTQuality Quality { get; private set; } = Settings.CLTQuality;

        /// <summary>The area of the CLT element when laid flat.</summary>
        public double Area { 
            get { return Length * Height; } 
        }


        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="interfaces">List of interfaces to base the Element upon.</param>
        public CLTElement(List<Interface> interfaces) {
            Interfaces = interfaces;
        }



        /// <summary>
        /// Sets the start joint
        /// </summary>
        /// <param name="point">Location of the new start joint</param>
        /// <param name="jointType">Type of the new start joint. Defaults to none.</param>
        public void SetStartJoint(Vector3 point, WallJointType jointType = WallJointType.None) {
            StartJoint = (point, jointType);
        }

        /// <summary>
        /// Sets the end joint
        /// </summary>
        /// <param name="point">Location of the new end joint</param>
        /// <param name="jointType">Type of the new end joint. Defaults to none.</param>
        public void SetEndJoint(Vector3 point, WallJointType jointType = WallJointType.None) {
            EndJoint = (point, jointType);
        }

        /// <summary>
        /// Sets the type of the start joint
        /// </summary>
        /// <param name="jointType">Type of the start joint.</param>
        public void SetStartJointType(WallJointType jointType) {
            StartJoint = (StartJoint.point, jointType);
        }

        /// <summary>
        /// Sets the type of the end joint
        /// </summary>
        /// <param name="jointType">Type of the end joint.</param>
        public void SetEndJointType(WallJointType jointType) {
            EndJoint = (EndJoint.point, jointType);
        }

        /// <summary>
        /// Sets the mid joints.
        /// </summary>
        /// <param name="points">List of mid joints on the form List<Vector3>. Their type will default to none.</param>
        public void SetMidJoints(List<Vector3> points) {
            MidJoints = new List<(Vector3 point, WallJointType jointType)>();
            points.ForEach(p => MidJoints.Add((p, WallJointType.None)));

            MidJoints = MidJoints.OrderBy(p => Vector3.Distance(p.point, StartJoint.point)).ToList();
        }

        /// <summary>
        /// Sets the mid joints.
        /// </summary>
        /// <param name="midpoints">List of midjoints on the form (Vector3, WallJointType)</param>
        public void SetMidJoints(List<(Vector3 point, WallJointType jointType)> midpoints) {
            this.MidJoints = midpoints;
            midpoints = midpoints.OrderBy(p => Vector3.Distance(p.point, StartJoint.point)).ToList();
        }

        /// <summary>
        /// Sets the type of the mid joint at the given location
        /// </summary>
        /// <param name="point">Location of mid joint to set.</param>
        /// <param name="jointType">Type of the mid joint.</param>
        public void SetMidJointType(Vector3 point, WallJointType jointType) {
            int index = MidJoints.Select(p => p.point).ToList().IndexOf(point);
            if (index == -1) {
                Debug.LogError("Trying to set midpoint joint of point not found among midpoints");
                return;
            }
            MidJoints[index] = (MidJoints[index].point, jointType);
        }

        /// <summary>
        /// ONLY works if there an X_secondary joint exists at the specific parameter.
        /// </summary>
        /// <param name="splitParameter"></param>
        /// <returns></returns>
        public List<CLTElement> SplitAtParameter(float splitParameter) {

            // Divide the assigned interfaces
            var splitInterfaces = Interfaces.GroupBy(i
                => Line.ParameterAtPoint(i.LocationLine.Midpoint) < splitParameter).ToList();

            // Create new elements
            CLTElement element1 = new CLTElement(splitInterfaces[0].ToList());
            CLTElement element2 = new CLTElement(splitInterfaces[1].ToList());

            // Index of joint point where X-joint is
            int splitIndex = MidJoints.Select(p => p.jointType).ToList().IndexOf(WallJointType.X_Secondary);

            element1.StartJoint = (StartJoint.point, StartJoint.jointType);
            element2.StartJoint = (MidJoints[splitIndex].point, MidJoints[splitIndex].jointType);

            if (!(MidJoints.Count == 1)) {
                element2.SetMidJoints(MidJoints.GetRange(splitIndex, MidJoints.Count - splitIndex));
                element1.SetMidJoints(MidJoints.GetRange(1, splitIndex - 1));
            };

            element1.EndJoint = (MidJoints[splitIndex].point, MidJoints[splitIndex].jointType);
            element2.EndJoint = (EndJoint.point, EndJoint.jointType);

            return new List<CLTElement> { element1, element2 };
        }
    }
}

