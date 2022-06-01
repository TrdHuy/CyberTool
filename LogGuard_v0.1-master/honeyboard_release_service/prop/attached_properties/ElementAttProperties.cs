using cyber_base.implement.command;
using cyber_base.implement.extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace honeyboard_release_service.prop.attached_properties
{
    internal enum PathType
    {
        Default = 0,
        Folder = 1,
        File = 2,
    }

    internal class ElementAttProperties : UIElement
    {
        #region PathTextBox
        public static readonly DependencyProperty PathTextBoxProperty =
           DependencyProperty.RegisterAttached(
           "PathTextBox",
           typeof(PathType),
           typeof(ElementAttProperties),
           new FrameworkPropertyMetadata(defaultValue: PathType.Default,
               flags: FrameworkPropertyMetadataOptions.AffectsRender,
               new PropertyChangedCallback(OnIsPathTextBoxChangedCallback)));

        private static void OnIsPathTextBoxChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pathType = (PathType)e.NewValue;
            switch (d)
            {
                case TextBox textBox:
                    if (pathType != PathType.Default && textBox != null)
                    {
                        textBox.PreviewKeyDown -= PathTextBoxPreviewKeyDown;
                        textBox.PreviewKeyDown += PathTextBoxPreviewKeyDown;
                    }
                    else if (pathType == PathType.Default && textBox != null)
                    {
                        textBox.PreviewKeyDown -= PathTextBoxPreviewKeyDown;
                    }
                    break;
                case Rectangle recognizeRec:
                    var parentTB = recognizeRec.FindParent<TextBox>();
                    if (parentTB != null)
                    {
                        if (pathType != PathType.Default && parentTB != null)
                        {
                            recognizeRec.InputBindings.Clear();

                            parentTB.PreviewKeyDown -= PathTextBoxPreviewKeyDown;
                            parentTB.PreviewKeyDown += PathTextBoxPreviewKeyDown;

                            MouseBinding onRecCmdMouseBinding = new MouseBinding();
                            onRecCmdMouseBinding.MouseAction = MouseAction.LeftClick;
                            onRecCmdMouseBinding.Command = new BaseDotNetCommandImpl((s) =>
                            {
                                if (pathType == PathType.Folder)
                                {
                                    var path = HoneyboardReleaseService
                                        .Current
                                        .ServiceManager?
                                        .App.OpenFolderChooserDialogWindow();
                                    parentTB.Text = path;
                                    (parentTB.GetValue(PathSelectedProperty) as ICommand)?.Execute(null);
                                }
                                else if (pathType == PathType.File)
                                {
                                    var path = HoneyboardReleaseService
                                        .Current
                                        .ServiceManager?
                                        .App.OpenFileChooserDialogWindow(parentTB.GetValue(DialogBoxTitleProperty)?.ToString() ?? "",
                                        parentTB.GetValue(FileBoxFilterProperty)?.ToString() ?? "");
                                    parentTB.Text = path;
                                    (parentTB.GetValue(PathSelectedProperty) as ICommand)?.Execute(null);
                                }
                            });
                            recognizeRec.InputBindings.Add(onRecCmdMouseBinding);
                        }
                        else if (pathType == PathType.Default && parentTB != null)
                        {
                            parentTB.PreviewKeyDown -= PathTextBoxPreviewKeyDown;
                            recognizeRec.InputBindings.Clear();
                        }
                    }
                    break;
            }
        }

        private static void PathTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = true;

            if (textBox != null && (e.Key == Key.Delete || e.Key == Key.Back))
            {
                textBox.Text = "";
            }
        }

        public static PathType GetPathTextBox(UIElement target) =>
            (PathType)target.GetValue(PathTextBoxProperty);
        public static void SetPathTextBox(UIElement target, PathType value) =>
            target.SetValue(PathTextBoxProperty, value);
        #endregion

        #region DialogBoxTitle
        public static readonly DependencyProperty DialogBoxTitleProperty =
           DependencyProperty.RegisterAttached(
           "DialogBoxTitle",
           typeof(string),
           typeof(ElementAttProperties),
           new FrameworkPropertyMetadata(defaultValue: "",
               flags: FrameworkPropertyMetadataOptions.AffectsRender));

        public static string GetDialogBoxTitle(UIElement target) =>
            (string)target.GetValue(DialogBoxTitleProperty);
        public static void SetDialogBoxTitle(UIElement target, string value) =>
            target.SetValue(DialogBoxTitleProperty, value);
        #endregion

        #region FileBoxFilter
        public static readonly DependencyProperty FileBoxFilterProperty =
           DependencyProperty.RegisterAttached(
           "FileBoxFilter",
           typeof(string),
           typeof(ElementAttProperties),
           new FrameworkPropertyMetadata(defaultValue: "",
               flags: FrameworkPropertyMetadataOptions.AffectsRender));

        public static string GetFileBoxFilter(UIElement target) =>
            (string)target.GetValue(FileBoxFilterProperty);
        public static void SetFileBoxFilter(UIElement target, string value) =>
            target.SetValue(FileBoxFilterProperty, value);
        #endregion

        #region PathSelected
        public static readonly DependencyProperty PathSelectedProperty =
           DependencyProperty.RegisterAttached(
           "PathSelected",
           typeof(ICommand),
           typeof(ElementAttProperties),
           new FrameworkPropertyMetadata(defaultValue: default(ICommand)));

        public static ICommand GetPathSelected(UIElement target) =>
            (ICommand)target.GetValue(PathSelectedProperty);
        public static void SetPathSelected(UIElement target, ICommand value) =>
            target.SetValue(PathSelectedProperty, value);
        #endregion

    }
}
