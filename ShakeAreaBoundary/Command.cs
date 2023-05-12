#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Document doc = uidoc.Document;

            // get all the area boundary lines in the project

            FilteredElementCollector colABLines = new FilteredElementCollector(doc);
            colABLines.OfCategory(BuiltInCategory.OST_AreaSchemeLines);

            // set some variables

            XYZ leftVector = new XYZ(.25,0,0);
            XYZ rightVector = new XYZ(-.25, 0, 0);

            Transform tfLeft = Transform.CreateTranslation(leftVector);
            Transform tfRight = Transform.CreateTranslation(rightVector);

            // start the transaction

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Shake Area Boundary Lines");

                // move them to the left

                foreach (Element curElem in colABLines)
                {
                    // move it to the left 3 inches
                }

                // move them back to the right

                foreach (Element curElem in colABLines)
                {
                    // move it to the right 3 inches
                }

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
