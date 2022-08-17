using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OsEngine.Entity;
using OsEngine.Market;
using OsEngine.Market.Servers;
using OsEngine.OsTrader.Panels;

namespace OsEngine.Robots.HFT
{
    public class HFTBot : BotPanel
    {
        #region Constructor -----------------------------------------------------------------------

        public HFTBot(string name, StartProgram startProgram) : base(name, startProgram)
        {
            ServerMaster.ServerCreateEvent += ServerMaster_ServerCreateEvent; // создаём сервер
        }

        #endregion --------------------------------------------------------------------------------
        #region Fields ----------------------------------------------------------------------------

        private List<IServer> _servers = new List<IServer>(); // список серверов
        private List<Portfolio> _portfolios = new List<Portfolio>(); // все номера счетов
        //private string _nameSecurity = "BTCUSDT"; // задаём имя бумаги
        private string _nameSecurity = "SBER+QJSIM"; // задаём имя бумаги
        //private ServerType _serverType = ServerType.Binance; // рабочий сервер
        private ServerType _serverType = ServerType.QuikLua; // рабочий сервер
        private Security _security = null; // сохраняем в переменную бумагу с которой будем работать
        private IServer _server;
        private CandleSeries _candleSeries = null;

        #endregion --------------------------------------------------------------------------------
        #region Method ----------------------------------------------------------------------------

        // сервер ---------------------------------------------------------------------------------

        private void ServerMaster_ServerCreateEvent(IServer newServer)
        {
            foreach(IServer server in _servers)
            {
                if(server == newServer)
                {
                    return;
                }
            }
            
            if(newServer.ServerType == _serverType)
            {
                _server = newServer;
            }
            
            _servers.Add(newServer);
            
            newServer.PortfoliosChangeEvent += NewServer_PortfoliosChangeEvent;// подписывается на новые счета
            newServer.SecuritiesChangeEvent += NewServer_SecuritiesChangeEvent;// подписывается на новые бумаги
            newServer.NeadToReconnectEvent += NewServer_NeadToReconnectEvent;
            newServer.NewMarketDepthEvent += NewServer_NewMarketDepthEvent;
            newServer.NewTradeEvent += NewServer_NewTradeEvent;
            newServer.NewOrderIncomeEvent += NewServer_NewOrderIncomeEvent;
            newServer.NewMyTradeEvent += NewServer_NewMyTradeEvent;
            newServer.ConnectStatusChangeEvent += NewServer_ConnectStatusChangeEvent;
        }

        private void NewServer_ConnectStatusChangeEvent(string obj)
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
            StartSecurity(_security);
        }

        private void NewServer_SecuritiesChangeEvent(List<Security> newSecurities)
        {
            for(int i = 0; i < newSecurities.Count; i++)
            {
                if(_security != null)
                {
                    return;
                }
                
                if(_nameSecurity == newSecurities[i].Name)
                {
                    _security = newSecurities[i];
                    StartSecurity(_security);
                    break;
                }
            }
        }

        private void StartSecurity(Security security)
        {
            if(security == null)
            {
                Debug.WriteLine("StartSecurity security = null");
                return;
            }

            Task.Run(() => // создаёт новый поток
            {
                while (true)
                {
                    // заказыват бумагу с сервера
                    _candleSeries = _server.StartThisSecurity(security.Name, new TimeFrameBuilder(), security.NameClass);

                    if(_candleSeries != null)
                    {
                        break;
                    }

                    Thread.Sleep(1000);
                }
            });
        }

        private void NewServer_PortfoliosChangeEvent(List<Portfolio> newPortfolios)
        {
            for (int x = 0; x < newPortfolios.Count; x++) // бегает по новым счетам
            {
                bool flag = true;

                for (int i = 0; i < _portfolios.Count; i++) // бегает по уже существующим счетам
                {
                    if (newPortfolios[x].Number == _portfolios[i].Number) // есть ли такой счет в нашем списке
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    _portfolios.Add(newPortfolios[x]); // добавляем новый счёт в наш список
                }
            }

        }

        // ----------------------------------------------------------------------------------------

        #endregion --------------------------------------------------------------------------------
        #region Abstract Class --------------------------------------------------------------------

        public override string GetNameStrategyType()
        {
            return nameof(HFTBot);
        }

        public override void ShowIndividualSettingsDialog()
        {
        }

        #endregion --------------------------------------------------------------------------------
    }
}
