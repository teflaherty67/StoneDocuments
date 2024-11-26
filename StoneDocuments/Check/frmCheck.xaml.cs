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
using System.Runtime.InteropServices;
using System.Windows.Navigation;


namespace StoneDocuments
{
    /// <summary>
    /// Interaction logic for frmCheck.xaml
    /// </summary>
    public partial class frmCheck : Window
    {
        private cmdCheck.RequestHandler m_Handler;
        private cmdCheck.CancelHandler c_Handler;
        private ExternalEvent m_ExternalEvent;
        private ExternalEvent c_ExternalEvent;

        public frmCheck(ExternalEvent exEvent, cmdCheck.RequestHandler rHandler, cmdCheck.CancelHandler cHandler, ExternalEvent cEvent, int count)
        {
            InitializeComponent();

            m_Handler = rHandler;
            c_Handler = cHandler;
            m_ExternalEvent = exEvent;
            c_ExternalEvent = cEvent;
            tbkCount.Text = count.ToString() + " elements selected";
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            m_ExternalEvent.Raise();
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            c_ExternalEvent.Raise();
            this.Close();
        }
    }
}

