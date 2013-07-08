using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Hardcodet.Wpf.TaskbarNotification;

namespace NotifyIconTest
{
    /// <summary>
    /// Interaction logic for ToastUI.xaml
    /// </summary>
    public partial class ToastUi : UserControl
    {
        public ToastUi(Toast toast)
        {
            InitializeComponent();
            Title.Text = toast.Title;
            if( toast.SubText != null) SubTitle.Text = toast.SubText;
            if (toast.SubSubText != null) SubSubTitle.Text = toast.SubSubText;
            if (! string.IsNullOrEmpty(toast.Icon))
            {
                // Load IconSource 
                Image.Source = new BitmapImage(new Uri(toast.Icon, UriKind.RelativeOrAbsolute));
            }
            Loaded += ToastUi_Loaded;
        }

        void ToastUi_Loaded(object sender, RoutedEventArgs e)
        {
            var dc = DataContext;
            TaskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            TaskbarIcon.AddBalloonClosingHandler(this, OnBaloonClosing);
            MouseLeave += ToastUi_MouseLeave;
        }

        private Popup needsClose;
        void ToastUi_MouseLeave(object sender, MouseEventArgs e)
        {
            if (needsClose != null)
            {
                needsClose.IsOpen = false;
            }
        }

        private TaskbarIcon TaskbarIcon { get; set; }

        public event EventHandler Activated;
        public event EventHandler Dismissed;
        public event EventHandler Timedout;

        private bool _isTimedout = true;
        public void OnBaloonClosing(object obj, RoutedEventArgs args)
        {
            if (IsMouseOver && !CloseButton.IsMouseOver)
            {
                if (TaskbarIcon != null)
                {
                    DisableBallonTimer();
                    needsClose = TaskbarIcon.CustomBalloon;
                    args.Handled = true;
                    return;
                }
            }
            if (_isTimedout)
            {
                FireEvent(Timedout);
            }
        }

        private void ToastActivated(object sender, MouseButtonEventArgs e)
        {
            _isTimedout = false;
            ForceClose();
            FireEvent(Activated);
        }

        private void ToastClosed(object sender, RoutedEventArgs e)
        {
            _isTimedout = false;
            ForceClose();
            FireEvent(Dismissed);
        }

        void DisableBallonTimer()
        {
            if (TaskbarIcon != null)
            {
                TaskbarIcon.ResetBalloonCloseTimer();
            }
        }

        void ForceClose()
        {
            if (TaskbarIcon != null)
            {
                TaskbarIcon.CloseBalloon();
            }
        }

        void FireEvent(EventHandler e)
        {
            if (e != null)
            {
                e(this, new EventArgs());
            }
        }
    }
}
