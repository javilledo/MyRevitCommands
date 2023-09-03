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
    public class PlaceLoopElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            // Get UIDocument
            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            // Get Document
            Document doc = uidoc.Document;

            // Get Level
            Level level = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Levels)
                .WhereElementIsNotElementType()
                .Cast<Level>()
                .First(x => x.Name == "Ground Floor");

            // Get Floor Type
            FloorType floorType = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Floors)
                .WhereElementIsElementType()
                .Cast<FloorType>()
                .First(x => x.Name == "Generic 150mm");

            // Create Points
            XYZ p1 = new XYZ(-10, -10, 0);
            XYZ p2 = new XYZ(10, -10, 0);
            XYZ p3 = new XYZ(15, 0, 0);
            XYZ p4 = new XYZ(10, 10, 0);
            XYZ p5 = new XYZ(-10, 10, 0);

            // Create Curves
            Line l1 = Line.CreateBound(p1, p2);
            Arc l2 = Arc.Create(p2, p4, p3);
            Line l3 = Line.CreateBound(p4, p5);
            Line l4 = Line.CreateBound(p5, p1);

            // Create Curve Loop
            CurveLoop curveLoop = new CurveLoop();
            curveLoop.Append(l1); 
            curveLoop.Append(l2); 
            curveLoop.Append(l3);
            curveLoop.Append(l4);
            double offset = UnitUtils.ConvertToInternalUnits(135, UnitTypeId.Millimeters);
            CurveLoop offsetCurveLoop = CurveLoop.CreateViaOffset(curveLoop, offset, new XYZ(0, 0, 1));

            try
            {

                using (Transaction transaction = new Transaction(doc, "Place Loop Element"))
                {
                    transaction.Start();

                    // Create Floor
                    Floor.Create(doc, new List<CurveLoop> { offsetCurveLoop }, floorType.Id, level.Id);

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
