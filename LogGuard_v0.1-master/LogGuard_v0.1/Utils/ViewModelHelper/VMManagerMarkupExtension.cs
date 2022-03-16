using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace LogGuard_v0._1.Utils.ViewModelHelper
{
    [MarkupExtensionReturnType(typeof(object))]
    public class VMManagerMarkupExtension : MarkupExtension
    {
        private static Dictionary<Type, object> ParentDataContextInstanceCache;

        [ConstructorArgument("dataContextType")]
        public Type DataContextType { get; set; }

        [ConstructorArgument("parentDataContextType")]
        public Type ParentDataContextType { get; set; }

        static VMManagerMarkupExtension()
        {
            ParentDataContextInstanceCache = new Dictionary<Type, object>();
        }

        public VMManagerMarkupExtension(Type dataContextType)
        {
            DataContextType = dataContextType;
        }

        public VMManagerMarkupExtension(Type dataContextType, Type parentDataContextType)
        {
            DataContextType = dataContextType;
            ParentDataContextType = parentDataContextType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // if null mean this view model is the most parent
            if(ParentDataContextType == null)
            {
                if (ParentDataContextInstanceCache.ContainsKey(DataContextType))
                {
                    return ParentDataContextInstanceCache[DataContextType];
                }
                var dataContext = Activator.CreateInstance(DataContextType);
                ParentDataContextInstanceCache.Add(DataContextType, dataContext);
                return dataContext;
            }
            else
            {
                // if not exist parent so child should not be exist
                if (!ParentDataContextInstanceCache.ContainsKey(ParentDataContextType))
                {
                    return null;
                }
                var parentInCache = ParentDataContextInstanceCache[ParentDataContextType];
                var childContext = Activator.CreateInstance(DataContextType, parentInCache);
                ParentDataContextInstanceCache.Add(DataContextType, childContext);
                return childContext;
            }

          
        }
    }
}
