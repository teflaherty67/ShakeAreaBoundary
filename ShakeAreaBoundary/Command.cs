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
using System.Windows.Controls;


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

            // start the transaction

            using (Transaction t = new Transaction(curDoc))
            {
                t.Start("Shake Area Boundary Lines");
                {
                    // loop through each view in the list
                    foreach (View curView in areaViews)
                    {
                        // get all area boundary lines in the active view
                        IEnumerable<Element> colABLines = new FilteredElementCollector(curDoc, curView.Id)
                            .OfCategory(BuiltInCategory.OST_AreaSchemeLines)
                            .WhereElementIsNotElementType();

                        if (colABLines.Count() > 0)
                        {
                            // get the first line in the list
                            Element lineToMove = colABLines.FirstOrDefault();

                            // get the location of the line
                            LocationCurve curLocation = lineToMove.Location as LocationCurve;

                            // create a vector to move the line
                            XYZ curPoint = curLocation.Curve.GetEndPoint(0) as XYZ;
                            XYZ newVector = new XYZ(.25 + curPoint.X, curPoint.Y, curPoint.Z);
                            XYZ oldVector = new XYZ(newVector.X - .25, newVector.Y, newVector.Z);

                            // move the line to the left
                            lineToMove.Location.Move(newVector);

                            // move the line back to the right                
                            lineToMove.Location.Move(oldVector);
                        }
                        else
                            continue;
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
