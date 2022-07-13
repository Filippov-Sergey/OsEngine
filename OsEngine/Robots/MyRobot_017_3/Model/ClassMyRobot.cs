using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;

namespace OsEngine.Robots.MyRobot_017.Model
{
    public class ClassMyRobot : BotPanel
    {
        #region Constructor -----------------------------------------------------------------------     
        
        public ClassMyRobot(string name, StartProgram startProgram) : base(name, startProgram)
        {
            this.TabCreate(BotTabType.Simple);
            _tab = TabsSimple[0];
            
            // создаём параметры
            this.CreateParameter("Mode", "Edit", new[] { "Edit", "Trade" }); // выбор режима
            _risk = CreateParameter("Risk %", 1m, 0.1m, 10m, 0.1m); // риск в процентах
            _profitKoef = CreateParameter("Koef Profit", 1m, 0.1m, 10m, 0.1m); // профит
            _countDownCandles = CreateParameter("Count Down Candles", 1, 1, 5, 1);
            _koefVolume = CreateParameter("Koef Volume", 2m, 2m, 10m, 0.5m);
            _countCandles = CreateParameter("Count Candles", 10, 5, 50, 1);

            // подписывается на событие закрытия свечи и создаёт метод
            _tab.CandleFinishedEvent += _tab_CandleFinishedEvent;

            // подписывается на событие открытия позиции и создаёт метод
            _tab.PositionOpeningSuccesEvent += _tab_PositionOpeningSuccesEvent;

            // подписывается на событие закрытия позиции и создаёт метод
            _tab.PositionClosingSuccesEvent += _tab_PositionClosingSuccesEvent;
        }

        #endregion --------------------------------------------------------------------------------
        #region Fields ----------------------------------------------------------------------------

        private BotTabSimple _tab;

        /// <summary>
        /// риск на сделку в процентах
        /// </summary>
        private StrategyParameterDecimal _risk;

        /// <summary>
        /// во сколько раз профит больше риска
        /// </summary>
        private StrategyParameterDecimal _profitKoef;

        /// <summary>
        /// во сколько раз текущий объём больше чем средний объём
        /// </summary>
        private StrategyParameterDecimal _koefVolume;

        /// <summary>
        /// количество падающих свечей перед объёмным разворотом
        /// </summary>
        private StrategyParameterInt _countDownCandles;

        /// <summary>
        /// средняя объёмов за определённое количество свечей
        /// </summary>
        private decimal _averageVolume;

        /// <summary>
        /// количество пунктов до стопа
        /// </summary>
        private int _punkts = 0;

        private decimal _lowCandle = 0;

        /// <summary>
        /// количество свечей для вычисления среднего объёма
        /// </summary>
        private StrategyParameterInt _countCandles;

        #endregion --------------------------------------------------------------------------------
        #region Methods ---------------------------------------------------------------------------

