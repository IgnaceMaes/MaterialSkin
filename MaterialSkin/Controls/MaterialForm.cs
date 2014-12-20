using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialSkin.Controls
{
    public class MaterialForm : Form, IMaterialControl
    {
        public int Depth { get; set; }
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
        public MouseState MouseState { get; set; }

        public bool Sizable { get; set; }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;
        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTBOTTOM = 15;
        private const int BORDER_WIDTH = 7;
        private ResizeDirection resizeDir;
        private ButtonState buttonState = ButtonState.None;

        private const int STATUS_BAR_BUTTON_WIDTH = (int) (STATUS_BAR_HEIGHT * 1.5);
        private const int STATUS_BAR_HEIGHT = 24;
        private const int ACTION_BAR_HEIGHT = 40;

        private enum ResizeDirection
        {
            BottomLeft,
            Left,
            Right,
            BottomRight,
            Bottom,
            None
        }

        private enum ButtonState
        {
            XOver,
            MaxOver,
            MinOver,
            XDown,
            MaxDown,
            MinDown,
            None
        }

        private Rectangle minButtonBounds;
        private Rectangle maxButtonBounds;
        private Rectangle xButtonBounds;
        private Rectangle actionBarBounds;
        private Rectangle statusBarBounds;

        public MaterialForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            Sizable = true;
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (DesignMode || IsDisposed) return;
            if (m.Msg == WM_LBUTTONDOWN && (statusBarBounds.Contains(PointToClient(Cursor.Position)) || actionBarBounds.Contains(PointToClient(Cursor.Position))) && !(minButtonBounds.Contains(PointToClient(Cursor.Position)) || maxButtonBounds.Contains(PointToClient(Cursor.Position)) || xButtonBounds.Contains(PointToClient(Cursor.Position))))
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (DesignMode) return;
            UpdateButtons(e);

            if (e.Button == MouseButtons.Left && WindowState != FormWindowState.Maximized)
                ResizeForm(resizeDir);
            base.OnMouseDown(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (DesignMode) return;
            buttonState = ButtonState.None;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (DesignMode) return;

            if (e.Location.X < BORDER_WIDTH && e.Location.Y > Height - BORDER_WIDTH)
            {
                resizeDir = ResizeDirection.BottomLeft;
                Cursor = Cursors.SizeNESW;
            }
            else if (e.Location.X < BORDER_WIDTH && e.Location.Y > 40)
            {
                resizeDir = ResizeDirection.Left;
                Cursor = Cursors.SizeWE;
            }
            else if (e.Location.X > Width - BORDER_WIDTH && e.Location.Y > Height - BORDER_WIDTH)
            {
                resizeDir = ResizeDirection.BottomRight;
                Cursor = Cursors.SizeNWSE;
            }
            else if (e.Location.X > Width - BORDER_WIDTH && e.Location.Y > 40)
            {
                resizeDir = ResizeDirection.Right;
                Cursor = Cursors.SizeWE;
            }
            else if (e.Location.Y > Height - BORDER_WIDTH)
            {
                resizeDir = ResizeDirection.Bottom;
                Cursor = Cursors.SizeNS;
            }
            else
            {
                resizeDir = ResizeDirection.None;
                Cursor = Cursors.Default;
            }

            UpdateButtons(e);
        }

        private void UpdateButtons(MouseEventArgs e, bool up = false)
        {
            if (DesignMode) return;
            ButtonState oldState = buttonState;

            if (e.Button == MouseButtons.Left && !up)
            {
                if (minButtonBounds.Contains(e.Location)) buttonState = ButtonState.MinDown;
                else if (maxButtonBounds.Contains(e.Location)) buttonState = ButtonState.MaxDown;
                else if (xButtonBounds.Contains(e.Location)) buttonState = ButtonState.XDown;
                else buttonState = ButtonState.None;
            }
            else
            {
                if (minButtonBounds.Contains(e.Location))
                {
                    buttonState = ButtonState.MinOver;
                    
                    if (oldState == ButtonState.MinDown)
                        WindowState = FormWindowState.Minimized;
                }
                else if (maxButtonBounds.Contains(e.Location))
                {
                    buttonState = ButtonState.MaxOver;

                    if (oldState == ButtonState.MaxDown)
                        WindowState = WindowState == FormWindowState.Normal ? FormWindowState.Maximized : FormWindowState.Normal;
                }
                else if (xButtonBounds.Contains(e.Location))
                {
                    buttonState = ButtonState.XOver;
                    if (oldState == ButtonState.XDown)
                        Close();
                }
                else buttonState = ButtonState.None;
            }

            if (oldState != buttonState) Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (DesignMode) return;
            UpdateButtons(e, true);

            base.OnMouseUp(e);
            ReleaseCapture();
        }

        private void ResizeForm(ResizeDirection direction)
        {
            if (DesignMode) return;
            int dir = -1;
            switch (direction)
            {
                case ResizeDirection.BottomLeft:
                    dir = HTBOTTOMLEFT;
                    break;
                case ResizeDirection.Left:
                    dir = HTLEFT;
                    break;
                case ResizeDirection.Right:
                    dir = HTRIGHT;
                    break;
                case ResizeDirection.BottomRight:
                    dir = HTBOTTOMRIGHT;
                    break;
                case ResizeDirection.Bottom:
                    dir = HTBOTTOM;
                    break;
            }

            ReleaseCapture();
            if (dir != -1)
            {
                SendMessage(Handle, WM_NCLBUTTONDOWN, dir, 0);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (DesignMode) return;

            minButtonBounds = new Rectangle(Width - 3 * STATUS_BAR_BUTTON_WIDTH, 0, STATUS_BAR_BUTTON_WIDTH, STATUS_BAR_HEIGHT);
            maxButtonBounds = new Rectangle(Width - 2 * STATUS_BAR_BUTTON_WIDTH, 0, STATUS_BAR_BUTTON_WIDTH, STATUS_BAR_HEIGHT);
            xButtonBounds = new Rectangle(Width - STATUS_BAR_BUTTON_WIDTH, 0, STATUS_BAR_BUTTON_WIDTH, STATUS_BAR_HEIGHT);
            statusBarBounds = new Rectangle(0, 0, Width, STATUS_BAR_HEIGHT);
            actionBarBounds = new Rectangle(0, STATUS_BAR_HEIGHT, Width, ACTION_BAR_HEIGHT);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode) { e.Graphics.Clear(Color.FromArgb(238, 238, 238)); return; }

            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.Clear(SkinManager.GetApplicationBackgroundColor());
            g.FillRectangle(SkinManager.PrimaryColorDarkBrush, statusBarBounds);
            g.FillRectangle(SkinManager.PrimaryColorBrush, actionBarBounds);

            //Draw border
            using (var borderPen = new Pen(SkinManager.GetDividersColor(), 1))
            {
                g.DrawLine(borderPen, new Point(0, actionBarBounds.Bottom), new Point(0, Height - 2));
                g.DrawLine(borderPen, new Point(Width - 1, actionBarBounds.Bottom), new Point(Width - 1, Height - 2));
                g.DrawLine(borderPen, new Point(0, Height - 1), new Point(Width - 1, Height - 1));
            }

            //Minimize, maximize and close states
            switch (buttonState)
            {
                case ButtonState.MinOver:
                    g.FillRectangle(SkinManager.GetFlatButtonHoverBackgroundBrush(), minButtonBounds);
                    break;
                case ButtonState.MinDown:
                    g.FillRectangle(SkinManager.GetFlatButtonPressedBackgroundBrush(), minButtonBounds);
                    break;
                case ButtonState.MaxOver:
                    g.FillRectangle(SkinManager.GetFlatButtonHoverBackgroundBrush(), maxButtonBounds);
                    break;
                case ButtonState.MaxDown:
                    g.FillRectangle(SkinManager.GetFlatButtonPressedBackgroundBrush(), maxButtonBounds);
                    break;
                case ButtonState.XOver:
                    g.FillRectangle(SkinManager.GetFlatButtonHoverBackgroundBrush(), xButtonBounds);
                    break;
                case ButtonState.XDown:
                    g.FillRectangle(SkinManager.GetFlatButtonPressedBackgroundBrush(), xButtonBounds);
                    break;
            }
            
            //Minimize, maximize and close
            g.DrawString("0", new Font("Marlett", 11), SkinManager.ACTION_BAR_TEXT, minButtonBounds, new StringFormat() {LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center});
            g.DrawString("1", new Font("Marlett", 11), SkinManager.ACTION_BAR_TEXT, maxButtonBounds, new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });
            g.DrawString("r", new Font("Marlett", 11), SkinManager.ACTION_BAR_TEXT, xButtonBounds, new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });

            //Form title
            g.DrawString(Text, SkinManager.FONT_TITLE, SkinManager.ACTION_BAR_TEXT, new Rectangle(10, STATUS_BAR_HEIGHT, Width, ACTION_BAR_HEIGHT), new StringFormat() { LineAlignment = StringAlignment.Center });
        }
    }
}
