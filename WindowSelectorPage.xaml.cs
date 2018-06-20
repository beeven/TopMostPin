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
        public ObservableCollection<PinnedWindowListItem> PinnedWindowsList { get => ((App)App.Current).pinnedWindowList; }

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
            int lenPinned = PinnedWindowsList.Count;
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
                    var p = PinnedWindowsList[j];
                    if (w.Key == p.Handle)
                    {
                        pinnedWindowStillOpend[j] = true;
                        exists = true;
                        if (p.Title != w.Value)
                        {
                            p.Title = w.Value;
                        }
                    }
                }
                if (!exists)
                {
                    PinnedWindowsList.Add(new PinnedWindowListItem { Pinned = false, Handle = w.Key, Title = w.Value });
                }
            }
            for (int i = pinnedWindowStillOpend.Length; i > 0; i--)
            {
                if (!pinnedWindowStillOpend[i - 1])
                {
                    PinnedWindowsList.RemoveAt(i - 1);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
