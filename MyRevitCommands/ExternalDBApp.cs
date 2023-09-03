﻿using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRevitCommands
{
    internal class ExternalDBApp : IExternalDBApplication
    {
        public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
        {
            application.DocumentChanged -= ElementChangedEvent;
            return ExternalDBApplicationResult.Succeeded;
        }

        public ExternalDBApplicationResult OnStartup(ControlledApplication application)
        {
            try
            {
                // Register event
                application.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(ElementChangedEvent);
            } 
            catch (Exception)
            {
                return ExternalDBApplicationResult.Failed;
            };

            return ExternalDBApplicationResult.Succeeded;
        }

        public void ElementChangedEvent (object sender, DocumentChangedEventArgs args) 
        { 
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Furniture);
            ElementId elementId = args.GetModifiedElementIds(filter).First();
            string name = args.GetTransactionNames().First();

            TaskDialog.Show("Modified element", elementId.ToString() + " changed by " + name);
        }
    }
}