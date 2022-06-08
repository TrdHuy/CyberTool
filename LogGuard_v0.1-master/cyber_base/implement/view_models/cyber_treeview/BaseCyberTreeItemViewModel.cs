using cyber_base.implement.command;
using cyber_base.implement.models.cyber_treeview;
using cyber_base.implement.utils;
using cyber_base.implement.views.cyber_treeview;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace cyber_base.implement.view_models.cyber_treeview
{
    public class BaseCyberTreeItemViewModel : INotifyPropertyChanged, ICyberTreeViewItem
    {
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
                var current = this as ICyberTreeViewItem;
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
                    current = current.Parent as ICyberTreeViewItem;
                }

                return absTitle;
            }
            set => throw new System.NotImplementedException();
        }
        public CyberTreeViewObservableCollection<ICyberTreeViewItem> Items { get; set; }
        public BaseCommandImpl? AddBtnCommand { get => _vo.AddCmd; }
        public BaseCommandImpl? RemoveBtnCommand { get => _vo.RmCmd; }
        public bool IsFirst
        {
            get { return _vo.IsFirst; }
            set
            {
                _vo.IsFirst = value;
                OnChanged();
            }
        }
        public bool IsLast
        {
            get { return _vo.IsLast; }
            set
            {
                _vo.IsLast = value;
                OnChanged();
            }
        }
        public int ItemsCount => Items.Count;
        public object? Parent => _vo.Parent;
        public bool IsOrphaned => Parent == null;
        public ICyberTreeViewItem? Last { get => _vo.Last; set => _vo.Last = value; }
        public ICyberTreeViewItem? First { get => _vo.First; set => _vo.First = value; }
        public bool IsFolder => Items.Count > 0;
        public bool IsSelected
        {
            get
            {
                return _vo.IsSelected;
            }
            set
            {
                _vo.IsSelected = value;
                OnChanged();
            }
        }

        public bool IsSelectable { get; set; }

        public BaseCyberTreeItemViewModel(BaseCyberTreeItemVO vo)
        {
            _vo = vo;
            Items = new CyberTreeViewObservableCollection<ICyberTreeViewItem>();
        }

        public BaseCyberTreeItemViewModel AddItem(ICyberTreeViewItem item)
        {
            Items.Add(item);
            var cast = item as BaseCyberTreeItemViewModel;
            if (cast != null)
            {
                cast._vo.Parent = this;
            }
            return this;
        }

        public BaseCyberTreeItemViewModel RemoveItem(ICyberTreeViewItem item)
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
