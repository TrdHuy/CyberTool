using System;

namespace cyber_base.implement.command
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
