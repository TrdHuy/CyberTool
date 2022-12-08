using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace LogGuard_v0._1.AppResources.AttachedProperties
{
    public interface IHighlightable
    {
        string SearchWord { get; }
        int StartIndex { get; }
        int WordLength { get; }
        string RawWord { get; }
    }

    public class TextBlockAttProperties : UIElement
    {

        #region RawText
        /// <summary>
        /// Property này dùng để lưu giá trị của string mà text bolck hiển thị
        /// Mặc dù đã có property Text để lưu trước đây
        /// Nhưng trường hợp khi Textblock kết hợp vs Virtualizing panel
        /// giá trị của Text sẽ ko được chính xác nữa
        /// </summary>
        public static readonly DependencyProperty RawTextProperty = DependencyProperty.RegisterAttached(
                 "RawText",
                 typeof(string),
                 typeof(TextBlockAttProperties),
                 new PropertyMetadata(default(string)));

        public static string GetRawText(UIElement obj)
        {
            return (string)obj.GetValue(RawTextProperty);
        }

        public static void SetRawText(UIElement obj, string value)
        {
            obj.SetValue(RawTextProperty, value);
        }
        #endregion

        #region Background
        public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.RegisterAttached(
               "BackgroundColor",
               typeof(Brush),
               typeof(TextBlockAttProperties),
               new PropertyMetadata(Brushes.Red));

        public static Brush GetBackgroundColor(UIElement obj)
        {
            return (Brush)obj.GetValue(BackgroundColorProperty);
        }

        public static void SetBackgroundColor(UIElement obj, Brush value)
        {
            obj.SetValue(BackgroundColorProperty, value);
        }
        #endregion

        #region Foreground
        public static readonly DependencyProperty ForegroundColorProperty = DependencyProperty.RegisterAttached(
                 "ForegroundColor",
                 typeof(Brush),
                 typeof(TextBlockAttProperties),
                 new PropertyMetadata(Brushes.Yellow));

      
        public static Brush GetForegroundColor(UIElement obj)
        {
            return (Brush)obj.GetValue(ForegroundColorProperty);
        }

        public static void SetForegroundColor(UIElement obj, Brush value)
        {
            obj.SetValue(ForegroundColorProperty, value);
        }

        #endregion

        #region HighlightSource
        public static readonly DependencyProperty HighlightSourceProperty =
            DependencyProperty.RegisterAttached(
            "HighlightSource",
            typeof(IEnumerable<IHighlightable>),
            typeof(TextBlockAttProperties),
            new FrameworkPropertyMetadata(defaultValue: default(IEnumerable<IHighlightable>),
                flags: FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnHighlightSourceChangeCallback)));

        private static void OnHighlightSourceChangeCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = d as TextBlock;
            if (textBlock == null)
            {
                return;
            }
            OnTextBlockHighlightPropertyChanged(textBlock);

        }

        public static IEnumerable<IHighlightable> GetHighlightSource(UIElement target) =>
            (IEnumerable<IHighlightable>)target.GetValue(HighlightSourceProperty);
        public static void SetHighlightSource(UIElement target, IEnumerable<IHighlightable> value) =>
            target.SetValue(HighlightSourceProperty, value);


        #endregion

        #region ExtraHighlightSource
        public static readonly DependencyProperty ExtraHighlightSourceProperty =
            DependencyProperty.RegisterAttached(
            "ExtraHighlightSource",
            typeof(IEnumerable<IHighlightable>),
            typeof(TextBlockAttProperties),
            new FrameworkPropertyMetadata(defaultValue: default(IEnumerable<IHighlightable>),
                flags: FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnExtraHighlightSourceChangeCallback)));

        private static void OnExtraHighlightSourceChangeCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = d as TextBlock;
            if (textBlock == null)
            {
                return;
            }
            OnTextBlockHighlightPropertyChanged(textBlock);

        }

        public static IEnumerable<IHighlightable> GetExtraHighlightSource(UIElement target) =>
            (IEnumerable<IHighlightable>)target.GetValue(ExtraHighlightSourceProperty);
        public static void SetExtraHighlightSource(UIElement target, IEnumerable<IHighlightable> value) =>
            target.SetValue(ExtraHighlightSourceProperty, value);


        #endregion
        private static void OnTextBlockHighlightPropertyChanged(TextBlock textBlock)
        {
            if (textBlock == null) return;
            var highlightSource = GetHighlightSource(textBlock);
            var extraHighlightSource = GetExtraHighlightSource(textBlock);
            var rawText = GetRawText(textBlock);

            if ((extraHighlightSource == null || extraHighlightSource.Count() == 0)
                && (highlightSource == null || highlightSource.Count() == 0))
            {
                textBlock.Inlines.Clear();
                textBlock.Inlines.AddRange(new Inline[] {
                        new Run(rawText)
                        });
                return;
            }
            else if(extraHighlightSource != null && extraHighlightSource.Count() > 0) 
            {
                highlightSource = extraHighlightSource;
            }
            rawText = highlightSource.ElementAt(0).RawWord;

            textBlock.Inlines.Clear();
            Brush backgroundColor = GetBackgroundColor(textBlock);
            Brush foregroundColor = GetForegroundColor(textBlock);

            int startDuetIdx = 0;
           
            foreach (var highlightable in highlightSource)
            {

                textBlock.Inlines.AddRange(new Inline[] {
                        new Run(rawText.Substring(startDuetIdx, highlightable.StartIndex - startDuetIdx))
                        });
                textBlock.Inlines.AddRange(new Inline[] {
                        new Run(rawText.Substring(highlightable.StartIndex, highlightable.WordLength))
                        {
                            Background = backgroundColor,
                            Foreground = foregroundColor,
                            FontWeight = FontWeights.Bold
                        }
                        });

                startDuetIdx = highlightable.StartIndex + highlightable.WordLength;
            }

            textBlock.Inlines.AddRange(new Inline[] {
                        new Run(rawText.Substring(startDuetIdx))
                        });

        }
    }
}
