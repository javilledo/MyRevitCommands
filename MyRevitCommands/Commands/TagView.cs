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
    public class TagView: IExternalCommand
    {
        [Obsolete]
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            // Get UIDocument and Document
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Tag Parameters
            TagMode tmode = TagMode.TM_ADDBY_CATEGORY;
            TagOrientation tOrient = TagOrientation.Horizontal;

            List<BuiltInCategory> cats = new List<BuiltInCategory>();
            cats.Add(BuiltInCategory.OST_Windows);
            cats.Add(BuiltInCategory.OST_Doors);

            ElementMulticategoryFilter filter = new ElementMulticategoryFilter(cats);

            IList<Element> tElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .WherePasses(filter)
                .WhereElementIsNotElementType()
                .ToElements();

            try
            {

                using (Transaction transaction = new Transaction(doc, "Create Plan View"))
                {
                    transaction.Start();

                    // Tag Elements
                    foreach (Element element in tElements)
                    {
                        Reference refElement = new Reference(element);
                        LocationPoint loc = element.Location as LocationPoint;
                        XYZ point = loc.Point;
                        IndependentTag tag = IndependentTag.Create(doc, doc.ActiveView.Id, refElement, true, tmode, tOrient, point);
                    }

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
