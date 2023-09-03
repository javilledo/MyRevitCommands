using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Collections;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class PlaceView: IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            // Get UIDocument and Document
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Find Sheet
            ViewSheet vSheet = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Sheets)
                .WhereElementIsNotElementType()
                .Cast<ViewSheet>()
                .First(x => x.Name == "My First Sheet");

            // Find View
            Element vPlan = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Views)
                .WhereElementIsNotElementType()
                .First(x => x.Name == "Our first plan!");

            // Get Midpoint
            BoundingBoxUV outline = vSheet.Outline;
            double u = (outline.Max.U + outline.Min.U) / 2;
            double v = (outline.Max.V + outline.Min.V) / 2;
            XYZ midpoint = new XYZ(u, v, 0);

            try
            {

                using (Transaction transaction = new Transaction(doc, "Create Sheet"))
                {
                    transaction.Start();

                    // Place View
                    Viewport vPort = Viewport.Create(doc, vSheet.Id, vPlan.Id, midpoint);


                    transaction.Commit();

                    return Result.Succeeded;

                };

            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }

        }
    }
}
