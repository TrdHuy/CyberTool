using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LogGuard_v0._1.LogGuard.Control
{
    public class CCControl: System.Windows.Controls.Control
    {
        #region Content
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                "Content",
                typeof(object),
                typeof(CCControl),
                new PropertyMetadata(default(object), new PropertyChangedCallback(ContentPropertyChangedCallback)));

        private static void ContentPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        #endregion

        public event ApplyTemplateHandler ApplyTemplate;
        public bool IsAppliedTemplate;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ApplyTemplate?.Invoke(this);
            IsAppliedTemplate = true;
        }
        public DependencyObject ForceGetTemplateChild(string childName)
        {
            return base.GetTemplateChild(childName) as DependencyObject;
        }
    }
    public delegate void ApplyTemplateHandler(object sender);
}
