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
using System.Windows.Media;
using System.Windows.Shapes;

namespace progtroll.views.controls.path_text_box
{
    internal class PathTextBox : TextBox
    {
        public enum Type
        {
            Default = 0,
            Folder = 1,
            File = 2,
        }

        private readonly string RECOGNIZE_INPUT_RECTANGLE_NAME = "RecogRec";

        #region IconGeometry
        public static readonly DependencyProperty IconGeometryProperty =
           DependencyProperty.RegisterAttached(
           "IconGeometry",
           typeof(Geometry),
           typeof(PathTextBox),
           new FrameworkPropertyMetadata(defaultValue: default(Geometry),
               flags: FrameworkPropertyMetadataOptions.AffectsRender));

        public Geometry IconGeometry
        {
            get { return (Geometry)GetValue(IconGeometryProperty); }
            set { SetValue(IconGeometryProperty, value); }
        }
        #endregion

        #region DialogBoxTitle
        public static readonly DependencyProperty DialogBoxTitleProperty =
           DependencyProperty.RegisterAttached(
           "DialogBoxTitle",
           typeof(string),
           typeof(PathTextBox),
           new FrameworkPropertyMetadata(defaultValue: "",
               flags: FrameworkPropertyMetadataOptions.AffectsRender));

        public string DialogBoxTitle
        {
            get { return (string)GetValue(DialogBoxTitleProperty); }
            set { SetValue(DialogBoxTitleProperty, value); }
        }
        #endregion

        #region FileBoxFilter
        public static readonly DependencyProperty FileBoxFilterProperty =
           DependencyProperty.RegisterAttached(
           "FileBoxFilter",
           typeof(string),
           typeof(PathTextBox),
           new FrameworkPropertyMetadata(defaultValue: "",
               flags: FrameworkPropertyMetadataOptions.AffectsRender));

        public string FileBoxFilter
        {
            get { return (string)GetValue(FileBoxFilterProperty); }
            set { SetValue(FileBoxFilterProperty, value); }
        }
        #endregion

        #region PathSelected
        public static readonly DependencyProperty PathSelectedProperty =
           DependencyProperty.RegisterAttached(
           "PathSelected",
           typeof(ICommand),
           typeof(PathTextBox),
           new FrameworkPropertyMetadata(defaultValue: default(ICommand)));

        public ICommand PathSelected
        {
            get { return (ICommand)GetValue(PathSelectedProperty); }
            set { SetValue(PathSelectedProperty, value); }
        }
        #endregion

        #region PathTextBox
        public static readonly DependencyProperty PathTypeProperty =
           DependencyProperty.RegisterAttached(
           "PathTextBox",
           typeof(Type),
           typeof(PathTextBox),
           new FrameworkPropertyMetadata(defaultValue: Type.Default,
               flags: FrameworkPropertyMetadataOptions.AffectsRender,
               new PropertyChangedCallback(OnIsPathTextBoxChangedCallback)));

        private static void OnIsPathTextBoxChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (PathTextBox)d;
            ctrl.OnPathTypeChanged();
        }

        public Type PathType
        {
            get { return (Type)GetValue(PathTypeProperty); }
            set { SetValue(PathTypeProperty, value); }
        }

        #endregion

        #region IsShouldOpenFileChooser
        public static readonly DependencyProperty IsShouldOpenFileChooserProperty =
           DependencyProperty.RegisterAttached(
           "IsShouldOpenFileChooser",
           typeof(Func<bool>),
           typeof(PathTextBox),
           new PropertyMetadata(defaultValue: default(Func<bool>)));

        public Func<bool> IsShouldOpenFileChooser
        {
            get { return (Func<bool>)GetValue(IsShouldOpenFileChooserProperty); }
            set { SetValue(IsShouldOpenFileChooserProperty, value); }
        }
        #endregion

        #region IsAutoTextPath
        public static readonly DependencyProperty IsAutoTextPathProperty =
           DependencyProperty.RegisterAttached(
           "IsAutoTextPath",
           typeof(bool),
           typeof(PathTextBox),
           new PropertyMetadata(defaultValue: true));

        public bool IsAutoTextPath
        {
            get { return (bool)GetValue(IsAutoTextPathProperty); }
            set { SetValue(IsAutoTextPathProperty, value); }
        }
        #endregion

        public PathTextBox()
        {
            DefaultStyleKey = typeof(PathTextBox);
        }

        private Rectangle? RecogRec;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            RecogRec = GetTemplateChild(RECOGNIZE_INPUT_RECTANGLE_NAME) as Rectangle;
            OnPathTypeChanged();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            e.Handled = true;

            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                Text = "";
            }
        }

        private void OnPathTypeChanged()
        {
            if (PathType != Type.Default && RecogRec != null)
            {
                RecogRec.InputBindings.Clear();

                MouseBinding onRecCmdMouseBinding = new MouseBinding();
                onRecCmdMouseBinding.MouseAction = MouseAction.LeftClick;
                onRecCmdMouseBinding.Command = new BaseDotNetCommandImpl((s) =>
                {
                    var isShouldContinue = IsShouldOpenFileChooser?.Invoke() ?? true;

                    if (isShouldContinue)
                    {
                        if (PathType == Type.Folder)
                        {
                            var path = HoneyboardReleaseService
                                .Current
                                .ServiceManager?
                                .App.OpenFolderChooserDialogWindow();
                            if (!string.IsNullOrEmpty(path))
                            {
                                if (IsAutoTextPath)
                                {
                                    Text = path;
                                }
                                PathSelected?.Execute(null);
                            }
                        }
                        else if (PathType == Type.File)
                        {
                            var path = HoneyboardReleaseService
                                .Current
                                .ServiceManager?
                                .App
                                .OpenFileChooserDialogWindow(DialogBoxTitle ?? "", FileBoxFilter ?? "");
                            if (!string.IsNullOrEmpty(path))
                            {
                                if (IsAutoTextPath)
                                {
                                    Text = path;
                                }
                                PathSelected?.Execute(path);
                            }
                        }
                    }
                    
                });
                RecogRec.InputBindings.Add(onRecCmdMouseBinding);
            }
            else if (PathType == Type.Default)
            {
                RecogRec?.InputBindings.Clear();
            }
        }
    }
}
