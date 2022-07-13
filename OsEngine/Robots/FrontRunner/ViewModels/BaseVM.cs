using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsEngine.Robots.FrontRunner.ViewModels
{
    public class BaseVM : INotifyPropertyChanged
    {
        #region---------------------------------- Method ------------------------------------------
        public void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion---------------------------------------------------------------------------------

        #region---------------------------------- Events ------------------------------------------

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion---------------------------------------------------------------------------------
    }
}
