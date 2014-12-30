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
    public class MouseMessageFilter : IMessageFilter
    {
        private const int WM_MOUSEMOVE = 0x0200;

        public static event MouseEventHandler MouseMove;

        public bool PreFilterMessage(ref Message m)
        {

            if (m.Msg == WM_MOUSEMOVE)
            {
                if (MouseMove != null)
                {
                    int x = Control.MousePosition.X, y = Control.MousePosition.Y;

                    MouseMove(null, new MouseEventArgs(MouseButtons.None, 0, x, y, 0));
                }
            }
            return false;
        }
    }

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

        [DllImport("user32.dll")]
        static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_RBUTTONDOWN = 0x0204;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;
        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTBOTTOM = 15;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int BORDER_WIDTH = 7;
        private ResizeDirection resizeDir;
        private ButtonState buttonState = ButtonState.None;

        private const int WMSZ_TOP = 3;
        private const int WMSZ_TOPLEFT = 4;
        private const int WMSZ_TOPRIGHT = 5;
        private const int WMSZ_LEFT = 1;
        private const int WMSZ_RIGHT = 2;
        private const int WMSZ_BOTTOM = 6;
        private const int WMSZ_BOTTOMLEFT = 7;
        private const int WMSZ_BOTTOMRIGHT = 8;

        private Dictionary<int, int> ResizingLocationsToCmd = new Dictionary<int, int>() 
        {
            {HTTOP,         WMSZ_TOP},
            {HTTOPLEFT,     WMSZ_TOPLEFT},
            {HTTOPRIGHT,    WMSZ_TOPRIGHT},
            {HTLEFT,        WMSZ_LEFT},
            {HTRIGHT,       WMSZ_RIGHT},
            {HTBOTTOM,      WMSZ_BOTTOM},
            {HTBOTTOMLEFT,  WMSZ_BOTTOMLEFT},
            {HTBOTTOMRIGHT, WMSZ_BOTTOMRIGHT}
        };

        private const int STATUS_BAR_BUTTON_WIDTH = STATUS_BAR_HEIGHT;
        private const int STATUS_BAR_HEIGHT = 24;
        private const int ACTION_BAR_HEIGHT = 40;

        private uint TPM_LEFTALIGN = 0x0000;
        private uint TPM_RETURNCMD = 0x0100;

        private const int WM_SYSCOMMAND = 0x0112;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int WS_SYSMENU = 0x00080000;

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

            // This enables the form to trigger the MouseMove event even when mouse is over another control
            Application.AddMessageFilter(new MouseMessageFilter());
            MouseMessageFilter.MouseMove += new MouseEventHandler(OnGlobalMouseMove);
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
            else if (m.Msg == WM_RBUTTONDOWN)
            {
                Point cursorPos = PointToClient(Cursor.Position);

                if (statusBarBounds.Contains(cursorPos) && !minButtonBounds.Contains(cursorPos) && !maxButtonBounds.Contains(cursorPos) && !xButtonBounds.Contains(cursorPos))
                {
                    // Show default system menu when right clicking titlebar
                    int id = TrackPopupMenuEx(
                        GetSystemMenu(this.Handle, false),
                        TPM_LEFTALIGN | TPM_RETURNCMD,
                        Cursor.Position.X, Cursor.Position.Y, this.Handle, IntPtr.Zero);

                    // Pass the command as a WM_SYSCOMMAND message
                    SendMessage(this.Handle, WM_SYSCOMMAND, id, 0);
                }
            }
            else if (m.Msg == WM_NCLBUTTONDOWN)
            {
                // This re-enables resizing by letting the application know when the
                // user is trying to resize a side. This is disabled by default when using WS_SYSMENU.

                byte bFlag = 0;

                // Get which side to resize from
                if (ResizingLocationsToCmd.ContainsKey((int)m.WParam))
                    bFlag = (byte)ResizingLocationsToCmd[(int)m.WParam];

                if (bFlag != 0)
                    SendMessage(this.Handle, WM_SYSCOMMAND, (int)(0xF000 | bFlag), (int)m.LParam);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams par = base.CreateParams;
                // WS_SYSMENU: Trigger the creation of the system menu
                // WS_MINIMIZEBOX: Allow minimizing from taskbar
                par.Style = par.Style | WS_MINIMIZEBOX | WS_SYSMENU; // Turn on the WS_MINIMIZEBOX style flag
                return par;
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

            // Don't show resize cursor when over a child control, since it wont work anyway
            if (GetChildAtPoint(e.Location) != null)
            {
                resizeDir = ResizeDirection.None;
                Cursor = Cursors.Default;
            }
            else
            {
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
            }

            UpdateButtons(e);
        }

        protected void OnGlobalMouseMove(object sender, MouseEventArgs e)
        {
            // Convert to client position and pass to Form.MouseMove
            Point clientCursorPos = PointToClient(e.Location);
            MouseEventArgs new_e = new MouseEventArgs(MouseButtons.None, 0, clientCursorPos.X, clientCursorPos.Y, 0);
            this.OnMouseMove(new_e);
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

            minButtonBounds = new Rectangle((Width - SkinManager.FORM_PADDING / 2)- 3 * STATUS_BAR_BUTTON_WIDTH, 0, STATUS_BAR_BUTTON_WIDTH, STATUS_BAR_HEIGHT);
            maxButtonBounds = new Rectangle((Width - SkinManager.FORM_PADDING / 2) - 2 * STATUS_BAR_BUTTON_WIDTH, 0, STATUS_BAR_BUTTON_WIDTH, STATUS_BAR_HEIGHT);
            xButtonBounds = new Rectangle((Width - SkinManager.FORM_PADDING / 2) - STATUS_BAR_BUTTON_WIDTH, 0, STATUS_BAR_BUTTON_WIDTH, STATUS_BAR_HEIGHT);
            statusBarBounds = new Rectangle(0, 0, Width, STATUS_BAR_HEIGHT);
            actionBarBounds = new Rectangle(0, STATUS_BAR_HEIGHT, Width, ACTION_BAR_HEIGHT);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
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

            using (var formButtonsPen = new Pen(SkinManager.ACTION_BAR_TEXT_SECONDARY_BRUSH, 2))
            {
                //Minimize
                g.DrawLine(formButtonsPen, minButtonBounds.X + (int) (minButtonBounds.Width * 0.33), minButtonBounds.Y + (int)(minButtonBounds.Height * 0.66), minButtonBounds.X + (int)(minButtonBounds.Width * 0.66), minButtonBounds.Y + (int)(minButtonBounds.Height * 0.66));
            
                //Maximize
                g.DrawRectangle(formButtonsPen, maxButtonBounds.X + (int) (maxButtonBounds.Width * 0.33), maxButtonBounds.Y + (int) (maxButtonBounds.Height * 0.36), (int) (maxButtonBounds.Width * 0.39), (int) (maxButtonBounds.Height * 0.31));
            
                //Close
                g.DrawLine(formButtonsPen, xButtonBounds.X + (int) (xButtonBounds.Width * 0.33), xButtonBounds.Y + (int) (xButtonBounds.Height * 0.33), xButtonBounds.X + (int) (xButtonBounds.Width * 0.66), xButtonBounds.Y + (int) (xButtonBounds.Height * 0.66));
                g.DrawLine(formButtonsPen, xButtonBounds.X + (int) (xButtonBounds.Width * 0.66), xButtonBounds.Y + (int)(xButtonBounds.Height * 0.33), xButtonBounds.X + (int)(xButtonBounds.Width * 0.33), xButtonBounds.Y + (int)(xButtonBounds.Height * 0.66));
            }

            //Form title
            g.DrawString(Text, SkinManager.ROBOTO_MEDIUM_12, SkinManager.ACTION_BAR_TEXT_BRUSH, new Rectangle(SkinManager.FORM_PADDING, STATUS_BAR_HEIGHT, Width, ACTION_BAR_HEIGHT), new StringFormat() { LineAlignment = StringAlignment.Center });
        }
    }
}
