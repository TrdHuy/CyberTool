using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace log_guard.views.others.tripple_toggle
{
    public enum DotStatus
    {
        DotOff = 0,
        DotNormal = 1,
        DotOn = 2,
    }
    [TemplatePart(Name = TrippleToggle.NormalDotName, Type = typeof(Ellipse))]
    [TemplatePart(Name = TrippleToggle.OffDotName, Type = typeof(Ellipse))]
    [TemplatePart(Name = TrippleToggle.OnDotName, Type = typeof(Ellipse))]
    [TemplatePart(Name = TrippleToggle.FeedbackDotName, Type = typeof(Ellipse))]
    [TemplatePart(Name = TrippleToggle.FeedbackDotColorName, Type = typeof(Brush))]
    public class TrippleToggle : Control
    {
        private const string NormalDotName = "DotNormal";
        private const string OffDotName = "DotOff";
        private const string OnDotName = "DotOn";
        private const string FeedbackDotName = "FeedBackDot";
        private const string FeedbackDotColorName = "FeedbackDotColor";


        public TrippleToggle()
        {
            this.DefaultStyleKey = typeof(TrippleToggle);
        }

        #region NormalDotCommand
        public static readonly DependencyProperty NormalDotCommandProperty =
            DependencyProperty.Register("NormalDotCommand",
                typeof(ICommand),
                typeof(TrippleToggle),
                new PropertyMetadata(default(ICommand)));

        public ICommand NormalDotCommand
        {
            get
            {
                return (ICommand)GetValue(NormalDotCommandProperty);
            }
            set
            {
                SetValue(NormalDotCommandProperty, value);
            }
        }
        #endregion

        #region OnDotCommand
        public static readonly DependencyProperty OnDotCommandProperty =
            DependencyProperty.Register("OnDotCommand",
                typeof(ICommand),
                typeof(TrippleToggle),
                new PropertyMetadata(default(ICommand)));

        public ICommand OnDotCommand
        {
            get
            {
                return (ICommand)GetValue(OnDotCommandProperty);
            }
            set
            {
                SetValue(OnDotCommandProperty, value);
            }
        }
        #endregion

        #region OffDotCommand
        public static readonly DependencyProperty OffDotCommandProperty =
            DependencyProperty.Register("OffDotCommand",
                typeof(ICommand),
                typeof(TrippleToggle),
                new PropertyMetadata(default(ICommand)));

        public ICommand OffDotCommand
        {
            get
            {
                return (ICommand)GetValue(OffDotCommandProperty);
            }
            set
            {
                SetValue(OffDotCommandProperty, value);
            }
        }
        #endregion

        #region Status
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status",
                typeof(DotStatus),
                typeof(TrippleToggle),
                new PropertyMetadata(DotStatus.DotNormal, new PropertyChangedCallback(DotStatusChagnedCallback)));

        private static void DotStatusChagnedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TrippleToggle tt = d as TrippleToggle;
            tt.OnApplyNewStatus();
        }

        public DotStatus Status
        {
            get
            {
                return (DotStatus)GetValue(StatusProperty);
            }
            set
            {
                SetValue(StatusProperty, value);
            }
        }
        #endregion

        #region DotOnFill
        public static readonly DependencyProperty DotOnFillProperty =
            DependencyProperty.Register(
                "DotOnFill",
                typeof(Color),
                typeof(TrippleToggle),
                new UIPropertyMetadata(Colors.Green));

        public Color DotOnFill
        {
            get { return (Color)GetValue(DotOnFillProperty); }
            set { SetValue(DotOnFillProperty, value); }
        }
        #endregion

        #region DotOffFill
        public static readonly DependencyProperty DotOffFillProperty =
            DependencyProperty.Register(
                "DotOffFill",
                typeof(Color),
                typeof(TrippleToggle),
                new UIPropertyMetadata(Colors.Red));

        public Color DotOffFill
        {
            get { return (Color)GetValue(DotOffFillProperty); }
            set { SetValue(DotOffFillProperty, value); }
        }
        #endregion

        #region DotNormalFill
        public static readonly DependencyProperty DotNormalFillProperty =
            DependencyProperty.Register(
                "DotNormalFill",
                typeof(Color),
                typeof(TrippleToggle),
                new UIPropertyMetadata(Colors.White));

        public Color DotNormalFill
        {
            get { return (Color)GetValue(DotNormalFillProperty); }
            set { SetValue(DotNormalFillProperty, value); }
        }
        #endregion

        private Ellipse NormalDot;
        private Ellipse OffDot;
        private Ellipse OnDot;
        private Ellipse FeedbackDot;
        private SolidColorBrush FeedbackDotBrush;

        private Grid MainGrid;
        private Thickness CurrentFeedbackMargin;
        private Color CurrentFeedbackColor;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            MainGrid = GetTemplateChild("MainGrid") as Grid;
            NormalDot = GetTemplateChild(NormalDotName) as Ellipse;
            OffDot = GetTemplateChild(OffDotName) as Ellipse;
            OnDot = GetTemplateChild(OnDotName) as Ellipse;
            FeedbackDot = GetTemplateChild(FeedbackDotName) as Ellipse;
            FeedbackDotBrush = GetTemplateChild(FeedbackDotColorName) as SolidColorBrush;

            MouseBinding NormalDotCmdMouseBinding = new MouseBinding();
            NormalDotCmdMouseBinding.MouseAction = MouseAction.LeftClick;
            NormalDotCmdMouseBinding.Command = new TrippleToggleCommand(OnDotNormalClick);

            MouseBinding OffDotCmdMouseBinding = new MouseBinding();
            OffDotCmdMouseBinding.MouseAction = MouseAction.LeftClick;
            OffDotCmdMouseBinding.Command = new TrippleToggleCommand(OnDotOffClick);

            MouseBinding OnDotCmdMouseBinding = new MouseBinding();
            OnDotCmdMouseBinding.MouseAction = MouseAction.LeftClick;
            OnDotCmdMouseBinding.Command = new TrippleToggleCommand(OnDotOnClick);


            NormalDot.InputBindings.Add(NormalDotCmdMouseBinding);
            OffDot.InputBindings.Add(OffDotCmdMouseBinding);
            OnDot.InputBindings.Add(OnDotCmdMouseBinding);


            CurrentFeedbackColor = 
                Status == DotStatus.DotOn ? DotOnFill 
                : Status == DotStatus.DotOff ? DotOffFill 
                : DotNormalFill;

            CurrentFeedbackMargin = new Thickness(0d, 0d
                , Status == DotStatus.DotOn ? -24d : Status == DotStatus.DotOff ? 24d : 0d
                , 0d);
            FeedbackDot.Margin = CurrentFeedbackMargin;
            FeedbackDotBrush.Color = CurrentFeedbackColor;
        }

        private void OnDotNormalClick(object paramater)
        {
            Status = DotStatus.DotNormal;
            NormalDotCommand?.Execute(paramater);
        }

        private void OnDotOnClick(object paramater)
        {
            Status = DotStatus.DotOn;
            OnDotCommand?.Execute(paramater);
        }

        private void OnDotOffClick(object paramater)
        {
            Status = DotStatus.DotOff;
            OffDotCommand?.Execute(paramater);
        }

        private void OnApplyNewStatus()
        {
            if (!IsInitialized)
            {
                return;
            }
            var anm = CreateFeedbackAnimation();
            anm.Begin(MainGrid);
        }


        private Storyboard CreateFeedbackAnimation()
        {
            Storyboard feedbackSB = new Storyboard();
            long anmDuration = 90;

            ThicknessAnimation thicknessAnimation = new ThicknessAnimation();
            thicknessAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(anmDuration));
            thicknessAnimation.From = CurrentFeedbackMargin;
            thicknessAnimation.To = new Thickness(0d, 0d
                , Status == DotStatus.DotOn ? -24d : Status == DotStatus.DotOff ? 24d : 0d
                , 0d);
            Storyboard.SetTargetName(thicknessAnimation, FeedbackDot.Name);
            Storyboard.SetTargetProperty(thicknessAnimation, new PropertyPath(Ellipse.MarginProperty));

            ColorAnimation colorAnimation = new ColorAnimation();
            colorAnimation.From = CurrentFeedbackColor;
            colorAnimation.To = Status == DotStatus.DotOn ? DotOnFill : Status == DotStatus.DotOff ? DotOffFill : DotNormalFill;
            colorAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(anmDuration));
            Storyboard.SetTargetName(colorAnimation, FeedbackDotColorName);
            Storyboard.SetTargetProperty(colorAnimation,
                new PropertyPath(SolidColorBrush.ColorProperty));


            CurrentFeedbackMargin = (Thickness)thicknessAnimation.To;
            CurrentFeedbackColor = (Color)colorAnimation.To;

            feedbackSB.Children.Add(thicknessAnimation);
            feedbackSB.Children.Add(colorAnimation);

            return feedbackSB;
        }

    }

    class TrippleToggleCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<object> actionObj;

        public TrippleToggleCommand(Action<object> act)
        {
            actionObj = act;
        }

        public bool CanExecute(object obj)
        {
            return true;
        }

        public void Execute(object obj)
        {
            actionObj?.Invoke(obj);
        }
    }
}
