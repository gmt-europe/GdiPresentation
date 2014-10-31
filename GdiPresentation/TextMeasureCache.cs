using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GdiPresentation
{
    internal class TextMeasureCache
    {
        public WordCache[][] Words { get; private set; }

        public Size DesiredSize { get; private set; }

        public void Measure(TextBlock textBlock)
        {
            if (textBlock == null)
                throw new ArgumentNullException("textBlock");

            if (Words == null || Words.Length != textBlock.Runs.Count)
                Words = new WordCache[textBlock.Runs.Count][];

            for (int i = 0; i < textBlock.Runs.Count; i++)
            {
                Words[i] = BuildRun(textBlock.Runs[i]);
            }
        }

        private WordCache[] BuildRun(Run run)
        {
            if (run.WordCache == null)
            {
                var words = new List<WordCache>();

                var font = run.GetFont();

                run.EllipsisSize = System.Windows.Forms.TextRenderer.MeasureText(
                    "...",
                    font,
                    new System.Drawing.Size(int.MaxValue, int.MaxValue),
                    WordCache.FormatFlags
                ).Width;

                if (String.IsNullOrEmpty(run.Text))
                {
                    words.Add(new WordCache(run, String.Empty, font));
                }
                else
                {
                    bool space = false;
                    int offset = 0;

                    for (int i = 0; i < run.Text.Length; i++)
                    {
                        char c = run.Text[i];

                        if (c == '\r')
                            continue;

                        if (c == '\n')
                        {
                            BuildWord(words, run, font, ref offset, i);

                            words.Add(new WordCache(run, "\n", font));
                        }
                        else
                        {
                            bool charSpace = Char.IsWhiteSpace(c);

                            if (space != charSpace)
                                BuildWord(words, run, font, ref offset, i);

                            space = charSpace;
                        }
                    }

                    BuildWord(words, run, font, ref offset, run.Text.Length);
                }

                run.WordCache = words.ToArray();
            }

            return run.WordCache;
        }

        private void BuildWord(List<WordCache> words, Run run, Font font, ref int start, int end)
        {
            if (start != end)
                words.Add(new WordCache(run, run.Text.Substring(start, end - start), font));

            start = end;
        }

        public void Layout(TextBlock textBlock, Size desiredSize, bool final)
        {
            var location = new Point(0, 0);
            var finalSize = new Size();
            int maxLineHeight = 0;
            int lastLineIndex = -1;
            int lastIndex = -1;

            for (int i = 0; i < Words.Length; i++)
            {
                var words = Words[i];

                for (int j = 0; j < words.Length; j++)
                {
                    words[j].Clip = WordClip.None;

                    maxLineHeight = Math.Max(maxLineHeight, words[j].Size.Height);

                    if (words[j].IsNewline)
                    {
                        words[j].Location = location;
                        words[j].IsLineBreakAfter = true;
                        location = new Point(0, location.Y + maxLineHeight);
                        maxLineHeight = 0;
                    }
                    else
                    {
                        words[j].IsLineBreakAfter = false;

                        int x = location.X + words[j].Size.Width;

                        if (
                            textBlock.Wrap &&
                            !words[j].IsSpace &&
                            x > desiredSize.Width
                        ) {
                            words[j].Location = new Point(0, location.Y + maxLineHeight);
                            location = new Point(words[j].Size.Width, location.Y + maxLineHeight);
                            finalSize.Height = location.Y + maxLineHeight;
                            maxLineHeight = 0;

                            if (lastLineIndex != -1)
                                UpdateLineBreakAfter(lastLineIndex, lastIndex);
                        }
                        else
                        {
                            words[j].Location = location;
                            location = new Point(x, location.Y);
                            finalSize.Height = location.Y + maxLineHeight;
                            finalSize.Width = Math.Max(finalSize.Width, x);
                        }
                    }

                    lastLineIndex = i;
                    lastIndex = j;
                }
            }

            if (Words.Length > 0)
                UpdateLineBreakAfter(Words.Length - 1, Words[Words.Length - 1].Length - 1);

            if (textBlock.TextTrimming != TextTrimming.None && final)
                ApplyClipping(desiredSize, textBlock.TextTrimming);

            DesiredSize = finalSize;
        }

        private void ApplyClipping(Size desiredSize, TextTrimming trimming)
        {
            bool isLast = true;
            bool trimNext = false;
            int lastSpaceSize = 0;

            for (int i = Words.Length - 1; i >= 0; i--)
            {
                var words = Words[i];

                for (int j = words.Length - 1; j >= 0; j--, isLast = false)
                {
                    var bounds = words[j].Bounds;

                    if (bounds.Bottom > desiredSize.Height)
                    {
                        // Everything that falls of the bottom can be discarded.

                        words[j].Clip = WordClip.Clip;
                        continue;
                    }

                    if (words[j].IsSpace)
                    {
                        lastSpaceSize = words[j].Size.Width;
                        continue;
                    }

                    if (words[j].IsLineBreakAfter)
                        lastSpaceSize = 0;

                    // At this point, at least the height would allow us to draw
                    // the text. See whether including the ellipsis would make
                    // this word too long.

                    int right = bounds.Right;

                    if (!isLast)
                        right += words[j].Run.EllipsisSize + lastSpaceSize;

                    if (right > desiredSize.Width)
                    {
                        if (trimming == TextTrimming.CharacterEllipsis)
                        {
                            // When we're applying character ellipsis, rendering
                            // this word as end ellipsis would give the correct result.

                            words[j].Clip = WordClip.Trim;
                        }
                        else if (i == 0 && j == 0)
                        {
                            // Otherwise, if this is the first word,
                            // just rendering an ellipsis gives the correct
                            // result.

                            words[j].Clip = WordClip.ReplaceWithEllipsis;
                            return;
                        }
                        else
                        {
                            // Otherwise, we need to clip this word and the
                            // next word needs to get the ellipsis appended
                            // as text when rendered.

                            words[j].Clip = WordClip.Clip;
                            trimNext = true;
                        }
                    }
                    else
                    {
                        // Append the ellipsis when rendering this word.
                        // Note that trimNext does not mean that the immediately
                        // next word will be trimmed. Instead, we search on until
                        // we find a word that just fits.

                        if (trimNext)
                            words[j].Clip = WordClip.AppendEllipsis;
                        return;
                    }
                }
            }
        }

        private void UpdateLineBreakAfter(int lastLineIndex, int lastIndex)
        {
            for (int i = lastLineIndex; i >= 0; i--)
            {
                var words = Words[i];

                for (int j = (lastIndex == -1 ? words.Length - 1 : lastIndex); j >= 0; j--)
                {
                    if (!words[j].IsSpace || words[j].IsLineBreakAfter)
                        return;

                    words[j].IsLineBreakAfter = true;
                }
            }
        }
    }
}
