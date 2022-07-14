using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Entity;
using OsEngine.Indicators;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;

namespace OsEngine.Robots.PriceChanel.Model
{
    public class PriceChannelFix : BotPanel
    {
        #region Constructors ----------------------------------------------------------------------

        public PriceChannelFix(string name, StartProgram startProgram) : base(name, startProgram)
        {
            TabCreate(BotTabType.Simple); // создаёт вкладку
            _tab = TabsSimple[0]; // получает вкладку

            // Indicators -------------------------------------------------------------------------

            LenghtUp = CreateParameter("Lenght Channel Up", 12, 5, 80, 2);
            LenghtDown = CreateParameter("Lenght Channel Down", 12, 5, 80, 2);
            Lot = CreateParameter("Lot", 10, 5, 20, 1);
            Risk = CreateParameter("Risk", 1m, 0.2m, 3m, 0.1m);
            Mode = CreateParameter("Mode", "Off", new[] {"Off", "On"});
            // создаём индикатор
            _pc = IndicatorsFactory.CreateIndicatorByName("PriceChannel", name + "PriceChannel", false);
            _pc.ParametersDigit[0].Value = LenghtUp.ValueInt; // параметры индикатора
            _pc.ParametersDigit[1].Value = LenghtDown.ValueInt; // параметры индикатора
            _pc = (Aindicator)_tab.CreateCandleIndicator(_pc, "Prime"); // вписываем индикатор для отображения
            _pc.Save(); // сохраняем

            // ------------------------------------------------------------------------------------

            _tab.CandleFinishedEvent += _tab_CandleFinishedEvent; // подписываемся на событие окончания свечи
        }

        #endregion --------------------------------------------------------------------------------
        #region Fields ----------------------------------------------------------------------------

        private BotTabSimple _tab; // вкладка
        
        // Indicators -----------------------------------------------------------------------------
        
        private Aindicator _pc; // индикатор

        // параметры индикатора
        private StrategyParameterString Mode;
        
        private StrategyParameterInt LenghtUp;
        private StrategyParameterInt LenghtDown;
        private StrategyParameterInt Lot;
        
        private StrategyParameterDecimal Risk;

        // ----------------------------------------------------------------------------------------

        #endregion --------------------------------------------------------------------------------
        #region Methods ---------------------------------------------------------------------------

        private void _tab_CandleFinishedEvent(List<Candle> candles)
        {
            if (Mode.ValueString == "Off")
            {
                return;
            }

            if (_pc.DataSeries[0].Values == null ||
                _pc.DataSeries[1].Values == null ||
                _pc.DataSeries[0].Values.Count < LenghtUp.ValueInt + 1 ||
                _pc.DataSeries[1].Values.Count < LenghtDown.ValueInt + 1)
            {
                return;
            }

            Candle candle = candles[candles.Count - 1]; // получает последнюю свечу
            
            // получает последнее значение для индикатора
            decimal lastUp = _pc.DataSeries[0].Values[_pc.DataSeries[0].Values.Count - 2];
            decimal lastDown = _pc.DataSeries[1].Values[_pc.DataSeries[1].Values.Count - 2];

            // trading logic ----------------------------------------------------------------------

            List<Position> positions = _tab.PositionsOpenAll;

            // открываем позицию в лонг -------------------------------------------------

            if (candle.Close > lastUp && candle.Open < lastUp && positions.Count == 0)
            {
                decimal riskMoney = _tab.Portfolio.ValueBegin * Risk.ValueDecimal / 100;
                decimal costPriceStep = _tab.Securiti.PriceStepCost;
                costPriceStep = 1; // для теста на Si
                decimal steps = (lastUp - lastDown) / _tab.Securiti.PriceStep;
                decimal lot = riskMoney / steps * costPriceStep;
                _tab.BuyAtMarket((int)lot);
            }

            //---------------------------------------------------------------------------

            if (positions.Count > 0)
            {
                Traling(positions); // выставляет треёлинг стоп
            }
            //-------------------------------------------------------------------------------------
        }
        
        // выставляет треёлинг стоп -----------------------------------------------------
        private void Traling(List<Position> positions)
        {
            decimal lastDown = _pc.DataSeries[1].Values.Last();

            foreach (Position pos in positions)
            {
                if (pos.State == PositionStateType.Open)
                {
                    if (pos.Direction == Side.Buy)
                    {
                        _tab.CloseAtTrailingStop(pos, lastDown, lastDown - 100 * _tab.Securiti.PriceStep);
                    }
                }
            }
        }
        //-------------------------------------------------------------------------------
        #endregion --------------------------------------------------------------------------------
        #region Abstract class --------------------------------------------------------------------

        public override string GetNameStrategyType()
        {
            return nameof(PriceChannelFix);
        }

        public override void ShowIndividualSettingsDialog()
        {
            throw new NotImplementedException();
        }
        
        #endregion --------------------------------------------------------------------------------
    }
}
