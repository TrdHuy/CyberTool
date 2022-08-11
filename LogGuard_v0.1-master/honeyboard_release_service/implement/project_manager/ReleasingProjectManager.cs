using cyber_base.async_task;
using cyber_base.implement.async_task;
using cyber_base.implement.utils;
using cyber_base.implement.views.cyber_treeview;
using honeyboard_release_service.definitions;
using honeyboard_release_service.implement.module;
using honeyboard_release_service.implement.project_manager.version_parser;
using honeyboard_release_service.implement.ui_event_handler.async_tasks.git_tasks;
using honeyboard_release_service.implement.ui_event_handler.async_tasks.others;
using honeyboard_release_service.implement.user_data_manager;
using honeyboard_release_service.models.VOs;
using honeyboard_release_service.utils;
using honeyboard_release_service.view_models.project_manager.items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace honeyboard_release_service.implement.project_manager
{
    internal class ReleasingProjectManager : BasePublisherModule
    {
        private ProjectVO? _currentImportedProjectVO;
        private Dictionary<string, ProjectVO> _importedProjects = new Dictionary<string, ProjectVO>();
        private BaseAsyncTask? _getVersionHistoryTaskCache;
        private CyberTreeViewObservableCollection<ICyberTreeViewItemContext>? _currentProjectBranchContextSource;
        private FirstLastObservableCollection<VersionHistoryItemViewModel> _versionHistoryItemContexts;
        private VersionAttributeParsingManager _versionAttrParsingManager;
        private VersionHistoryItemViewModel? _currentForcusVersionCommitVM;

        private event UserDataImportedHandler? _userDataImported;
        private event ImportedProjectsCollectionChangedHandler? _importedProjectsCollectionChanged;
        private event CurrentProjectChangedHandler? _currentProjectChanged;
        private event CurrentProjectBranchContextSourceChangedHandler? _currentProjectBranchContextSourceChanged;
        private event LatestVersionUpCommitChangedHandler? _latestVersionUpCommitChanged;
        private event PreUpdateVersionTimelineBackgroundHandler? _preUpdateVersionTimelineBackground;
        private event VersionTimelineUpdatedHandler? _versionTimelineUpdated;
        private event VersionPropertiesFoundHandler? _versionPropertiesFound;
        private event CurrentProjectVersionFilePathChangedHandler? _currentProjectVersionFilePathChanged;
        private event CurrentFocusVersionCommitChangedHandler? _currentFocusVersionCommitChanged;

        public static ReleasingProjectManager Current
        {
            get
            {
                return PublisherModuleManager.RPM_Instance;
            }
        }

        public event CurrentProjectVersionFilePathChangedHandler CurrentProjectVersionFilePathChanged
        {
            add
            {
                _currentProjectVersionFilePathChanged += value;
            }
            remove
            {
                _currentProjectVersionFilePathChanged -= value;
            }

        }

        public event VersionPropertiesFoundHandler VersionPropertiesFound
        {
            add
            {
                _versionPropertiesFound += value;
            }
            remove
            {
                _versionPropertiesFound -= value;
            }

        }

        public event VersionTimelineUpdatedHandler VersionTimelineUpdated
        {
            add
            {
                _versionTimelineUpdated += value;
            }
            remove
            {
                _versionTimelineUpdated -= value;
            }

        }

        public event PreUpdateVersionTimelineBackgroundHandler PreUpdateVersionTimelineBackground
        {
            add
            {
                _preUpdateVersionTimelineBackground += value;
            }
            remove
            {
                _preUpdateVersionTimelineBackground -= value;
            }
        }

        public event LatestVersionUpCommitChangedHandler LatestVersionUpCommitChanged
        {
            add
            {
                _latestVersionUpCommitChanged += value;
            }
            remove
            {
                _latestVersionUpCommitChanged -= value;
            }
        }

        public event CurrentProjectBranchContextSourceChangedHandler CurrentProjectBranchContextSourceChanged
        {
            add
            {
                _currentProjectBranchContextSourceChanged += value;
            }
            remove
            {
                _currentProjectBranchContextSourceChanged -= value;
            }
        }

        public event CurrentFocusVersionCommitChangedHandler CurrentFocusVersionCommitChanged
        {
            add
            {
                _currentFocusVersionCommitChanged += value;
            }
            remove
            {
                _currentFocusVersionCommitChanged -= value;
            }
        }

        public event CurrentProjectChangedHandler CurrentProjectChanged
        {
            add
            {
                _currentProjectChanged += value;
            }
            remove
            {
                _currentProjectChanged -= value;
            }
        }

        public event UserDataImportedHandler UserDataImported
        {
            add
            {
                _userDataImported += value;
            }
            remove
            {
                _userDataImported -= value;
            }
        }

        public event ImportedProjectsCollectionChangedHandler ImportedProjectsCollectionChanged
        {
            add
            {
                _importedProjectsCollectionChanged += value;
            }
            remove
            {
                _importedProjectsCollectionChanged -= value;
            }
        }

        public CyberTreeViewObservableCollection<ICyberTreeViewItemContext>? CurrentProjectBranchContextSource
        {
            get
            {
                return _currentProjectBranchContextSource;
            }
            private set
            {
                var oldValue = _currentProjectBranchContextSource;
                _currentProjectBranchContextSource = value;
                if (oldValue != value)
                {
                    _currentProjectBranchContextSourceChanged?.Invoke(this, oldValue, value);
                }
            }
        }

        public ProjectVO? CurrentImportedProjectVO { get => _currentImportedProjectVO; }

        public VersionAttributeParsingManager VAParsingManager { get => _versionAttrParsingManager; }

        public Dictionary<string, ProjectVO> ImportedProjects
        {
            get => _importedProjects;
            private set
            {
                _importedProjects = value;
            }
        }

        public VersionHistoryItemViewModel? CurrentFocusVersionCommitVM
        {
            get
            {
                return _currentForcusVersionCommitVM;
            }
            set
            {
                if (_currentForcusVersionCommitVM != value)
                {
                    _currentForcusVersionCommitVM = value;
                    _currentFocusVersionCommitChanged?.Invoke(this);
                }
            }
        }

        public VersionHistoryItemViewModel? LatestCommitVM
        {
            get
            {
                return _versionHistoryItemContexts.First;
            }

        }

        public VersionUpCommitVO? LatestCommitVO
        {
            get
            {
                return _versionHistoryItemContexts.First?.VersionCommitVO;
            }

        }

        public string SelectedBranchPath
        {
            get
            {
                return _currentImportedProjectVO?.OnBranch?.BranchPath ?? "";
            }
        }

        public string ProjectPath
        {
            get
            {
                return _currentImportedProjectVO?.Path ?? "";
            }
        }

        public string VersionPropertiesPath
        {
            get
            {
                return _currentImportedProjectVO?.VersionFilePath ?? "";
            }
        }

        public FirstLastObservableCollection<VersionHistoryItemViewModel> VersionHistoryItemContexts
        {
            get
            {
                return _versionHistoryItemContexts;
            }
        }

        private ReleasingProjectManager()
        {
            _versionAttrParsingManager = new VersionAttributeParsingManager();
            _versionHistoryItemContexts = new FirstLastObservableCollection<VersionHistoryItemViewModel>();
            _versionHistoryItemContexts.FirstChanged -= OnVersionHistoryItemContextsFirstChanged;
            _versionHistoryItemContexts.FirstChanged += OnVersionHistoryItemContextsFirstChanged;
        }

        public override void OnModuleStart()
        {
            base.OnModuleStart();
            _versionAttrParsingManager.LoadParserInformationFromFile();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            // Memory leak issue
            // Ref: https://michaelscodingspot.com/5-techniques-to-avoid-memory-leaks-by-events-in-c-net-you-should-know/
            _versionHistoryItemContexts.FirstChanged -= OnVersionHistoryItemContextsFirstChanged;
        }

        /// <summary>
        /// Set model cho project hiện tại
        /// Thông thường sẽ được gọi khi người dùng import project đã được lưu
        /// lại trước đó.
        /// </summary>
        /// <param name="projectVO"></param>
        public void SetCurrentImportedProject(ProjectVO? projectVO)
        {
            var oldProject = _currentImportedProjectVO;
            _currentImportedProjectVO = projectVO;
            UserDataManager.Current.SetCurrentImportedProject(_currentImportedProjectVO);

            if (oldProject != projectVO)
            {
                var branchSource = CreateBranchSourceForImportProject(projectVO);
                SetCurrentProjectBranchContextSource(branchSource);
                _currentProjectChanged?.Invoke(this, oldProject, _currentImportedProjectVO);
            }

            if (projectVO != null)
            {
                _versionAttrParsingManager.SetCurrentParserSyntax(projectVO.VersionAttrSyntax);
                var versionFileContent = "";
                if (File.Exists(projectVO.VersionFilePath))
                {
                    versionFileContent = File.ReadAllText(projectVO.VersionFilePath);
                }
                _versionAttrParsingManager.SetVersionAttrFileContent(versionFileContent);
            }
        }

        /// <summary>
        /// Set đường dẫn và cú pháp cho file version của project hiện tại 
        /// </summary>
        /// <param name="filePath">Đường dẫn tới file version attr</param>
        /// <param name="syntax">Cú pháp phân tích file version attr</param>
        /// <param name="versionAttrFileContent"> Nội dung của file version attr</param>
        public void SetVersionAttrFilePathAndSyntaxOfCurrentImportProject(string filePath
            , string syntax
            , string versionAttrFileContent)
        {
            if (CurrentImportedProjectVO != null)
            {
                var oldValue = CurrentImportedProjectVO.VersionFilePath;
                CurrentImportedProjectVO.VersionFilePath = filePath;
                CurrentImportedProjectVO.VersionAttrSyntax = syntax;
                _versionAttrParsingManager.SetCurrentParserSyntax(syntax);
                _versionAttrParsingManager.SetVersionAttrFileContent(versionAttrFileContent);

                if (oldValue != filePath)
                {
                    var arg = new ReleasingProjectEventArg(new
                    {
                        OldPath = oldValue,
                        NewPath = filePath,
                    });
                    _currentProjectVersionFilePathChanged?.Invoke(this, arg);
                }
            }
        }

        /// <summary>
        /// Lấy ra nhánh trong 1 project theo key là đường dẫn đến nhánh đó
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public BranchVO? GetBranchOfCurrentProjectFromPath(string path)
        {
            return CurrentImportedProjectVO?.Branchs[path];
        }

        /// <summary>
        /// Tạo mới project hiện tại từ 1 đường dẫn đến project đó
        /// </summary>
        /// <param name="proPath"></param>
        public void CreateNewProjectForCurrentProjectVO(string proPath)
        {
            var oldProject = _currentImportedProjectVO;
            _currentImportedProjectVO = new ProjectVO(proPath);
            if (string.IsNullOrEmpty(proPath))
            {
                _currentImportedProjectVO = null;
            }
            else
            {
                if (!ImportedProjects.ContainsKey(proPath))
                {
                    ImportedProjects[proPath] = _currentImportedProjectVO;
                    _importedProjectsCollectionChanged?.Invoke(this
                        , new ProjectsCollectionChangedEventArg(_currentImportedProjectVO
                            , null
                            , ProjectsCollectionChangedType.Add));
                }
                else
                {
                    var oldProjectVO = ImportedProjects[proPath];
                    ImportedProjects[proPath] = _currentImportedProjectVO;
                    _importedProjectsCollectionChanged?.Invoke(this
                        , new ProjectsCollectionChangedEventArg(_currentImportedProjectVO
                            , oldProjectVO
                            , ProjectsCollectionChangedType.Modified));
                }
                UserDataManager.Current.AddImportedProject(proPath, _currentImportedProjectVO);
                UserDataManager.Current.SetCurrentImportedProject(_currentImportedProjectVO);
            }
            _currentProjectChanged?.Invoke(this, oldProject, _currentImportedProjectVO);
        }

        /// <summary>
        /// Set source context (view model) cho các nhánh của project hiện tại
        /// </summary>
        /// <param name="source"></param>
        public void SetCurrentProjectBranchContextSource(
            CyberTreeViewObservableCollection<ICyberTreeViewItemContext> source)
        {
            CurrentProjectBranchContextSource = source;
        }

        /// <summary>
        /// Set nhánh check out hiện tại của project
        /// </summary>
        /// <param name="branch"></param>
        public void SetCurrentProjectOnBranch(string branch)
        {
            if (CurrentImportedProjectVO?.OnBranch?.BranchPath != branch)
            {
                if (CurrentImportedProjectVO != null)
                {
                    CurrentImportedProjectVO.SetOnBranch(branch);
                }
            }
        }

        /// <summary>
        /// Thêm thông tin nhánh cho project hiện tại
        /// Thường được gọi sau quá trình import mới project, fetch project
        /// </summary>
        /// <param name="bVO"></param>
        public void AddBranchToCurrentProject(BranchVO bVO)
        {
            CurrentImportedProjectVO?.AddProjectBranch(bVO);
        }

        /// <summary>
        /// Cập nhật lại version history timeline của project hiện tại
        /// </summary>
        public async void UpdateVersionHistoryTimelineInBackground(Level updateLevel = Level.Hard)
        {
            if (_getVersionHistoryTaskCache != null
                && !_getVersionHistoryTaskCache.IsCompleted
                && !_getVersionHistoryTaskCache.IsCanceled
                && !_getVersionHistoryTaskCache.IsFaulted)
            {
                _getVersionHistoryTaskCache.Cancel();
            }
            dynamic eventData = new
            {
                UpdateLevel = updateLevel,
            };

            var eventArg = new ReleasingProjectEventArg(eventData);

            if (_currentImportedProjectVO != null)
            {
                BaseAsyncTask getVersionHistory = new GetVersionHistoryTask(
                new string[]
                {
                    ProjectPath,
                    VersionPropertiesPath
                }
                , versionPropertiesFoundCallback: (prop, task) =>
                {
                    dynamic data = prop;
                    VersionUpCommitVO vVO = new VersionUpCommitVO()
                    {
                        CommitTitle = data.Title,
                        ReleaseDateTime = DateTime.ParseExact(data.DateTime, "HH:mm:ss yyyy-MM-dd",
                                   System.Globalization.CultureInfo.InvariantCulture),
                        AuthorEmail = data.Email,
                        CommitId = data.HashId,
                    };

                    // Xử lý trên model
                    var vOInCurrentBranch = _currentImportedProjectVO?.AddCommitVOToCurrentBranch(vVO);

                    // Xử lý trên viewmodel
                    if (vOInCurrentBranch != null)
                    {
                        _versionHistoryItemContexts.Add(
                            new VersionHistoryItemViewModel(vOInCurrentBranch));

                        dynamic versionUpCommitFoundEventData = new
                        {
                            UpdateLevel = updateLevel,
                            VersionUpCommit = vOInCurrentBranch,
                        };

                        _versionPropertiesFound?.Invoke(this, new ReleasingProjectEventArg(versionUpCommitFoundEventData));
                    }
                }
                , taskFinishedCallback: (s) =>
                {
                    _versionTimelineUpdated?.Invoke(this, eventArg);
                });
                _getVersionHistoryTaskCache = getVersionHistory;
                _versionHistoryItemContexts.Clear();

                _preUpdateVersionTimelineBackground?.Invoke(this, eventArg);

                await getVersionHistory.Execute();
            }
        }

        /// <summary>
        /// Chỉ gọi hàm này khi user data đã được load thành công
        /// </summary>
        /// <param name="currentProject"></param>
        /// <param name="importedProjects"></param>
        public void UpdateWorkingProjectsAfterLoadedFromUserData(ProjectVO? currentProject
            , Dictionary<string, ProjectVO> importedProjects)
        {
            if (_getVersionHistoryTaskCache != null
                && !_getVersionHistoryTaskCache.IsCompleted
                && !_getVersionHistoryTaskCache.IsCanceled
                && !_getVersionHistoryTaskCache.IsFaulted)
            {
                _getVersionHistoryTaskCache.Cancel();
            }

            var oldProject = _currentImportedProjectVO;
            _currentImportedProjectVO = currentProject;
            ImportedProjects = importedProjects;

            if (currentProject != null && currentProject.OnBranch != null)
            {

                BaseAsyncTask setParserTask = new CancelableAsyncTask(
                    mainFunc: async (cts, res) =>
                    {
                        _versionAttrParsingManager.SetCurrentParserSyntax(currentProject.VersionAttrSyntax);
                        var versionFileContent = "";
                        if (File.Exists(currentProject.VersionFilePath))
                        {
                            versionFileContent = await File.ReadAllTextAsync(currentProject.VersionFilePath);
                        }
                        _versionAttrParsingManager.SetVersionAttrFileContent(versionFileContent);
                        return res;
                    }
                    , cancellationTokenSource: new CancellationTokenSource()
                    , estimatedTime: 2000
                    , delayTime: 2000
                    , name: "Setting version attribute parser");

                BaseAsyncTask importProjectBranchTask = new ParseProjectBranchsFromVOTask(
                   new object[] { currentProject.Branchs, currentProject.OnBranch }
                   , (result) =>
                   {
                       var source = result.Result
                        as CyberTreeViewObservableCollection<ICyberTreeViewItemContext>;
                       CurrentProjectBranchContextSource = source;
                   });

                List<BaseAsyncTask> tasks = new List<BaseAsyncTask>();
                tasks.Add(setParserTask);
                tasks.Add(importProjectBranchTask);

                MultiAsyncTask multiTask = new MultiAsyncTask(tasks
                  , new CancellationTokenSource()
                  , null
                  , name: "Importing branchs from user data"
                  , delayTime: 0
                  , reportDelay: 100);
                HoneyboardReleaseService.Current.ServiceManager?.App.OpenMultiTaskBox(
                    title: "Importing branchs from user data"
                   , task: multiTask
                   , isCancelable: false
                   , isUseMultiTaskReport: false);
            }
            _userDataImported?.Invoke(this);
            UpdateVersionHistoryTimelineInBackground(updateLevel: Level.Normal);
            // Nên gọi sự kiện này sau khi toàn bộ user data đã được imported thành công
            _currentProjectChanged?.Invoke(this, oldProject, _currentImportedProjectVO);
        }

        private void OnVersionHistoryItemContextsFirstChanged(object sender, VersionHistoryItemViewModel? oldFirst, VersionHistoryItemViewModel? newFirst)
        {
            _latestVersionUpCommitChanged?.Invoke(this);
        }

        private CyberTreeViewObservableCollection<ICyberTreeViewItemContext> CreateBranchSourceForImportProject(ProjectVO? projectVO)
        {
            var source = new CyberTreeViewObservableCollection<ICyberTreeViewItemContext>();

            if (projectVO == null)
            {
                return source;
            }

            foreach (var branch in projectVO.Branchs)
            {
                var path = branch.Key;

                var splits = path.Split("/", StringSplitOptions.TrimEntries);
                string rootFolder = "";
                int startFolderIndex = 1;
                int lenght = splits.Length;
                var isRemote = false;
                BranchItemViewModel? parents;

                if (splits[0].Equals("remotes", StringComparison.CurrentCultureIgnoreCase))
                {
                    rootFolder = "Remote";
                    isRemote = true;
                }
                else if (splits[0].Equals("origin", StringComparison.CurrentCultureIgnoreCase))
                {
                    rootFolder = "Remote";
                    isRemote = true;
                    startFolderIndex = 0;
                }
                else
                {
                    rootFolder = "Local";
                    startFolderIndex = 0;
                }

                parents = source[rootFolder] as BranchItemViewModel;

                if (parents == null)
                {
                    var bVO = new BranchVO("", rootFolder);
                    parents = new BranchItemViewModel(bVO);
                    source.Add(parents);
                }

                string branchPath = "";

                for (int i = startFolderIndex; i < lenght; i++)
                {
                    var current = parents?.Items[splits[i]];

                    if (i == lenght - 1)
                        branchPath += splits[i];
                    else
                        branchPath += splits[i] + "/";

                    if (current == null)
                    {
                        var bVO = new BranchVO(path: i == lenght - 1 ? branchPath : ""
                            , title: splits[i]
                            , isNode: i == lenght - 1
                            , isRemote: isRemote);
                        current = new BranchItemViewModel(bVO);
                        parents?.AddItem(current);
                    }
                    parents = current as BranchItemViewModel;
                }
            }

            return source;
        }
    }

    internal delegate void CurrentFocusVersionCommitChangedHandler(object sender);
    internal delegate void UserDataImportedHandler(object sender);
    internal delegate void PreUpdateVersionTimelineBackgroundHandler(object sender, ReleasingProjectEventArg arg);
    internal delegate void VersionTimelineUpdatedHandler(object sender, ReleasingProjectEventArg arg);
    internal delegate void VersionPropertiesFoundHandler(object sender, ReleasingProjectEventArg arg);
    internal delegate void CurrentProjectVersionFilePathChangedHandler(object sender, ReleasingProjectEventArg arg);
    internal delegate void LatestVersionUpCommitChangedHandler(object sender);
    internal delegate void ImportedProjectsCollectionChangedHandler(object sender, ProjectsCollectionChangedEventArg arg);
    internal delegate void CurrentProjectChangedHandler(object sender, ProjectVO? oldProject, ProjectVO? newProject);
    internal delegate void CurrentProjectBranchContextSourceChangedHandler(object sender
        , CyberTreeViewObservableCollection<ICyberTreeViewItemContext>? oldSource
        , CyberTreeViewObservableCollection<ICyberTreeViewItemContext>? newSource);

    internal class ReleasingProjectEventArg
    {
        public object? EventData { get; private set; }

        public ReleasingProjectEventArg(object? eventData)
        {
            EventData = eventData;
        }
    }

    internal class ProjectsCollectionChangedEventArg : ReleasingProjectEventArg
    {
        public ProjectVO? OldValue { get; private set; }
        public ProjectVO? NewValue { get; private set; }
        public ProjectsCollectionChangedType ChangedType { get; private set; }
        public ProjectsCollectionChangedEventArg(
            ProjectVO? newValue
            , ProjectVO? oldValue
            , ProjectsCollectionChangedType changedType) : base(null)
        {
            OldValue = oldValue;
            NewValue = newValue;
            ChangedType = changedType;
        }
    }
    internal enum ProjectsCollectionChangedType
    {
        Add = 1,
        Remove = 2,
        Modified = 3
    }
}
