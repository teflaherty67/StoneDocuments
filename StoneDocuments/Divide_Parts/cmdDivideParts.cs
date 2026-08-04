using StoneDocuments.Common;

namespace StoneDocuments
{
    [Transaction(TransactionMode.Manual)]
    public class cmdDivideParts : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document curDoc = uidoc.Document;

            vmDivideParts viewModel = new vmDivideParts(curDoc);

            SelectWallsHandler selectHandler = new SelectWallsHandler(viewModel);
            ExternalEvent selectEvent = ExternalEvent.Create(selectHandler);

            CreatePartsHandler createHandler = new CreatePartsHandler(viewModel);
            ExternalEvent createEvent = ExternalEvent.Create(createHandler);

            frmDivideParts curForm = new frmDivideParts(viewModel, selectEvent, createEvent)
            {
                Width = 400,
                Height = 360,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            selectHandler.Form = curForm;

            curForm.Show();

            return Result.Succeeded;
        }

        public class SelectWallsHandler : IExternalEventHandler
        {
            private readonly vmDivideParts viewModel;

            internal frmDivideParts Form { get; set; }

            internal SelectWallsHandler(vmDivideParts vm)
            {
                viewModel = vm;
            }

            public string GetName()
            {
                return "Select walls for Divide Parts";
            }

            public void Execute(UIApplication uiapp)
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;

                try
                {
                    IList<Reference> picked = uidoc.Selection.PickObjects(
                        ObjectType.Element,
                        new WallSelectionFilter(),
                        "Select walls to create & divide parts, then click Finish");

                    viewModel.SelectedWallIds = picked.Select(r => r.ElementId).ToList();
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    // user pressed Escape - keep the previous selection
                }
                finally
                {
                    Form?.OnWallsSelected();
                }
            }
        }

        public class CreatePartsHandler : IExternalEventHandler
        {
            private readonly vmDivideParts viewModel;

            internal CreatePartsHandler(vmDivideParts vm)
            {
                viewModel = vm;
            }

            public string GetName()
            {
                return "Create & divide parts";
            }

            public void Execute(UIApplication uiapp)
            {
                string resultMessage = viewModel.Run();

                Utils.TaskDialogInformation("Divide Parts", "Divide Parts", resultMessage);
            }
        }

        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCmd1_3";
            string buttonTitle = "Divide\rParts";

            clsButtonData myButtonData = new clsButtonData(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.DivideParts_32,
                Properties.Resources.DivideParts_16,
                "Creates parts from selected walls and divides them using named reference planes.");

            return myButtonData.Data;
        }
    }
}
