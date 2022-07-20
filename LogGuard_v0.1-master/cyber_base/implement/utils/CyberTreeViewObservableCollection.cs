using cyber_base.implement.views.cyber_treeview;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.implement.utils
{
    public class CyberTreeViewObservableCollection<T> : ObservableCollection<T>, ICyberTreeViewObservableCollection<T>
        where T : ICyberTreeViewItemContext
    {
        private Dictionary<string, T> _parts = new Dictionary<string, T>();

        public T? this[string key]
        {
            get
            {
                try
                {
                    return _parts[key];
                }
                catch
                {
                    return default(T);
                }
            }
            set
            {
                if (value != null)
                    _parts[key] = value;
            }
        }

        public CyberTreeViewObservableCollection()
        {
        }

        public new void Add(T item)
        {
            if (this[item.Title] == null)
            {
                _parts.Add(item.Title, item);
                base.Add(item);
            }
        }

        public new void Remove(T item)
        {
            if (this[item.Title] != null)
            {
                _parts.Remove(item.Title);
                base.Remove(item);
            }
        }
    }

    public interface ICyberTreeViewObservableCollection<T>
        where T: ICyberTreeViewItemContext
    {
        T? this[string key] { get; set; }
    }

}
