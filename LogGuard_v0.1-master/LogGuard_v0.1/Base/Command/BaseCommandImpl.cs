using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogGuard_v0._1.Base.Command
{
    public class BaseCommandImpl
    {
        private Action<object, object> _act;

        public BaseCommandImpl(Action<object, object> a)
        {
            _act = a;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter == null)
            {
                _act?.Invoke(null, null);

            }
            var param = parameter as object[];
            if (param != null)
            {
                _act?.Invoke(param[0], param[1]);
            }
            else
            {
                _act?.Invoke(parameter, null);
            }
        }
    }
}
