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
    public class ViewFilter: IExternalCommand
    {
        [Obsolete]
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            // Get UIDocument and Document
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Create Filter
            List<ElementId> cats = new List<ElementId>();
            cats.Add(new ElementId(BuiltInCategory.OST_Sections));

            ElementParameterFilter filter = new ElementParameterFilter(ParameterFilterRuleFactory.CreateContainsRule(new ElementId(BuiltInParameter.VIEW_NAME), "WIP", false));

            try
            {

                using (Transaction transaction = new Transaction(doc, "Create Plan View"))
                {
                    transaction.Start();

                    // Apply Filter
                    ParameterFilterElement filterElement = ParameterFilterElement.Create(doc, "My first filter", cats, filter);
                    doc.ActiveView.AddFilter(filterElement.Id);
                    doc.ActiveView.SetFilterVisibility(filterElement.Id, false);    

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
