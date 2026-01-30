using StoneDocuments.Common;

namespace StoneDocuments
{
    [Transaction(TransactionMode.Manual)]
    public class cmdCheck : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Revit application and document variables
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document curDoc = uidoc.Document;

            // check current view - make sure it's a sheet
            ViewSheet curSheet;
            if (curDoc.ActiveView is ViewSheet)
            {
                curSheet = curDoc.ActiveView as ViewSheet;
            }
            else
            {
                TaskDialog.Show("Error", "Please make the active view a sheet");
                return Result.Failed;
            }

            // get schedule from sheet
            List<ViewSchedule> schedList = Utils.GetAllSchedulesOnSheet(curDoc, curSheet);

            // check if sheet has schedule
            if (schedList.Count == 0)
            {
                TaskDialog.Show("Error", "The current sheet does not have a schedule. Please select another sheet.");
                return Result.Failed;
            }

            // get elements from schedule
            List<Element> elemList = Utils.GetElementsFromSchedule(curDoc, schedList[0]);
            List<ElementId> elemIdList = new List<ElementId>();

            if (Utils.DoesElementListContainAssemblies(curDoc, elemList) == true)
            {
                // loop through each assemebly instance in the list
                foreach (AssemblyInstance curAssembly in elemList)
                {
                    List<ElementId> curAsmId = curAssembly.GetMemberIds().ToList();
                    elemIdList.AddRange(curAsmId);
                }
                // get the element Ids of the assembly members
            }
            else
            {
                elemIdList = Utils.GetElementIdsFromList(curDoc, elemList);
            }

            string userName = uiapp.Application.Username;

            // set current view to 3D view
            View curView;

            if (curDoc.IsWorkshared == true)
                curView = Utils.GetViewByName(curDoc, "{3D - " + userName + "}");
            else
                curView = Utils.GetViewByName(curDoc, "{3D}");

            if (curView == null)
            {
                TaskDialog tdNullView = new TaskDialog("Error");
                tdNullView.MainIcon = Icon.TaskDialogIconInformation;
                tdNullView.Title = "Null View";
                tdNullView.TitleAutoPrefix = false;
                tdNullView.MainContent = "The default 3D view does not exist. Please create it and set the properties as required and try again.";
                tdNullView.CommonButtons = TaskDialogCommonButtons.Close;

                TaskDialogResult tdNullViewRes = tdNullView.Show();

                return Result.Failed;
            }

            uidoc.ActiveView = curView;

            uidoc.Selection.SetElementIds(elemIdList);

            // create handler and event then open form
            RequestHandler rHandler = new RequestHandler();
            ExternalEvent exEvent = ExternalEvent.Create(rHandler);
            CancelHandler cHandler = new CancelHandler();
            ExternalEvent cEvent = ExternalEvent.Create(cHandler);

            frmCheck curForm = new frmCheck(exEvent, rHandler, cHandler, cEvent, elemList.Count)
            {
                Width = 365,
                Height = 150,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            curForm.Show();

            return Result.Succeeded;
        }

        public class RequestHandler : IExternalEventHandler
        {
            public String GetName()
            {
                return "Change selected element overrides";
            }
            public void Execute(UIApplication uiapp)
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                // set override settings
                Autodesk.Revit.DB.Color color = new Autodesk.Revit.DB.Color(255, 0, 0);
                OverrideGraphicSettings colSet = new OverrideGraphicSettings();
                colSet.SetSurfaceForegroundPatternColor(color);

                FillPatternElement curFPE = Utils.GetFillPatternByName(doc, "<Solid fill>");
                colSet.SetSurfaceForegroundPatternId(curFPE.Id);

                ICollection<ElementId> selElements = uidoc.Selection.GetElementIds();

                // update element overrides in view
                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Set element color");

                    foreach (ElementId curId in selElements)
                    {
                        doc.ActiveView.SetElementOverrides(curId, colSet);
                    }

                    t.Commit();
                }

                selElements.Clear();

                uidoc.Selection.SetElementIds(selElements);

                uidoc.RefreshActiveView();

                return;
            }
        }

        public class CancelHandler : IExternalEventHandler
        {
            public String GetName()
            {
                return "Cancel and close form";
            }

            public void Execute(UIApplication uiapp)
            {

                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                ICollection<ElementId> selElements = uidoc.Selection.GetElementIds();

                selElements.Clear();

                uidoc.Selection.SetElementIds(selElements);

                uidoc.RefreshActiveView();

                return;
            }
        }

        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCmd1_1";
            string buttonTitle = "Check\rParts";

            clsButtonData myButtonData = new clsButtonData(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Check_32,
                Properties.Resources.Check_16,
                "Check parts by schedule and overrides surface foreground color.");

            return myButtonData.Data;
        }
    }
}
