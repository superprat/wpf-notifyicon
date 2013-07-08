using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using ReactiveUI.Xaml;

namespace NotifyIconTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TrayIconViewModel trayVm;
        public App()
        {
            InitializeComponent();
            MainWindow = new MainWindow();
            trayVm = new TrayIconViewModel(this);
            MainWindow.Show();
        }

        
    }
}
