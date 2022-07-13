using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Robots.My_Robot.Model;

namespace OsEngine.Robots.My_Robot.ViewModel
{
    public class VM : BaseVM // наследуемся от базового класса
    {
        #region Constructor -----------------------------------------------------------------------

        public VM(MyRobot robot) // создаём конструктор
        {
            _robot = robot;
        }
        private MyRobot _robot; // создаём поле

        #endregion --------------------------------------------------------------------------------

        #region Properties ------------------------------------------------------------------------

        public int Robot_Lot // создаём свойство для Robot_Lot из MainWindow
        {
            get => _robot_Lot;
            set
            {
                _robot_Lot = value;
                OnPropertyChanged(nameof(Robot_Lot)); // извещаем вьюшку что здесь изменилось значение
            }
        }
        private int _robot_Lot = 1; // поле под Robot_Lot

        public int Robot_Take // создаём свойство для Robot_Take из MainWindow
        {
            //get => _robot_Lot;
            get => _robot_Take;
            set
            {
                _robot_Take = value;
                OnPropertyChanged(nameof(Robot_Take)); // извещаем вьюшку что здесь изменилось значение
            }
        }
        private int _robot_Take = 2; // поле под Robot_Take

        public int Robot_Stop // создаём свойство для Robot_Stop из MainWindow
        {
            //get => _robot_Lot;
            get => _robot_Stop;
            set
            {
                _robot_Stop = value;
                OnPropertyChanged(nameof(Robot_Stop)); // извещаем вьюшку что здесь изменилось значение
            }
        }
        private int _robot_Stop = 3; // поле под Robot_Stop

        #endregion --------------------------------------------------------------------------------
    }
}
