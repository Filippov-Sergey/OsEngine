using System;
using System.Collections.Generic;
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

        public decimal ShowBigVolume;
        public decimal ShowPriceBigVolume;

        private BotTabSimple _tab;
        //private bool _stateBid = true;
        //private bool _stateAsk = true;
        //public Edit _edit = ViewModels.Edit.Stop;

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
                        decimal takePrice = Position.EntryPrice - Take * _tab.Securiti.PriceStep;
                        //decimal takePrice = pos.EntryPrice - Take * _tab.Securiti.PriceStep;
                        _tab.CloseAtProfit(Position, takePrice, takePrice);
                        //_tab.CloseAtProfit(pos, takePrice, takePrice);
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

            for (int i = 0; i < marketDepth.Asks.Count; i++)
            {
                if (marketDepth.Asks[i].Ask >= BigVolume
                    && Position == null
                    //&& _stateAsk
                    )
                {
                    decimal price = marketDepth.Asks[i].Price - Offset * _tab.Securiti.PriceStep;
                    //_stateAsk = false;
                    //_tab.SellAtLimit(Lot, price);
                }
                if (Position != null
                    && marketDepth.Asks[i].Price == Position.EntryPrice
                    && marketDepth.Asks[i].Ask > BigVolume / 2)
                {
                    //_tab.CloseAtMarket(Position, Position.OpenVolume);
                }
            }

            for (int i = 0; i < marketDepth.Bids.Count; i++)
            {
                ShowBigVolume = marketDepth.Bids[i].Bid;
                
                if (marketDepth.Bids[i].Bid >= BigVolume
                    && (Position == null || Position.State == PositionStateType.Done
                     || Position.State == PositionStateType.Deleted
                     || Position.State == PositionStateType.OpeningFail)
                    //&& _stateBid
                    )
                {
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
                }
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
    }
}
