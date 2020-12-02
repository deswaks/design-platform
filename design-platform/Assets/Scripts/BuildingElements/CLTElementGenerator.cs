using DesignPlatform.Geometry;
using DesignPlatform.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace DesignPlatform.Core {


    public partial class Building {

        /// <summary> All CLT elements of this building </summary>
        private List<CLTElement> cltElements = new List<CLTElement>();

        /// <summary>
        /// All CLT elements of this building.
        /// </summary>
        public List<CLTElement> CLTElements {
            get {
                if (cltElements == null || cltElements.Count == 0) BuildAllCLTElements();
                return cltElements;
            }
        }

        /// <summary>
        /// Builds all CLT elements of the whole building.
        /// </summary>
        /// <returns>All CLT elements of the building.</returns>
        public static List<CLTElement> BuildAllCLTElements() {

            // Find relevant interfaces to base the elements upon
            List<Interface> interfaces = Instance.InterfacesVertical.Where(i => i.EndPoint != i.StartPoint).ToList();

            // Create full-length continuous elements (with no joint information yet)
            List<CLTElement> continuousElements = CreateContinuousCLTElements(interfaces);

            // Classify all the joints
            ClassifyAllJoints(continuousElements);

            // Split elements that are part of an X joint
            List<CLTElement> splitElements = new List<CLTElement>();
            foreach (CLTElement element in continuousElements) {

                if (element.MidJoints.Any(j => j.jointType == WallJointType.X_Secondary)) {
                    Vector3 xJointPoint = element.MidJoints.Find(j => j.jointType == WallJointType.X_Secondary).point;
                    float xJointParameter = element.Line.ParameterAtPoint(xJointPoint);
                    List<CLTElement> elementParts = element.SplitAtParameter(xJointParameter);
                    splitElements.Add(elementParts[0]); splitElements.Add(elementParts[1]);
                }

                else splitElements.Add(element);
            }

            // Add to building
            Instance.cltElements = splitElements;
            return Instance.cltElements;
        }

        /// <summary>
        /// Classifies all the joints for all the given CLT elements.
        /// </summary>
        /// <param name="cltElements">CLT elements whose joints shoudl be classified.</param>
        private static void ClassifyAllJoints(List<CLTElement> cltElements) {
            List<Vector3> distinctJointPoints = cltElements.SelectMany(element => element.AllJoints.Select(joint => joint.point)).Distinct().ToList();

            foreach (Vector3 point in distinctJointPoints) {

                // Finds wall elements that have an endpoint in the given point and elements with midpoint(s) in the given point
                // In total, only two elements should always be found. 
                List<CLTElement> endJointWallElements = cltElements.Where(w => w.StartJoint.point == point || w.EndJoint.point == point).ToList();
                List<CLTElement> midJointWallElements = cltElements.Where(w => w.MidJoints.Select(p => p.point).Contains(point)).ToList();

                // X-joint                                         // PRIMARY ELEMENT IS CHOSEN AS THE LONGEST ELEMENT
                if (midJointWallElements.Count == 2) {
                    CLTElement primaryElement = midJointWallElements.OrderBy(e => e.Length).Last();
                    CLTElement secondaryElement = midJointWallElements.OrderBy(e => e.Length).First();

                    if (primaryElement.MidJoints.Select(p => p.point).Contains(point)) primaryElement.SetMidJointType(point, WallJointType.X_Primary);
                    if (secondaryElement.MidJoints.Select(p => p.point).Contains(point)) secondaryElement.SetMidJointType(point, WallJointType.X_Secondary);
                }

                // T-joint
                else if (midJointWallElements.Count == 1) {
                    midJointWallElements.First().SetMidJointType(midJointWallElements.First().MidJoints.First(p => p.point == point).point, WallJointType.T_Primary);
                    if (point == endJointWallElements.First().StartJoint.point) endJointWallElements.First().SetStartJointType(WallJointType.T_Secondary);
                    if (point == endJointWallElements.First().EndJoint.point) endJointWallElements.First().SetEndJointType(WallJointType.T_Secondary);
                }

                // L-joint                                         // PRIMARY ROLE IS ASSIGNED TO THE LONGEST ELEMENT
                else if (endJointWallElements.Count == 2) {
                    CLTElement primaryElement = endJointWallElements.OrderBy(e => e.Length).Last();
                    CLTElement secondaryElement = endJointWallElements.OrderBy(e => e.Length).First();

                    if (point == primaryElement.StartJoint.point) primaryElement.SetStartJointType(WallJointType.Corner_Primary);
                    if (point == primaryElement.EndJoint.point) primaryElement.SetEndJointType(WallJointType.Corner_Primary);

                    if (point == secondaryElement.StartJoint.point) secondaryElement.SetStartJointType(WallJointType.Corner_Secondary);
                    if (point == secondaryElement.EndJoint.point) secondaryElement.SetEndJointType(WallJointType.Corner_Secondary);
                }

            }
        }

        /// <summary>
        /// Identifies parallel joint walls and combines them (avoiding each wall being split by small, separate interfaces), providing vertex pairs for the resulting full wall elements. 
        /// </summary>
        /// <returns>Returns full, un-split, wall elements with no joint information.</returns>
        private static List<CLTElement> CreateContinuousCLTElements(List<Interface> interfaces) {

            List<CLTElement> output = new List<CLTElement>();

            // Group interfaces making up a single continuous element
            List<List<Interface>> groupedInterfaces = GroupAdjoiningInterfaces(interfaces);

            // Join the interfaces in each group
            foreach (List<Interface> adjoiningInterfaces in groupedInterfaces) {
                CLTElement cltElement = CreateCLTElementByJoiningInterfaces(adjoiningInterfaces);
                output.Add(cltElement);
            }

            return output;
        }

        /// <summary>
        /// Creates a CLT element that covers all the given interfaces.
        /// </summary>
        /// <param name="interfaces">List of adjoining parallel interfaces.</param>
        /// <returns>Continuous CLT element that covers all the given interfaces.</returns>
        private static CLTElement CreateCLTElementByJoiningInterfaces(List<Interface> interfaces) {
            CLTElement cltElement = new CLTElement(interfaces);

            List<Vector3> currentWallVertices = interfaces.SelectMany(i => new List<Vector3> { i.EndPoint, i.StartPoint }).Distinct().ToList();
            
            List<float> xValues = currentWallVertices.Select(p => p.x).ToList();
            List<float> zValues = currentWallVertices.Select(p => p.z).ToList();

            // Endpoints are (Xmin,0,Zmin);(Xmax,0,Zmax) ////////////////// ONLY TRUE FOR WALLS LYING ALONG X-/Z-AXIS
            cltElement.SetStartJoint(new Vector3(xValues.Min(), 0, zValues.Min()));
            cltElement.SetEndJoint(new Vector3(xValues.Max(), 0, zValues.Max()));

            // Removes endpoints from vertex-list so that only midpoints remain
            currentWallVertices.RemoveAll(p => p == cltElement.StartJoint.point || p == cltElement.EndJoint.point);
            cltElement.SetMidJoints(currentWallVertices);

            if (cltElement.Length > 16.5) Debug.Log("Panel surpasses maximum length of 16.5m");

            return cltElement;
        }

        /// <summary>
        /// Groups all interfaces that are parallel and connected.
        /// </summary>
        /// <param name="interfaces">Interfaces to group.</param>
        /// <returns>Grouped interfaces.</returns>
        public static List<List<Interface>> GroupAdjoiningInterfaces(List<Interface> interfaces) {

            // Group number for each interface is initialized as -1
            List<int> groupNumbers = Enumerable.Repeat(-1, interfaces.Count).ToList();

            // Find group number for each interface
            for (int j = 0; j < interfaces.Count; j++) {
                // Find adjoining interfaces of current interface
                List<Interface> adjoiningInterfaces = FindParallelAndConnectedInterfaces(interfaces, interfaces[j]);

                // Sees if one of parallel interfaces belongs to a wall and saves its ID
                int? currentGroup = adjoiningInterfaces.Select(i => groupNumbers[interfaces.IndexOf(i)])
                    .Where(group => group != -1)?.FirstOrDefault();

                // If no one interface already has the ID attached a new one is created, otherwise all identified parallel walls gets this ID.
                if (currentGroup == 0) currentGroup = groupNumbers.Max() + 1;
                adjoiningInterfaces.ForEach(i => groupNumbers[interfaces.IndexOf(i)] = currentGroup.Value);
                groupNumbers[j] = currentGroup.Value;
            }

            // Group the interfaces
            IEnumerable<IGrouping<int, Interface>> groupedInterfaces = interfaces.GroupBy(i => groupNumbers[interfaces.IndexOf(i)]);

            return groupedInterfaces.Select(iGroup => iGroup.ToList()).ToList();
        }

        /// <summary>
        /// Finds all the interfaces that are parallel and connected to the given interface.
        /// </summary>
        /// <param name="interfaces">Interfaces to search among.</param>
        /// <param name="thisIF">Interface to compare to.</param>
        /// <returns></returns>
        private static List<Interface> FindParallelAndConnectedInterfaces(List<Interface> interfaces, Interface thisIF) {

            List<Interface> output = new List<Interface>();

            output.AddRange(interfaces.Where(otherIF =>
                   thisIF != otherIF
                && thisIF.LocationLine.IsParallelTo(otherIF.LocationLine)
                && thisIF.LocationLine.IsConnectedTo(otherIF.LocationLine)).ToList());

            return output;
        }


        /// <summary>
        /// Removes all interfaces of the whole building.
        /// </summary>
        public void DeleteAllCLTElements() {
            foreach (CLTElement cltElement in CLTElements) {
                DeleteCLTElement(cltElement);
            }
        }

        /// <summary>
        /// Removes a CLT element from the building.
        /// </summary>
        public void DeleteCLTElement(CLTElement cltElement) {
            if (cltElements.Contains(cltElement)) cltElements.Remove(cltElement);
        }

    }
}