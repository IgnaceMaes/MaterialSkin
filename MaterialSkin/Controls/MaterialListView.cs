namespace MaterialSkin.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class MaterialListView : ListView, IMaterialControl
    {
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        [Browsable(false)]
        public Point MouseLocation { get; set; }

        [Browsable(false)]
        private ListViewItem HoveredItem { get; set; }

        public MaterialListView()
        {
            GridLines = false;
            FullRowSelect = true;
            HeaderStyle = ColumnHeaderStyle.Nonclickable;
            View = View.Details;
            OwnerDraw = true;
            ResizeRedraw = true;
            BorderStyle = BorderStyle.None;
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);

            //Fix for hovers, by default it doesn't redraw
            MouseLocation = new Point(-1, -1);
            MouseState = MouseState.OUT;
            MouseEnter += delegate
            {
                MouseState = MouseState.HOVER;
            };
            MouseLeave += delegate
            {
                MouseState = MouseState.OUT;
                MouseLocation = new Point(-1, -1);
                HoveredItem = null;
                Invalidate();
            };
            MouseDown += delegate
            {
                MouseState = MouseState.DOWN;
            };
            MouseUp += delegate
            {
                MouseState = MouseState.HOVER;
            };
            MouseMove += delegate (object sender, MouseEventArgs args)
            {
                MouseLocation = args.Location;
                var currentHoveredItem = GetItemAt(MouseLocation.X, MouseLocation.Y);
                if (HoveredItem != currentHoveredItem)
                {
                    HoveredItem = currentHoveredItem;
                    Invalidate();
                }
            };
        }

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(SkinManager.GetApplicationBackgroundColor()), new Rectangle(e.Bounds.X, e.Bounds.Y, Width, e.Bounds.Height));
            e.Graphics.DrawString(e.Header.Text,
                SkinManager.ROBOTO_MEDIUM_10,
                SkinManager.GetSecondaryTextBrush(),
                new Rectangle(e.Bounds.X + ITEM_PADDING, e.Bounds.Y + ITEM_PADDING, e.Bounds.Width - ITEM_PADDING * 2, e.Bounds.Height - ITEM_PADDING * 2),
                getStringFormat());
        }

        private const int ITEM_PADDING = 12;

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            //We draw the current line of items (= item with subitems) on a temp bitmap, then draw the bitmap at once. This is to reduce flickering.
            var b = new Bitmap(e.Item.Bounds.Width, e.Item.Bounds.Height);
            var g = Graphics.FromImage(b);

            //always draw default background
            g.Clear(SkinManager.GetApplicationBackgroundColor());

            if (e.Item.Selected)
            {
                //selected background
                g.FillRectangle(SkinManager.GetButtonPressedBackgroundBrush(), new Rectangle(new Point(e.Bounds.X, 0), e.Bounds.Size));
            }
            else if (e.Bounds.Contains(MouseLocation) && MouseState == MouseState.HOVER)
            {
                //hover background
                g.FillRectangle(SkinManager.GetButtonHoverBackgroundBrush(), new Rectangle(new Point(e.Bounds.X, 0), e.Bounds.Size));
            }


            //Draw separator
            g.DrawLine(new Pen(SkinManager.GetDividersColor()), e.Bounds.Left, 0, e.Bounds.Right, 0);

            foreach (ListViewItem.ListViewSubItem subItem in e.Item.SubItems)
            {
                //Draw text
                g.DrawString(subItem.Text, SkinManager.ROBOTO_MEDIUM_10, SkinManager.GetPrimaryTextBrush(),
                                 new Rectangle(subItem.Bounds.X + ITEM_PADDING, ITEM_PADDING, subItem.Bounds.Width - 2 * ITEM_PADDING, subItem.Bounds.Height - 2 * ITEM_PADDING),
                                 getStringFormat());
            }

            e.Graphics.DrawImage((Image)b.Clone(), new Point(0, e.Item.Bounds.Location.Y));
            g.Dispose();
            b.Dispose();
        }

        private StringFormat getStringFormat()
        {
            return new StringFormat
            {
                FormatFlags = StringFormatFlags.LineLimit,
                Trimming = StringTrimming.EllipsisCharacter,
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            };
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class LogFont
        {
            public int lfHeight = 0;
            public int lfWidth = 0;
            public int lfEscapement = 0;
            public int lfOrientation = 0;
            public int lfWeight = 0;
            public byte lfItalic = 0;
            public byte lfUnderline = 0;
            public byte lfStrikeOut = 0;
            public byte lfCharSet = 0;
            public byte lfOutPrecision = 0;
            public byte lfClipPrecision = 0;
            public byte lfQuality = 0;
            public byte lfPitchAndFamily = 0;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName = string.Empty;
        }
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            // This hack tries to apply the Roboto (24) font to all ListViewItems in this ListView
            // It only succeeds if the font is installed on the system.
            // Otherwise, a default sans serif font is used.
            var roboto24 = new Font(SkinManager.ROBOTO_MEDIUM_12.FontFamily, 24);
            var roboto24Logfont = new LogFont();
            roboto24.ToLogFont(roboto24Logfont);

            try
            {
                // Font.FromLogFont is the method used when drawing ListViewItems. I 'test' it in this safer context to avoid unhandled exceptions later.
                Font = Font.FromLogFont(roboto24Logfont);
            }
            catch (ArgumentException)
            {
                Font = new Font(FontFamily.GenericSansSerif, 24);
            }
        }
    }
}
