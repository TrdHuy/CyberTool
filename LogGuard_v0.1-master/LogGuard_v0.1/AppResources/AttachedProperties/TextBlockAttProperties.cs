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
        public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.RegisterAttached(
               "BackgroundColor",
               typeof(Brush),
               typeof(TextBlockAttProperties),
               new PropertyMetadata(Brushes.Red));

        public static readonly DependencyProperty ForegroundColorProperty = DependencyProperty.RegisterAttached(
                 "ForegroundColor",
                 typeof(Brush),
                 typeof(TextBlockAttProperties),
                 new PropertyMetadata(Brushes.Yellow));

        public static Brush GetBackgroundColor(UIElement obj)
        {
            return (Brush)obj.GetValue(BackgroundColorProperty);
        }

        public static void SetBackgroundColor(UIElement obj, Brush value)
        {
            obj.SetValue(BackgroundColorProperty, value);
        }

        public static Brush GetForegroundColor(UIElement obj)
        {
            return (Brush)obj.GetValue(ForegroundColorProperty);
        }

        public static void SetForegroundColor(UIElement obj, Brush value)
        {
            obj.SetValue(ForegroundColorProperty, value);
        }

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


        private static void OnTextBlockHighlightPropertyChanged(TextBlock textBlock)
        {
            if (textBlock == null) return;
            var highlightSource = GetHighlightSource(textBlock);
            if (highlightSource == null || highlightSource.Count() == 0)
            {
                return;
            }

            textBlock.Inlines.Clear();
            Brush backgroundColor = GetBackgroundColor(textBlock);
            Brush foregroundColor = GetForegroundColor(textBlock);

            int startDuetIdx = 0;
            var rawText = highlightSource.ElementAt(0).RawWord;

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
