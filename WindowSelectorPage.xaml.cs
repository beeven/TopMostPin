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
        public ObservableCollection<PinnedWindowListItem> PinnedWindowList { get => ((App)App.Current).pinnedWindowList; }

        private Command togglePinnedCommand = new Command
        {
            CommandAction = (e) => { MessageBox.Show(e.ToString()); }
        };
        public ICommand TogglePinnedCommand
        {
            get => togglePinnedCommand;
        }

        public WindowSelectorPage()
        {
            InitializeComponent();
            this.DataContext = this;
            //RefreshList();
            ((App)App.Current).notifyIcon.TrayPopupOpen += NotifyIcon_TrayPopupOpen;
        }

        private void NotifyIcon_TrayPopupOpen(object sender, RoutedEventArgs e)
        {
            RefreshList();
        }

        public void RefreshList()
        {
            var windows = API.EnumerateWindows();
            int lenPinned = PinnedWindowList.Count;
            bool[] pinnedWindowStillOpend = new bool[lenPinned];
            for (int i = 0; i < lenPinned; i++)
            {
                pinnedWindowStillOpend[i] = false;
            }
            foreach (var w in windows)
            {
                bool exists = false;
                for (int j = 0; j < lenPinned; j++)
                {
                    if(pinnedWindowStillOpend[j]) { continue; }
                    var p = PinnedWindowList[j];
                    if (w.Handle == p.Handle)
                    {
                        pinnedWindowStillOpend[j] = true;
                        exists = true;
                        if (p.Title != w.Title)
                        {
                            p.Title = w.Title;
                        }
                    }
                }
                if (!exists)
                {
                    PinnedWindowList.Add(w);
                }
            }
            for (int i = pinnedWindowStillOpend.Length; i > 0; i--)
            {
                if (!pinnedWindowStillOpend[i - 1])
                {
                    PinnedWindowList.RemoveAt(i - 1);
                }
            }
        }



        private void ListViewItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var item = ((ListViewItem)sender).Content as PinnedWindowListItem;
            item.Pinned = !item.Pinned;
            if(item.Pinned)
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
