using System;
using System.IO;
using System.Windows.Controls.Primitives;
using Hardcodet.Wpf.TaskbarNotification;
using NotifyIconTest;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace NotifyIconTest
{
    public enum ToastType
    {
        Default
    };

    public class Toast
    {
        public Toast()
        {
            Type = ToastType.Default;
        }
        public ToastType Type { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
        public string SubText { get; set; }
        public string SubSubText { get; set; }
        public Action OnActivated { get; set; }
    }

    public interface IToastManager
    {
        void Notify(Toast toast);
    }

    public static class ToastManagerFactory
    {
        public const string AppId = "Skrymsli.Samples.DesktopToastsSample"; 

        private static IToastManager _manager; 
        public static IToastManager ToastManager
        {
            get
            {
                if (_manager == null)
                {
                    if (IsWindows8OrHigher())
                    {
                        _manager = new Windows8ToastManager(AppId);
                    }
                    else
                    {
                        // TODO: Dependency injection to get this...
                        var taskbar = (TaskbarIcon)(App.Current).FindResource("TrayIcon");
                        _manager = new TaskbarToastManager(taskbar);
                    }
                }
                return _manager;
            }
        }

        private static bool IsWindows8OrHigher()
        {
            return Environment.OSVersion.Version.Major >= 6 &&
                   Environment.OSVersion.Version.Minor >= 2;
        }
    }

    internal class TaskbarToastManager : IToastManager
    {
        private readonly TaskbarIcon _taskbar;
        internal TaskbarToastManager(TaskbarIcon icon)
        {
            _taskbar = icon;            
        }

        public void Notify(Toast toast)
        {
            var toastUi = new ToastUi(toast);
            if (toast.OnActivated != null)
            {
                toastUi.Activated += (s, e) => toast.OnActivated();
            }
            _taskbar.ShowCustomBalloon(toastUi, PopupAnimation.Slide, 4000);            
        }        
    }



    internal class Windows8ToastManager : IToastManager
    {
        private string _appId;
        internal Windows8ToastManager(string appId)
        {
            _appId = appId;
        }
        

        private ToastNotifier _notifier;
        public void Notify(Toast toast)
        {
            if( _notifier == null )
                _notifier = ToastNotificationManager.CreateToastNotifier(_appId);
            var toastNotification = new ToastNotification(FormatXml(toast));
            if (toast.OnActivated != null)
            {
                toastNotification.Activated += (s, e) => toast.OnActivated();
            }
            _notifier.Show(toastNotification);
        }

        public XmlDocument FormatXml(Toast toast)
        {
            // Get a toast XML template
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);
            SetText(toastXml, toast.Title, toast.SubText, toast.SubSubText);
            SetImagePath(toastXml, toast.Icon);
            return toastXml;
        }

        private static void SetText(XmlDocument doc, string title, string subtext, string subsubtext)
        {
            // Fill in the text elements
            var stringElements = doc.GetElementsByTagName("text");
            for (var i = 0; i < stringElements.Length; i++)
            {
                string txt = null;
                if (i == 0) txt = title;
                else if (i == 1) txt = subtext;
                else if (i == 2) txt = subsubtext;
                if (txt != null)
                    stringElements[i].AppendChild(doc.CreateTextNode(txt));
            }
        }

        private static void SetImagePath(XmlDocument doc, string icon)
        {
            // Specify the absolute path to an image
            var imagePath = "file:///" + Path.GetFullPath(icon);
            var imageElements = doc.GetElementsByTagName("image");
            if (imageElements == null) return;
            var imageElement = imageElements[0];
            if (imageElement.Attributes == null) return;
            var item = imageElement.Attributes.GetNamedItem("src");
            if (item != null)
            {
                item.NodeValue = imagePath;
            }
        }
    }
}
