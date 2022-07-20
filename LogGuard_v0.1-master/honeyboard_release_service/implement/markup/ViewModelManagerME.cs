using cyber_base.implement.utils;
using cyber_base.view_model;
using honeyboard_release_service.implement.view_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace honeyboard_release_service.implement.markup
{
    internal class ViewModelManagerME : MarkupExtension
    {
        private ObservableDictionary<Type, object> DataContextCache = ViewModelManager.Current.DataContextCache;

        public static event OnDataContextGeneratedHandler? DataContextGenerated;
        public static event OnDataContextDestroyedHandler? DataContextDestroyed;

        public Type? DataContextType { get; set; }

        public Type? ParentDataContextType { get; set; }

        public DataContextGeneratorType GeneratorType { get; set; }

        public ViewModelManagerME()
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
                                DataContextCache.Remove(DataContextType);
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
                                    DataContextCache.Remove(DataContextType);
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
