using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public class Cursor
    {
        public static Cursor AppStarting = new Cursor(System.Windows.Forms.Cursors.AppStarting);
        public static Cursor Arrow = new Cursor(System.Windows.Forms.Cursors.Arrow);
        public static Cursor Cross = new Cursor(System.Windows.Forms.Cursors.Cross);
        public static Cursor Default = new Cursor(System.Windows.Forms.Cursors.Default);
        public static Cursor IBeam = new Cursor(System.Windows.Forms.Cursors.IBeam);
        public static Cursor No = new Cursor(System.Windows.Forms.Cursors.No);
        public static Cursor SizeAll = new Cursor(System.Windows.Forms.Cursors.SizeAll);
        public static Cursor SizeNESW = new Cursor(System.Windows.Forms.Cursors.SizeNESW);
        public static Cursor SizeNS = new Cursor(System.Windows.Forms.Cursors.SizeNS);
        public static Cursor SizeNWSE = new Cursor(System.Windows.Forms.Cursors.SizeNWSE);
        public static Cursor SizeWE = new Cursor(System.Windows.Forms.Cursors.SizeWE);
        public static Cursor UpArrow = new Cursor(System.Windows.Forms.Cursors.UpArrow);
        public static Cursor WaitCursor = new Cursor(System.Windows.Forms.Cursors.WaitCursor);
        public static Cursor Help = new Cursor(System.Windows.Forms.Cursors.Help);
        public static Cursor HSplit = new Cursor(System.Windows.Forms.Cursors.HSplit);
        public static Cursor VSplit = new Cursor(System.Windows.Forms.Cursors.VSplit);
        public static Cursor NoMove2D = new Cursor(System.Windows.Forms.Cursors.NoMove2D);
        public static Cursor NoMoveHoriz = new Cursor(System.Windows.Forms.Cursors.NoMoveVert);
        public static Cursor NoMoveVert = new Cursor(System.Windows.Forms.Cursors.NoMoveVert);
        public static Cursor PanEast = new Cursor(System.Windows.Forms.Cursors.PanEast);
        public static Cursor PanNE = new Cursor(System.Windows.Forms.Cursors.PanNE);
        public static Cursor PanNorth = new Cursor(System.Windows.Forms.Cursors.PanNorth);
        public static Cursor PanNW = new Cursor(System.Windows.Forms.Cursors.PanNW);
        public static Cursor PanSE = new Cursor(System.Windows.Forms.Cursors.PanSE);
        public static Cursor PanSouth = new Cursor(System.Windows.Forms.Cursors.PanSouth);
        public static Cursor PanSW = new Cursor(System.Windows.Forms.Cursors.PanSW);
        public static Cursor PanWest = new Cursor(System.Windows.Forms.Cursors.PanWest);
        public static Cursor Hand = new Cursor(System.Windows.Forms.Cursors.Hand);

        internal System.Windows.Forms.Cursor NativeCursor { get; private set; }

        private Cursor(System.Windows.Forms.Cursor nativeCursor)
        {
            NativeCursor = nativeCursor;
        }

        public override string ToString()
        {
            return NativeCursor.ToString();
        }
    }
}
