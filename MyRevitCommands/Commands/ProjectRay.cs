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
    public class ProjectRay: IExternalCommand
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

                    // Project Ray
                    LocationPoint locP = element.Location as LocationPoint;
                    XYZ p1 = locP.Point;

                    // Ray
                    XYZ rayd = new XYZ(0, 0, 1);

                    ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Roofs);
                    ReferenceIntersector refI = new ReferenceIntersector(filter, FindReferenceTarget.Face, (View3D)doc.ActiveView);
                    ReferenceWithContext refC = refI.FindNearest(p1, rayd);
                    Reference reference = refC.GetReference();
                    XYZ intPoint = reference.GlobalPoint;
                    Double dist = p1.DistanceTo(intPoint);

                    TaskDialog.Show("Ray", string.Format("Distance to roof {0}", dist));

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
