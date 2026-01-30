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
            RibbonPanel panel03 = Utils.CreateRibbonPanel(app, "Stone Documents", "Manage");
            RibbonPanel panel04 = Utils.CreateRibbonPanel(app, "Stone Documents", "Support Tools");

            // create button data instances for panel 01
            PushButtonData btnData1_1 = cmdCheck.GetButtonData();
            PushButtonData btnData1_2 = cmdClear.GetButtonData();

            // create button data instances for panel 02
            PushButtonData btnData2_1 = cmdScheduleSwap.GetButtonData();
            PushButtonData btnData2_2 = cmdSheetMaker.GetButtonData();
            PushButtonData btnData2_3 = cmdSelectSheets.GetButtonData();
            PushButtonData btnData2_4 = cmdIncrementSheets.GetButtonData();
            PushButtonData btnData2_5 = cmdDecrementSheets.GetButtonData();

            // create button data instances for panel 03
            PushButtonData btnData3_1 = cmdBatchUpdate.GetButtonData();
            PushButtonData btnData3_2 = cmdDeleteBackups.GetButtonData();

            // create button data instances for panel 04
            PushButtonData btnData4_1 = cmdReportBugs.GetButtonData();

            // create buttons for panel 01
            PushButton myButton1_1 = panel01.AddItem(btnData1_1) as PushButton;
            PushButton myButton1_2 = panel01.AddItem(btnData1_2) as PushButton;

            // create buttons for panel 02
            PushButton myButton2_1 = panel02.AddItem(btnData2_1) as PushButton;
            PushButton myButton2_2 = panel02.AddItem(btnData2_2) as PushButton;
            PushButton myButton2_3 = panel02.AddItem(btnData2_3) as PushButton;
            PushButton myButton2_4 = panel02.AddItem(btnData2_4) as PushButton;
            PushButton myButton2_5 = panel02.AddItem(btnData2_5) as PushButton;

            // create buttons for panel 03
            PushButton myButton3_1 = panel03.AddItem(btnData3_1) as PushButton;
            PushButton myButton3_2 = panel03.AddItem(btnData3_2) as PushButton;

            // create buttons for panel 04
            PushButton myButton4_1 = panel03.AddItem(btnData4_1) as PushButton;

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
