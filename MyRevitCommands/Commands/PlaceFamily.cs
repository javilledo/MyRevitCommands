using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Xml.Linq;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class PlaceFamily : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            // Get UIDocument
            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            // Get Document
            Document doc = uidoc.Document;

            // Get Family Symbol
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            //IList<Element> symbols = collector.OfClass(typeof(FamilySymbol)).WhereElementIsElementType().ToElements();
            FamilySymbol familySymbol = collector.OfClass(typeof(FamilySymbol))
                .WhereElementIsElementType()
                .Cast<FamilySymbol>()
                .First(x => x.Name == "1525 x 762mm");

            //FamilySymbol familySymbol = null;
            //foreach (Element element in symbols)
            //{
            //    if(element.Name == "1525 x 762mm")
            //    {
            //        familySymbol = element as FamilySymbol;
            //        break;
            //    }
            //}

            try
            {

                using (Transaction transaction = new Transaction(doc, "Place Family"))
                {
                    transaction.Start();

                    if(!familySymbol.IsActive)
                    {
                        familySymbol.Activate();
                    }

                    doc.Create.NewFamilyInstance(new XYZ(0, 0, 0), familySymbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

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
