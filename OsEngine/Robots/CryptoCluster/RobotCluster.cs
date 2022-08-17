using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Entity;
using OsEngine.Indicators;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;

namespace OsEngine.Robots.CryptoCluster
{
    public class RobotCluster : BotPanel
    {
        #region Constructor -----------------------------------------------------------------------

        public RobotCluster(string name, StartProgram startProgram) : base(name, startProgram)
        {
            TabCreate(BotTabType.Simple);
            _tabSimple = TabsSimple[0]; // список где храним вкладки типа TabsSimple

            TabCreate(BotTabType.Cluster); // вкладка позволит работать с кластерами
            _tabCluster = TabsCluster[0];  // список где храним вкладки типа TabsCluster

            Mode = CreateParameter("Mode", false);
            Koef = CreateParameter("Koef", 3, 3, 9, 1);
            CountCandles = CreateParameter("CountCandles", 5, 3, 9, 1);
            Risk = CreateParameter("Risk %", 1m, 0.1m, 2m, 0.1m);
            Stop = CreateParameter("Stop ATR", 1, 1, 1, 1);
            Take = CreateParameter("Take ATR", 3, 1, 10, 1);
            Depo = CreateParameter("Depo", 1000, 1000, 1000, 1);
            MinVolumeDollar = CreateParameter("MinVolumeDollar", 100000, 100000, 100000, 50000);

            _atr = IndicatorsFactory.CreateIndicatorByName("ATR", name + "ATR", false);
            _atr.ParametersDigit[0].Value = 100;
            _atr = (Aindicator)_tabSimple.CreateCandleIndicator(_atr, "Second"); // вписываем индикатор для отображения
            _atr.PaintOn = true;
            _atr.Save(); // сохраняем


            _tabSimple.CandleFinishedEvent += _tabSimple_CandleFinishedEvent;
            //_tabSimple.PositionOpenLong +=

        }

        #endregion --------------------------------------------------------------------------------
        #region Fields ----------------------------------------------------------------------------

        private BotTabSimple _tabSimple;
        private BotTabCluster _tabCluster;

        // параметры робота -----------------------------------------------------------------------

        public StrategyParameterBool Mode; // включен робот или нет

        // во сколько раз объём последней свечи был больше среднего арифмитического объёмов последних 4 свечей
        public StrategyParameterInt Koef;

        public StrategyParameterInt CountCandles; // сколько свечей будем анализировать
        public StrategyParameterDecimal Risk; // процент риска на 1 сделку
        public StrategyParameterInt Stop;
        public StrategyParameterInt Take;
        public StrategyParameterInt Depo;
        public StrategyParameterInt MinVolumeDollar;

        Aindicator _atr; // индикатор

        private decimal _stopPrice = 0;
        private decimal _takePrice = 0;
        //-----------------------------------------------------------------------------------------

        #endregion --------------------------------------------------------------------------------
        #region Method ----------------------------------------------------------------------------

        private void _tabSimple_CandleFinishedEvent(List<Candle> candles)
        {
            if (candles.Count < CountCandles.ValueInt ||
                _tabCluster.VolumeClusters.Count < CountCandles.ValueInt)
            {
                return;
            }

            List<Position> positions = _tabSimple.PositionOpenLong;

            if (positions == null || positions.Count == 0)
            {
                decimal average = 0;
                for (int i = _tabCluster.VolumeClusters.Count - CountCandles.ValueInt;
                        i < _tabCluster.VolumeClusters.Count - 2; i++)
                {
                    average += _tabCluster.VolumeClusters[i].MaxSummVolumeLine.VolumeSumm;
                    average /= (CountCandles.ValueInt - 1);
                    HorizontalVolumeLine last = _tabCluster.VolumeClusters[_tabCluster.VolumeClusters.Count - 2].MaxSummVolumeLine;

                    if (last.VolumeSumm > average * Koef.ValueInt &&
                        last.VolumeDelta < 0 && last.VolumeSumm * last.Price > MinVolumeDollar.ValueInt)
                    {
                        decimal lastATR = _atr.DataSeries[0].Last;
                        decimal moneyRisk = Depo.ValueInt * Risk.ValueDecimal / 100;
                        decimal volume = moneyRisk / (lastATR * Stop.ValueInt);

                        _tabSimple.BuyAtMarket(volume);
                        
                        _stopPrice = candles[candles.Count - 1].Close - lastATR;
                        _takePrice = candles[candles.Count - 1].Close + lastATR * Take.ValueInt;
                    }
                }
            }
            else
            {
                foreach (Position pos in positions)
                {
                     if (pos.State == PositionStateType.Open)
                    {
                        _tabSimple.CloseAtStop(pos, _stopPrice, _stopPrice - 100 * _tabSimple.Securiti.PriceStep);
                        _tabSimple.CloseAtProfit(pos, _takePrice, _takePrice);
                    }
                }
            }
        }


        #endregion --------------------------------------------------------------------------------
        #region Abstract Class --------------------------------------------------------------------

        public override string GetNameStrategyType()
        {
            return nameof(RobotCluster);
        }

        public override void ShowIndividualSettingsDialog()
        {
            throw new NotImplementedException();
        }
        
        #endregion --------------------------------------------------------------------------------
    }
}
