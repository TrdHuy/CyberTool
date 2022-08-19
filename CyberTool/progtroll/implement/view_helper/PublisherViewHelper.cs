using progtroll.@base.module;
using progtroll.definitions;
using progtroll.implement.module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace progtroll.implement.view_helper
{
    internal class PublisherViewHelper : BasePublisherModule
    {
        private Dictionary<PublisherViewKeyDefinition, object> ViewMap;

        #region ViewKey
        public static readonly DependencyProperty ViewKeyProperty = DependencyProperty.RegisterAttached(
                 "ViewKey",
                 typeof(PublisherViewKeyDefinition),
                 typeof(PublisherViewHelper),
                 new PropertyMetadata(default(PublisherViewKeyDefinition), OnViewKeyChangedCallback));

        private static void OnViewKeyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var key = (PublisherViewKeyDefinition)e.NewValue;
                if (Current.ViewMap.ContainsKey(key))
                {
                    Current.ViewMap.Remove(key);
                }
                Current.ViewMap.Add(key, d);
            }
            catch
            {

            }

        }

        public static PublisherViewKeyDefinition GetViewKey(UIElement obj)
        {
            return (PublisherViewKeyDefinition)obj.GetValue(ViewKeyProperty);
        }

        public static void SetViewKey(UIElement obj, PublisherViewKeyDefinition value)
        {
            obj.SetValue(ViewKeyProperty, value);
        }
        #endregion

        public static PublisherViewHelper Current
        {
            get
            {
                return PublisherModuleManager.PVH_Instance;
            }
        }

        public PublisherViewHelper()
        {
            ViewMap = new Dictionary<PublisherViewKeyDefinition, object>();
        }


        public object GetViewByKey(PublisherViewKeyDefinition key)
        {
            return ViewMap[key];
        }

    }

}
