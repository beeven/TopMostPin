using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TopMostPin
{
    public class NotifyIconViewModel : INotifyPropertyChanged
    {
        public ICommand UnpinAllCommand { get; } = new Command
        {
            CanExecuteFunc = (e) => ((App)App.Current).pinnedWindowList.Count > 0,
            CommandAction = (e) =>
            {
                var pinnedWindowList = ((App)App.Current).pinnedWindowList;
                foreach (var item in pinnedWindowList)
                {
                    API.UnPinWindow(item.Handle);
                }
                pinnedWindowList.Clear();
            }
        };

        public ICommand ShowSettingsWindowCommand { get; } = new Command
        {
            CanExecuteFunc = (e) => Application.Current.MainWindow == null,
            CommandAction = (e) =>
            {
                Application.Current.MainWindow = new MainWindow();
                Application.Current.MainWindow.Show();
            }
        };


        public ICommand ExitApplicationCommand { get; } = new Command
        {
            CommandAction = (e) => Application.Current.Shutdown()
        };

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
