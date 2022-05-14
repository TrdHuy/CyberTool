using cyber_base.implement.utils;
using cyber_base.view_model;
using log_guard.implement.flow.view_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace log_guard.implement.markup
{

    [MarkupExtensionReturnType(typeof(object))]
    internal class ViewModelManagerME : MarkupExtension
    {
        private ObservableDictionary<Type, object> DataContextCache = ViewModelManager.Current.DataContextCache;

        public static event OnDataContextGeneratedHandler DataContextGenerated;
        public static event OnDataContextDestroyedHandler DataContextDestroyed;


        [ConstructorArgument("dataContextType")]
        public Type DataContextType { get; set; }

        [ConstructorArgument("parentDataContextType")]
        public Type ParentDataContextType { get; set; }

        public DataContextGeneratorType GeneratorType { get; set; }

        public ViewModelManagerME(Type dataContextType
            , DataContextGeneratorType generatorType = DataContextGeneratorType.CreateNew)
        {
            DataContextType = dataContextType;
            GeneratorType = generatorType;
        }

        public ViewModelManagerME(Type dataContextType
            , Type parentDataContextType)
        {
            DataContextType = dataContextType;
            ParentDataContextType = parentDataContextType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (GeneratorType == DataContextGeneratorType.Reuse)
            {
                return DataContextCache?[DataContextType];
            }
            else
            {
                // if null mean this view model is the most parent
                if (ParentDataContextType == null)
                {
                    var dataContext = Activator.CreateInstance(DataContextType);

                    if (DataContextCache.ContainsKey(DataContextType))
                    {
                        DataContextCache.Remove(DataContextType);
                    }
                    DataContextCache.Add(DataContextType, dataContext);
                    DataContextGenerated?.Invoke(this, new DataContextGeneratedArgs(dataContext));
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

                    var parentVM = parentInCache as BaseViewModel;
                    if (DataContextCache.ContainsKey(DataContextType))
                    {
                        DataContextCache.Remove(DataContextType);
                    }
                    DataContextCache.Add(DataContextType, childContext);
                    DataContextGenerated?.Invoke(this, new DataContextGeneratedArgs(childContext));

                    return childContext;
                }
            }

        }

        public static void OnPageViewModelLoaded(Type dataContextType)
        {
            if (dataContextType == null) return;
            var parentInCache = ViewModelManager.Current.DataContextCache[dataContextType];
            //var pageVM = parentInCache as IPageViewModel;
            //pageVM?.OnLoaded();
        }

        public static void OnPageViewModelUnloaded(Type dataContextType)
        {
            if (dataContextType == null) return;
            var parentInCache = ViewModelManager.Current.DataContextCache[dataContextType];
            //var pageVM = parentInCache as IPageViewModel;
            //var shouldRemoveAllPageChildVM = (bool)pageVM?.OnUnloaded();
            //if (shouldRemoveAllPageChildVM)
            //{
            //    var baseVM = parentInCache as BaseViewModel;
            //    foreach (var child in baseVM.ChildModels)
            //    {
            //        try
            //        {
            //            var data = DataContextCache[child.GetType()];
            //            DataContextCache.Remove(child.GetType());
            //            DataContextDestroyed(DataContextCache
            //                , new DataContextDestroyedArgs(data));
            //        }
            //        catch (Exception ex)
            //        {

            //        }

            //    }
            //}
            //var data2 = DataContextCache[dataContextType];
            //DataContextCache.Remove(dataContextType);
            //DataContextDestroyed(DataContextCache
            //    , new DataContextDestroyedArgs(data2));
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
