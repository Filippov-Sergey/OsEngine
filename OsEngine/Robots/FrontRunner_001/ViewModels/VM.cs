﻿using System;
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
            //get => _bigVolume;
            get => _bot.BigVolume; // привязываем модель к вьюмодели, обращаемя к полям внутри бота
            set
            {
                //_bigVolume = value;
                _bot.BigVolume = value;
                OnPropertyChanged(nameof(BigVolume));
            }
        }
        //private decimal _bigVolume = 20000;
        
        public int Offset
        {
            //get => _offset;
            get => _bot.Offset;
            set
            {
                //_offset = value;
                _bot.Offset = value;
                OnPropertyChanged(nameof(Offset));
            }
        }
        //decimal _offset;
        
        public int Take
        {
            //get => _take;
            get => _bot.Take;
            set
            {
                //_take = value;
                _bot.Take = value;
                OnPropertyChanged(nameof(Take));
            }
        }
        //decimal _take;
        
        public decimal Lot
        {
            //get => _lot;
            get => _bot.Lot;
            set
            {
                //_lot = value;
                _bot.Lot = value;
                OnPropertyChanged(nameof(Lot));
            }
        }
        //decimal _lot;

        public decimal ShowBigVolume
        {
            get => _showBigVolume;
            set
            {
                if (_showBigVolume != value) return;
                _showBigVolume = value;
                OnPropertyChanged(nameof(ShowBigVolume));
            }
        }
        private decimal _showBigVolume = 0;
        
        public decimal ShowPriceLimit
        {
            get => _showPriceLimit;
            set
            {
                _showPriceLimit = value;
                OnPropertyChanged(nameof(ShowPriceLimit));
            }
        }
        private decimal _showPriceLimit = 0;
        
        public decimal ShowPriceBigVolume
        {
            get => _showPriceBigVolume;
            set
            {
                _showPriceBigVolume = value;
                OnPropertyChanged(nameof(ShowPriceBigVolume));
            }
        }
       private decimal _showPriceBigVolume = 0;

        public decimal ShowPriceTakeProfit
        {
            get => _showPriceTakeProfit;
            set
            {
                _showPriceTakeProfit = value;
                OnPropertyChanged(nameof(ShowPriceTakeProfit));
            }
        }
       private decimal _showPriceTakeProfit = 0;

        public Edit Edit
        {
            //get => _edit;
            get => _bot.Edit;
            set
            {
                //_edit = value;
                _bot.Edit = value;
                OnPropertyChanged(nameof(Edit));
            }
        }
        //Edit _edit;

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
            ShowBigVolume = _bot.ShowBigVolume;
            ShowPriceBigVolume = _bot.ShowPriceBigVolume;
            ShowPriceTakeProfit = _bot.ShowPriceTakeProfit;
            ShowPriceLimit = _bot.ShowPriceLimit;
        }

        #endregion --------------------------------------------------------------------------------
    }
    public enum Edit
    {
        Start,
        Stop
    }
}