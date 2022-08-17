using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Market;

namespace OsEngine.MyEntity
{
    public class ExChenge : BaseVM
    {
        #region Constructors ----------------------------------------------------------------------

        public ExChenge(ServerType type) // передаём наш ServerType
        {
            Server = type; // присваеваем свойству Server
        }

        #endregion --------------------------------------------------------------------------------
        #region Properties ------------------------------------------------------------------------

        public ServerType Server
        {
            get => _server;
            set
            {
                _server = value;
                OnPropertyChanged(nameof(Server));
            }
        }
        private ServerType _server;

        #endregion --------------------------------------------------------------------------------
    }
}
