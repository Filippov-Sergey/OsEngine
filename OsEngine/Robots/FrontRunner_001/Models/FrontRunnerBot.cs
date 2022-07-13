﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

        public decimal ShowBigVolume { get; private set; }

        public decimal ShowPriceBigVolume { get; private set; }
        public decimal ShowPriceTakeProfit { get; private set; }
        public decimal ShowPriceLimit { get; private set; }

        private BotTabSimple _tab;

        #endregion --------------------------------------------------------------------------------
        #region Properties ------------------------------------------------------------------------
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
                    //_tab.CloseAllOrderToPosition(Position);
                    _tab.CloseAllOrderInSystem();
                }
            }
        }
        public Edit _edit = ViewModels.Edit.Stop;

        //-----------------------------------------------------------
        /*
        public decimal ShowBigVolume
        {
            get => _showBigVolume;
            set
            {
                if (_showBigVolume != value)
                {
                    _showBigVolume = value;
                }
            }
        }
        private decimal _showBigVolume;
        */
        //-----------------------------------------------------------
        #endregion --------------------------------------------------------------------------------
        #region Methods ---------------------------------------------------------------------------

        private void _tab_MarketDepthUpdateEvent(MarketDepth marketDepth)
        {
            if (Edit == Edit.Stop)
            {
                return;
            }

            if (marketDepth.SecurityNameCode != _tab.Securiti.Name)
            {
                return;
            }

            List<Position> positions = _tab.PositionsOpenAll;

            if (positions != null
               && positions.Count > 0)
            {
                foreach (Position pos in positions)
                {
                    if (pos.Direction == Side.Sell)
                    {
                        //decimal takePrice = Position.EntryPrice - Take * _tab.Securiti.PriceStep;
                        decimal takePrice = pos.EntryPrice - Take * _tab.Securiti.PriceStep;
                        //_tab.CloseAtProfit(Position, takePrice, takePrice);
                        _tab.CloseAtProfit(pos, takePrice, takePrice);
                    }
                    else if (pos.Direction == Side.Buy)
                    {
                        decimal takePrice = Position.EntryPrice + Take * _tab.Securiti.PriceStep;
                        //decimal takePrice = pos.EntryPrice + Take * _tab.Securiti.PriceStep;
                        _tab.CloseAtProfit(Position, takePrice, takePrice);
                        //_tab.CloseAtProfit(pos, takePrice, takePrice);
                    }
                }
            }
            /*
            for (int i = 0; i < marketDepth.Asks.Count; i++)
            {
                if (marketDepth.Asks[i].Ask >= BigVolume
                    && Position == null
                    )
                {
                    decimal price = marketDepth.Asks[i].Price - Offset * _tab.Securiti.PriceStep;
                    Position = _tab.SellAtLimit(Lot, price);
                    if (Position.State != PositionStateType.Open
                        && Position.State != PositionStateType.Opening)
                    {
                        Position = null;
                    }
                }

                if (Position != null)
                {
                    // цена максимального объёма в продажу, от которого открыли позицию
                    decimal PriceBigVolumeBuy = Position.EntryPrice - Offset * _tab.Securiti.PriceStep;
                    //ShowPriceBigVolume = PriceBigVolumeBuy;

                    if (marketDepth.Asks[i].Price > PriceBigVolumeBuy)
                    {
                        if (marketDepth.Asks[i].Ask >= BigVolume)
                        {
                            if (Position.State == PositionStateType.Opening)
                            {
                                _tab.CloseAllOrderInSystem();
                                Position = null;
                                break;
                            }
                        }
                    }

                    if (marketDepth.Asks[i].Price == PriceBigVolumeBuy)
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
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (marketDepth.Asks[i].Price < PriceBigVolumeBuy)
                    {
                        {
                            if (Position.State == PositionStateType.Open)
                            {
                                _tab.CloseAtMarket(Position, Position.OpenVolume);
                            }
                            else if (Position.State == PositionStateType.Opening)
                            {
                                _tab.CloseAllOrderInSystem();
                            }
                        }
                    }
                }
            }
            */
            for (int i = 0; i < marketDepth.Bids.Count; i++)
            {
                if (marketDepth.Bids[i].Bid >= BigVolume
                    && Position == null)
                     //|| Position.State == PositionStateType.Done
                     //|| Position.State == PositionStateType.Deleted
                     //|| Position.State == PositionStateType.OpeningFail
                {
                    ShowBigVolume = marketDepth.Bids[i].Bid;
                    //EventTradeDelegate?.Invoke();

                    decimal price = marketDepth.Bids[i].Price + Offset * _tab.Securiti.PriceStep;
                    //_stateBid = false;
                    Position = _tab.BuyAtLimit(Lot, price); // обновим переменную в момент выставления заявки,
                                                            // а не после того, как заявка выставится
                    if (Position.State != PositionStateType.Open
                        && Position.State != PositionStateType.Opening)
                    {
                        Position = null;
                    }
                }

                if (Position != null)
                {
                    // цена максимального объёма в покупку, от которого открыли позицию
                    decimal PriceBigVolumeBuy = Position.EntryPrice - Offset * _tab.Securiti.PriceStep;
                    
                    ShowPriceBigVolume = PriceBigVolumeBuy;
                    ShowPriceTakeProfit = Position.ProfitOrderPrice;
                    ShowPriceLimit = Position.EntryPrice;

                    //EventTradeDelegate?.Invoke();

                    if (marketDepth.Bids[i].Price > PriceBigVolumeBuy)
                    {
                        if (marketDepth.Bids[i].Bid >= BigVolume)
                        {
                            if (Position.State == PositionStateType.Opening)
                            {
                                _tab.CloseAllOrderInSystem();
                                Position = null;
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
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (marketDepth.Bids[i].Price < PriceBigVolumeBuy)
                    {
                        {
                            if (Position.State == PositionStateType.Open)
                            {
                                _tab.CloseAtMarket(Position, Position.OpenVolume);
                            }
                            else if (Position.State == PositionStateType.Opening)
                            {
                                _tab.CloseAllOrderInSystem();
                            }
                        }
                    }
                    
                    if (Position.State == PositionStateType.OpeningFail)
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