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
    public class ChangeLocation: IExternalCommand
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

                    using (Transaction transaction = new Transaction(doc, "Change Location"))
                    {

                        // Set Location
                        LocationPoint locp = element.Location as LocationPoint;

                        if (locp != null)
                        {
                            transaction.Start();

                            XYZ loc = locp.Point;
                            XYZ newloc = new XYZ(loc.X + 3, loc.Y, loc.Z);
                            locp.Point = newloc;

                            transaction.Commit();

                        };

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
