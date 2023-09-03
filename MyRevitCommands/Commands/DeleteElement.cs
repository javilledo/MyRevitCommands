using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Events;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class DeleteElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get UIDocument
            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            // Get Document
            Document doc = uidoc.Document;

            try
            {
                // Pick Object
                Reference pickedObject = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

                // Delete Element
                if(pickedObject != null)
                {
                    using (Transaction transaction = new Transaction(doc, "Delete Element"))
                    {
                        transaction.Start();
                        doc.Delete(pickedObject.ElementId);

                        TaskDialog taskDialog = new TaskDialog("Delete element");
                        taskDialog.MainContent = "Are you sure you want to delete the element?";
                        taskDialog.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;

                        if(taskDialog.Show() == TaskDialogResult.Ok)
                        {
                            transaction.Commit();
                            TaskDialog.Show("Delete Element", pickedObject.ElementId.ToString() + " deleted!");
                        }
                        else
                        {
                            transaction.RollBack();
                            TaskDialog.Show("Delete Element", pickedObject.ElementId.ToString() + " NOT deleted!");

                        }

                    };
                };

            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }





            return Result.Succeeded;
        }
    }
}
