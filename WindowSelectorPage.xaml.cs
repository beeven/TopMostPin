using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TopMostPin
{
    /// <summary>
    /// Interaction logic for WindowSelectorPage.xaml
    /// </summary>
    public partial class WindowSelectorPage : Page
    {
        public ObservableCollection<PinnedWindowListItem> PinWindows { get; } = new ObservableCollection<PinnedWindowListItem>();

        public ICommand TogglePinnedCommand { get; } = new Command
        {
            CommandAction = (e) => { MessageBox.Show(e.ToString()); }
        };

        public WindowSelectorPage()
        {
            InitializeComponent();
            this.DataContext = this;
            ((App)Application.Current).notifyIcon.TrayPopupOpen += NotifyIcon_TrayPopupOpen;
            this.IsVisibleChanged += WindowSelectorPage_IsVisibleChanged;
        }

        private void WindowSelectorPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
            {
                ((App)Application.Current).pinnedWindowList = PinWindows.Where(x => x.Pinned == true).ToList();
            }
        }

        private void NotifyIcon_TrayPopupOpen(object sender, RoutedEventArgs e)
        {
            RefreshList();
        }

        public void RefreshList()
        {
            var windows = API.EnumerateWindows();

            var pinnedWindowList = ((App)Application.Current).pinnedWindowList;

            foreach (var p in pinnedWindowList)
            {
                for (int i = 0; i < windows.Count; i++)
                {
                    var w = windows[i];
                    if (w.Pinned) { continue; }
                    if (w.Handle == p.Handle)
                    {
                        w.Pinned = true;
                    }
                }
            }

            PinWindows.Clear();
            foreach (var w in windows)
            {
                PinWindows.Add(w);
            }

        }



        private void ListViewItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var item = ((ListViewItem)sender).Content as PinnedWindowListItem;
            item.Pinned = !item.Pinned;
            if (item.Pinned)
            {
                API.PinWindow(item.Handle);
            }
            else
            {
                API.UnPinWindow(item.Handle);
            }

        }
    }
}
