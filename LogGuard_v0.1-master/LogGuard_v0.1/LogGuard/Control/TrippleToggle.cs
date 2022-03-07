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

namespace LogGuard_v0._1.LogGuard.Control
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
    public class TrippleToggle : System.Windows.Controls.Control
    {
        private const string NormalDotName = "DotNormal";
        private const string OffDotName = "DotOff";
        private const string OnDotName = "DotOn";


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
                new PropertyMetadata(default(DotStatus), new PropertyChangedCallback(DotStatusChagnedCallback)));

        private static void DotStatusChagnedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TrippleToggle tt = d as TrippleToggle;
            tt.OnApplyNewStatus((DotStatus)e.NewValue);
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

        private Ellipse NormalDot;
        private Ellipse OffDot;
        private Ellipse OnDot;
        private Brush NormalDotCacheBrush;
        private Brush OnDotCacheBrush;
        private Brush OffDotCacheBrush;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            NormalDot = GetTemplateChild(NormalDotName) as Ellipse;
            OffDot = GetTemplateChild(OffDotName) as Ellipse;
            OnDot = GetTemplateChild(OnDotName) as Ellipse;

            NormalDotCacheBrush = NormalDot.Fill;
            OnDotCacheBrush = OnDot.Fill;
            OffDotCacheBrush = OffDot.Fill;

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

            OnApplyNewStatus(DotStatus.DotNormal);
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


        private void OnApplyNewStatus(DotStatus newValue)
        {
            if (!IsInitialized) return;
            Brush invisibleBrush = new SolidColorBrush(Colors.Transparent);
            NormalDot.Fill = invisibleBrush;
            OnDot.Fill = invisibleBrush;
            OffDot.Fill = invisibleBrush;

            switch (newValue)
            {
                case DotStatus.DotNormal:
                    NormalDot.Fill = NormalDotCacheBrush;
                    break;
                case DotStatus.DotOff:
                    OffDot.Fill = OffDotCacheBrush;
                    break;
                case DotStatus.DotOn:
                    OnDot.Fill = OnDotCacheBrush;
                    break;
                default:
                    NormalDot.Fill = NormalDotCacheBrush;
                    break;
            }
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
