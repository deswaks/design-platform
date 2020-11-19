using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DesignPlatform.Database;
using DesignPlatform.Core;
using System.Numerics;
using System;
using System.Text;

namespace DesignPlatform
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class ImportWalls : IExternalCommand
    {
        const float m_to_ft = 1.0f / 0.3048f;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document uidoc = commandData.Application.ActiveUIDocument.Document;
            Document doc = uiDoc.Document;

            FilteredElementCollector lvlCollector = new FilteredElementCollector(doc);
            ICollection<Element> lvlCollection = lvlCollector.OfClass(typeof(Level)).ToElements();

            List<WallType> wallTypes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType().Cast<WallType>().ToList();
            WallType chosenWallType = wallTypes.First(w => w.Name.Contains("CLT"));

            Element level = lvlCollection.First();
            ElementId levelId = level.Id;


            // Gets all InterfaceNodes from json file:
            List<WallElementNode> interfaceNodes = LocalDatabase.LoadInterfaceNodesFromJson().ToList();

            Transaction t = new Transaction(doc);
            t.Start("Import walls");

            // Loops through all interfaces and creates corresponding wall
            foreach (WallElementNode wallElement in interfaceNodes){
                // Creates wall using endpoints from json
                List<Vector3> endPoints = GraphUtils.StringListToVector3List(wallElement.vertices);

                XYZ start = new XYZ(endPoints[0].X* m_to_ft, endPoints[0].Z* m_to_ft, 0);
                XYZ end   = new XYZ(endPoints[1].X* m_to_ft, endPoints[1].Z* m_to_ft, 0);

                // Adjusts end vertices
                XYZ direction = (end - start).Normalize(); //Vector from start to end point
                if (wallElement.startJointType.Contains("Secondary")) {
                    start += direction * chosenWallType.Width / 2;
                }                
                if (wallElement.startJointType.Contains("Primary")) {
                    start -= direction * chosenWallType.Width / 2;
                }                
                if (wallElement.endJointType.Contains("Secondary")) {
                    end -= direction * chosenWallType.Width / 2;
                }                
                if (wallElement.endJointType.Contains("Primary")) {
                    end += direction * chosenWallType.Width / 2;
                }

                Line curve = Line.CreateBound(start, end);

                Wall wall = Wall.Create(doc, curve, levelId, false);
                wall.WallType = chosenWallType;

                // Disallows wall joints
                WallUtils.DisallowWallJoinAtEnd(wall, 0);
                WallUtils.DisallowWallJoinAtEnd(wall, 1);

                // Sets joint parameters
                wall.LookupParameter("Endpoint Joint").Set(wallElement.endJointType);
                wall.LookupParameter("Startpoint Joint").Set(wallElement.startJointType);
                string midpointJointTypes = "";
                wallElement.midPointJointTypes.ToList().ForEach(jt => midpointJointTypes += jt+" \n");
                wall.LookupParameter("Midpoint Joints").Set(midpointJointTypes);

            }

            t.Commit();

            return Result.Succeeded;
        }
    }

    // The Availability Class must be added before the project can be loaded into Revit:
    public class Availability : IExternalCommandAvailability{
        public bool IsCommandAvailable(UIApplication a, CategorySet b){return true;}
    }
}
