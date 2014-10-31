using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GdiPresentation.Demo
{
    [DisplayName("Text")]
    internal class TextDemo : ElementDemo
    {
        public override Element BuildContent()
        {
            var trimming = new Grid();

            trimming.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(GridUnitType.Star, 1)));

            AppendLine(trimming, TextTrimming.CharacterEllipsis, "Character ellipsis: aaaaaaaaa aaaaaaaaa aaaaaaaaa aaaaaaaaa");
            AppendLine(trimming, TextTrimming.CharacterEllipsis, "Character ellipsis: a a a a a a a a a a a a a a a a a a a a a a a a a a a a a a a a");
            AppendLine(trimming, TextTrimming.WordEllipsis, "Word ellipsis: aaaaaaaaa aaaaaaaaa aaaaaaaaa aaaaaaaaa");
            AppendLine(trimming, TextTrimming.WordEllipsis, "Word ellipsis: a a a a a a a a a a a a a a a a a a a a a a a a a a a a a a a a");
            AppendLine(trimming, new TextBlock("This is some underlined text.\nAnd this is the second line.") { FontStyle = FontStyle.Underline });
            AppendLine(trimming, new TextBlock("This is some bold text.\nAnd this is the second line.") { FontStyle = FontStyle.Bold });
            AppendLine(trimming, new TextBlock("This is some italic text.\nAnd this is the second line.") { FontStyle = FontStyle.Italic });
            AppendLine(trimming, new TextBlock("This is some strikeout text.\nAnd this is the second line.") { FontStyle = FontStyle.Strikeout });
            AppendLine(trimming, new TextBlock("This is some text with all options.\nAnd this is the second line.") { FontStyle = FontStyle.Bold | FontStyle.Italic | FontStyle.Strikeout | FontStyle.Underline });

            return new Border
            {
                Background = Brush.White,
                Content = new Border
                {
                    BorderThickness = new Thickness(5),
                    BorderBrush = Brush.Red,
                    Margin = new Thickness(80),
                    Content = trimming
                }
            };
        }

        private void AppendLine(Grid grid, TextTrimming trimming, params string[] text)
        {
            var textBlock = new TextBlock
            {
                Wrap = false,
                TextTrimming = trimming
            };

            foreach (string line in text)
            {
                textBlock.Runs.Add(new Run(line));
            }

            AppendLine(grid, textBlock);
        }

        private static void AppendLine(Grid grid, TextBlock textBlock)
        {
            textBlock.Margin = new Thickness(5);

            grid.RowDefinitions.Add(new RowDefinition(new GridLength(GridUnitType.Auto)));

            grid.Children.Add(textBlock);

            Grid.SetRow(textBlock, grid.RowDefinitions.Count - 1);
        }

        public override System.Windows.Forms.ScrollBars AllowedScrollBars
        {
            get { return System.Windows.Forms.ScrollBars.None; }
        }
    }
}
