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
    public class SelectGeometry: IExternalCommand
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

                    // Get Geometry
                    Options gOptions = new Options();
                    gOptions.DetailLevel = ViewDetailLevel.Fine;
                    GeometryElement geom = element.get_Geometry(gOptions);

                    // Traverse Geometry
                    foreach(GeometryObject geomObj in geom)
                    {
                        Solid gSolid = geomObj as Solid;
                        if (gSolid != null)
                        {
                            int faces = 0;
                            double area = 0.0;

                            foreach(Face face in gSolid.Faces)
                            {
                                area += face.Area;
                                faces++;
                            };

                            area = UnitUtils.ConvertFromInternalUnits(area, UnitTypeId.SquareMeters);

                            TaskDialog.Show("Geometry", string.Format("Number of faces: {0}" + Environment.NewLine + "Total area: {1}", faces, area));

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
