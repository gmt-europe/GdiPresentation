// Ported from Moonlight; original file at https://github.com/mono/moon/blob/master/src/grid.cpp.

// Moonlight license:

// Unless explicitly stated, this code is licensed under the
// terms of the GNU LGPL 2 license only (no "later versions").
//
// In addition to the GNU LGPL, this code is available for
// relicensing for non-LGPL use, contact Novell for details
// (mono@novell.com).
//
// We consider non-LGPL use instances where you use this on an
// embedded system where the end user is not able to upgrade the
// Moonlight installation or distribution that is part of your
// product (Section 6 and 7), you would have to obtain a
// commercial license from Novell (consider software burned into
// a ROM, systems where end users would not be able to upgrade,
// an embedded console, a game console that imposes limitations
// on the distribution and access to the code, a phone platform
// that prevents end users from upgrading Moonlight).
//
// This code might contain code that optionally links to LGPL and
// GPL code, in those cases, if the library is built with those
// bits the code is covered under those licenses.

using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public class Grid : ContainerElement
    {
        private static readonly NamedObject ColumnProperty = new NamedObject("Grid.Column");
        private static readonly NamedObject ColumnSpanProperty = new NamedObject("Grid.ColumnSpan");
        private static readonly NamedObject RowProperty = new NamedObject("Grid.Row");
        private static readonly NamedObject RowSpanProperty = new NamedObject("Grid.RowSpan");

        private ColumnDefinitionCollection _columnDefinitions;
        private RowDefinitionCollection _rowDefinitions;
        private int row_matrix_dim;
        private int col_matrix_dim;
        private Segment[,] row_matrix;
        private Segment[,] col_matrix;

        public RowDefinitionCollection RowDefinitions
        {
            get
            {
                if (_rowDefinitions == null)
                {
                    _rowDefinitions = new RowDefinitionCollection(this);
                    _rowDefinitions.CollectionChanged += _definitions_CollectionChanged;
                }

                return _rowDefinitions;
            }
        }

        public ColumnDefinitionCollection ColumnDefinitions
        {
            get
            {
                if (_columnDefinitions == null)
                {
                    _columnDefinitions = new ColumnDefinitionCollection(this);
                    _columnDefinitions.CollectionChanged += _definitions_CollectionChanged;
                }

                return _columnDefinitions;
            }
        }

        private static int Clamp(int val, int min, int max)
        {
            if (val < min)
                return min;
            else if (val > max)
                return max;
            return val;
        }

        void _definitions_CollectionChanged(object sender, EventArgs e)
        {
            InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size totalSize = availableSize;

            ColumnDefinitionCollection columns = _columnDefinitions;
            RowDefinitionCollection rows = _rowDefinitions;

            int col_count = columns != null ? columns.Count : 0;
            int row_count = rows != null ? rows.Count : 0;
            Size total_stars = new Size(0, 0);

            bool empty_rows = row_count == 0;
            bool empty_cols = col_count == 0;
            bool hasChildren = Children.Count > 0;

            if (empty_rows) row_count = 1;
            if (empty_cols) col_count = 1;

            CreateMatrices(row_count, col_count);

            if (empty_rows)
            {
                row_matrix[0,0] = new Segment(0, 0, int.MaxValue, GridUnitType.Star);
                row_matrix[0,0].stars = 1;
                total_stars.Height += 1;
            }
            else
            {
                for (int i = 0; i < row_count; i++)
                {
                    RowDefinition rowdef = rows[i];
                    GridLength height = rowdef.Height;

                    rowdef.ActualHeight = int.MaxValue;
                    row_matrix[i,i] = new Segment(0, rowdef.MinHeight, rowdef.MaxHeight, height.GridUnitType);

                    if (height.GridUnitType == GridUnitType.Pixel)
                    {
                        row_matrix[i,i].offered_size = Clamp(height.Value, row_matrix[i,i].min, row_matrix[i,i].max);
                        row_matrix[i,i].desired_size = row_matrix[i,i].offered_size;
                        rowdef.ActualHeight = row_matrix[i,i].offered_size;
                    }
                    else if (height.GridUnitType == GridUnitType.Star)
                    {
                        row_matrix[i,i].stars = height.Value;
                        total_stars.Height += height.Value;
                    }
                    else if (height.GridUnitType == GridUnitType.Auto)
                    {
                        row_matrix[i,i].offered_size = Clamp(0, row_matrix[i,i].min, row_matrix[i,i].max);
                        row_matrix[i,i].desired_size = row_matrix[i,i].offered_size;
                    }
                }
            }

            if (empty_cols)
            {
                col_matrix[0,0] = new Segment(0, 0, int.MaxValue, GridUnitType.Star);
                col_matrix[0,0].stars = 1;
                total_stars.Width += 1;
            }
            else
            {
                for (int i = 0; i < col_count; i++)
                {
                    ColumnDefinition coldef = columns[i];
                    GridLength width = coldef.Width;

                    coldef.ActualWidth = int.MaxValue;
                    col_matrix[i,i] = new Segment(0, coldef.MinWidth, coldef.MaxWidth, width.GridUnitType);

                    if (width.GridUnitType == GridUnitType.Pixel)
                    {
                        col_matrix[i,i].offered_size = Clamp(width.Value, col_matrix[i,i].min, col_matrix[i,i].max);
                        col_matrix[i,i].desired_size = col_matrix[i,i].offered_size;
                        coldef.ActualWidth = col_matrix[i,i].offered_size;
                    }
                    else if (width.GridUnitType == GridUnitType.Star)
                    {
                        col_matrix[i,i].stars = width.Value;
                        total_stars.Width += width.Value;
                    }
                    else if (width.GridUnitType == GridUnitType.Auto)
                    {
                        col_matrix[i,i].offered_size = Clamp(0, col_matrix[i,i].min, col_matrix[i,i].max);
                        col_matrix[i,i].desired_size = col_matrix[i,i].offered_size;
                    }
                }
            }

            LinkedList<GridNode> sizes = new LinkedList<GridNode>();
            GridNode node;
            var separator = sizes.AddLast(new GridNode(null, 0, 0, 0));

            // Pre-process the grid children so that we know what types of elements we have so
            // we can apply our special measuring rules.
            GridWalker grid_walker = new GridWalker(this, row_matrix, row_matrix_dim, col_matrix, col_matrix_dim);
            for (int i = 0; i < 6; i++)
            {
                // These bools tell us which grid element type we should be measuring. i.e.
                // 'star/auto' means we should measure elements with a star row and auto col
                bool auto_auto = i == 0;
                bool star_auto = i == 1;
                bool auto_star = i == 2;
                bool star_auto_again = i == 3;
                bool non_star = i == 4;
                bool remaining_star = i == 5;

                if (hasChildren)
                {
                    ExpandStarCols(totalSize);
                    ExpandStarRows(totalSize);
                }

                for (int j = 0, count = Children.Count; j < count; j++)
                {
                    var child = Children[j];

                    int col, row;
                    int colspan, rowspan;
                    Size child_size = new Size(0, 0);
                    bool star_col = false;
                    bool star_row = false;
                    bool auto_col = false;
                    bool auto_row = false;

                    col = Math.Min(GetColumn(child), col_count - 1);
                    row = Math.Min(GetRow(child), row_count - 1);
                    colspan = Math.Min(GetColumnSpan(child), col_count - col);
                    rowspan = Math.Min(GetRowSpan(child), row_count - row);

                    for (int r = row; r < row + rowspan; r++)
                    {
                        star_row |= row_matrix[r,r].type == GridUnitType.Star;
                        auto_row |= row_matrix[r,r].type == GridUnitType.Auto;
                    }
                    for (int c = col; c < col + colspan; c++)
                    {
                        star_col |= col_matrix[c,c].type == GridUnitType.Star;
                        auto_col |= col_matrix[c,c].type == GridUnitType.Auto;
                    }

                    // This series of if statements checks whether or not we should measure
                    // the current element and also if we need to override the sizes
                    // passed to the Measure call. 

                    // If the element has Auto rows and Auto columns and does not span Star
                    // rows/cols it should only be measured in the auto_auto phase.
                    // There are similar rules governing auto/star and star/auto elements.
                    // NOTE: star/auto elements are measured twice. The first time with
                    // an override for height, the second time without it.
                    if (auto_row && auto_col && !star_row && !star_col)
                    {
                        if (!auto_auto)
                            continue;
                        child_size.Width = int.MaxValue;
                        child_size.Height = int.MaxValue;
                    }
                    else if (star_row && auto_col && !star_col)
                    {
                        if (!(star_auto || star_auto_again))
                            continue;

                        if (star_auto && grid_walker.HasAutoStar())
                            child_size.Height = int.MaxValue;
                        child_size.Width = int.MaxValue;
                    }
                    else if (auto_row && star_col && !star_row)
                    {
                        if (!auto_star)
                            continue;

                        child_size.Height = int.MaxValue;
                    }
                    else if ((auto_row || auto_col) && !(star_row || star_col))
                    {
                        if (!non_star)
                            continue;
                        if (auto_row)
                            child_size.Height = int.MaxValue;
                        if (auto_col)
                            child_size.Width = int.MaxValue;
                    }
                    else if (!(star_row || star_col))
                    {
                        if (!non_star)
                            continue;
                    }
                    else
                    {
                        if (!remaining_star)
                            continue;
                    }

                    if (child_size.Height < int.MaxValue)
                    {
                        for (int r = row; r < row + rowspan; r++)
                        {
                            child_size.Height += row_matrix[r, r].offered_size;
                        }
                    }
                    if (child_size.Width < int.MaxValue)
                    {
                        for (int c = col; c < col + colspan; c++)
                        {
                            child_size.Width += col_matrix[c, c].offered_size;
                        }
                    }

                    child.Measure(child_size);
                    Size desired = child.DesiredSize;

                    // Elements distribute their height based on two rules:
                    // 1) Elements with rowspan/colspan == 1 distribute their height first
                    // 2) Everything else distributes in a LIFO manner.
                    // As such, add all UIElements with rowspan/colspan == 1 after the separator in
                    // the list and everything else before it. Then to process, just keep popping
                    // elements off the end of the list.
                    if (!star_auto)
                    {
                        node = new GridNode(row_matrix, row + rowspan - 1, row, desired.Height);
                        if (node.row == node.col)
                        {
                            if (separator.Next == null)
                                sizes.AddLast(node);
                            else
                                sizes.AddBefore(separator.Next, node);
                        }
                        else
                        {
                            sizes.AddBefore(separator, node);
                        }
                    }
                    node = new GridNode(col_matrix, col + colspan - 1, col, desired.Width);
                    if (node.row == node.col)
                    {
                        if (separator.Next == null)
                            sizes.AddLast(node);
                        else
                            sizes.AddBefore(separator.Next, node);
                    }
                    else
                    {
                        sizes.AddBefore(separator, node);
                    }
                }

                sizes.Remove(separator);

                LinkedListNode<GridNode> list_node;
                while ((list_node = sizes.Last) != null)
                {
                    node = list_node.Value;
                    node.matrix[node.row,node.col].desired_size = Math.Max(node.matrix[node.row,node.col].desired_size, node.size);
                    AllocateDesiredSize(row_count, col_count);
                    sizes.RemoveLast();
                }

                sizes.AddLast(separator);
            }

            // Once we have measured and distributed all sizes, we have to store
            // the results. Every time we want to expand the rows/cols, this will
            // be used as the baseline.
            SaveMeasureResults();

            sizes.Remove(separator);

            Size grid_size = new Size(0, 0);
            for (int c = 0; c < col_count; c++)
                grid_size.Width += col_matrix[c,c].desired_size;
            for (int r = 0; r < row_count; r++)
                grid_size.Height += row_matrix[r,r].desired_size;

            return grid_size;
        }

        private void ExpandStarRows(Size availableSize)
        {
            RowDefinitionCollection rows = _rowDefinitions;
            int row_count = rows != null ? rows.Count : 0;

            // When expanding star rows, we need to zero out their height before
            // calling AssignSize. AssignSize takes care of distributing the 
            // available size when there are Mins and Maxs applied.
            for (int i = 0; i < row_matrix_dim; i++)
            {
                if (row_matrix[i,i].type == GridUnitType.Star)
                    row_matrix[i,i].offered_size = 0;
                else
                    availableSize.Height = Math.Max(availableSize.Height - row_matrix[i,i].offered_size, 0);
            }

            availableSize.Height = AssignSize(row_matrix, 0, row_matrix_dim - 1, availableSize.Height, GridUnitType.Star, false);
            if (row_count > 0)
            {
                for (int i = 0; i < row_matrix_dim; i++)
                    if (row_matrix[i,i].type == GridUnitType.Star)
                        rows[i].ActualHeight = row_matrix[i,i].offered_size;
            }
        }

        private void ExpandStarCols(Size availableSize)
        {
            ColumnDefinitionCollection columns = _columnDefinitions;
            int columns_count = columns != null ? columns.Count : 0;

            for (int i = 0; i < col_matrix_dim; i++)
            {
                if (col_matrix[i,i].type == GridUnitType.Star)
                    col_matrix[i,i].offered_size = 0;
                else
                    availableSize.Width = Math.Max(availableSize.Width - col_matrix[i,i].offered_size, 0);
            }

            availableSize.Width = AssignSize(col_matrix, 0, col_matrix_dim - 1, availableSize.Width, GridUnitType.Star, false);

            if (columns_count > 0)
            {
                for (int i = 0; i < col_matrix_dim; i++)
                    if (col_matrix[i,i].type == GridUnitType.Star)
                        columns[i].ActualWidth = col_matrix[i,i].offered_size;
            }
        }

        private void AllocateDesiredSize(int row_count, int col_count)
        {
            // First allocate the heights of the RowDefinitions, then allocate
            // the widths of the ColumnDefinitions.
            for (int i = 0; i < 2; i++)
            {
                Segment[,] matrix = i == 0 ? row_matrix : col_matrix;
                int count = i == 0 ? row_count : col_count;

                for (int row = count - 1; row >= 0; row--)
                {
                    for (int col = row; col >= 0; col--)
                    {
                        bool spans_star = false;
                        for (int j = row; j >= col; j--)
                            spans_star |= matrix[j,j].type == GridUnitType.Star;

                        // This is the amount of pixels which must be available between the grid rows
                        // at index 'col' and 'row'. i.e. if 'row' == 0 and 'col' == 2, there must
                        // be at least 'matrix [row,col].size' pixels of height allocated between
                        // all the rows in the range col . row.
                        int current = matrix[row,col].desired_size;

                        // Count how many pixels have already been allocated between the grid rows
                        // in the range col . row. The amount of pixels allocated to each grid row/column
                        // is found on the diagonal of the matrix.
                        int total_allocated = 0;
                        for (int j = row; j >= col; j--)
                            total_allocated += matrix[j,j].desired_size;

                        // If the size requirement has not been met, allocate the additional required
                        // size between 'pixel' rows, then 'star' rows, finally 'auto' rows, until all
                        // height has been assigned.
                        if (total_allocated < current)
                        {
                            int additional = current - total_allocated;
                            if (spans_star)
                            {
                                AssignSize(matrix, col, row, ref additional, GridUnitType.Star, true);
                            }
                            else
                            {
                                AssignSize(matrix, col, row, ref additional, GridUnitType.Pixel, true);
                                AssignSize(matrix, col, row, ref additional, GridUnitType.Auto, true);
                            }
                        }
                    }
                }
            }
            for (int r = 0; r < row_matrix_dim; r++)
                row_matrix[r,r].offered_size = row_matrix[r,r].desired_size;
            for (int c = 0; c < col_matrix_dim; c++)
                col_matrix[c,c].offered_size = col_matrix[c,c].desired_size;
        }

        private int AssignSize(Segment[,] matrix, int start, int end, int size, GridUnitType type, bool desired_size)
        {
            AssignSize(matrix, start, end, ref size, type, desired_size);
            return size;
        }

        private void AssignSize(Segment[,] matrix, int start, int end, ref int size, GridUnitType type, bool desired_size)
        {
            int count = 0;
            bool assigned;

            // Count how many segments are of the correct type. If we're measuring Star rows/cols
            // we need to count the number of stars instead.
            for (int i = start; i <= end; i++)
            {
                int segment_size = desired_size ? matrix[i,i].desired_size : matrix[i,i].offered_size;
                if (segment_size < matrix[i,i].max)
                    count += type == GridUnitType.Star ? matrix[i,i].stars : 1;
            }
            do
            {
                assigned = false;
                double contribution = (double)size / count;

                for (int i = start; i <= end; i++)
                {
                    int segment_size = desired_size ? matrix[i,i].desired_size : matrix[i,i].offered_size;
                    if (!(matrix[i,i].type == type && segment_size < matrix[i,i].max))
                        continue;

                    int newsize = Math.Min(
                        double.IsInfinity(contribution) ? 0 : (int)(segment_size + contribution * (type == GridUnitType.Star ? matrix[i,i].stars : 1)),
                        matrix[i,i].max
                    );
                    assigned |= newsize > segment_size;
                    size -= newsize - segment_size;
                    if (desired_size)
                        matrix[i,i].desired_size = newsize;
                    else
                        matrix[i,i].offered_size = newsize;
                }
            } while (assigned);
        }

        private void CreateMatrices(int row_count, int col_count)
        {
            if (row_matrix == null || col_matrix == null || row_matrix_dim != row_count || col_matrix_dim != col_count)
            {
                row_matrix = null;
                col_matrix = null;

                row_matrix_dim = row_count;
                row_matrix = new Segment[row_count,row_count];

                col_matrix_dim = col_count;
                col_matrix = new Segment[col_count,col_count];
            }

            for (int r = 0; r < row_count; r++)
                for (int rr = 0; rr <= r; rr++)
                    row_matrix[r,rr] = new Segment(0, 0, int.MaxValue, GridUnitType.Pixel);

            for (int c = 0; c < col_count; c++)
                for (int cc = 0; cc <= c; cc++)
                    col_matrix[c,cc] = new Segment(0, 0, int.MaxValue, GridUnitType.Pixel);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            ColumnDefinitionCollection columns = _columnDefinitions;
            RowDefinitionCollection rows = _rowDefinitions;

            int col_count = columns != null ? columns.Count : 0;
            int row_count = rows != null ? rows.Count : 0;

            RestoreMeasureResults();

            Size total_consumed = new Size(0, 0);
            for (int c = 0; c < col_matrix_dim; c++)
            {
                col_matrix[c,c].offered_size = col_matrix[c,c].desired_size;
                total_consumed.Width += col_matrix[c,c].offered_size;
            }
            for (int r = 0; r < row_matrix_dim; r++)
            {
                row_matrix[r,r].offered_size = row_matrix[r,r].desired_size;
                total_consumed.Height += row_matrix[r,r].offered_size;
            }

            if (total_consumed.Width != finalSize.Width)
                ExpandStarCols(finalSize);
            if (total_consumed.Height != finalSize.Height)
                ExpandStarRows(finalSize);

            for (int c = 0; c < col_count; c++)
                columns[c].ActualWidth = col_matrix[c,c].offered_size;
            for (int r = 0; r < row_count; r++)
                rows[r].ActualHeight = row_matrix[r,r].offered_size;

            for (int i = 0, count = Children.Count; i < count; i++)
            {
                var child = Children[i];

                int col = Math.Min(GetColumn(child), col_matrix_dim - 1);
                int row = Math.Min(GetRow(child), row_matrix_dim - 1);
                int colspan = Math.Min(GetColumnSpan(child), col_matrix_dim - col);
                int rowspan = Math.Min(GetRowSpan(child), row_matrix_dim - row);

                Rect child_final = new Rect(0, 0, 0, 0);
                for (int c = 0; c < col; c++)
                    child_final.X += col_matrix[c,c].offered_size;
                for (int c = col; c < col + colspan; c++)
                    child_final.Width += col_matrix[c,c].offered_size;

                for (int r = 0; r < row; r++)
                    child_final.Y += row_matrix[r,r].offered_size;
                for (int r = row; r < row + rowspan; r++)
                    child_final.Height += row_matrix[r,r].offered_size;

                child.Arrange(new Rect(
                    child_final.X,
                    child_final.Y,
                    child_final.Width,
                    child_final.Height
                ));
            }

            return finalSize;
        }

        private void SaveMeasureResults()
        {
            for (int i = 0; i < row_matrix_dim; i++)
                for (int j = 0; j < row_matrix_dim; j++)
                    row_matrix[i,j].original_size = row_matrix[i,j].offered_size;

            for (int i = 0; i < col_matrix_dim; i++)
                for (int j = 0; j < col_matrix_dim; j++)
                    col_matrix[i,j].original_size = col_matrix[i,j].offered_size;
        }

        private void RestoreMeasureResults()
        {
            for (int i = 0; i < row_matrix_dim; i++)
                for (int j = 0; j < row_matrix_dim; j++)
                    row_matrix[i,j].offered_size = row_matrix[i,j].original_size;

            for (int i = 0; i < col_matrix_dim; i++)
                for (int j = 0; j < col_matrix_dim; j++)
                    col_matrix[i,j].offered_size = col_matrix[i,j].original_size;
        }

        public static int GetColumn(Element element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return ((int?)element.GetAttachedValue(ColumnProperty)).GetValueOrDefault(0);
        }

        public static void SetColumn(Element element, int index)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            index = Math.Max(index, 0);

            element.SetAttachedValue(ColumnProperty, index == 0 ? null : (int?)index);

            AttachedValueChanged(element);
        }

        public static int GetColumnSpan(Element element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return ((int?)element.GetAttachedValue(ColumnSpanProperty)).GetValueOrDefault(1);
        }

        public static void SetColumnSpan(Element element, int span)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            span = Math.Max(span, 1);

            element.SetAttachedValue(ColumnSpanProperty, span == 1 ? null : (int?)span);

            AttachedValueChanged(element);
        }

        public static int GetRow(Element element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return ((int?)element.GetAttachedValue(RowProperty)).GetValueOrDefault(0);
        }

        public static void SetRow(Element element, int index)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            index = Math.Max(index, 0);

            element.SetAttachedValue(RowProperty, index == 0 ? null : (int?)index);

            AttachedValueChanged(element);
        }

        public static int GetRowSpan(Element element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return ((int?)element.GetAttachedValue(RowSpanProperty)).GetValueOrDefault(1);
        }

        public static void SetRowSpan(Element element, int span)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            span = Math.Max(span, 1);

            element.SetAttachedValue(RowSpanProperty, span == 1 ? null : (int?)span);

            AttachedValueChanged(element);
        }

        private static void AttachedValueChanged(Element element)
        {
            var grid = element.Parent as Grid;

            if (grid != null)
                grid.InvalidateMeasure();
        }

        private struct Segment
        {
            public int original_size;
            public readonly int max;
            public readonly int min;
            public int desired_size;
            public int offered_size;
            public int stars;
            public readonly GridUnitType type;

            public Segment(int offered_size, int min, int max, GridUnitType type)
            {
                desired_size = 0;
                this.max = max;
                this.min = min;
                stars = 0;
                this.type = type;

                this.offered_size = Clamp(offered_size, min, max);
                original_size = this.offered_size;
            }
        }

        private class GridWalker
        {
            private readonly bool has_auto_auto;
            private readonly bool has_star_auto;
            private readonly bool has_auto_star;

            public bool HasAutoAuto() { return has_auto_auto; }
            public bool HasStarAuto() { return has_star_auto; }
            public bool HasAutoStar() { return has_auto_star; }

            public GridWalker(Grid grid, Segment[,] row_matrix, int row_count, Segment[,] col_matrix, int col_count)
            {
                has_auto_auto = false;
                has_star_auto = false;
                has_auto_star = false;

                var children = grid.Children;

                for (int i = 0, count = children.Count; i < count; i++)
                {
                    var child = children[i];

                    bool star_col = false;
                    bool star_row = false;
                    bool auto_col = false;
                    bool auto_row = false;

                    int col = Math.Min(Grid.GetColumn(child), col_count - 1);
                    int row = Math.Min(Grid.GetRow(child), row_count - 1);
                    int colspan = Math.Min(Grid.GetColumnSpan(child), col_count - col);
                    int rowspan = Math.Min(Grid.GetRowSpan(child), row_count - row);

                    for (int r = row; r < row + rowspan; r++)
                    {
                        star_row |= row_matrix[r,r].type == GridUnitType.Star;
                        auto_row |= row_matrix[r,r].type == GridUnitType.Auto;
                    }
                    for (int c = col; c < col + colspan; c++)
                    {
                        star_col |= col_matrix[c,c].type == GridUnitType.Star;
                        auto_col |= col_matrix[c,c].type == GridUnitType.Auto;
                    }

                    has_auto_auto |= auto_row && auto_col && !star_row && !star_col;
                    has_star_auto |= star_row && auto_col;
                    has_auto_star |= auto_row && star_col;
                }
            }
        }

        private class GridNode
        {
            public readonly int row;
            public readonly int col;
            public readonly int size;
            public readonly Segment[,] matrix;

            public GridNode(Segment[,] matrix, int row, int col, int size)
            {
                this.matrix = matrix;
                this.row = row;
                this.col = col;
                this.size = size;
            }
        }
    }
}
