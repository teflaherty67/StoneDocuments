
using StoneDocuments.Common;

namespace StoneDocuments
{
    [Transaction(TransactionMode.Manual)]
    public class cmdClear : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Revit application and document variables
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            string userName = uiapp.Application.Username;

            // set current view to 3D view
            View curView;

            if (doc.IsWorkshared == true)
                curView = Utils.GetViewByName(doc, "{3D - " + userName + "}");
            else
                curView = Utils.GetViewByName(doc, "{3D}");

            // get all elements in view
            List<Element> viewElements = Utils.GetElementsFromView(doc, curView);

            // set override settings
            OverrideGraphicSettings colSet = new OverrideGraphicSettings();

            // update element overrides in view
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Reset elements");

                foreach (Element curElem in viewElements)
                {
                    doc.ActiveView.SetElementOverrides(curElem.Id, colSet);
                }

                t.Commit();
            }

            return Result.Succeeded;
        }
        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCommand1";
            string buttonTitle = "Button 1";

            clsButtonData myButtonData = new clsButtonData(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Clear_32,
                Properties.Resources.Clear_16,
                "This is a tooltip for Button 1");

            return myButtonData.Data;
        }
    }
}
