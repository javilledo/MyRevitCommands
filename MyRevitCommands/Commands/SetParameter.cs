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
    public class SetParameter: IExternalCommand
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

                if (pickedObj != null)
                {

                    // Retrieve Element
                    ElementId elementId = pickedObj.ElementId;
                    Element element = doc.GetElement(elementId);

                    // Get Parameter Value
                    Parameter param = element.get_Parameter(BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM);

                    TaskDialog.Show("Parameter Values", string.Format("Parameter storage type {0} and value {1}",
                        param.StorageType.ToString(),
                        param.AsDouble()));

                    using (Transaction transaction = new Transaction(doc, "Set Parameter"))
                    {
                        transaction.Start();

                        param.Set(7.5);

                        transaction.Commit();

                    };


                }
                
                return Result.Succeeded;

            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }

        }
    }
}
