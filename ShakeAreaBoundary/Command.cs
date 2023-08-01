#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;


#endregion

namespace ShakeAreaBoundary
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document curDoc = uidoc.Document;

            // get all the area plans in the project
            List<View> areaViews = new List<View>();

            List<View> areaFloor = Utils.GetAllViewsByCategory(curDoc, "10:Floor Areas");
            List<View> areaFrame = Utils.GetAllViewsByCategory(curDoc, "11:Frame Areas");
            List<View> areaAttic = Utils.GetAllViewsByCategory(curDoc, "12:Attic Areas");

            areaViews.AddRange(areaFloor);
            areaViews.AddRange(areaFrame);
            areaViews.AddRange(areaAttic);

            // get all the area boundary lines in the project

           

            // set some variables

            XYZ leftVector = new XYZ(.25,0,0);
            XYZ rightVector = new XYZ(-.25, 0, 0);

            Transform tfLeft = Transform.CreateTranslation(leftVector);
            Transform tfRight = Transform.CreateTranslation(rightVector);

            // start the transaction

            using (Transaction t = new Transaction(curDoc))
            {
                t.Start("Shake Area Boundary Lines");
                {
                    // loop through each view in the list
                    foreach (View curView in areaViews)
                    {
                        // make the view active view
                        uidoc.ActiveView = curView;

                        // get all area boundary lines in the active view
                        FilteredElementCollector colABLines = new FilteredElementCollector(curDoc, curView.Id)
                            .OfCategory(BuiltInCategory.OST_AreaSchemeLines);

                        // get the first line in the list
                        Element lineToMove = colABLines.FirstElement();

                        // get the location of the line
                        LocationPoint curLocation = lineToMove.Location as LocationPoint;

                        // create a vector to move the line
                        XYZ curPoint = curLocation.Point as XYZ;
                        XYZ newVector = new XYZ(.25 + curPoint.X, curPoint.Y, curPoint.Z);
                        XYZ oldVector = new XYZ(newVector.X - .25, newVector.Y, newVector.Z);

                        // move the line to the left
                        lineToMove.Location.Move(newVector);

                        // move the line back to the right                
                        lineToMove.Location.Move(oldVector);
                    }
                }

                t.Commit();
            }            

            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
