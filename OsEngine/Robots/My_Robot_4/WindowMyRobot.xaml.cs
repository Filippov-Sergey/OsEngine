using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using OsEngine.OsTrader.Panels.Tab;
using OsEngine.Robots.MyRobots.ViewModels;

namespace OsEngine.Robots.MyRobots
{
    /// <summary>
    /// Логика взаимодействия для WindowMyRobot.xaml
    /// </summary>
    public partial class WindowMyRobot : Window
    {
        public WindowMyRobot(MyRobot robot)
        {
            InitializeComponent();
            vm = new VM(robot);
            DataContext = vm; // привязали поле vm к DataContext
        }
        private VM vm; // создали VM
    }
}
