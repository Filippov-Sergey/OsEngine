using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            get => _robot.paramIntLot.ValueInt;
            set
            {
                if(_robot.paramIntLot.ValueInt != value)
                {
                    _robot.paramIntLot.ValueInt = value;
                    OnPropertyChanged(nameof(Robot_Lot)); // извещаем вьюшку что здесь изменилось значение
                }
            }
        }

        public int Robot_Take // создаём свойство для Robot_Take из MainWindow
        {
            get => _robot.paramIntTake.ValueInt;
            set
            {
                if (_robot.paramIntTake.ValueInt != value)
                {
                    _robot.paramIntTake.ValueInt = value;
                    OnPropertyChanged(nameof(Robot_Take));
                }
            }
        }

        public int Robot_Stop // создаём свойство для Robot_Stop из MainWindow
        {
            get => _robot.paramIntStop.ValueInt;
            set
            {
                if (_robot.paramIntStop.ValueInt != value)
                {
                    _robot.paramIntStop.ValueInt = value;
                    OnPropertyChanged(nameof(Robot_Stop));
                }
            }
        }

        #endregion --------------------------------------------------------------------------------
    }
}
