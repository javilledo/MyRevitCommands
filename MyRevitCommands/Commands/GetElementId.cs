using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.ReadOnly)]
    public class GetElementId : IExternalCommand
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
                Reference pickedObj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                
                // Retrieve Element
                ElementId elementId = pickedObj.ElementId;

                Element element = doc.GetElement(elementId);

                // Get Element Type
                ElementId elementTypeId = element.GetTypeId();
                ElementType elementType = doc.GetElement(elementTypeId) as ElementType;

                // Display Element Id
                if (pickedObj != null)
                {
                    TaskDialog.Show("Element Classification", elementId.ToString() + Environment.NewLine
                        + "Category: " + element.Category.Name + Environment.NewLine
                        + "Name: " + element.Name + Environment.NewLine
                        + "Family Name: " + elementType.FamilyName + Environment.NewLine 
                        + "Type Name: " + elementType.Name);
                }

                return Result.Succeeded;

            } catch (Exception e) { 

                message = e.Message;

                return Result.Failed;

            }

        }
    }
}
