using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OsEngine.Robots.FrontRunner.Commands;
using OsEngine.Robots.FrontRunner.Models;

namespace OsEngine.Robots.FrontRunner.ViewModels
{
    public class VM : BaseVM
    {
        public VM(FrontRunnerBot bot)
        {
            _bot = bot;
            _bot.EventTradeDelegate += _bot_EventTradeDelegate;
        }

        #region Fields ----------------------------------------------------------------------------
        
        private FrontRunnerBot _bot;
        
        #endregion --------------------------------------------------------------------------------
        #region Properties ------------------------------------------------------------------------
        
        public decimal BigVolume
        {
            get => _bot.BigVolume; // привязываем модель к вьюмодели, обращаемя к полям внутри бота
            set
            {
                _bot.BigVolume = value;
                OnPropertyChanged(nameof(BigVolume));
            }
        }
        
        public int Offset
        {
            get => _bot.Offset;
            set
            {
                _bot.Offset = value;
                OnPropertyChanged(nameof(Offset));
            }
        }
        
        public int Take
        {
            get => _bot.Take;
            set
            {
                _bot.Take = value;
                OnPropertyChanged(nameof(Take));
            }
        }
        
        public decimal Lot
        {
            get => _bot.Lot;
            set
            {
                _bot.Lot = value;
                OnPropertyChanged(nameof(Lot));
            }
        }

        public string ShowDirection
        {
            get => _showDirection;
            set
            {
                _showDirection = value;
                OnPropertyChanged(nameof(ShowDirection));
            }
        }
        private string _showDirection = "Робот остановлен";
        
        public decimal ShowOpenVolume
        {
            get => _showOpenVolume;
            set
            {
                _showOpenVolume = value;
                OnPropertyChanged(nameof(ShowOpenVolume));
            }
        }
        private decimal _showOpenVolume;

        public decimal ShowOpenPrice
        {
            get => _showOpenPrice;
            set
            {
                _showOpenPrice = value;
                OnPropertyChanged(nameof(ShowOpenPrice));
            }
        }
        private decimal _showOpenPrice;

        public decimal ShowTakePrice
        {
            get => _showTakePrice;
            set
            {
                _showTakePrice = value;
                OnPropertyChanged(nameof(ShowTakePrice));
            }
        }
        private decimal _showTakePrice;

        public decimal ShowVariationMargin
        {
            get => _showVariationMargin;
            set
            {
                _showVariationMargin = value;
                OnPropertyChanged(nameof(ShowVariationMargin));
            }
        }
        private decimal _showVariationMargin;

        public decimal ShowAccumulatedProfit
        {
            get => _showAccumulatedProfit;
            set
            {
                _showAccumulatedProfit = value;
                OnPropertyChanged(nameof(ShowAccumulatedProfit));
            }
        }
        private decimal _showAccumulatedProfit;

        public Edit Edit
        {
            get => _bot.Edit;
            set
            {
                _bot.Edit = value;
                OnPropertyChanged(nameof(Edit));
            }
        }

        #endregion --------------------------------------------------------------------------------
        #region Commands --------------------------------------------------------------------------

        private DelegateCommand commandStart;
        public ICommand CommandStart
        {
            get
            {
                if(commandStart == null)
                {
                    commandStart = new DelegateCommand(Start);
                }
                return commandStart;
            }
            set
            {

            }
        }

        #endregion --------------------------------------------------------------------------------
        #region Methods ---------------------------------------------------------------------------
        private void Start(object obj)
        {
            if(Edit == Edit.Start)
            {
                Edit = Edit.Stop;
            }
            else
            {
                Edit = Edit.Start;
            }
        }

        private void _bot_EventTradeDelegate()
        {
            ShowDirection  = _bot.ShowDirection;
            ShowOpenVolume = _bot.ShowOpenVolume;
            ShowOpenPrice  = _bot.ShowOpenPrice;
            ShowTakePrice  = _bot.ShowTakePrice;
            ShowVariationMargin = _bot.ShowVariationMargin;
            ShowAccumulatedProfit = _bot.ShowAccumulatedProfit;
        }

        #endregion --------------------------------------------------------------------------------
    }
    public enum Edit
    {
        Start,
        Stop
    }
}
