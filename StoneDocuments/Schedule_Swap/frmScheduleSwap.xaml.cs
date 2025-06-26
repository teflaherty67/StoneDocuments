using System;
using System.Collections.Generic;
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
using ComboBox = System.Windows.Controls.ComboBox;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace StoneDocuments
{
    /// <summary>
    /// Interaction logic for frmScheduleSwap.xaml
    /// </summary>
    public partial class frmScheduleSwap : Window
    {
        public vmScheduleSwap viewModel;

        private List<ViewSchedule> allSchedules;

        public frmScheduleSwap(UIApplication uiapp)
        {
            InitializeComponent();

            viewModel = new vmScheduleSwap(uiapp);

            // Store the original list for filtering
            allSchedules = viewModel.viewSchedules.ToList();

            cmbNewSchedules.ItemsSource = viewModel.viewSchedules;
            cmbCurSchedules.ItemsSource = viewModel.viewSheetSched;
            cmbSearchSchedules.ItemsSource = viewModel.viewSchedules;

            cmbNewSchedules.SelectedIndex = 0;
            cmbCurSchedules.SelectedIndex = 0;
        }

        public ViewSchedule GetComboBoxViewScheduleSelectedItem()
        {
            return cmbNewSchedules.SelectedItem as ViewSchedule;
        }

        private void cmbSearchSchedules_KeyUp(object sender, KeyEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string searchText = comboBox.Text;

            // Don't filter on navigation keys
            if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Enter || e.Key == Key.Tab)
                return;

            // Filter schedules based on search text
            if (string.IsNullOrEmpty(searchText))
            {
                viewModel.viewSchedules.Clear();
                foreach (var schedule in allSchedules)
                {
                    viewModel.viewSchedules.Add(schedule);
                }
            }
            else
            {
                var filteredSchedules = allSchedules
                    .Where(s => s.Name.ToLower().StartsWith(searchText.ToLower()))
                    .ToList();

                viewModel.viewSchedules.Clear();
                foreach (var schedule in filteredSchedules)
                {
                    viewModel.viewSchedules.Add(schedule);
                }
            }

            // Keep the dropdown open and show all matches
            comboBox.IsDropDownOpen = true;
        }

        private void cmbSearchSchedules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox searchComboBox = sender as ComboBox;

            if (searchComboBox.SelectedItem != null)
            {
                // Get the selected ViewSchedule from the search box
                ViewSchedule selectedSchedule = searchComboBox.SelectedItem as ViewSchedule;

                // Set the main ComboBox to the same selection
                cmbNewSchedules.SelectedItem = selectedSchedule;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Run(cmbCurSchedules.SelectedItem as ViewSchedule, cmbNewSchedules.SelectedItem as ViewSchedule);
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://lifestyle-usa-design.atlassian.net/l/cp/eL0qinyA");
        }
    }
}
