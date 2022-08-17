using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ControlzEx.Standard;
using System.Windows.Documents;
using OsEngine.Market;
using OsEngine.Robots.FrontRunner.Commands;
using OsEngine.Views;

namespace OsEngine.ViewModels
{
    public class RobotWindowVM : BaseVM
    {
        #region Constructor -----------------------------------------------------------------------

        public RobotWindowVM()
        {
            /*
            Robots.Add(new MyRobotVM() // создаём вкладку 1
            {
                Header = "Tab_1",
                StartPoint = 1000
            });

            Robots.Add(new MyRobotVM() // создаём вкладку 2
            {
                Header = "Tab_2",
                StartPoint = 2000
            });
            */
        }

        #endregion --------------------------------------------------------------------------------
        #region Properties ------------------------------------------------------------------------

        public ObservableCollection<MyRobotVM> Robots { get; set; } = new ObservableCollection<MyRobotVM>();

        #endregion --------------------------------------------------------------------------------
        #region Fields ----------------------------------------------------------------------------

        public static ChangeEmitentWindow ChangeEmitentWindow = null;

        #endregion --------------------------------------------------------------------------------
        #region Commands --------------------------------------------------------------------------

        private DelegateCommand commandServersToConnect;

        public DelegateCommand CommandServersToConnect
        {
            get
            {
                if (commandServersToConnect == null)
                {
                    commandServersToConnect = new DelegateCommand(ServersToConnect);
                }

                return commandServersToConnect;
            }
        }

        private DelegateCommand commandAddEmitent;

        public DelegateCommand CommandAddEmitent
        {
            get
            {
                if (commandAddEmitent == null)
                {
                    commandAddEmitent = new DelegateCommand(AddTabEmitent);
                }

                return commandAddEmitent;
            }
        }

        private DelegateCommand commandDeleteEmitent;

        public DelegateCommand CommandDeleteEmitent
        {
            get
            {
                if (commandDeleteEmitent == null)
                {
                    commandDeleteEmitent = new DelegateCommand(DeleteTabEmitent);
                }

                return commandDeleteEmitent;
            }
        }

        #endregion --------------------------------------------------------------------------------
        #region Methods ---------------------------------------------------------------------------

        private void ServersToConnect(object e)
        {
            ServerMaster.ShowDialog(false);
        }

        private void AddTabEmitent(object e)
        {
            Robots.Add(new MyRobotVM()
            {
                Header = "Tab " + (Robots.Count + 1)
            });
        }

        private void DeleteTabEmitent(object obj)
        {
            string header = obj as string;

            MyRobotVM delRobot = null;

            foreach (var robot in Robots)
            {
                if (robot.Header == header)
                {
                    delRobot = robot;
                    break;
                }
            }

            if (delRobot != null)
            {
                MessageBoxResult res = MessageBox.Show("Remove tab " + header + " ?", header, MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    Robots.Remove(delRobot);
                }
            }
        }

        #endregion --------------------------------------------------------------------------------
    }
}
