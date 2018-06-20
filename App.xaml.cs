using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Shell;


namespace TopMostPin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {

        public TaskbarIcon notifyIcon;
        public ObservableCollection<PinnedWindowListItem> pinnedWindowList = new ObservableCollection<PinnedWindowListItem>();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            notifyIcon.ShowBalloonTip("TopMostPin is running.", "TopMostPin is running in the background.", BalloonIcon.Info);
            
        }

        

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon.Dispose();
            base.OnExit(e);
        }



        [STAThread]
        public static void Main()
        {
            if(SingleInstance<App>.InitializeAsFirstInstance("me.beeven.TopMostPin"))
            {
                var application = new App();
                application.InitializeComponent();
                application.Run();
                SingleInstance<App>.Cleanup();
            }
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            return true;
        }
    }
}
