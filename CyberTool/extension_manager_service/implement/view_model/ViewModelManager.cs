using cyber_base.implement.utils;
using cyber_base.view_model;
using extension_manager_service.@base;
using extension_manager_service.implement.module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace extension_manager_service.implement.view_model
{
    internal class ViewModelManager : MarkupExtension, IExtensionManagerModule
    {
        private static ObservableDictionary<Type, object> DataContextCache { get; }

        public static event OnDataContextGeneratedHandler? DataContextGenerated;
        public static event OnDataContextDestroyedHandler? DataContextDestroyed;
        
        public Type? DataContextType { get; set; }

        public Type? ParentDataContextType { get; set; }

        public DataContextGeneratorType GeneratorType { get; set; }

        public static ViewModelManager Current
        {
            get
            {
                return ModuleManager.VMM_Instance;
            }
        }

        static ViewModelManager()
        {
            DataContextCache = new ObservableDictionary<Type, object>();
        }

        public ViewModelManager()
        {
        }

        #region override field
        public void OnModuleStart()
        {
        }

        public void OnViewInstantiated()
        {
        }

        public void OnModuleDestroy()
        {
        }

        public override object? ProvideValue(IServiceProvider serviceProvider)
        {
            if (DataContextType != null)
            {
                if (GeneratorType == DataContextGeneratorType.Reuse)
                {
                    if (DataContextCache.ContainsKey(DataContextType))
                    {
                        return DataContextCache[DataContextType];
                    }
                    return null;
                }
                else
                {
                    // Đặt trong try catch để tránh lỗi null trong lúc design time
                    try
                    {
                        // if null mean this view model is the most parent
                        if (ParentDataContextType == null)
                        {
                            var dataContext = Activator.CreateInstance(DataContextType);
                            if (DataContextCache.ContainsKey(DataContextType))
                            {
                                var oldContext = DataContextCache.Remove(DataContextType);
                                DataContextDestroyed?.Invoke(this, new DataContextDestroyedArgs(oldContext));
                            }
                            if (dataContext != null)
                            {
                                DataContextCache.Add(DataContextType, dataContext);
                                DataContextGenerated?.Invoke(this, new DataContextGeneratedArgs(dataContext));
                            }
                            return dataContext;
                        }
                        else
                        {
                            // if not exist parent so child should not be exist
                            if (!DataContextCache.ContainsKey(ParentDataContextType))
                            {
                                return null;
                            }
                            var parentInCache = DataContextCache[ParentDataContextType];
                            var childContext = Activator.CreateInstance(DataContextType, parentInCache);

                            if (childContext != null)
                            {
                                var parentVM = parentInCache as BaseViewModel;
                                if (DataContextCache.ContainsKey(DataContextType))
                                {
                                    var oldContext = DataContextCache.Remove(DataContextType);
                                    DataContextDestroyed?.Invoke(this, new DataContextDestroyedArgs(oldContext));
                                }
                                DataContextCache.Add(DataContextType, childContext);
                                DataContextGenerated?.Invoke(this, new DataContextGeneratedArgs(childContext));
                            }

                            return childContext;
                        }
                    }
                    catch { }

                }
            }
            return null;

        }
        #endregion

    }

    public delegate void OnDataContextGeneratedHandler(object sender, DataContextGeneratedArgs e);

    public class DataContextGeneratedArgs : EventArgs
    {
        public object DataContext { get; }

        public DataContextGeneratedArgs(object dataContext)
        {
            DataContext = dataContext;
        }
    }

    public delegate void OnDataContextDestroyedHandler(object sender, DataContextDestroyedArgs e);

    public class DataContextDestroyedArgs : EventArgs
    {
        public object DataContext { get; }

        public DataContextDestroyedArgs(object dataContext)
        {
            DataContext = dataContext;
        }
    }

    public enum DataContextGeneratorType
    {
        Reuse = 1,
        CreateNew = 2,
    }
}
