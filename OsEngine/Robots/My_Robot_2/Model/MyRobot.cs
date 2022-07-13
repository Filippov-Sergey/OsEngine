using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;
using OsEngine.Robots.My_Robot.View;

namespace OsEngine.Robots.My_Robot.Model
{
    public class MyRobot : BotPanel
    {
        #region Constructor -----------------------------------------------------------------------

        // не забыть добавить робота в BotFactory, в public static List<string> GetNamesStrategy() и ниже в
        // public static BotPanel GetStrategyForName(string nameClass, string name, StartProgram startProgram, bool isScript)

        public MyRobot(string name, StartProgram startProgram) : base(name, startProgram)
        {
            this.TabCreate(BotTabType.Simple); // создаём вкладку для работы с инструментом
            //this.TabCreate(BotTabType.Simple); // создаём вторую вкладку для работы со вторым инструментом

            _tab_1 = TabsSimple[0]; // получаем вкладку
            //_tab_2 = TabsSimple[1]; // получаем вторую вкладку

            this.CreateParameter("Mode", "Edit", new[] { "Edit", "Trade" }); // создаём параметр для оптимизации
            this.CreateParameter("Lot", 1, 1, 100, 1); // создаём другой параметр для оптимизации
            this.CreateParameter("Take", 1, 1, 100, 1);
            this.CreateParameter("Stop", 1, 1, 100, 1);
        }

        #endregion --------------------------------------------------------------------------------

        #region Fields ----------------------------------------------------------------------------

        private BotTabSimple _tab_1; // создаём вкладку
        //private BotTabSimple _tab_2; // создаём вторую вкладку 

        #endregion --------------------------------------------------------------------------------

        #region Properties ------------------------------------------------------------------------


        #endregion --------------------------------------------------------------------------------

        // реализуем абстрактный класс
        public override string GetNameStrategyType()
        {
            //return "MyRobot"; // можно так
            return nameof(MyRobot); // можно так
        }

        // метод настройки торговли бота, можем запустить своё окно для настройки параметров
        public override void ShowIndividualSettingsDialog() // создаём экземпляр окна
        {
            WindowMyRobot window = new WindowMyRobot(this); // добавляем наше окошко

            StrategyParameterString paramString = (StrategyParameterString)Parameters[0]; // для первого строкового параметра
            StrategyParameterInt paramIntLot = (StrategyParameterInt)Parameters[1]; // для второго int параметра
            StrategyParameterInt paramIntTake = (StrategyParameterInt)Parameters[2];
            StrategyParameterInt paramIntStop = (StrategyParameterInt)Parameters[3];

            window.TextRobotLot.Text = "Lot = " + paramIntLot.ValueInt; // чтобы отобразилось наше окно с параметром
            window.TextRobotTake.Text = "Take = " + paramIntTake.ValueInt;
            window.TextRobotStop.Text = "Stop = " + paramIntStop.ValueInt;

            window.ShowDialog(); // чтобы отобразилось наше окно
        }
    }
}
