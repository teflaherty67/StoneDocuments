using Autodesk.Revit.DB;
using StoneDocuments.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneDocuments
{
    public class vmScheduleSwap
    {
        public ObservableCollection<ViewSchedule> viewSchedules { get; set; }

        public ObservableCollection<ViewSchedule> viewSheetSched { get; set; }

        public Document curDoc;

        public ViewSheet curSheet;

        internal vmScheduleSwap(UIApplication uiapp)
        {
            curDoc = uiapp.ActiveUIDocument.Document;

            // get all the schedules in the project
            List<ViewSchedule> schedNames = Utils.GetAllSchedules(curDoc);

            viewSchedules = new ObservableCollection<ViewSchedule>(schedNames);

            curSheet = curDoc.ActiveView as ViewSheet;

            // get all schedules on sheet
            List<ViewSchedule> schedList = Utils.GetAllSchedulesOnSheet(curDoc, curSheet);
            viewSheetSched = new ObservableCollection<ViewSchedule>(schedList);            
        }

        internal void Run(ViewSchedule curSched, ViewSchedule newSched)
        {
            // set some variables
            ElementId curSheetId = curSheet.Id;

            // get the current schedule & it's location
            ScheduleSheetInstance curSchedule = Utils.GetScheduleOnSheetByName(curDoc, curSheet, curSched);
            XYZ schedLoc = curSchedule.Point;

            // create & start a transaction
            using (Transaction t = new Transaction(curDoc))
            {
                t.Start("Swap Schedules");

                // delete the current schedule
                curDoc.Delete(curSchedule.Id);

                // add the new schedule at the same location point
                ScheduleSheetInstance newSSI = ScheduleSheetInstance.Create(curDoc, curSheetId, newSched.Id, schedLoc);

                t.Commit();
            }
        }
    }
}