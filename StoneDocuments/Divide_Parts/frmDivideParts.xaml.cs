using System.Windows;

namespace StoneDocuments
{
    /// <summary>
    /// Interaction logic for frmDivideParts.xaml
    /// </summary>
    public partial class frmDivideParts : Window
    {
        private readonly vmDivideParts viewModel;
        private readonly ExternalEvent selectEvent;
        private readonly ExternalEvent createEvent;

        public frmDivideParts(vmDivideParts vm, ExternalEvent selectWallsEvent, ExternalEvent createPartsEvent)
        {
            InitializeComponent();

            viewModel = vm;
            selectEvent = selectWallsEvent;
            createEvent = createPartsEvent;

            cmbHorizontal.ItemsSource = viewModel.SubcategoryNames;
            cmbVertical.ItemsSource = viewModel.SubcategoryNames;

            if (viewModel.SubcategoryNames.Count > 0)
            {
                cmbHorizontal.SelectedIndex = 0;
                cmbVertical.SelectedIndex = 0;
            }

            UpdateWallCountText();
        }

        internal void OnWallsSelected()
        {
            UpdateWallCountText();
            Show();
            Activate();
        }

        private void UpdateWallCountText()
        {
            int count = viewModel.SelectedWallIds?.Count ?? 0;
            tbkWallCount.Text = count == 1 ? "1 wall selected" : $"{count} walls selected";
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            selectEvent.Raise();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.SelectedWallIds == null || viewModel.SelectedWallIds.Count == 0)
            {
                MessageBox.Show("Select at least one wall first.", "Divide Parts");
                return;
            }

            if (cmbHorizontal.SelectedItem == null || cmbVertical.SelectedItem == null)
            {
                MessageBox.Show("Choose a reference plane type for both directions.", "Divide Parts");
                return;
            }

            viewModel.Gap = cmbGap.Text;
            viewModel.HorizontalType = cmbHorizontal.SelectedItem as string;
            viewModel.VerticalType = cmbVertical.SelectedItem as string;

            createEvent.Raise();
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://lifestyle-usa-design.atlassian.net/wiki/spaces/MFS/pages/611450881/Schedule+Swap");
        }
    }
}
