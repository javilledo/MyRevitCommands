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
    public class GetParameter: IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            // Get UIDocument and Document
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;


            try
            {

                // Pick Object
                Reference pickedObj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

                using (Transaction transaction = new Transaction(doc, "Place Family"))
                {
                    transaction.Start();

                    // Retrieve Element
                    ElementId elementId = pickedObj.ElementId;
                    Element element = doc.GetElement(elementId);

                    // Get Parameter
                    Parameter param = element.LookupParameter("Altura de extremo inicial");
                    InternalDefinition paramDef = param.Definition as InternalDefinition;

                    TaskDialog.Show("Parameters: ", string.Format("{0} parameter of type {1} with builtinparameter {2}",
                        paramDef.Name,
                        paramDef.GetType().Name,
                        paramDef.BuiltInParameter));

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
