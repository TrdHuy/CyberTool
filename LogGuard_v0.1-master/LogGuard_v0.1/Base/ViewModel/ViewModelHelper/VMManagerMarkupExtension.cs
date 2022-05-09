using LogGuard_v0._1.Base.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace LogGuard_v0._1.Base.ViewModel.ViewModelHelper
{
    [MarkupExtensionReturnType(typeof(object))]
    public class VMManagerMarkupExtension : MarkupExtension
    {
        private static Dictionary<Type, object> DataContextCache;

        public static event OnDataContextGeneratedHandler DataContextGenerated;
        public static event OnDataContextDestroyedHandler DataContextDestroyed;


        [ConstructorArgument("dataContextType")]
        public Type DataContextType { get; set; }

        [ConstructorArgument("parentDataContextType")]
        public Type ParentDataContextType { get; set; }

        public DataContextGeneratorType GeneratorType { get; set; }

        static VMManagerMarkupExtension()
        {
            DataContextCache = new Dictionary<Type, object>();
        }

        public VMManagerMarkupExtension(Type dataContextType
            , DataContextGeneratorType generatorType = DataContextGeneratorType.CreateNew)
        {
            DataContextType = dataContextType;
            GeneratorType = generatorType;
        }

        public VMManagerMarkupExtension(Type dataContextType
            , Type parentDataContextType)
        {
            DataContextType = dataContextType;
            ParentDataContextType = parentDataContextType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {

            if (GeneratorType == DataContextGeneratorType.Reuse)
            {
                return DataContextCache[DataContextType];
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
            var parentInCache = DataContextCache[dataContextType];
            var pageVM = parentInCache as IPageViewModel;
            pageVM?.OnLoaded();
        }

        public static void OnPageViewModelUnloaded(Type dataContextType)
        {
            if (dataContextType == null) return;
            var parentInCache = DataContextCache[dataContextType];
            var pageVM = parentInCache as IPageViewModel;
            var shouldRemoveAllPageChildVM = (bool)pageVM?.OnUnloaded();
            if (shouldRemoveAllPageChildVM)
            {
                var baseVM = parentInCache as BaseViewModel;
                foreach (var child in baseVM.ChildModels)
                {
                    try
                    {
                        var data = DataContextCache[child.GetType()];
                        DataContextCache.Remove(child.GetType());
                        DataContextDestroyed(DataContextCache
                            , new DataContextDestroyedArgs(data));
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }
            var data2 = DataContextCache[dataContextType];
            DataContextCache.Remove(dataContextType);
            DataContextDestroyed(DataContextCache
                , new DataContextDestroyedArgs(data2));
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
