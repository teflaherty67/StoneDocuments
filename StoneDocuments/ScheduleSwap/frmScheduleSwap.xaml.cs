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

namespace StoneDocuments
{
    /// <summary>
    /// Interaction logic for frmScheduleSwap.xaml
    /// </summary>
    public partial class frmScheduleSwap : Window
    {
        public vmScheduleSwap viewModel;
        public frmScheduleSwap(UIApplication uiapp)
        {
            InitializeComponent();

            viewModel = new vmScheduleSwap(uiapp);

            cmbNewSchedules.ItemsSource = viewModel.viewSchedules;
            cmbCurSchedules.ItemsSource = viewModel.viewSheetSched;

            cmbNewSchedules.SelectedIndex = 0;
            cmbCurSchedules.SelectedIndex = 0;
        }

        public ViewSchedule GetComboBoxViewScheduleSelectedItem()
        {
            return cmbNewSchedules.SelectedItem as ViewSchedule;
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
    }
}
