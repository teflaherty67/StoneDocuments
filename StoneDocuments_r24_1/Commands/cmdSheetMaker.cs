#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;

#endregion

namespace StoneDocuments_r24_1
{
    [Transaction(TransactionMode.Manual)]
    public class cmdSheetMaker : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document curDoc = uidoc.Document;

            // put any code needed for the form here
            FilteredElementCollector tblockCollector = new FilteredElementCollector(curDoc)
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .WhereElementIsElementType();

            List<clsWrapperTBlockType> tblockTypeList = new List<clsWrapperTBlockType>();
            foreach (FamilySymbol curTblockType in tblockCollector)
            {
                clsWrapperTBlockType tblockWrapper = new clsWrapperTBlockType(curTblockType);
                tblockTypeList.Add(tblockWrapper);
            }

            // sort list by family and type
            List<clsWrapperTBlockType> sortedList = tblockTypeList.OrderBy(o => o.FamilyAndType).ToList();

            // create list for category names
            List<string> catList = Utils.GetAllSheetCategoriesByName(curDoc, "Category");
            catList.Sort();
         
            // create a list for group names
            List<string> grpList = Utils.GetAllSheetGroupsByName(curDoc, "Group");
            grpList.Sort();

            // get a list of all the schedules not already on a sheet

            // create a list of all the schedules by name
            List<string> schedNames = Utils.GetAllScheduleNames(curDoc);

            // create a list of all schedules already on a sheet
            List<string> schedInstances = Utils.GetAllSSINames(curDoc);

            // compare the 2 lists and create a list of schedules not used by name
            List<string> schedNotUsed = Utils.GetSchedulesNotUsed(schedNames, schedInstances);

            // convert the list of schedule names to a list of View Schedules
            List<ViewSchedule> schedToUse = Utils.GetSchedulesToUse(curDoc, schedNotUsed);

            // open form
            frmSheetMaker curForm = new frmSheetMaker(sortedList, catList, grpList, Utils.GetViews(curDoc), schedToUse)
            {
                Width = 880,
                Height = 450,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            curForm.ShowDialog();            

            if (curForm.DialogResult == true)
            {
                using (Transaction t = new Transaction(curDoc))
                {
                    t.Start("Create new sheets");

                    // get form data and do something
                    foreach (clsSheetData curData in curForm.GetSheetData())
                    {
                        try
                        {
                            ViewSheet newSheet;

                            newSheet = ViewSheet.Create(curDoc, curForm.GetComboBoxTitleblock().Id);

                            newSheet.SheetNumber = curData.SheetNumber.ToUpper();
                            newSheet.Name = curData.SheetName.ToUpper();

                            string newCategory = curForm.GetComboBoxCategory();
                            string newGroup = curForm.GetComboBoxGroup();

                            if (curData.SelectedView != null)
                            {
                                Viewport curVP = Viewport.Create(curDoc, newSheet.Id, curData.SelectedView.Id, new XYZ(.25,.25,0));
                            }

                            if (curData.SelectedSchedule != null)
                            {
                                ScheduleSheetInstance curSSI = ScheduleSheetInstance.Create(curDoc, newSheet.Id, curData.SelectedSchedule.Id, new XYZ(.25,.65,0));
                            }

                            if (curForm.GetComboBoxCategory() != null)
                            {
                                Utils.SetParameterByName(newSheet, "Category", newCategory);
                            }

                            if(curForm.GetComboBoxGroup() != null)
                            {
                                Utils.SetParameterByName(newSheet, "Group", newGroup);
                            }
                        }
                        catch (Exception ex)
                        {
                            TaskDialog tdError = new TaskDialog("Error");
                            tdError.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                            tdError.Title = "Sheet Maker";
                            tdError.TitleAutoPrefix = false;
                            tdError.MainContent = "An error occured: " + ex.Message;
                            tdError.CommonButtons = TaskDialogCommonButtons.Close;

                            TaskDialogResult tdErrorRes = tdError.Show();
                        }
                    }

                    t.Commit();
                }
            }

            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
