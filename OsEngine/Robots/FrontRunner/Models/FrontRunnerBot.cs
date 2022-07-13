using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;
using OsEngine.Robots.FrontRunner.ViewModels;
using OsEngine.Robots.FrontRunner.Views;

namespace OsEngine.Robots.FrontRunner.Models
{
    public class FrontRunnerBot : BotPanel
    {
        #region Constructors ----------------------------------------------------------------------
        public FrontRunnerBot(string name, StartProgram startProgram) : base(name, startProgram)
        {
            TabCreate(BotTabType.Simple);
            _tab = TabsSimple[0];
            _tab.MarketDepthUpdateEvent += _tab_MarketDepthUpdateEvent; // подписывается на событие изменение стакана
        }

        #endregion --------------------------------------------------------------------------------
        #region Fields ----------------------------------------------------------------------------

        public decimal BigVolume = 20000;
        public int Offset = 1;
        public int Take = 5;
        public decimal Lot = 2;
        public Position Position = null;
        public int sleep = 1000;
        private BotTabSimple _tab;

        #endregion --------------------------------------------------------------------------------
        #region Properties ------------------------------------------------------------------------
        public string ShowDirection { get; private set; }
        public decimal ShowOpenVolume { get; private set; }
        public decimal ShowOpenPrice { get; private set; }
        public string ShowTake { get; private set; }
        public decimal ShowTakePrice { get; private set; }

        public Edit Edit
        {
            get => _edit;
            set
            {
                _edit = value;

                if (_edit == Edit.Stop
                    && Position != null
                    && Position.State == PositionStateType.Opening)
                {
                    _tab.CloseAllOrderInSystem();
                }
            }
        }
        public Edit _edit = ViewModels.Edit.Stop;

        #endregion --------------------------------------------------------------------------------
        #region Methods ---------------------------------------------------------------------------

