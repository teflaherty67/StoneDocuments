using StoneDocuments.Common;

namespace StoneDocuments
{
    internal class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication app)
        {
            // create ribbon tab
            string tabName = "Stone Documents";
            try
            {
                app.CreateRibbonTab(tabName);
            }
            catch (Exception)
            {
                Debug.Print("Tab already exists.");
            }

            // create ribbon panel 
            RibbonPanel panel01 = Utils.CreateRibbonPanel(app, "Stone Documents", "Parts");
            RibbonPanel panel02 = Utils.CreateRibbonPanel(app, "Stone Documents", "Sheets");

            // create button data instances for panel 01
            PushButtonData btnData1_1 = cmdCheck.GetButtonData();
            PushButtonData btnData1_2 = cmdClear.GetButtonData();

            // create button data instances for panel 02
            PushButtonData btnData2_1 = cmdScheduleSwap.GetButtonData();
            PushButtonData btnData2_2 = cmdSheetMaker.GetButtonData();

            // create buttons for panel 01
            PushButton myButton1 = panel01.AddItem(btnData1_1) as PushButton;
            PushButton myButton2 = panel02.AddItem(btnData1_2) as PushButton;

            // create buttons for panel 02

            //NOTE:
            //    To create a new tool, copy lines 35 and 39 and rename the variables to "btnData3" and "myButton3".
            //     Change the name of the tool in the arguments of line

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }

}
