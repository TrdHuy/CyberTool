using honeyboard_release_service.views.elements.commit_data_grid.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace honeyboard_release_service.views.elements.commit_data_grid.prop
{
    internal class CommitDataGridTextBlockAttProperties : UIElement
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
                 typeof(CommitDataGridTextBlockAttProperties),
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
               typeof(CommitDataGridTextBlockAttProperties),
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
                 typeof(CommitDataGridTextBlockAttProperties),
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
            typeof(IEnumerable<IMatchedWord>),
            typeof(CommitDataGridTextBlockAttProperties),
            new FrameworkPropertyMetadata(defaultValue: default(IEnumerable<IMatchedWord>),
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

        public static IEnumerable<IMatchedWord> GetHighlightSource(UIElement target) =>
            (IEnumerable<IMatchedWord>)target.GetValue(HighlightSourceProperty);
        public static void SetHighlightSource(UIElement target, IEnumerable<IMatchedWord> value) =>
            target.SetValue(HighlightSourceProperty, value);
        #endregion

        private static async void OnTextBlockHighlightPropertyChanged(TextBlock textBlock)
        {
            if (textBlock == null) return;
            var highlightSource = GetHighlightSource(textBlock);

            if (highlightSource == null || highlightSource.Count() == 0)
            {
                var rawText = textBlock.GetValue(RawTextProperty).ToString();
                textBlock.Inlines.Clear();
                textBlock.Inlines.AddRange(new Inline[] {
                        new Run(rawText)
                        });
                return;
            }

            await Task.Delay(1);

            lock (textBlock)
            {
                textBlock.Inlines.Clear();
                Brush backgroundColor = GetBackgroundColor(textBlock);
                Brush foregroundColor = GetForegroundColor(textBlock);
                var rawText = highlightSource.ElementAt(0).RawWord;

                int startDuetIdx = 0;

                foreach (var highlightable in highlightSource)
                {

                    textBlock.Inlines.AddRange(new Inline[] {
                        new Run(rawText.Substring(startDuetIdx, highlightable.StartIndex - startDuetIdx))
                        });
                    textBlock.Inlines.AddRange(new Inline[] {
                        new Run(rawText.Substring(highlightable.StartIndex, highlightable.SearchWord.Length))
                        {
                            Background = backgroundColor,
                            Foreground = foregroundColor,
                            FontWeight = FontWeights.Bold
                        }
                        });

                    startDuetIdx = highlightable.StartIndex + highlightable.SearchWord.Length;
                }

                textBlock.Inlines.AddRange(new Inline[] {
                        new Run(rawText.Substring(startDuetIdx))
                        });
            }
        }
    }

}
