using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OsEngine.Entity;
using OsEngine.Market;
using OsEngine.Market.Servers;
using OsEngine.MyEntity;
using OsEngine.Robots.FrontRunner.Commands;
using OsEngine.Views;

namespace OsEngine.ViewModels
{
    public class MyRobotVM : BaseVM
    {
        #region Constructor -----------------------------------------------------------------------

        public MyRobotVM()
        {
            ServerMaster.ServerCreateEvent += ServerMaster_ServerCreateEvent;
        }

        #endregion --------------------------------------------------------------------------------
        #region Properties ------------------------------------------------------------------------

        public ObservableCollection<string> ListSecurities { get; set; } = new ObservableCollection<string>();
        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                OnPropertyChanged(nameof(Header));
            }
        }
        private string _header;

        public string SelectedSecurity
        {
            get => _selectedSecurity;
            set
            {
                _selectedSecurity = value;
                OnPropertyChanged(nameof(SelectedSecurity));
                _security = GetSecurityForName(_selectedSecurity);
                StartSecurity(_security);
            }
        }
        private string _selectedSecurity = "";

        public decimal StartPoint
        {
            get => _startPoint;
            set
            {
                _startPoint = value;
                OnPropertyChanged(nameof(StartPoint));
            }
        }
        private decimal _startPoint;

        public int CountLevels
        {
            get => _countLevels;
            set
            {
                _countLevels = value;
                OnPropertyChanged(nameof(CountLevels));
            }
        }
        private int _countLevels;

        public Direction Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                OnPropertyChanged(nameof(CountLevels));
            }
        }
        private Direction _direction;

        public List<Direction> Directions { get; set; } = new List<Direction>()
        {
            Direction.BUY, Direction.SELL, Direction.BUYSELL
        };

        public decimal Lot
        {
            get => _lot;
            set
            {
                _lot = value;
                OnPropertyChanged(nameof(Lot));
            }
        }
        private decimal _lot;

        public StepType StepType
        {
            get => _stepType;
            set
            {
                _stepType = value;
                OnPropertyChanged(nameof(StepType));
            }
        }
        private StepType _stepType;

        public List<StepType> StepTypes { get; set; } = new List<StepType>()
        {
            StepType.PERCENT, StepType.PUNKT
        };


        #endregion --------------------------------------------------------------------------------
        #region Fields ----------------------------------------------------------------------------

        IServer _server;
        List<Security> _securities = new List<Security>();
        Security _security;

        #endregion --------------------------------------------------------------------------------
        #region Commands --------------------------------------------------------------------------

        private DelegateCommand _commandSelectSecurity;
        public DelegateCommand CommandSelectSecurity
        {
            get
            {
                if (_commandSelectSecurity == null)
                {
                    _commandSelectSecurity = new DelegateCommand(SelectSecurity);
                }
                return _commandSelectSecurity;
            }
        }

        #endregion --------------------------------------------------------------------------------
        #region Methods ---------------------------------------------------------------------------

        void SelectSecurity(object o)
        {
            if (RobotWindowVM.ChangeEmitentWindow != null)
            {
                return;
            }

            RobotWindowVM.ChangeEmitentWindow = new ChangeEmitentWindow();
            RobotWindowVM.ChangeEmitentWindow.ShowDialog();
            RobotWindowVM.ChangeEmitentWindow = null;
        }

        private Security GetSecurityForName(string name)
        {
            for(int i = 0; i < _securities.Count; i++)
            {
                if (_securities[i].Name == name)
                {
                    return _securities[i];
                }
            }
            
            return null;
        }
        
        private void StartSecurity(Security security)
        {
            if (security == null)
            {
                Debug.WriteLine("StartSecurity security = null");
                return;
            }

            Task.Run(() => // создаёт новый поток
            {
                while (true)
                {
                    // заказыват бумагу с сервера
                    var series = _server.StartThisSecurity(security.Name, new TimeFrameBuilder(), security.NameClass);

                    if (series != null)
                    {
                        break;
                    }

                    Thread.Sleep(1000);
                }
            });
        }

        private void ServerMaster_ServerCreateEvent(IServer newServer)
        {
            if (newServer == _server)
            {
                return;
            }

            _server = newServer;

            _server.PortfoliosChangeEvent += NewServer_PortfoliosChangeEvent; // подписывается на новые счета
            _server.SecuritiesChangeEvent += NewServer_SecuritiesChangeEvent; // подписывается на новые бумаги
            _server.NeadToReconnectEvent += NewServer_NeadToReconnectEvent;
            _server.NewMarketDepthEvent += NewServer_NewMarketDepthEvent;
            _server.NewTradeEvent += NewServer_NewTradeEvent;
            _server.NewOrderIncomeEvent += NewServer_NewOrderIncomeEvent;
            _server.NewMyTradeEvent += NewServer_NewMyTradeEvent;
            _server.ConnectStatusChangeEvent += NewServer_ConnectStatusChangeEvent;
        }

        private void NewServer_ConnectStatusChangeEvent(string obj)
        {
        }

        private void NewServer_SecuritiesChangeEvent(List<Security> securities)
        {
            ObservableCollection<string> listSecurities = new ObservableCollection<string>();

            for(int i = 0; i < securities.Count; i++)
            {
                listSecurities.Add(securities[i].Name);
            }

            ListSecurities = listSecurities;
            OnPropertyChanged(nameof(ListSecurities));
            _securities = securities;
        }

        private void NewServer_PortfoliosChangeEvent(List<Portfolio> portfolios)
        {
        }

        private void NewServer_NewMyTradeEvent(MyTrade myTrade)
        {
        }

        private void NewServer_NewOrderIncomeEvent(Order order)
        {
        }

        private void NewServer_NewTradeEvent(List<Trade> trades)
        {
        }

        private void NewServer_NewMarketDepthEvent(MarketDepth marketDepth)
        {
        }

        private void NewServer_NeadToReconnectEvent()
        {
        }

        #endregion --------------------------------------------------------------------------------
    }
}
