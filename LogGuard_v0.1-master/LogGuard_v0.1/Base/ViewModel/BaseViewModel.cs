using LogGuard_v0._1.Base.Observable;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.ViewModel
{
    public class BaseViewModel : BaseObservable<object>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public BaseViewModel ParentsModel { get; private set; }
        public BaseViewModel()
        {
        }

        public BaseViewModel(BaseViewModel parent)
        {
            ParentsModel = parent;
        }

        public void OnChanged(object viewModel, string propertyName)
        {
            VerifyPropertyName(propertyName);
            PropertyChanged?.Invoke(viewModel, new PropertyChangedEventArgs(propertyName));
        }

        [Conditional("DEBUG")]
        private void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                throw new ArgumentNullException(GetType().Name + " does not contain property: " + propertyName);
        }

        public void Invalidate(string propName)
        {
            OnChanged(this, propName);
        }

        public void InvalidateOwn([CallerMemberName()] string propName = null)
        {
            OnChanged(this, propName);
        }

        public void RefreshViewModel()
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this);

            foreach (PropertyDescriptor property in properties)
            {
                var attr = property.Attributes;

                if (attr[typeof(BindableAttribute)].Equals(BindableAttribute.Yes))
                {
                    Invalidate(property.Name);
                }
            }
        }

    }
}
