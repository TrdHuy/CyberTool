using cyber_base.implement.command;
using cyber_base.implement.models.cyber_treeview;
using cyber_base.implement.utils;
using cyber_base.implement.views.cyber_treeview;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace cyber_base.implement.view_models.cyber_treeview
{
    public class BaseCyberTreeItemViewModel : INotifyPropertyChanged, ICyberTreeViewItemContext
    {
        private ICyberTreeViewItemContext? _last;
        private ICyberTreeViewItemContext? _first;
        private ICyberTreeViewItemContext? _parents;
        private bool _isFirst;
        private bool _isLast;
        private bool _isSelected;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected BaseCyberTreeItemVO _vo;

        public string Title
        {
            get
            {
                return _vo.Title;
            }
            set
            {
                _vo.Title = value;
                OnChanged();
            }
        }
        public string AbsoluteTitle
        {
            get
            {
                var current = this as ICyberTreeViewItemContext;
                string absTitle = "";
                while (current != null)
                {
                    if (current.Parent == null)
                    {
                        absTitle = absTitle.Insert(0, current.Title);
                    }
                    else
                    {
                        absTitle = absTitle.Insert(0, "/" + current.Title);
                    }
                    current = current.Parent as ICyberTreeViewItemContext;
                }

                return absTitle;
            }
            set => throw new System.NotImplementedException();
        }
        public CyberTreeViewObservableCollection<ICyberTreeViewItemContext> Items { get; set; }
        public bool IsFirst
        {
            get { return _isFirst; }
            set
            {
                _isFirst = value;
                OnChanged();
            }
        }
        public bool IsLast
        {
            get { return _isLast; }
            set
            {
                _isLast = value;
                OnChanged();
            }
        }
        public int ItemsCount => Items.Count;
        public ICyberTreeViewItemContext? Parent => _parents;
        public bool IsOrphaned => Parent == null;
        public ICyberTreeViewItemContext? Last { get => _last; set => _last = value; }
        public ICyberTreeViewItemContext? First { get => _first; set => _first = value; }
        public bool IsFolder => Items.Count > 0;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnChanged();
            }
        }
        public bool IsSelectable { get; set; }

        public BaseCyberTreeItemViewModel(BaseCyberTreeItemVO vo)
        {
            _vo = vo;
            Items = new CyberTreeViewObservableCollection<ICyberTreeViewItemContext>();
        }

        public BaseCyberTreeItemViewModel AddItem(ICyberTreeViewItemContext item)
        {
            Items.Add(item);
            var cast = item as BaseCyberTreeItemViewModel;
            if (cast != null)
            {
                cast._parents = this;
            }
            return this;
        }

        public BaseCyberTreeItemViewModel RemoveItem(ICyberTreeViewItemContext item)
        {
            Items.Remove(item);
            return this;
        }

        private void OnChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