        private void _tab_MarketDepthUpdateEvent(MarketDepth marketDepth)
        {
            if (Edit == Edit.Stop)
            {
                ShowDirection = "Робот остановлен";
                ShowOpenVolume = 0;
                ShowOpenPrice = 0;
                ShowTake = "Не выставлен";
                ShowTakePrice = 0;

                EventTradeDelegate?.Invoke();
                
                return;
            }

            if (marketDepth.SecurityNameCode != _tab.Securiti.Name) return;

            List<Position> positions = _tab.PositionsOpenAll;

            if (positions != null)
            {
                if (positions.Count > 0)
                {
                    foreach (Position pos in positions)
                    {
                        if (pos.Direction == Side.Sell)
                        {
                            if (pos.State == PositionStateType.Open)
                            {
                                ShowDirection = "Открыта позиция в Short";
                            }
                            else if (pos.State == PositionStateType.Opening)
                            {
                                ShowDirection = "Выставлена заявка в Short";
                            }
                            
                            decimal takePrice = pos.EntryPrice - Take * _tab.Securiti.PriceStep;
                            _tab.CloseAtProfit(pos, takePrice, takePrice);
                            Thread.Sleep(sleep);
                        }
                        else if (pos.Direction == Side.Buy)
                        {
                            if (pos.State == PositionStateType.Open)
                            {
                                ShowDirection = "Открыта позиция в Long";
                            }
                            else if (pos.State == PositionStateType.Opening)
                            {
                                ShowDirection = "Выставлена заявка в Long";
                            }
                            
                            decimal takePrice = pos.EntryPrice + Take * _tab.Securiti.PriceStep;
                            _tab.CloseAtProfit(pos, takePrice, takePrice);
                            Thread.Sleep(sleep);
                        }
                    }
                }
            }

            if (Position == null)
            {
                ShowDirection = "Определяю";
                ShowOpenVolume = 0;
                ShowOpenPrice = 0;
                ShowTake = "Не выставлен";
                ShowTakePrice = 0;
            }
            else
            {
                ShowOpenVolume = Position.MaxVolume;
                ShowOpenPrice = Position.EntryPrice;
            }

            bool FlagBigValumeShort = false;
            
            for (int i = 0; i < marketDepth.Asks.Count; i++)
            {
                if (Position == null)
                {
                    if (marketDepth.Asks[i].Ask >= BigVolume)
                    {
                        decimal price = marketDepth.Asks[i].Price - Offset * _tab.Securiti.PriceStep;
                        Position = _tab.SellAtLimit(Lot, price);
                        Thread.Sleep(sleep);
                    }
                }
                
                if (Position != null)
                {
                    if (Position.State == PositionStateType.Open)
                    {
                        if (Position.ProfitOrderIsActiv)
                        {
                            ShowTake = "Выставлен";
                        }

                        ShowTakePrice = Position.ProfitOrderPrice;
                    }

                    // цена максимального объёма в продажу, от которого открыли позицию
                    decimal PriceBigVolumeSell = Position.EntryPrice + Offset * _tab.Securiti.PriceStep;

                    if (marketDepth.Asks[i].Price < PriceBigVolumeSell)
                    {
                        if (marketDepth.Asks[i].Ask >= BigVolume)
                        {
                            if (Position.State == PositionStateType.Opening)
                            {
                                _tab.CloseAllOrderInSystem();
                                Position = null;
                                Thread.Sleep(sleep);
                                break;
                            }
                        }
                    }

                    if (marketDepth.Asks[i].Price == PriceBigVolumeSell)
                    {
                        if (marketDepth.Asks[i].Ask < BigVolume / 2)
                        {
                            if (Position.State == PositionStateType.Open)
                            {
                                _tab.CloseAtMarket(Position, Position.OpenVolume);
                            }
                            else if (Position.State == PositionStateType.Opening)
                            {
                                _tab.CloseAllOrderInSystem();
                            }
                            
                            Thread.Sleep(sleep);
                        }
                        else
                        {
                            FlagBigValumeShort = true;
                        }
                    }
                    
                    if (marketDepth.Asks[i].Price > PriceBigVolumeSell)
                    {
                        if (Position.Direction == Side.Sell)
                        {
                            if (!FlagBigValumeShort)
                            {
                                if (Position.State == PositionStateType.Open)
                                {
                                    _tab.CloseAtMarket(Position, Position.OpenVolume);
                                }
                                else if (Position.State == PositionStateType.Opening)
                                {
                                    _tab.CloseAllOrderInSystem();
                                }
                                
                                Thread.Sleep(sleep);
                            }
                        }
                    }
                    
                    if (Position.State != PositionStateType.Open
                        && Position.State != PositionStateType.Opening)
                    {
                        Position = null;
                    }
                }

                EventTradeDelegate?.Invoke();
            }
            
            bool FlagBigValumeLong = false;

            for (int i = 0; i < marketDepth.Bids.Count; i++)
            {
                if (Position == null)
                {
                    if (marketDepth.Bids[i].Bid >= BigVolume)
                    {
                        decimal price = marketDepth.Bids[i].Price + Offset * _tab.Securiti.PriceStep;
                        Position = _tab.BuyAtLimit(Lot, price); // обновим переменную в момент выставления заявки,
                                                                // а не после того, как заявка выставится
                        Thread.Sleep(sleep);
                    }
                }

                if (Position != null)
                {
                    // цена максимального объёма в покупку, от которого открыли позицию
                    decimal PriceBigVolumeBuy = Position.EntryPrice - Offset * _tab.Securiti.PriceStep;
                    
                    if (Position.State == PositionStateType.Open)
                    {
                        if (Position.ProfitOrderIsActiv)
                        {
                            ShowTake = "Выставлен";
                        }

                        ShowTakePrice  = Position.ProfitOrderPrice;
                    }

                    if (marketDepth.Bids[i].Price > PriceBigVolumeBuy)
                    {
                        if (marketDepth.Bids[i].Bid >= BigVolume)
                        {
                            if (Position.State == PositionStateType.Opening)
                            {
                                _tab.CloseAllOrderInSystem();
                                Position = null;
                                Thread.Sleep(sleep);
                                break;
                            }
                        }
                    }

                    if (marketDepth.Bids[i].Price == PriceBigVolumeBuy)
                    {
                        if (marketDepth.Bids[i].Bid < BigVolume / 2)
                        {
                            if (Position.State == PositionStateType.Open)
                            {
                                _tab.CloseAtMarket(Position, Position.OpenVolume);
                            }
                            else if (Position.State == PositionStateType.Opening)
                            {
                                _tab.CloseAllOrderInSystem();
                            }
                            
                            Thread.Sleep(sleep);
                        }
                        else
                        {
                            FlagBigValumeLong = true;
                        }
                    }
                    
                    if (marketDepth.Bids[i].Price < PriceBigVolumeBuy)
                    {
                        if (Position.Direction == Side.Buy)
                        {
                            if (!FlagBigValumeLong)
                            {
                                if (Position.State == PositionStateType.Open)
                                {
                                    _tab.CloseAtMarket(Position, Position.OpenVolume);
                                }
                                else if (Position.State == PositionStateType.Opening)
                                {
                                    _tab.CloseAllOrderInSystem();
                                }
                                
                                Thread.Sleep(sleep);
                            }
                        }
                    }
                    
                    if (Position.State != PositionStateType.Open
                        && Position.State != PositionStateType.Opening)
                    {
                        Position = null;
                    }
                }

                EventTradeDelegate?.Invoke();
            }
        }

        public override string GetNameStrategyType()
        {
            return "FrontRunnerBot";
        }

        public override void ShowIndividualSettingsDialog()
        {
            FrontRunnerUi window = new FrontRunnerUi(this); // добавляем наше окошко
            window.Show(); // чтобы отобразилось наше окно
        }
        
        #endregion --------------------------------------------------------------------------------
        #region---------------------------------- Events ------------------------------------------

        public delegate void tradeDelegate();
        public event tradeDelegate EventTradeDelegate;

        #endregion---------------------------------------------------------------------------------
    }
}
