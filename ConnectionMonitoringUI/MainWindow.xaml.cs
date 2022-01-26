using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using ConnectionMonitoringLibrary;
using System.Text.RegularExpressions;
using System.Timers;
using System.Linq;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Collections.Generic;
using System.Windows.Media;

namespace ConnectionMonitoringUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BindingList<string> addresses = new BindingList<string>();
        BindingList<IStatus> addressLog = new BindingList<IStatus>();
        BindingList<IStatus> ntpLog = new BindingList<IStatus>();

        Timer timer = new Timer();
        Timer ntpTimer = new Timer();

        public MainWindow()
        {
            InitializeComponent();

            BitmapImage icon = new BitmapImage(new Uri("icon.png", UriKind.Relative));
            mainWindow.Icon = icon;

            pathTextBox.Text = GetDefaultLogPath();
            ntpPathTextBox.Text = GetDefaultLogPath();

            addressesListBox.ItemsSource = addresses;
            addressLogListBox.ItemsSource = addressLog;

            ntpLogListBox.ItemsSource = ntpLog;
        }

        private void selectPathButton_Click(object sender, RoutedEventArgs e)
        {
            selectPath(pathTextBox);
        }

        private void selectPath(System.Windows.Controls.TextBox pathTextBox)
        {
            string filepath = GetDefaultLogPath();

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text Files | *.txt";
            dialog.DefaultExt = "txt";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            dialog.ShowDialog();

            if (dialog.FileName != null && dialog.FileName.Length > 0)
            {
                filepath = Path.GetFullPath(dialog.FileName);
            }

            pathTextBox.Text = filepath;
        }

        public string GetDefaultLogPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\log.txt";
        }

        private void addAddressButton_Click(object sender, RoutedEventArgs e)
        {
            addresses.Add(addressTextBox.Text);
            addressTextBox.Text = "";
            addressTextBox.Focus();
        }

        private void deleteAddressButton_Click(object sender, RoutedEventArgs e)
        {
            addresses.RemoveAt(addressesListBox.SelectedIndex);
        }

        private void OnTimedEventAddress(Object source, ElapsedEventArgs e)
        {
            foreach (string address in addresses)
            {
                PingStatusToLog(address);
            }
        }

        private void OnTimedEventNtp(Object source, ElapsedEventArgs e)
        {
            NtpStatusToLog();
        }

        private void PingStatusToLog(string address)
        {
            PingStatus pingStatus = new PingStatus { Address = address, CurrentDateTime = DateTime.Now, Status = PingChecker.CheckPing(address) };
            List<PingStatus> pingStatusLog = addressLog.ToList().OfType<PingStatus>().ToList().Where(x => x.Address == address).ToList();

            Application.Current.Dispatcher.Invoke(delegate
            {
                addressLog.Add(pingStatus);

                if (addressLogSavingCheckBox.IsChecked == true)
                {
                    FileProcessor.WriteLog(addressLog.ToList(), pathTextBox.Text);
                }

                if (pingStatusLog.Count > 0)
                {
                    PingStatus previousPingStatus = pingStatusLog[pingStatusLog.Count - 1];

                    if (pingStatus.Status == false && previousPingStatus.Status == true)
                    {
                        disconnectTextBlock.Foreground = Brushes.Red;
                        disconnectTextBlock.Text = (Convert.ToInt32(disconnectTextBlock.Text) + 1).ToString();
                        if (addressNotifyCheckBox.IsChecked == true)
                        {
                            new ToastContentBuilder().AddText("Connection lost").Show();
                        }
                    }
                    else if (pingStatus.Status == true && previousPingStatus.Status == false)
                    {
                        if (addressNotifyCheckBox.IsChecked == true)
                        {
                            new ToastContentBuilder().AddText("Connection restored").Show();
                        }
                    }
                }
            });
        }

        private static readonly Regex _regex = new Regex("[^0-9]+");
        private void addressTimeTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void addressSaveLogButton_Click(object sender, RoutedEventArgs e)
        {
            FileProcessor.WriteLog(addressLog.ToList(), pathTextBox.Text);
        }

        private void addressClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            addressLog.Clear();
        }

        private void startAddressButton_Click(object sender, RoutedEventArgs e)
        {
            startButtonPressedCheckBox.IsChecked = !startButtonPressedCheckBox.IsChecked;

            if (startButtonPressedCheckBox.IsChecked == true)
            {
                if (addressesListBox.Items.IsEmpty == false)
                {
                    if (!timer.Enabled)
                    {
                        timer.Interval = Convert.ToDouble(addressTimeTextBox.Text) * 1000;
                        timer.Elapsed += OnTimedEventAddress;
                        timer.Start();
                        addressLog.Add(new MonitoringStatus { CurrentDateTime = DateTime.Now, Status = true });
                        foreach (string address in addresses)
                        {
                            PingStatusToLog(address);
                        }

                        startAddressButton.Content = "Pause monitoring";
                    }
                }
                else
                {
                    MessageBox.Show("Please enter at least one address");
                    startButtonPressedCheckBox.IsChecked = !startButtonPressedCheckBox.IsChecked;
                }
            }
            else
            {
                addressLog.Add(new MonitoringStatus { CurrentDateTime = DateTime.Now, Status = false });
                timer.Elapsed -= OnTimedEventAddress;
                timer.Stop();
                startAddressButton.Content = "Start monitoring";
            }
        }

        private void ntpSelectPathButton_Click(object sender, RoutedEventArgs e)
        {
            selectPath(ntpPathTextBox);
        }

        private void startNtpButton_Click(object sender, RoutedEventArgs e)
        {
            ntpStartButtonPressedCheckBox.IsChecked = !ntpStartButtonPressedCheckBox.IsChecked;

            if (ntpStartButtonPressedCheckBox.IsChecked == true)
            {
                if (!ntpTimer.Enabled)
                {
                    ntpTimer.Interval = Convert.ToDouble(ntpTimeTextBox.Text) * 1000;
                    ntpTimer.Elapsed += OnTimedEventNtp;
                    ntpTimer.Start();
                    ntpLog.Add(new MonitoringStatus { CurrentDateTime = DateTime.Now, Status = true });
                    NtpStatusToLog();

                    startNtpButton.Content = "Pause monitoring";
                }
            }
            else
            {
                ntpLog.Add(new MonitoringStatus { CurrentDateTime = DateTime.Now, Status = false });
                ntpTimer.Elapsed -= OnTimedEventNtp;
                ntpTimer.Stop();
                startNtpButton.Content = "Start monitoring";
            }
        }

        private void NtpStatusToLog()
        {
            bool[] ntpStatuses = NtpChecker.CheckNtp("time.windows.com");
            NtpStatus ntpStatus = new NtpStatus { CurrentDateTime = DateTime.Now, DnsStatus = ntpStatuses[0], SendStatus = ntpStatuses[1], Status = ntpStatuses[2]};
            List<NtpStatus> ntpStatusLog = addressLog.ToList().OfType<NtpStatus>().ToList();

            Application.Current.Dispatcher.Invoke(delegate
            {
                ntpLog.Add(ntpStatus);

                if (ntpLogSavingCheckBox.IsChecked == true)
                {
                    FileProcessor.WriteLog(ntpLog.ToList(), ntpPathTextBox.Text);
                }

                if (ntpStatusLog.Count > 0)
                {
                    NtpStatus previousNtpStatus = ntpStatusLog[ntpStatusLog.Count - 1];

                    if (previousNtpStatus.Status == false && previousNtpStatus.Status == true)
                    {
                        ntpDisconnectTextBlock.Foreground = Brushes.Red;
                        ntpDisconnectTextBlock.Text = (Convert.ToInt32(ntpDisconnectTextBlock.Text) + 1).ToString();
                        if (ntpNotifyCheckBox.IsChecked == true)
                        {
                            new ToastContentBuilder().AddText("NTP connection lost").Show();
                        }
                    }
                    else if (ntpStatus.Status == true && previousNtpStatus.Status == false)
                    {
                        if (addressNotifyCheckBox.IsChecked == true)
                        {
                            new ToastContentBuilder().AddText("NTP connection restored").Show();
                        }
                    }
                }
            });
        }

        private void ntpSaveLogButton_Click(object sender, RoutedEventArgs e)
        {
            FileProcessor.WriteLog(ntpLog.ToList(), ntpPathTextBox.Text);
        }

        private void ntpClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            ntpLog.Clear();
        }
    }
}
