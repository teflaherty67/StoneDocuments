
using System.Windows.Forms;

namespace StoneDocuments.Common
{
    internal static class Utils
    {
        #region Elements

        internal static List<ElementId> GetElementIdsFromList(Document doc, List<Element> elemList)
        {
            List<ElementId> returnList = new List<ElementId>();

            foreach (Element curElem in elemList)
                returnList.Add(curElem.Id);

            return returnList;
        }

        internal static List<Element> GetElementsFromSchedule(Document doc, ViewSchedule curView)
        {
            IList<Element> elements = new List<Element>();
            FilteredElementCollector finalCollector = new FilteredElementCollector(doc, curView.Id);

            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_Parts);
            builtInCats.Add(BuiltInCategory.OST_GenericModel);
            builtInCats.Add(BuiltInCategory.OST_Assemblies);

            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);
            finalCollector.WherePasses(filter1);

            return finalCollector.ToElements() as List<Element>;
        }

        internal static List<Element> GetElementsFromView(Document doc, View curView)
        {
            IList<Element> elements = new List<Element>();
            FilteredElementCollector finalCollector = new FilteredElementCollector(doc, curView.Id);

            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_Parts);
            builtInCats.Add(BuiltInCategory.OST_GenericModel);
            builtInCats.Add(BuiltInCategory.OST_Assemblies);

            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);
            finalCollector.WherePasses(filter1);
            elements = finalCollector.ToElements();

            return finalCollector.ToElements() as List<Element>;
        }

        internal static FillPatternElement GetFillPatternByName(Document doc, string name)
        {
            FillPatternElement curFPE = null;

            curFPE = FillPatternElement.GetFillPatternElementByName(doc, FillPatternTarget.Drafting, name);

            return curFPE;
        }

        internal static bool DoesElementListContainAssemblies(Document doc, List<Element> elemList)
        {
            foreach (Element curElem in elemList)
            {
                if (curElem is AssemblyInstance)
                    return true;
            }
            return false;
        }

        #endregion

        #region Parameters

        internal static string GetParameterValueByName(Element element, string paramName)
        {
            IList<Parameter> paramList = element.GetParameters(paramName);

            if (paramList != null)
                try
                {
                    Parameter param = paramList[0];
                    string paramValue = param.AsValueString();
                    return paramValue;
                }
                catch (System.ArgumentOutOfRangeException)
                {
                    return null;
                }

            return "";
        }

        internal static string SetParameterByName(Element curElem, string paramName, string value)
        {
            Parameter curParam = GetParameterByNameAndWritable(curElem, paramName);

            curParam.Set(value);
            return curParam.ToString();
        }

        internal static Parameter GetParameterByNameAndWritable(Element curElem, string paramName)
        {
            foreach (Parameter curParam in curElem.Parameters)
            {
                if (curParam.Definition.Name.ToString() == paramName && curParam.IsReadOnly == false)
                    return curParam;
            }

            return null;
        }

        #endregion

        #region Ribbon

        internal static RibbonPanel CreateRibbonPanel(UIControlledApplication app, string tabName, string panelName)
        {
            RibbonPanel currentPanel = GetRibbonPanelByName(app, tabName, panelName);

            if (currentPanel == null)
                currentPanel = app.CreateRibbonPanel(tabName, panelName);

            return currentPanel;
        }

        internal static RibbonPanel GetRibbonPanelByName(UIControlledApplication app, string tabName, string panelName)
        {
            foreach (RibbonPanel tmpPanel in app.GetRibbonPanels(tabName))
            {
                if (tmpPanel.Name == panelName)
                    return tmpPanel;
            }

            return null;
        }

        internal static List<string> GetCommandsFromRibbonTab(UIApplication uiapp, string tabName)
        {
            List<string> m_cmdList = new List<string>();

            foreach (RibbonPanel curPanel in uiapp.GetRibbonPanels(tabName))
            {
                foreach (RibbonItem curItem in curPanel.GetItems())
                {
                    if (curItem is PushButton pushButton)
                    {
                        m_cmdList.Add(pushButton.ItemText.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " "));
                    }
                    else if (curItem is PulldownButton pulldownButton)
                    {
                        m_cmdList.Add(pulldownButton.ItemText.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " "));
                        foreach (RibbonItem pulldownItem in pulldownButton.GetItems())
                        {
                            if (pulldownItem is PushButton subButton)
                            {
                                m_cmdList.Add(subButton.ItemText.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " "));
                            }
                        }
                    }
                    else if (curItem is SplitButton splitButton)
                    {
                        m_cmdList.Add(splitButton.ItemText.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " "));
                        foreach (RibbonItem splitButtonItem in splitButton.GetItems())
                        {
                            if (splitButtonItem is PushButton subButton)
                            {
                                m_cmdList.Add(subButton.ItemText.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " "));
                            }
                        }
                    }
                }
            }

            return m_cmdList;
        }

        #endregion

        #region Schedules

        internal static List<ViewSchedule> GetAllSchedulesOnSheet(Document curDoc, ViewSheet curSheet)
        {
            List<ViewSchedule> schedList = new List<ViewSchedule>();

            FilteredElementCollector curCollector = new FilteredElementCollector(curDoc, curSheet.Id);
            curCollector.OfClass(typeof(ScheduleSheetInstance));

            //loop through views and check if schedule - if so then put into schedule list
            foreach (ScheduleSheetInstance curView in curCollector)
            {
                ViewSchedule curSched = curDoc.GetElement(curView.ScheduleId) as ViewSchedule;
                schedList.Add(curSched);
            }

            return schedList;
        }

        internal static List<ViewSchedule> GetAllSchedules(Document curDoc)
        {
            List<ViewSchedule> m_schedList = new List<ViewSchedule>();

            FilteredElementCollector m_curCollector = new FilteredElementCollector(curDoc)
                .OfClass(typeof(ViewSchedule))
                .WhereElementIsNotElementType();

            //loop through views and check if schedule - if so then put into schedule list
            foreach (ViewSchedule curView in m_curCollector)
            {
                if (curView.ViewType == ViewType.Schedule)
                {
                    if (curView.IsTemplate == false)
                    {
                        if (curView.Name.Contains("<") && curView.Name.Contains(">"))
                            continue;
                        else
                            m_schedList.Add((ViewSchedule)curView);
                    }
                }
            }

            return m_schedList;
        }

        internal static List<string> GetAllScheduleNames(Document curDoc)
        {
            List<ViewSchedule> m_schedList = GetAllSchedules(curDoc);

            List<string> m_Names = new List<string>();

            foreach (ViewSchedule curSched in m_schedList)
            {
                m_Names.Add(curSched.Name);
            }

            return m_Names;
        }

        internal static List<string> GetAllSSINames(Document curDoc)
        {
            FilteredElementCollector m_colSSI = new FilteredElementCollector(curDoc);
            m_colSSI.OfClass(typeof(ScheduleSheetInstance));

            List<string> m_returnList = new List<string>();

            foreach (ScheduleSheetInstance curInstance in m_colSSI)
            {
                string schedName = curInstance.Name as string;
                m_returnList.Add(schedName);
            }

            return m_returnList;
        }

        internal static List<string> GetSchedulesNotUsed(List<string> schedNames, List<string> schedInstances)
        {
            IEnumerable<string> m_returnList;

            m_returnList = schedNames.Except(schedInstances);

            return m_returnList.ToList();
        }

        internal static List<ViewSchedule> GetSchedulesToUse(Document curDoc, List<string> schedNotUsed)
        {
            List<ViewSchedule> m_returnList = new List<ViewSchedule>();

            foreach (string schedName in schedNotUsed)
            {
                string curName = schedName;

                ViewSchedule curSched = GetViewScheduleByName(curDoc, curName);

                if (curSched != null)
                {
                    m_returnList.Add(curSched);
                }
            }

            return m_returnList;
        }

        internal static ViewSchedule GetViewScheduleByName(Document curDoc, string viewScheduleName)
        {
            List<ViewSchedule> m_SchedList = GetAllSchedules(curDoc);

            ViewSchedule m_viewSchedNotFound = null;

            foreach (ViewSchedule curViewSched in m_SchedList)
            {
                if (curViewSched.Name == viewScheduleName)
                {
                    return curViewSched;
                }
            }

            return m_viewSchedNotFound;
        }

        internal static ScheduleSheetInstance GetScheduleOnSheetByName(Document curDoc, ViewSheet curSheet, ViewSchedule curSched)
        {
            FilteredElementCollector m_colSSI = new FilteredElementCollector(curDoc, curSheet.Id)
                 .OfClass(typeof(ScheduleSheetInstance));

            foreach (ScheduleSheetInstance curSchedule in m_colSSI)
            {
                if (curSchedule.Name == curSched.Name)
                    return curSchedule;
            }

            return null;
        }

        internal static bool SheetHasSchedule(Document curDoc, ViewSheet curSheet)
        {
            // get schedule from sheet
            List<ViewSchedule> schedList = Utils.GetAllSchedulesOnSheet(curDoc, curSheet);

            // check if sheet has schedule
            if (schedList.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        internal static bool DoesScheduleContainAssemblies(Document doc, ViewSchedule curView)
        {
            FilteredElementCollector m_colAssembly = new FilteredElementCollector(doc, curView.Id)
                .OfCategory(BuiltInCategory.OST_Assemblies);

            if (m_colAssembly.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Sheets       

        internal static List<string> GetAllSheetCategoriesByName(Document curDoc, string paramName)
        {
            List<ViewSheet> m_sheetList = GetAllSheets(curDoc);

            List<string> m_catNames = new List<string>();

            string catName = "";

            foreach (ViewSheet curSheet in m_sheetList)
            {
                catName = GetParameterValueByName(curSheet, paramName);
                m_catNames.Add(catName);
            }

            List<string> m_distinctList = m_catNames.Distinct().ToList();

            return m_distinctList;
        }

        internal static List<string> GetAllSheetGroupsByName(Document curDoc, string paramName)
        {
            List<ViewSheet> m_sheetList = GetAllSheets(curDoc);

            List<string> m_grpNames = new List<string>();

            string grpName = "";

            foreach (ViewSheet curSheet in m_sheetList)
            {
                grpName = GetParameterValueByName(curSheet, paramName);
                m_grpNames.Add(grpName);
            }

            List<string> m_distinctList = m_grpNames.Distinct().ToList();

            return m_distinctList;
        }

        public static List<ViewSheet> GetAllSheets(Document curDoc)
        {
            //get all sheets
            FilteredElementCollector m_colViews = new FilteredElementCollector(curDoc);
            m_colViews.OfCategory(BuiltInCategory.OST_Sheets);

            List<ViewSheet> m_sheets = new List<ViewSheet>();
            foreach (ViewSheet x in m_colViews.ToElements())
            {
                m_sheets.Add(x);
            }

            return m_sheets;
        }

        #endregion

        #region Views

        internal static List<View> GetAllViews(Document curDoc)
        {
            FilteredElementCollector m_colviews = new FilteredElementCollector(curDoc);
            m_colviews.OfCategory(BuiltInCategory.OST_Views);

            List<View> m_views = new List<View>();
            foreach (View x in m_colviews.ToElements())
            {
                m_views.Add(x);
            }

            return m_views;
        }

        internal static View GetViewByName(Document curDoc, string viewName)
        {
            List<View> viewList = GetAllViews(curDoc);

            //loop through views in the collector
            foreach (View curView in viewList)
            {
                if (curView.Name == viewName && curView.IsTemplate == false)
                {
                    return curView;
                }
            }

            return null;
        }

        internal static List<View> GetViews(Document curDoc)
        {
            List<View> m_returnList = new List<View>();

            FilteredElementCollector m_viewCollector = new FilteredElementCollector(curDoc)
                .OfCategory(BuiltInCategory.OST_Views);

            FilteredElementCollector m_sheetCollector = new FilteredElementCollector(curDoc)
                .OfCategory(BuiltInCategory.OST_Sheets);

            foreach (View curView in m_viewCollector)
            {
                if (curView.IsTemplate == false)
                {
                    if (Viewport.CanAddViewToSheet(curDoc, m_sheetCollector.FirstElementId(), curView.Id) == true)
                        m_returnList.Add(curView);
                }
            }

            return m_returnList;
        }

        

        #endregion
    }
}
