using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace cyber_base.implement.extension
{
    public static class CyberExtensionMethod
    {
        public static T FindChild<T>(this DependencyObject dO, string childName) where T : DependencyObject
        {
            if (dO == null)
            {
                return null;
            }

            T val = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(dO);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(dO, i);
                if (child as T == null)
                {
                    val = child.FindChild<T>(childName);
                    if (val != null)
                    {
                        break;
                    }

                    continue;
                }

                if (!string.IsNullOrEmpty(childName))
                {
                    FrameworkElement frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        val = (T)child;
                        break;
                    }

                    continue;
                }

                val = (T)child;
                break;
            }

            return val;
        }

        public static T? FindParent<T>(this DependencyObject dO, string parentsName) where T : DependencyObject
        {
            if (dO == null)
            {
                return null;
            }

            T? val = null;
            var parentDO = VisualTreeHelper.GetParent(dO);

            if (parentDO != null && !string.IsNullOrEmpty(parentsName))
            {
                FrameworkElement frameworkElement = parentDO as FrameworkElement;
                if (frameworkElement != null && frameworkElement.Name == parentsName)
                {
                    val = (T)parentDO;
                }
                else if(frameworkElement != null)
                {
                    val = FindParent<T>(frameworkElement, parentsName);
                }

            }

            return val;
        }

        public static T? FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }
    }
}
