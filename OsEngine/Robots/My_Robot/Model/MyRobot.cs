using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;
using OsEngine.Robots.My_Robot.View;
using OsEngine.Robots.My_Robot.ViewModel;

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
            
            paramIntLot  = CreateParameter("Lot", 1, 1, 100, 1); // создаём другой параметр для оптимизации
            paramIntTake = CreateParameter("Take", 1, 1, 100, 1);
            paramIntStop = CreateParameter("Stop", 1, 1, 100, 1);
        }

        #endregion --------------------------------------------------------------------------------

        #region Fields ----------------------------------------------------------------------------

        private BotTabSimple _tab_1; // создаём вкладку
        //private BotTabSimple _tab_2; // создаём вторую вкладку 

        #endregion --------------------------------------------------------------------------------

        #region Properties ------------------------------------------------------------------------

        public StrategyParameterInt paramIntLot  { get; set; }
        public StrategyParameterInt paramIntTake { get; set; }
        public StrategyParameterInt paramIntStop { get; set; }

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
            window.ShowDialog(); // чтобы отобразилось наше окно
        }
    }
}
