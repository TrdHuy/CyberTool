using cyber_base.implement.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace cyber_base.view_model
{
    public abstract class AbstractViewModelManager : MarkupExtension
    {
        public static event OnDataContextGeneratedHandler? DataContextGenerated;
        public static event OnDataContextDestroyedHandler? DataContextDestroyed;

        public Type? DataContextType { get; set; }

        public Type? ParentDataContextType { get; set; }

        public DataContextGeneratorType GeneratorType { get; set; }

        public override object? ProvideValue(IServiceProvider serviceProvider)
        {
            if (DataContextType != null)
            {
                if (GeneratorType == DataContextGeneratorType.Reuse)
                {
                    if (IsDataContextTypeExistInCache(DataContextType))
                    {
                        return GetDataContextByTypeFromCache(DataContextType);
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
                            if (IsDataContextTypeExistInCache(DataContextType))
                            {
                                var oldContext = RemoveDataContextByTypeFromCache(DataContextType);
                                DataContextDestroyed?.Invoke(this, new DataContextDestroyedArgs(oldContext));
                            }
                            if (dataContext != null)
                            {
                                AddDataContextByTypeToCache(DataContextType, dataContext);
                                DataContextGenerated?.Invoke(this, new DataContextGeneratedArgs(dataContext));
                            }
                            return dataContext;
                        }
                        else
                        {
                            // if not exist parent so child should not be exist
                            if (!IsDataContextTypeExistInCache(ParentDataContextType))
                            {
                                return null;
                            }
                            var parentInCache = GetDataContextByTypeFromCache(ParentDataContextType);
                            var childContext = Activator.CreateInstance(DataContextType, parentInCache);

                            if (childContext != null)
                            {
                                var parentVM = parentInCache as BaseViewModel;
                                if (IsDataContextTypeExistInCache(DataContextType))
                                {
                                    var oldContext = RemoveDataContextByTypeFromCache(DataContextType);
                                    DataContextDestroyed?.Invoke(this, new DataContextDestroyedArgs(oldContext));
                                }
                                AddDataContextByTypeToCache(DataContextType, childContext);
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

        protected abstract void AddDataContextByTypeToCache(Type dataContextType, object dataContext);
        protected abstract object RemoveDataContextByTypeFromCache(Type dataContextType);
        protected abstract object? GetDataContextByTypeFromCache(Type dataContextType);
        protected abstract bool IsDataContextTypeExistInCache(Type dataContextType);
        public abstract void Dispose();
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
