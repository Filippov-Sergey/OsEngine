using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Entity;
using OsEngine.Market;
using OsEngine.Market.Servers;
using OsEngine.MyEntity;
using OsEngine.Robots.FrontRunner.Commands;

namespace OsEngine.ViewModels
{
    public class ChangeEmitentVM : BaseVM
    {
        #region Constructor -----------------------------------------------------------------------
        
        public ChangeEmitentVM()
        {
            Init();
        }

        #endregion --------------------------------------------------------------------------------
        #region Properties ------------------------------------------------------------------------

        public ObservableCollection<ExChenge> ExChanges { get; set; } = new ObservableCollection<ExChenge>();
        public ObservableCollection<string> EmitClasses { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<Emitent> Securities { get; set; } = new ObservableCollection<Emitent>();

        #endregion --------------------------------------------------------------------------------
        #region Commands --------------------------------------------------------------------------

        private DelegateCommand commandSetExChange;
        public DelegateCommand CommandSetExChange
        {
            get
            {
                if (commandSetExChange == null)
                {
                    commandSetExChange = new DelegateCommand(SetExChange);
                }
                return commandSetExChange;
            }
        }

        #endregion --------------------------------------------------------------------------------
        #region Methods ---------------------------------------------------------------------------

        void SetExChange(object obj)
        {
            ServerType type = (ServerType)obj;
        }

        void Init()
        {
            List<IServer> servers = ServerMaster.GetServers(); // считываем все сервера
            
            ExChanges.Clear(); // очищаем ObservableCollection

            foreach (IServer server in servers) // бежим по всем запущенным серверам
            {
                ExChanges.Add(new ExChenge(server.ServerType)); // добавляем в ExChanges новый экземпляр класса ExChenge
            }

            OnPropertyChanged(nameof(ExChanges));
        }

        #endregion --------------------------------------------------------------------------------
    }
}
