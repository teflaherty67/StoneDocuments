using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StoneDocuments
{
    /// <summary>
    /// Interaction logic for frmSheetMaker.xaml
    /// </summary>
    public partial class frmSheetMaker : Window
    {
        ObservableCollection<clsSheetData> SheetList { get; set; }
        ObservableCollection<View> ViewData { get; set; }
        ObservableCollection<ViewSchedule> ScheduleData { get; set; }
        ObservableCollection<string> CategoryData { get; set; }
        ObservableCollection<clsWrapperTBlockType> Types { get; set; }
        ObservableCollection<string> GroupData { get; set; }

        public List<Element> elemList;

        public frmSheetMaker(List<clsWrapperTBlockType> typeList, List<string> CategoryList, List<string> GroupList, List<View> ViewList, List<ViewSchedule> ScheduleList)
        {
            InitializeComponent();

            SheetList = new ObservableCollection<clsSheetData>();
            Types = new ObservableCollection<clsWrapperTBlockType>(typeList);
            CategoryData = new ObservableCollection<string>(CategoryList);
            GroupData = new ObservableCollection<string>(GroupList);
            ViewData = new ObservableCollection<View>(ViewList);
            ScheduleData = new ObservableCollection<ViewSchedule>(ScheduleList);


            sheetGrid.ItemsSource = SheetList;
            cmbTitleblock.ItemsSource = Types;
            cmbCategory.ItemsSource = CategoryData;
            cmbGroup.ItemsSource = GroupData;
            cmbViews.ItemsSource = ViewData;
            cmbSchedules.ItemsSource = ScheduleData;
        }

        internal FamilySymbol GetComboBoxTitleblock()
        {
            clsWrapperTBlockType selectedItem = cmbTitleblock.SelectedValue as clsWrapperTBlockType;
            return selectedItem.TitleblockType;
        }

        internal string GetComboBoxCategory()
        {
            return cmbCategory.SelectedValue as string;
        }

        internal string GetComboBoxGroup()
        {
            return cmbGroup.SelectedValue as string;
        }

        public List<clsSheetData> GetSheetData()
        {
            return SheetList.ToList();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            SheetList.Add(new clsSheetData());
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (clsSheetData curRow in SheetList)
                {
                    if (sheetGrid.SelectedItem == curRow)
                        SheetList.Remove(curRow);
                }
            }
            catch (Exception)
            { }
        }

        private void btnAddCat_Click(object sender, RoutedEventArgs e)
        {
            CategoryData.Add(tbxAddCat.Text);
        }

        private void btnAddGrp_Click(object sender, RoutedEventArgs e)
        {
            GroupData.Add(tbxAddGrp.Text);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://lifestyle-usa-design.atlassian.net/l/cp/eL0qinyA");
        }
    }
}
