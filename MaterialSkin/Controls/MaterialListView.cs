using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialSkin.Controls
{
	public class MaterialListView : ListView, IMaterialControl
	{
		[Browsable(false)]
		public int Depth { get; set; }
		[Browsable(false)]
		public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
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
			//TODO: should only redraw when the hovered line changed, this to reduce unnecessary redraws
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
			MouseDown += delegate { MouseState = MouseState.DOWN; };
			MouseUp += delegate{ MouseState = MouseState.HOVER; };
			MouseMove += delegate(object sender, MouseEventArgs args)
			{
				MouseLocation = args.Location;
                var currentHoveredItem = this.GetItemAt(MouseLocation.X, MouseLocation.Y);
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
            float fillPercent = 0;
            Color highlightColor = Color.White;
            if (e.Item is MaterialListViewItem)
            {
                MaterialListViewItem mlvi = e.Item as MaterialListViewItem;
                fillPercent = mlvi.fillPercent;
                highlightColor = mlvi.highlightColor;
            }


			//We draw the current line of items (= item with subitems) on a temp bitmap, then draw the bitmap at once. This is to reduce flickering.
			var b = new Bitmap(e.Item.Bounds.Width, e.Item.Bounds.Height);
			var g = Graphics.FromImage(b);

            //always draw default background
            int divideSpot = (int)((fillPercent * e.Bounds.Size.Width) / 100);      //The spot where the progress splits into background color

            Rectangle firstRect = e.Bounds;     //Set the rectnagle of where
            firstRect.Width = divideSpot;       //the progress is drawn

            g.FillRectangle(new SolidBrush(e.Item.BackColor), new Rectangle(new Point(e.Bounds.X, 0), e.Bounds.Size));      //Draw the item's normal background
            g.FillRectangle(new SolidBrush(highlightColor), new Rectangle(new Point(e.Bounds.X, 0), firstRect.Size));       //Fill with highlight
            

            if (e.State.HasFlag(ListViewItemStates.Selected))
			{
				//selected background
				g.FillRectangle(SkinManager.GetFlatButtonPressedBackgroundBrush(), new Rectangle(new Point(e.Bounds.X, 0), e.Bounds.Size));
			}
			else if (e.Bounds.Contains(MouseLocation) && MouseState == MouseState.HOVER)
			{
				//hover background
				g.FillRectangle(SkinManager.GetFlatButtonHoverBackgroundBrush(), new Rectangle(new Point(e.Bounds.X, 0), e.Bounds.Size));
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

			e.Graphics.DrawImage((Image) b.Clone(), new Point(0, e.Item.Bounds.Location.Y));
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
            Font roboto24 = new Font(SkinManager.ROBOTO_MEDIUM_12.FontFamily, 24);
            LogFont roboto24logfont = new LogFont();
            roboto24.ToLogFont(roboto24logfont);

            try
            {
                // Font.FromLogFont is the method used when drawing ListViewItems. I 'test' it in this safer context to avoid unhandled exceptions later.
                Font = Font.FromLogFont(roboto24logfont);
            }
            catch (ArgumentException)
            {
                Font = new Font(FontFamily.GenericSansSerif, 24);
            }
        }
	}
}
