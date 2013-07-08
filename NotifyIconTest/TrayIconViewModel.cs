using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Hardcodet.Wpf.TaskbarNotification;
using ReactiveUI;
using ReactiveUI.Xaml;

namespace NotifyIconTest
{

    class ToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var result = Visibility.Collapsed;
            if (value is int)
            {
                result = (int)value > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (value is bool)
            {
                result = (bool)value ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                result = value != null ? Visibility.Visible : Visibility.Collapsed;
            }
            if (parameter != null)
            {
                result = result == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class TrayIconViewModel : ReactiveObject
    {
        private readonly App _theApp;
        private readonly TaskbarIcon _trayIcon;
        private bool _trayModeEnabled = true;

        public TrayIconViewModel(App app)
        {
            _theApp = app;
            _trayIcon = (TaskbarIcon) app.FindResource("TrayIcon");
            if (_trayIcon == null) return;

            InTray = false;
            _trayIcon.DataContext = this;
            // TODO: Load from settings
            if (!_trayModeEnabled)
            {
                _trayIcon.Visibility = Visibility.Collapsed;
                _trayIcon.Dispose();
                return;
            }
            var cm = new ContextMenu();
            cm.Items.Add(new MenuItem
                {
                    Header = "Exit Tray Sample App",
                    Command = ExitApplication
                });
            _trayIcon.ContextMenu = cm;
            _theApp.MainWindow.Closing += MainWindow_Closing;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var window = sender as MainWindow;
            if (window == null) return;
            if (InTray) return;
            window.Hide();
            InTray = true;
            e.Cancel = true;
        }

        #region Property InTray

        private bool _pInTray;

        public bool InTray
        {
            get { return _pInTray; }
            set { this.RaiseAndSetIfChanged(ref _pInTray, value); }
        }

        #endregion
        
        #region Command: ShowApplication

        private IReactiveCommand _pShowApplication = null;

        public IReactiveCommand ShowApplication
        {
            get
            {
                if (_pShowApplication == null)
                {
                    _pShowApplication = new ReactiveCommand( /* Observable for CanExecute Here */);
                    _pShowApplication.Subscribe((param) =>
                        {
                            // ShowApplication executed
                            Application.Current.MainWindow.Show();
                            InTray = false;
                        });
                }
                return _pShowApplication;
            }
        }

        #endregion
        #region Command: ExitApplication

        private IReactiveCommand _pExitApplication = null;

        public IReactiveCommand ExitApplication
        {
            get
            {
                if (_pExitApplication == null)
                {
                    _pExitApplication = new ReactiveCommand( /* Observable for CanExecute Here */);
                    _pExitApplication.Subscribe((param) =>
                        {
                            // ExitApplication executed
                            _trayIcon.Dispose();
                            _theApp.Shutdown();
                        });
                }
                return _pExitApplication;
            }
        }
        #endregion
    }
}
