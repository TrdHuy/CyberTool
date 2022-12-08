using log_guard.@base.flow.source_filter;
using log_guard.models.vo;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace log_guard.implement.flow.filter_engines
{
    internal class SyntaxFilterEngine : NormalFilterEngine, ISeparableSourceFilterEngine
    {
        private string[] separatingStrings = { " | ", " || " };
        private ObservableCollection<string> _sourceParts;

        public event SourcePartsCollectionChangedHandler PartsCollectionChanged;

        public ObservableCollection<string> SourceParts
        {
            get
            {
                return _sourceParts;
            }
            private set
            {
                _sourceParts = value;
                _sourceParts.CollectionChanged -= OnPartsCollectionChanged;
                _sourceParts.CollectionChanged += OnPartsCollectionChanged;
            }
        }

        public SyntaxFilterEngine() : base()
        {
            SourceParts = new ObservableCollection<string>();
        }

        private void OnPartsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateHelperContent();
            PartsCollectionChanged?.Invoke(this, e);
        }

        public override bool Contain(string input)
        {
            MatchedWords.Clear();

            if (_sourceParts.Count == 0)
            {
                return true;
            }
            else
            {
                foreach (var str in _sourceParts)
                {
                    var idx = input.IndexOf(str);
                    if (idx != -1)
                    {
                        MatchedWords.Add(new MatchedWordVO(idx, str, input));
                        return true;
                    }
                }
                return false;
            }
        }

        public override bool ContainIgnoreCase(string input)
        {
            MatchedWords.Clear();

            if (_sourceParts.Count == 0)
            {
                return true;
            }
            else
            {
                foreach (var str in _sourceParts)
                {
                    var idx = input.IndexOf(str, StringComparison.InvariantCultureIgnoreCase);
                    if (idx != -1)
                    {
                        MatchedWords.Add(new MatchedWordVO(idx, str, input));
                        return true;
                    }
                }
                return false;
            }
        }

        public override bool IsVaild()
        {
            return true;
        }

        protected override void UpdatingSource(string source)
        {
            ComparableSource = source;
            var partSourceArr =
                ComparableSource.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
            SourceParts = new ObservableCollection<string>(partSourceArr);
            UpdateHelperContent();
        }

        private void UpdateHelperContent()
        {
            if (SourceParts.Count > 0)
            {
                var condition = "";
                for (int i = 0; i < SourceParts.Count; i++)
                {
                    if (i == SourceParts.Count - 1)
                    {
                        condition += "\"" + SourceParts[i] + "\"";
                    }
                    else
                    {
                        condition += "\"" + SourceParts[i] + "\"" + " or ";
                    }
                }
                HelperContent = "{ " + condition + " }";
            }
            else
            {
                HelperContent = "";
            }
        }
    }
}
