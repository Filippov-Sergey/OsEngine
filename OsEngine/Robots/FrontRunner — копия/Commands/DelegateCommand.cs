using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OsEngine.Robots.FrontRunner.Commands
{
    public class DelegateCommand : ICommand // при нажатии на кнопку попадём в метод Execute
    {
        public DelegateCommand(DelegateFunction function) // конструктор для приёма функции
        {
            _function = function;
        }
        public delegate void DelegateFunction(object obj); // для приёма функции создаём делегат

        public event EventHandler CanExecuteChanged;

        #region Fields ----------------------------------------------------------------------------

        private DelegateFunction _function;

        #endregion --------------------------------------------------------------------------------
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _function?.Invoke(parameter);
        }
    }
}
