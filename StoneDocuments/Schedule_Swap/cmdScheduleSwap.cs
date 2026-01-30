using StoneDocuments.Common;

namespace StoneDocuments
{
    [Transaction(TransactionMode.Manual)]
    public class cmdScheduleSwap : IExternalCommand
    {
        public ViewSheet curSheet;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Revit application and document variables
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document curDoc = uidoc.Document;

            // check current view - make sure it's a sheet

            if (!(curDoc.ActiveView is ViewSheet))
            {
                TaskDialog.Show("Error", "Please make the active view a sheet");
                return Result.Failed;
            }

            curSheet = curDoc.ActiveView as ViewSheet;

            if (Utils.SheetHasSchedule(curDoc, curSheet) == false)
            {
                TaskDialog.Show("Error", "The current sheet does not have a schedule. Please select another sheet.");
                return Result.Failed;
            }

            // open form
            frmScheduleSwap curForm = new frmScheduleSwap(uiapp)
            {
                Width = 450,
                Height = 200,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            curForm.ShowDialog();

            return Result.Succeeded;
        }
        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCmd2_1";
            string buttonTitle = "Schedule\rSwapper";

            clsButtonData myButtonData = new clsButtonData(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.ScheduleSwap_32,
                Properties.Resources.ScheduleSwap_16,
                "Replaces the selected schedule with a user specified schedule.");

            return myButtonData.Data;
        }
    }
}
