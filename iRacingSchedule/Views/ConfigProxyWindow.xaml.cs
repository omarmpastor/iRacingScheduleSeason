using iRacingSchedule.Events;
using iRacingSchedule.Services;
using System.Windows;

namespace iRacingSchedule.Views
{
    public partial class ConfigProxyWindow : Window
    {
        public ConfigProxyWindow()
        {
            InitializeComponent();
            cbActiveProxy.IsChecked = Properties.Settings.Default.ProxyEnabled;
            tbProxy.Text = Properties.Settings.Default.ProxyServer;
            cbUserProxy.IsChecked = Properties.Settings.Default.ProxyUser;
            tbUserNameProxy.Text = Properties.Settings.Default.ProxyUsername;
            tbUserPasswordProxy.Text = Properties.Settings.Default.ProxyPassword;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ProxyEnabled = cbActiveProxy.IsChecked == true;
            Properties.Settings.Default.ProxyServer = tbProxy.Text;
            Properties.Settings.Default.ProxyUser = cbUserProxy.IsChecked == true;
            Properties.Settings.Default.ProxyUsername = tbUserNameProxy.Text;
            Properties.Settings.Default.ProxyPassword = tbUserPasswordProxy.Text;
            Properties.Settings.Default.Save();
            this.Close();
            
             var config = new ProxyEventArgs {
                ProxyEnabled = cbActiveProxy.IsChecked,
                ProxyServer = tbProxy.Text,
                ProxyUser = cbUserProxy.IsChecked,
                ProxyUsername = tbUserNameProxy.Text,
                ProxyPassword = tbUserPasswordProxy.Text
            };
            
            DependencyService.Get<DataLoadWriteService>().ChangeInternetConfiguration(config);
        }

        private void cbActiveProxy_Unchecked(object sender, RoutedEventArgs e)
        {
            cbUserProxy.IsChecked = false;
        }
    }
}
