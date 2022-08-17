using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Entity;

namespace OsEngine.MyEntity
{
    public class Emitent : BaseVM
    {
        #region Constructors ----------------------------------------------------------------------
        
        public Emitent(Security security)
        {
            _security = security;
        }

        #endregion --------------------------------------------------------------------------------
        #region Properties ------------------------------------------------------------------------

        public string NameSec
        {
            get => _security.Name;
        }
        #endregion --------------------------------------------------------------------------------

        private Security _security;
    }
}
