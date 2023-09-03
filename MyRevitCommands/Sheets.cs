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
    public class Sheets: IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            // Get UIDocument and Document
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Get Family Symbol
            FamilySymbol tBlock = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .WhereElementIsElementType()
                .Cast<FamilySymbol>()
                .First();

            try
            {

                using (Transaction transaction = new Transaction(doc, "Create Sheet"))
                {
                    transaction.Start();

                    // Create Sheet
                    ViewSheet vSheet = ViewSheet.Create(doc, tBlock.Id);
                    vSheet.Name = "My first Sheet";
                    vSheet.SheetNumber = "J101";

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
