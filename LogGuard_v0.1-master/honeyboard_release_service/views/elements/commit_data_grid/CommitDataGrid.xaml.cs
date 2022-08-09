using cyber_base.implement.utils;
using honeyboard_release_service.views.elements.commit_data_grid.@base;
using honeyboard_release_service.views.elements.commit_data_grid.converter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace honeyboard_release_service.views.elements.commit_data_grid
{
    /// <summary>
    /// Interaction logic for CommitDataGrid.xaml
    /// </summary>
    public partial class CommitDataGrid : UserControl
    {
        private readonly Regex PLM_CASE_CODE_REGEX = new Regex(@"\[P([0-9]{6})-([0-9]{5})\]");

        private const int COMMIT_ID_SEARCH_MODE = 0b00001;
        private const int TASK_ID_SEARCH_MODE = 0b00010;
        private const int TITLE_SEARCH_MODE = 0b00100;
        private const int DATE_TIME_SEARCH_MODE = 0b01000;
        private const int AUTHOR_SEARCH_MODE = 0b10000;
        private const int ALL_SEARCH_MODE = 0b11111;

        private class MatchedWord : IMatchedWord
        {
            public string SearchWord { get; }
            public int StartIndex { get; }
            public string RawWord { get; }
            public bool IsMatch { get; }
            public MatchedWord(int startIndex, string searchWord, string rawWord)
            {
                StartIndex = startIndex;
                SearchWord = searchWord;
                RawWord = rawWord;
                IsMatch = startIndex >= 0;
            }
        }
        private class SourceManager : IDisposable
        {
            private ObservableCollection<ICommitDataGridItemContext>? _rawSource;
            private CancellationTokenSource? _filterCancelTokenSource;
            private SemaphoreSlim _filterSemaphore = new SemaphoreSlim(1, 1);

            public RangeObservableCollection<ICommitDataGridItemContext>? CurrentDisplaySource;

            public SourceManager()
            {
            }

            public void SwitchSource(DataGrid sourceDisplayer, bool isUseRaw)
            {
                if (isUseRaw && sourceDisplayer.ItemsSource != _rawSource)
                {
                    sourceDisplayer.ItemsSource = _rawSource;
                }
                else if (!isUseRaw && sourceDisplayer.ItemsSource != CurrentDisplaySource)
                {
                    sourceDisplayer.ItemsSource = CurrentDisplaySource;
                }
            }
            public void SetRawSource(DataGrid sourceDisplayer, ObservableCollection<ICommitDataGridItemContext>? newSource)
            {
                if (_rawSource != null)
                {
                    _rawSource.CollectionChanged -= OnRawSourceCollectionChanged;
                }

                _rawSource = newSource;

                if (_rawSource != null)
                {
                    _rawSource.CollectionChanged -= OnRawSourceCollectionChanged;
                    _rawSource.CollectionChanged += OnRawSourceCollectionChanged;
                }

                if (newSource != null)
                {

                    CurrentDisplaySource = new RangeObservableCollection<ICommitDataGridItemContext>(newSource);
                }
                else
                {
                    CurrentDisplaySource = null;
                }

                sourceDisplayer.ItemsSource = CurrentDisplaySource;
            }


            public async void FindItemInRawSource(Func<ICommitDataGridItemContext, bool> condition
                , Action<ICommitDataGridItemContext>? preHandleDisplaySourceItem = null)
            {
                _filterCancelTokenSource?.Cancel();
                _filterCancelTokenSource?.Dispose();
                _filterCancelTokenSource = new CancellationTokenSource();
                await _filterSemaphore.WaitAsync();

                try
                {
                    if (CurrentDisplaySource != null)
                    {
                        if (preHandleDisplaySourceItem != null)
                        {
                            PreHandleDisplaySourceItem(preHandleDisplaySourceItem, _filterCancelTokenSource);
                        }
                        await CurrentDisplaySource.AddNewRangeAsync(StartFilterItem(_filterCancelTokenSource
                            , condition)
                        , isNotifyWhenAllFinished: true);
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _filterSemaphore.Release();
                }

            }

            public void Dispose()
            {
                if (_rawSource != null)
                {
                    _rawSource.CollectionChanged -= OnRawSourceCollectionChanged;
                }
            }

            private void OnRawSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    if (e.NewItems != null)
                    {
                        CurrentDisplaySource?.AddRange(e.NewItems);
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    if (e.OldItems != null)
                    {
                        CurrentDisplaySource?.RemoveRange(e.OldItems);
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    CurrentDisplaySource?.Clear();
                }
            }

            private void PreHandleDisplaySourceItem(Action<ICommitDataGridItemContext> handler
                , CancellationTokenSource token)
            {
                if (CurrentDisplaySource != null)
                {
                    foreach (var item in CurrentDisplaySource)
                    {

                        if (token.IsCancellationRequested)
                        {
                            throw new OperationCanceledException();
                        }
                        handler.Invoke(item);
                    }
                }
            }

            private async IAsyncEnumerable<ICommitDataGridItemContext> StartFilterItem(CancellationTokenSource token
                , Func<ICommitDataGridItemContext, bool> condition
                , bool isUseRaw = true)
            {
                await Task.Delay(1);
                if (isUseRaw)
                {
                    if (_rawSource != null)
                    {
                        foreach (var item in _rawSource)
                        {

                            if (token.IsCancellationRequested)
                            {
                                throw new OperationCanceledException();
                            }
                            if (condition.Invoke(item))
                            {
                                yield return item;
                            }
                        }
                    }
                }
                else
                {
                    if (CurrentDisplaySource != null)
                    {
                        foreach (var item in CurrentDisplaySource)
                        {

                            if (token.IsCancellationRequested)
                            {
                                throw new OperationCanceledException();
                            }
                            if (condition.Invoke(item))
                            {
                                yield return item;
                            }
                        }
                    }
                }

            }

        }

        public class SearchAndFilterUserDataCache
        {
            public const int NONE_STATE = 0;
            public const int HIGHLIGHT_STATE = 1;
            public const int FILTER_STATE = 2;

            public string LastSearchTextCache { get; set; } = "";
            public FilterState LastFilterStateCache { get; set; } = FilterState.All;
            public bool IsUseRawSourceCache { get; set; } = true;
            public int LastSearchState { get; set; } = NONE_STATE;
        }

        private SearchAndFilterUserDataCache _userDataCache = new SearchAndFilterUserDataCache();
        private SourceManager _sourceManager = new SourceManager();
        private bool _isShouldRefilterItemWhenFilterStateChanged = false;

        #region ItemsSource
        public static readonly DependencyProperty ItemsSourceProperty
            = DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<ICommitDataGridItemContext>), typeof(CommitDataGrid),
                                          new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChangedCallback)));

        private static void OnItemsSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as CommitDataGrid;
            ctrl?.OnItemsSourceChanged();
        }

        public ObservableCollection<ICommitDataGridItemContext>? ItemsSource
        {
            get { return GetValue(ItemsSourceProperty) as ObservableCollection<ICommitDataGridItemContext>; }
            set
            {
                if (value == null)
                {
                    ClearValue(ItemsSourceProperty);
                }
                else
                {
                    SetValue(ItemsSourceProperty, value);
                }
            }
        }
        #endregion

        #region SearchText
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(
                "SearchText",
                typeof(string),
                typeof(CommitDataGrid),
                new PropertyMetadata("", new PropertyChangedCallback(OnSearchTextPropertyChangedCallback)));

        private static void OnSearchTextPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as CommitDataGrid;
        }

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }
        #endregion

        public int CurrentSearchMode { get; private set; } = ALL_SEARCH_MODE;
        public FilterState CurrentFilterState { get; private set; } = FilterState.All;

        public CommitDataGrid()
        {
            InitializeComponent();
            InitMenuContextSearchModeItems();
            InitMenuContextColumnVisibilityItems();
            InitMenuContextGridLineItems();
        }

        private void InitMenuContextSearchModeItems()
        {
            PART_SearchModeMenuItem.Items.Clear();
            int curMode = 1;

            foreach (var column in PART_CommitDataGrid.Columns)
            {
                var item = new System.Windows.Controls.MenuItem()
                {
                    Header = column.Header,
                    IsCheckable = true,
                    IsChecked = true,
                    StaysOpenOnClick = true,
                };
                var itemValue = new
                {
                    Value = curMode,
                };

                PART_SearchModeMenuItem.Items.Add(item);
                curMode = curMode << 1;

                // Xử lý check và uncheck sau khi set binding
                item.Unchecked += (s, e) =>
                {
                    CurrentSearchMode -= itemValue.Value;
                };
                item.Checked += (s, e) =>
                {
                    CurrentSearchMode += itemValue.Value;
                };

            }
        }

        private void InitMenuContextGridLineItems()
        {
            Func<DataGridGridLinesVisibility, string, System.Windows.Controls.MenuItem> createNewMenuItem = (gridLineParamater, header) =>
            {
                var item = new System.Windows.Controls.MenuItem()
                {
                    IsCheckable = true,
                    Header = header,
                    StaysOpenOnClick = true,
                };

                Binding b = new Binding();
                b.Path = new PropertyPath("GridLinesVisibility");
                b.Converter = new GridLineToBooleanConverter();
                b.ConverterParameter = gridLineParamater;
                b.Source = PART_CommitDataGrid;

                item.SetBinding(System.Windows.Controls.MenuItem.IsCheckedProperty, b);

                return item;
            };

            PART_TurnOnOffGridLineMenuItem.Items.Clear();

            var i1 = createNewMenuItem.Invoke(DataGridGridLinesVisibility.All, "All");
            var i2 = createNewMenuItem.Invoke(DataGridGridLinesVisibility.None, "None");
            var i3 = createNewMenuItem.Invoke(DataGridGridLinesVisibility.Horizontal, "Horizontal");
            var i4 = createNewMenuItem.Invoke(DataGridGridLinesVisibility.Vertical, "Vertical");
            PART_TurnOnOffGridLineMenuItem.Items.Add(i1);
            PART_TurnOnOffGridLineMenuItem.Items.Add(i2);
            PART_TurnOnOffGridLineMenuItem.Items.Add(i3);
            PART_TurnOnOffGridLineMenuItem.Items.Add(i4);
        }

        private void InitMenuContextColumnVisibilityItems()
        {
            PART_ColumnVisibilityItem.Items.Clear();
            foreach (var column in PART_CommitDataGrid.Columns)
            {
                var item = new System.Windows.Controls.MenuItem()
                {
                    Header = column.Header,
                    IsCheckable = true,
                    StaysOpenOnClick = true,
                };
                Binding b = new Binding();
                b.Path = new PropertyPath("Visibility");
                b.Converter = new VisibilityToBooleanConverter();
                b.Source = column;

                item.SetBinding(System.Windows.Controls.MenuItem.IsCheckedProperty, b);
                PART_ColumnVisibilityItem.Items.Add(item);
            }
        }

        private void HandleFilterRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            var ctrl = sender as RadioButton;
            if (ctrl != null)
            {
                switch (ctrl.Name)
                {
                    case "PART_AllFilterRadioButton":
                        {
                            UpdateCurrentFilterState(FilterState.All);
                            break;
                        }
                    case "PART_PLMFilterRadioButton":
                        {
                            UpdateCurrentFilterState(FilterState.PLM);
                            break;
                        }
                    case "PART_JiraFilterRadioButton":
                        {
                            UpdateCurrentFilterState(FilterState.Jira);
                            break;
                        }
                }
                return;
            }
        }

        private void HandleButtonAndMenuItemClick(object sender, RoutedEventArgs e)
        {
            var ctrl = sender as Button;
            if (ctrl != null)
            {
                switch (ctrl.Name)
                {
                    case "PART_MenuButton":
                        {
                            PART_ControlPanelContextMenu.IsOpen = true;
                            break;
                        }
                    case "PART_SearchButton":
                        {
                            if (CurrentFilterState == FilterState.All)
                            {
                                _sourceManager.SwitchSource(PART_CommitDataGrid, true);
                                _userDataCache.IsUseRawSourceCache = true;
                            }

                            if (SearchText != _userDataCache.LastSearchTextCache
                                || _userDataCache.LastSearchState != SearchAndFilterUserDataCache.HIGHLIGHT_STATE)
                            {
                                _userDataCache.LastSearchTextCache = SearchText;
                                FindAndHighlightItemInSource(isHighlightOnly: true);
                            }
                            _userDataCache.LastSearchState = SearchAndFilterUserDataCache.HIGHLIGHT_STATE;
                            break;
                        }
                    case "PART_FilterButton":
                        {
                            _sourceManager.SwitchSource(PART_CommitDataGrid, false);
                            _userDataCache.IsUseRawSourceCache = false;

                            if (SearchText != _userDataCache.LastSearchTextCache
                                || _isShouldRefilterItemWhenFilterStateChanged
                                || _userDataCache.LastSearchState != SearchAndFilterUserDataCache.FILTER_STATE)
                            {
                                _userDataCache.LastSearchTextCache = SearchText;
                                FindAndHighlightItemInSource(isHighlightOnly: false);
                                _isShouldRefilterItemWhenFilterStateChanged = false;
                            }
                            _userDataCache.LastSearchState = SearchAndFilterUserDataCache.FILTER_STATE;
                            break;
                        }

                }
                return;
            }
        }

        private void FindAndHighlightItemInSource(bool isHighlightOnly)
        {
            _sourceManager.FindItemInRawSource(condition: (item) =>
            {
                bool condition = IsItemMatchSearchText(item);
                var extraFilterContdition = true;
                switch (CurrentFilterState)
                {
                    case FilterState.All:
                        break;
                    case FilterState.PLM:
                        extraFilterContdition = PLM_CASE_CODE_REGEX.Match(item.TaskId).Success;
                        break;
                    case FilterState.Jira:
                        extraFilterContdition = !PLM_CASE_CODE_REGEX.Match(item.TaskId).Success;
                        break;
                }

                return (condition || isHighlightOnly) && extraFilterContdition;
            }
                , preHandleDisplaySourceItem: (item) =>
                {
                    item.CommitIdHighlightSource = null;
                    item.TaskIdHighlightSource = null;
                    item.AuthorHighlightSource = null;
                    item.TitleHighlightSource = null;
                    item.DateTimeHighlightSource = null;
                });
        }

        private bool IsItemMatchSearchText(ICommitDataGridItemContext item
            , bool isNeedToSetHighlight = true)
        {
            var isAuthorMatched = true;
            var isTitleMatched = true;
            var isCommitMatched = true;
            var isTaskIdMatched = true;
            var isDateTimeMatched = true;

            if (!string.IsNullOrEmpty(SearchText))
            {
                if ((CurrentSearchMode & DATE_TIME_SEARCH_MODE) != 0)
                {
                    var dateTimeMatchedSearchIndex = item.DateTime.IndexOf(SearchText);
                    if (dateTimeMatchedSearchIndex >= 0 && isNeedToSetHighlight)
                    {
                        List<IMatchedWord> dateTimeMatchedWords = new List<IMatchedWord>();
                        dateTimeMatchedWords.Add(new MatchedWord(dateTimeMatchedSearchIndex
                            , SearchText
                            , item.DateTime));
                        item.DateTimeHighlightSource = dateTimeMatchedWords;
                    }
                    else if (dateTimeMatchedSearchIndex < 0)
                    {
                        isDateTimeMatched = false;
                    }
                }

                if ((CurrentSearchMode & AUTHOR_SEARCH_MODE) != 0)
                {
                    var authorMatchedSearchIndex = item.Author.IndexOf(SearchText);
                    if (authorMatchedSearchIndex >= 0 && isNeedToSetHighlight)
                    {
                        List<IMatchedWord> authorMatchedWords = new List<IMatchedWord>();
                        authorMatchedWords.Add(new MatchedWord(authorMatchedSearchIndex
                            , SearchText
                            , item.Author));
                        item.AuthorHighlightSource = authorMatchedWords;
                    }
                    else if (authorMatchedSearchIndex < 0)
                    {
                        isAuthorMatched = false;
                    }
                }

                if ((CurrentSearchMode & TITLE_SEARCH_MODE) != 0)
                {
                    var titleMatchedSearchIndex = item.Title.IndexOf(SearchText);
                    if (titleMatchedSearchIndex >= 0 && isNeedToSetHighlight)
                    {
                        List<IMatchedWord> titleMatchedWords = new List<IMatchedWord>();
                        titleMatchedWords.Add(new MatchedWord(titleMatchedSearchIndex
                            , SearchText
                            , item.Title));
                        item.TitleHighlightSource = titleMatchedWords;
                    }
                    else if (titleMatchedSearchIndex < 0)
                    {
                        isTitleMatched = false;
                    }
                }

                if ((CurrentSearchMode & COMMIT_ID_SEARCH_MODE) != 0)
                {
                    var matchedSearchIndex = item.CommitId.IndexOf(SearchText);
                    if (matchedSearchIndex >= 0 && isNeedToSetHighlight)
                    {
                        List<IMatchedWord> matchedWords = new List<IMatchedWord>();
                        matchedWords.Add(new MatchedWord(matchedSearchIndex
                            , SearchText
                            , item.CommitId));
                        item.CommitIdHighlightSource = matchedWords;
                    }
                    else if (matchedSearchIndex < 0)
                    {
                        isCommitMatched = false;
                    }
                }

                if ((CurrentSearchMode & TASK_ID_SEARCH_MODE) != 0)
                {
                    var matchedSearchIndex = item.TaskId.IndexOf(SearchText);
                    if (matchedSearchIndex >= 0 && isNeedToSetHighlight)
                    {
                        List<IMatchedWord> matchedWords = new List<IMatchedWord>();
                        matchedWords.Add(new MatchedWord(matchedSearchIndex
                            , SearchText
                            , item.TaskId));
                        item.TaskIdHighlightSource = matchedWords;
                    }
                    else if (matchedSearchIndex < 0)
                    {
                        isTaskIdMatched = false;
                    }
                }
            }
            var condition = isAuthorMatched
                || isTitleMatched
                || isCommitMatched
                || isTaskIdMatched
                || isDateTimeMatched;
            return condition;
        }

        private void FilterItemInSource()
        {
            var matchSubject = PLM_CASE_CODE_REGEX;

            switch (CurrentFilterState)
            {
                case FilterState.PLM:
                    _sourceManager.FindItemInRawSource(condition: (item) =>
                    {
                        bool condition = IsItemMatchSearchText(item, false);
                        var match = matchSubject.Match(item.TaskId);
                        return match.Success && condition;
                    });
                    break;
                case FilterState.Jira:
                    _sourceManager.FindItemInRawSource(condition: (item) =>
                    {
                        bool condition = IsItemMatchSearchText(item, false);
                        var match = matchSubject.Match(item.TaskId);
                        return !match.Success && condition;
                    });
                    break;
                case FilterState.All:
                    _sourceManager.FindItemInRawSource(condition: (item) =>
                    {
                        bool condition = IsItemMatchSearchText(item, false);
                        return condition;
                    });
                    break;
            }
            _userDataCache.LastSearchState = SearchAndFilterUserDataCache.FILTER_STATE;

        }

        private void UpdateCurrentFilterState(FilterState newState)
        {
            if (CurrentFilterState != newState)
            {
                _isShouldRefilterItemWhenFilterStateChanged = true;
                CurrentFilterState = newState;

                switch (CurrentFilterState)
                {
                    case FilterState.PLM:
                    case FilterState.Jira:
                    case FilterState.All:
                        _sourceManager.SwitchSource(PART_CommitDataGrid, false);
                        _userDataCache.IsUseRawSourceCache = false;
                        break;
                }
                FilterItemInSource();
                _userDataCache.LastFilterStateCache = newState;
            }
        }

        private void OnItemsSourceChanged()
        {
            _sourceManager.SetRawSource(PART_CommitDataGrid, ItemsSource);
            ApplyUserDataCache();
        }

        private void ApplyUserDataCache()
        {
            // TODO: Khi source mới được update cần apply
            // trạng thái filter và search của source cũ
        }
    }

}