        // создаёт метод на событие закрытия новой свечи ------------
        private void _tab_CandleFinishedEvent(List<Candle> candles) // получает список свечей
        {
            // если свечей меньше, чем нам необходимо для расчёта падающих свечей или для расчёта средней объёмов
            if (candles.Count < _countDownCandles.ValueInt + 1 || candles.Count < _countCandles.ValueInt + 1)
            {
                return;
            }

            _averageVolume = 0; // сбрасываем среднюю объёмов

            for (int i = candles.Count - 2; i > candles.Count - _countCandles.ValueInt - 2; i--)
            {
                _averageVolume += candles[i].Volume; // просуммировали объёмы
            }

            _averageVolume /= _countCandles.ValueInt; // посчитали среднюю объёмов

            List<Position> positions = _tab.PositionOpenLong; // есть ли открытые позиции

            if (positions.Count > 0)
            {
                // передвигает стоп в безубыток если цена выросла больше величины одного стопа и есть открытая позиция
                if (_tab.PositionsLast.OpenVolume > 0) // количество лотов
                {
                    // считает размер стопа последней позиции
                    decimal sizeStop = _tab.PositionsLast.EntryPrice - _tab.PositionsLast.StopOrderPrice;
                    
                    // если цена закрытия свечи выше цены входа на величину стопа
                    if (candles[candles.Count - 1].Close >= _tab.PositionsLast.EntryPrice + sizeStop)
                    {
                        // переставляем стоп в бузубыток
                        _tab.CloseAtStop(_tab.PositionsLast, _tab.PositionsLast.EntryPrice, _tab.PositionsLast.EntryPrice);
                    }
                }
                return;
            }

            Candle candle = candles[candles.Count - 1]; // последняя закрывшаяся свеча

            // если закрытие свечи ниже середины или объём свечи меньше чем необходимо нам
            if (candle.Close < (candle.High + candle.Low) / 2 || candle.Volume < _averageVolume * _koefVolume.ValueDecimal)
            {
                return;
            }
            
            // проверяем необходимое нам количество свечей на соответствие нашему условию
            for (int i = candles.Count - 2; i > candles.Count - 2 - _countDownCandles.ValueInt; i--)
            {
                if (candles[i].Close > candles[i].Open)
                {
                    return;
                }
            }

            // если все проверки пройдены формируем сигнал на открытие позиции
            _punkts = (int)((candle.Close - candle.Low) / _tab.Securiti.PriceStep); // количество пунктов

            if (_punkts < 5)
            {
                return;
            }

            decimal amountStop = _punkts * _tab.Securiti.PriceStepCost; // риск в шагах цены на 1 стоп
            decimal amountRisk = _tab.Portfolio.ValueBegin * _risk.ValueDecimal / 100; // считаем риск в деньгах
            decimal volume = amountRisk / amountStop; // сколько лотов можем открыть
            decimal go = 10000; // гарантийное обеспечение

            if (_tab.Securiti.Go > 1)
            {
                go = _tab.Securiti.Go;
            }

            decimal maxLot = _tab.Portfolio.ValueBegin / go; // сколько максимум лотов можем открыть

            if (volume < maxLot)
            {
                _lowCandle = candle.Low;
                _tab.BuyAtMarket(volume); // покупка по рыночной цене
            }
        }

        // создаёт метод на событие открытия позиции
        private void _tab_PositionOpeningSuccesEvent(Position pos)
        {
            decimal priceTake = pos.EntryPrice + _punkts * _profitKoef.ValueDecimal; // цена тейк профита
            
            _tab.CloseAtProfit(pos, priceTake, priceTake);
            _tab.CloseAtStop(pos, _lowCandle, _lowCandle - 100 * _tab.Securiti.PriceStep);
        }

        // создаёт метод на событие закрытия позиции
        private void _tab_PositionClosingSuccesEvent(Position pos)
        {
            SaveCSV(pos);
        }
        //-----------------------------------------------------------
        // записывает данные в файл для анализа ---------------------
        private void SaveCSV(Position pos)
        {
            if (!File.Exists(@"Engine\trades.csv")) // если файл не существует
            {
                string header = ";Позиция;Символ;Лоты;Изменение/Максимум Лотов;Исполнение входа;Сигнал входа;Бар входа;Дата входа;" +
                    "Время входа;Цена входа;Комиссия входа;Исполнение выхода;Сигнал выхода;Бар выхода;Дата выхода;Время выхода;" +
                    "Цена выхода;Комиссия выхода;Средневзвешенная цена входа;П/У;П/У сделки;П/У с одного лота;Зафиксированная П/У;" +
                    "Открытая П/У;Продолж. (баров);Доход/Бар;Общий П/У;% изменения;MAE;MAE %;MFE;MFE %";

                using (StreamWriter writer = new StreamWriter(@"Engine\trades.csv", false))
                {
                    writer.WriteLine(header);
                    writer.Close();
                }
            }

            using (StreamWriter writer = new StreamWriter(@"Engine\trades.csv", true))
            {
                string str = ";;;;;;;;" + pos.TimeOpen.ToShortDateString();
                str += ";" + pos.TimeOpen.TimeOfDay;
                str += ";;;;;;;;;;;;;;" + pos.ProfitPortfolioPunkt + ";;;;;;;;";
                writer.WriteLine(str);
                writer.Close();
            }
        }
        #endregion --------------------------------------------------------------------------------
        public override string GetNameStrategyType()
        {
            return nameof(ClassMyRobot);
        }

        public override void ShowIndividualSettingsDialog()
        {
            
        }
    }
}
