namespace MaterialSkin.Controls
{
    using MaterialSkin.Animations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Text;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Channels;
    using System.Windows.Forms;

    public class MaterialForm : Form, IMaterialControl
    {
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; Invalidate(); }
        }

        public new FormBorderStyle FormBorderStyle
        {
            get { return base.FormBorderStyle; }
            set { base.FormBorderStyle = value; }
        }

        [Category("Layout")]
        public bool Sizable { get; set; }

        public enum FormStyles
        {
            StatusAndActionBar_None,
            ActionBar_None,
            ActionBar_40,
            ActionBar_48,
            ActionBar_56,
            ActionBar_64,
        }

        private FormStyles _formStyle;
        [Category("Material Skin"), Browsable(true), DisplayName("Form Style"), DefaultValue(FormStyles.ActionBar_40)]
        public FormStyles FormStyle
        {
            get { return _formStyle; }
            set
            {
                _formStyle = value;
                if (_formStyle == FormStyles.StatusAndActionBar_None)
                {
                    ACTION_BAR_HEIGHT = 0;
                    STATUS_BAR_HEIGHT = 0;
                }
                else if (_formStyle == FormStyles.ActionBar_None)
                {
                    ACTION_BAR_HEIGHT = 0;
                    STATUS_BAR_HEIGHT = STATUS_BAR_HEIGHT_DEFAULT;
                }
                else if (_formStyle == FormStyles.ActionBar_40)
                {
                    ACTION_BAR_HEIGHT = ACTION_BAR_HEIGHT_DEFAULT;
                    STATUS_BAR_HEIGHT = STATUS_BAR_HEIGHT_DEFAULT;
                }
                else if (_formStyle == FormStyles.ActionBar_48)
                {
                    ACTION_BAR_HEIGHT = 48;
                    STATUS_BAR_HEIGHT = STATUS_BAR_HEIGHT_DEFAULT;
                }
                else if (_formStyle == FormStyles.ActionBar_56)
                {
                    ACTION_BAR_HEIGHT = 56;
                    STATUS_BAR_HEIGHT = STATUS_BAR_HEIGHT_DEFAULT;
                }
                else if (_formStyle == FormStyles.ActionBar_64)
                {
                    ACTION_BAR_HEIGHT = 64;
                    STATUS_BAR_HEIGHT = STATUS_BAR_HEIGHT_DEFAULT;
                }
                else
                {
                    ACTION_BAR_HEIGHT = ACTION_BAR_HEIGHT_DEFAULT;
                    STATUS_BAR_HEIGHT = STATUS_BAR_HEIGHT_DEFAULT;
                }

                Padding = new Padding(_drawerShowIconsWhenHidden ? drawerControl.MinWidth : PADDING_MINIMUM, STATUS_BAR_HEIGHT + ACTION_BAR_HEIGHT, Padding.Right, Padding.Bottom);
                originalPadding = Padding;

                if (DrawerTabControl != null)
                {
                    int H = Size.Height - (STATUS_BAR_HEIGHT + ACTION_BAR_HEIGHT);
                    int Y = Location.Y + (STATUS_BAR_HEIGHT + ACTION_BAR_HEIGHT);
                    drawerOverlay.Size = new Size(Size.Width, H);
                    drawerOverlay.Location = new Point(Location.X, Y);
                    drawerForm.Size = new Size(DrawerWidth, H);
                    drawerForm.Location = new Point(Location.X, Y);
                }

                UpdateRects();
                Invalidate();
            }
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out] MONITORINFOEX info);

        private Form drawerOverlay = new Form();
        private Form drawerForm = new Form();

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_LBUTTONDBLCLK = 0x0203;
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

        private ResizeDirection _resizeDir;
        private ButtonState _buttonState = ButtonState.None;

        private const int WMSZ_TOP = 3;
        private const int WMSZ_TOPLEFT = 4;
        private const int WMSZ_TOPRIGHT = 5;
        private const int WMSZ_LEFT = 1;
        private const int WMSZ_RIGHT = 2;
        private const int WMSZ_BOTTOM = 6;
        private const int WMSZ_BOTTOMLEFT = 7;
        private const int WMSZ_BOTTOMRIGHT = 8;

        private readonly Dictionary<int, int> _resizingLocationsToCmd = new Dictionary<int, int>
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

        private const int STATUS_BAR_BUTTON_WIDTH = 24;
        private const int STATUS_BAR_HEIGHT_DEFAULT = 24;
        private int STATUS_BAR_HEIGHT = 24;
        private const int ICON_SIZE = 24;
        private const int PADDING_MINIMUM = 3;
        private const int TITLE_LEFT_PADDING = 72;
        private const int ACTION_BAR_PADDING = 16;
        private const int ACTION_BAR_HEIGHT_DEFAULT = 40;
        private int ACTION_BAR_HEIGHT = 40;   
        private const uint TPM_LEFTALIGN = 0x0000;
        private const uint TPM_RETURNCMD = 0x0100;
        private const int WM_SYSCOMMAND = 0x0112;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int WS_SYSMENU = 0x00080000;
        private const int MONITOR_DEFAULTTONEAREST = 2;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        public class MONITORINFOEX
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] szDevice = new char[32];
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public int Width()
            {
                return right - left;
            }

            public int Height()
            {
                return bottom - top;
            }
        }

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
            DrawerOver,
            XDown,
            MaxDown,
            MinDown,
            DrawerDown,
            None
        }

        private readonly Cursor[] _resizeCursors = { Cursors.SizeNESW, Cursors.SizeWE, Cursors.SizeNWSE, Cursors.SizeWE, Cursors.SizeNS };

        private Rectangle _minButtonBounds;
        private Rectangle _maxButtonBounds;
        private Rectangle _xButtonBounds;
        private Rectangle _actionBarBounds;
        private Rectangle _drawerButtonBounds;

        public Rectangle UserArea
        {
            get
            {
                return new Rectangle(0, STATUS_BAR_HEIGHT + ACTION_BAR_HEIGHT, Width, Height - (STATUS_BAR_HEIGHT + ACTION_BAR_HEIGHT));
            }
        }

        private Rectangle _statusBarBounds;
        private bool _maximized;
        private Size _previousSize;
        private Point _previousLocation;
        private bool _headerMouseDown;

        private Padding originalPadding;

        public MaterialForm()
        {
            DrawerWidth = 200;
            DrawerIsOpen = false;
            DrawerShowIconsWhenHidden = false;
            DrawerAutoHide = true;
            DrawerAutoShow = false;
            DrawerIndicatorWidth = 0;
            DrawerHighlightWithAccent = true;
            DrawerBackgroundWithAccent = false;

            FormBorderStyle = FormBorderStyle.None;
            Sizable = true;
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            FormStyle = FormStyles.ActionBar_40;

            Padding = new Padding(PADDING_MINIMUM, STATUS_BAR_HEIGHT+ ACTION_BAR_HEIGHT, PADDING_MINIMUM, PADDING_MINIMUM);      //Keep space for resize by mouse

            _clickAnimManager = new AnimationManager()
            {
                AnimationType = AnimationType.EaseOut,
                Increment = 0.04
            };
            _clickAnimManager.OnAnimationProgress += sender => Invalidate();

            // Drawer
            Shown += (sender, e) =>
            {
                if (DesignMode || IsDisposed)
                    return;
                AddDrawerOverlayForm();
            };
        }

        // Drawer overlay and speed improvements
        private bool _drawerShowIconsWhenHidden;

        [Category("Drawer")]
        public bool DrawerShowIconsWhenHidden
        {
            get { return _drawerShowIconsWhenHidden; }
            set
            {
                _drawerShowIconsWhenHidden = value;
                if (drawerControl != null)
                {
                    drawerControl.ShowIconsWhenHidden = _drawerShowIconsWhenHidden;
                    drawerControl.Refresh();
                }
            }
        }

        [Category("Drawer")]
        public int DrawerWidth { get; set; }

        private bool _drawerAutoHide;
        [Category("Drawer")]
        public bool DrawerAutoHide 
        {
            get
            {
                return _drawerAutoHide;
            }
            set
            {
                _drawerAutoHide = value;
                drawerControl.AutoHide = _drawerAutoHide;
            }
        }

        private bool _drawerAutoShow;
        [Category("Drawer")]
        public bool DrawerAutoShow 
        {
            get
            {
                return _drawerAutoShow;
            }
            set
            {
                _drawerAutoShow = value;
                drawerControl.AutoShow = _drawerAutoShow;
            }
        }

        [Category("Drawer")]
        public int DrawerIndicatorWidth { get; set; }

        private bool _drawerIsOpen;

        [Category("Drawer")]
        public bool DrawerIsOpen
        {
            get
            {
                return _drawerIsOpen;
            }
            set
            {
                _drawerIsOpen = value;
                if (drawerControl != null)
                {
                    if (value)
                        drawerControl.Show();
                    else
                        drawerControl.Hide();
                }
            }
        }

        private bool _drawerUseColors;

        [Category("Drawer")]
        public bool DrawerUseColors
        {
            get
            {
                return _drawerUseColors;
            }
            set
            {
                _drawerUseColors = value;
                if (drawerControl != null)
                {
                    drawerControl.UseColors = value;
                    drawerControl.Refresh();
                }
            }
        }

        private bool _drawerHighlightWithAccent;

        [Category("Drawer")]
        public bool DrawerHighlightWithAccent
        {
            get
            {
                return _drawerHighlightWithAccent;
            }
            set
            {
                _drawerHighlightWithAccent = value;
                if (drawerControl != null)
                {
                    drawerControl.HighlightWithAccent = value;
                    drawerControl.Refresh();
                }
            }
        }

        private bool _backgroundWithAccent;

        [Category("Drawer")]
        public bool DrawerBackgroundWithAccent
        {
            get
            {
                return _backgroundWithAccent;
            }
            set
            {
                _backgroundWithAccent = value;
                if (drawerControl != null)
                {
                    drawerControl.BackgroundWithAccent = value;
                    drawerControl.Refresh();
                }
            }
        }

        private MaterialDrawer drawerControl = new MaterialDrawer();

        [Category("Drawer")]
        public MaterialTabControl DrawerTabControl { get; set; }

        private AnimationManager _drawerShowHideAnimManager;

        protected void AddDrawerOverlayForm()
        {
            if (DrawerTabControl == null)
                return;

            // Form opacity fade animation;
            _drawerShowHideAnimManager = new AnimationManager
            {
                AnimationType = AnimationType.EaseInOut,
                Increment = 0.04
            };

            _drawerShowHideAnimManager.OnAnimationProgress += (sender) =>
            {
                drawerOverlay.Opacity = (float)(_drawerShowHideAnimManager.GetProgress() * 0.55f);
            };

            int H = Size.Height - _statusBarBounds.Height - _actionBarBounds.Height;
            int Y = Location.Y + _statusBarBounds.Height + _actionBarBounds.Height;

            // Overlay Form definitions
            drawerOverlay.BackColor = Color.Black;
            drawerOverlay.Opacity = 0;
            drawerOverlay.MinimizeBox = false;
            drawerOverlay.MaximizeBox = false;
            drawerOverlay.Text = "";
            drawerOverlay.ShowIcon = false;
            drawerOverlay.ControlBox = false;
            drawerOverlay.FormBorderStyle = FormBorderStyle.None;
            drawerOverlay.Visible = true;
            drawerOverlay.Size = new Size(Size.Width, H);
            drawerOverlay.Location = new Point(Location.X, Y);
            drawerOverlay.ShowInTaskbar = false;
            drawerOverlay.Owner = this;
            drawerOverlay.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            // Drawer Form definitions
            drawerForm.BackColor = Color.LimeGreen;
            drawerForm.TransparencyKey = Color.LimeGreen;
            drawerForm.MinimizeBox = false;
            drawerForm.MaximizeBox = false;
            drawerForm.Text = "";
            drawerForm.ShowIcon = false;
            drawerForm.ControlBox = false;
            drawerForm.FormBorderStyle = FormBorderStyle.None;
            drawerForm.Visible = true;
            drawerForm.Size = new Size(DrawerWidth, H);
            drawerForm.Location = new Point(Location.X, Y);
            drawerForm.ShowInTaskbar = false;
            drawerForm.Owner = drawerOverlay;
            drawerForm.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            // Add drawer to overlay form
            drawerForm.Controls.Add(drawerControl);
            drawerControl.Location = new Point(0, 0);
            drawerControl.Size = new Size(DrawerWidth, H);
            drawerControl.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom);
            drawerControl.BaseTabControl = DrawerTabControl;
            drawerControl.ShowIconsWhenHidden = true;

            // Init Options
            drawerControl.IsOpen = DrawerIsOpen;
            drawerControl.ShowIconsWhenHidden = DrawerShowIconsWhenHidden;
            drawerControl.AutoHide = DrawerAutoHide;
            drawerControl.AutoShow = DrawerAutoShow;
            drawerControl.IndicatorWidth = DrawerIndicatorWidth;
            drawerControl.HighlightWithAccent = DrawerHighlightWithAccent;
            drawerControl.BackgroundWithAccent = DrawerBackgroundWithAccent;

            // Changing colors or theme
            SkinManager.ThemeChanged += sender =>
            {
                drawerForm.Refresh();
            };
            SkinManager.ColorSchemeChanged += sender =>
            {
                drawerForm.Refresh();
            };

            // Visible, Resize and move events
            VisibleChanged += (sender, e) =>
            {
                drawerForm.Visible = Visible;
                drawerOverlay.Visible = Visible;
            };

            Resize += (sender, e) =>
            {
                H = Size.Height - _statusBarBounds.Height - _actionBarBounds.Height;
                drawerForm.Size = new Size(DrawerWidth, H);
                drawerOverlay.Size = new Size(Size.Width, H);
            };

            Move += (sender, e) =>
            {
                Point pos = new Point(Location.X, Location.Y + _statusBarBounds.Height + _actionBarBounds.Height);
                drawerForm.Location = pos;
                drawerOverlay.Location = pos;
            };

            // Close when click outside menu
            drawerOverlay.Click += (sender, e) =>
            {
                drawerControl.Hide();
            };

            //Resize form when mouse over drawer
            drawerControl.MouseDown += (sender, e) =>
            {
                ResizeForm(_resizeDir);
            };

            // Animation and visibility
            drawerControl.DrawerBeginOpen += (sender) =>
            {
                _drawerShowHideAnimManager.StartNewAnimation(AnimationDirection.In);
            };

            drawerControl.DrawerBeginClose += (sender) =>
            {
                _drawerShowHideAnimManager.StartNewAnimation(AnimationDirection.Out);
            };
            drawerControl.CursorUpdate += (sender, drawerCursor) =>
            {
                if (Sizable && !_maximized)
                {
                    if (drawerCursor == Cursors.SizeNESW)
                        _resizeDir = ResizeDirection.BottomLeft;
                    else if (drawerCursor == Cursors.SizeWE)
                        _resizeDir = ResizeDirection.Left;
                    else if (drawerCursor == Cursors.SizeNS)
                        _resizeDir = ResizeDirection.Bottom;
                    else
                        _resizeDir = ResizeDirection.None;
                }
                else
                    _resizeDir = ResizeDirection.None;
                Cursor = drawerCursor;
            };

            // Form Padding corrections

            if (Padding.Top < (_statusBarBounds.Height + _actionBarBounds.Height))
                Padding = new Padding(Padding.Left, (_statusBarBounds.Height + _actionBarBounds.Height), Padding.Right, Padding.Bottom);

            originalPadding = Padding;

            drawerControl.DrawerShowIconsWhenHiddenChanged += FixFormPadding;
            FixFormPadding(this);

            // Fix Closing the Drawer or Overlay form with Alt+F4 not exiting the app
            drawerOverlay.FormClosed += TerminateOnClose;
            drawerForm.FormClosed += TerminateOnClose;
        }

        private void TerminateOnClose(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void FixFormPadding(object sender)
        {
            if (drawerControl.ShowIconsWhenHidden)
                Padding = new Padding(Padding.Left < drawerControl.MinWidth ? drawerControl.MinWidth : Padding.Left, originalPadding.Top, originalPadding.Right, originalPadding.Bottom);
            else
                Padding = new Padding(PADDING_MINIMUM, originalPadding.Top, originalPadding.Right, originalPadding.Bottom);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (DesignMode || IsDisposed)
                return;

            // Drawer
            if (DrawerTabControl != null && (m.Msg == WM_LBUTTONDOWN || m.Msg == WM_LBUTTONDBLCLK) && _drawerIconRect.Contains(PointToClient(Cursor.Position)))
            {
                drawerControl.Toggle();
                _clickAnimManager.SetProgress(0);
                _clickAnimManager.StartNewAnimation(AnimationDirection.In);
                _animationSource = (PointToClient(Cursor.Position));
            }
            // Double click to maximize
            else if (m.Msg == WM_LBUTTONDBLCLK)
            {
                if((_statusBarBounds.Contains(PointToClient(Cursor.Position)) || _actionBarBounds.Contains(PointToClient(Cursor.Position))) &&
                !(_minButtonBounds.Contains(PointToClient(Cursor.Position)) || _maxButtonBounds.Contains(PointToClient(Cursor.Position)) || _xButtonBounds.Contains(PointToClient(Cursor.Position))))
                    MaximizeWindow(!_maximized);
            }
            // move a maximized window
            else if (m.Msg == WM_MOUSEMOVE && _maximized &&
                (_statusBarBounds.Contains(PointToClient(Cursor.Position)) || _actionBarBounds.Contains(PointToClient(Cursor.Position))) &&
                !(_minButtonBounds.Contains(PointToClient(Cursor.Position)) || _maxButtonBounds.Contains(PointToClient(Cursor.Position)) || _xButtonBounds.Contains(PointToClient(Cursor.Position))))
            {
                if (_headerMouseDown)
                {
                    _maximized = false;
                    _headerMouseDown = false;

                    var mousePoint = PointToClient(Cursor.Position);
                    if (mousePoint.X < Width / 2)
                        Location = mousePoint.X < _previousSize.Width / 2 ?
                            new Point(Cursor.Position.X - mousePoint.X, Cursor.Position.Y - mousePoint.Y) :
                            new Point(Cursor.Position.X - _previousSize.Width / 2, Cursor.Position.Y - mousePoint.Y);
                    else
                        Location = Width - mousePoint.X < _previousSize.Width / 2 ?
                            new Point(Cursor.Position.X - _previousSize.Width + Width - mousePoint.X, Cursor.Position.Y - mousePoint.Y) :
                            new Point(Cursor.Position.X - _previousSize.Width / 2, Cursor.Position.Y - mousePoint.Y);

                    Size = _previousSize;
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
            // Status bar buttons
            else if (m.Msg == WM_LBUTTONDOWN &&
                (_statusBarBounds.Contains(PointToClient(Cursor.Position)) || _actionBarBounds.Contains(PointToClient(Cursor.Position))) &&
                !(_minButtonBounds.Contains(PointToClient(Cursor.Position)) || _maxButtonBounds.Contains(PointToClient(Cursor.Position)) || _xButtonBounds.Contains(PointToClient(Cursor.Position))))
            {
                if (!_maximized)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
                else
                {
                    _headerMouseDown = true;
                }
            }
            // Default context menu
            else if (m.Msg == WM_RBUTTONDOWN)
            {
                Point cursorPos = PointToClient(Cursor.Position);

                if (_statusBarBounds.Contains(cursorPos) && !_minButtonBounds.Contains(cursorPos) &&
                    !_maxButtonBounds.Contains(cursorPos) && !_xButtonBounds.Contains(cursorPos))
                {
                    // Show default system menu when right clicking titlebar
                    var id = TrackPopupMenuEx(GetSystemMenu(Handle, false), TPM_LEFTALIGN | TPM_RETURNCMD, Cursor.Position.X, Cursor.Position.Y, Handle, IntPtr.Zero);

                    // Pass the command as a WM_SYSCOMMAND message
                    SendMessage(Handle, WM_SYSCOMMAND, id, 0);
                }
            }
            else if (m.Msg == WM_NCLBUTTONDOWN)
            {
                // This re-enables resizing by letting the application know when the
                // user is trying to resize a side. This is disabled by default when using WS_SYSMENU.
                if (!Sizable || !_resizeCursors.Contains(Cursor))
                    return;

                byte bFlag = 0;

                // Get which side to resize from
                if (_resizingLocationsToCmd.ContainsKey((int)m.WParam))
                    bFlag = (byte)_resizingLocationsToCmd[(int)m.WParam];

                if (bFlag != 0)
                    SendMessage(Handle, WM_SYSCOMMAND, 0xF000 | bFlag, (int)m.LParam);
            }
            else if (m.Msg == WM_LBUTTONUP)
            {
                _headerMouseDown = false;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var par = base.CreateParams;
                // WS_SYSMENU: Trigger the creation of the system menu
                // WS_MINIMIZEBOX: Allow minimizing from taskbar
                par.Style = par.Style | WS_MINIMIZEBOX | WS_SYSMENU; // Turn on the WS_MINIMIZEBOX style flag
                return par;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (DesignMode)
                return;
            UpdateButtons(e);

            if (e.Button == MouseButtons.Left && !_maximized && _resizeCursors.Contains(Cursor))
                ResizeForm(_resizeDir);
            base.OnMouseDown(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Cursor = Cursors.Default;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (DesignMode)
                return;
            _buttonState = ButtonState.None;
            _resizeDir = ResizeDirection.None;
            //Only reset the cursor when needed
            if (_resizeCursors.Contains(Cursor))
            {
                Cursor = Cursors.Default;
            }

            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (DesignMode)
                return;

            if (Sizable)
            {
                //True if the mouse is hovering over a child control
                var isChildUnderMouse = GetChildAtPoint(e.Location) != null;

                if (e.Location.X < BORDER_WIDTH && e.Location.Y > Height - BORDER_WIDTH && !isChildUnderMouse && !_maximized)
                {
                    _resizeDir = ResizeDirection.BottomLeft;
                    Cursor = Cursors.SizeNESW;
                }
                else if (e.Location.X < BORDER_WIDTH && (!isChildUnderMouse || DrawerTabControl != null) && !_maximized)
                {
                    _resizeDir = ResizeDirection.Left;
                    Cursor = Cursors.SizeWE;
                }
                else if (e.Location.X > Width - BORDER_WIDTH && e.Location.Y > Height - BORDER_WIDTH && !isChildUnderMouse && !_maximized)
                {
                    _resizeDir = ResizeDirection.BottomRight;
                    Cursor = Cursors.SizeNWSE;
                }
                else if (e.Location.X > Width - BORDER_WIDTH && !isChildUnderMouse && !_maximized)
                {
                    _resizeDir = ResizeDirection.Right;
                    Cursor = Cursors.SizeWE;
                }
                else if (e.Location.Y > Height - BORDER_WIDTH && !isChildUnderMouse && !_maximized)
                {
                    _resizeDir = ResizeDirection.Bottom;
                    Cursor = Cursors.SizeNS;
                }
                else
                {
                    _resizeDir = ResizeDirection.None;

                    //Only reset the cursor when needed, this prevents it from flickering when a child control changes the cursor to its own needs
                    if (_resizeCursors.Contains(Cursor))
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }

            UpdateButtons(e);
        }

        private void UpdateButtons(MouseEventArgs e, bool up = false)
        {
            if (DesignMode)
                return;
            var oldState = _buttonState;
            bool showMin = MinimizeBox && ControlBox;
            bool showMax = MaximizeBox && ControlBox;

            if (e.Button == MouseButtons.Left && !up)
            {
                if (showMin && !showMax && _maxButtonBounds.Contains(e.Location))
                    _buttonState = ButtonState.MinDown;
                else if (showMin && showMax && _minButtonBounds.Contains(e.Location))
                    _buttonState = ButtonState.MinDown;
                else if (showMax && _maxButtonBounds.Contains(e.Location))
                    _buttonState = ButtonState.MaxDown;
                else if (ControlBox && _xButtonBounds.Contains(e.Location))
                    _buttonState = ButtonState.XDown;
                else if (_drawerButtonBounds.Contains(e.Location))
                    _buttonState = ButtonState.DrawerDown;
                else
                    _buttonState = ButtonState.None;
            }
            else
            {
                if (showMin && !showMax && _maxButtonBounds.Contains(e.Location))
                {
                    _buttonState = ButtonState.MinOver;

                    if (oldState == ButtonState.MinDown && up)
                        WindowState = FormWindowState.Minimized;
                }
                else if (showMin && showMax && _minButtonBounds.Contains(e.Location))
                {
                    _buttonState = ButtonState.MinOver;

                    if (oldState == ButtonState.MinDown && up)
                        WindowState = FormWindowState.Minimized;
                }
                else if (MaximizeBox && ControlBox && _maxButtonBounds.Contains(e.Location))
                {
                    _buttonState = ButtonState.MaxOver;

                    if (oldState == ButtonState.MaxDown && up)
                        MaximizeWindow(!_maximized);
                }
                else if (ControlBox && _xButtonBounds.Contains(e.Location))
                {
                    _buttonState = ButtonState.XOver;

                    if (oldState == ButtonState.XDown && up)
                        Close();
                }
                else if (_drawerButtonBounds.Contains(e.Location))
                {
                    _buttonState = ButtonState.DrawerOver;
                }
                else
                {
                    _buttonState = ButtonState.None;
                }
            }

            if (oldState != _buttonState)
                Invalidate();
        }

        private void MaximizeWindow(bool maximize)
        {
            if (!MaximizeBox || !ControlBox)
                return;

            _maximized = maximize;

            if (maximize)
            {
                var monitorHandle = MonitorFromWindow(Handle, MONITOR_DEFAULTTONEAREST);
                var monitorInfo = new MONITORINFOEX();
                GetMonitorInfo(new HandleRef(null, monitorHandle), monitorInfo);
                _previousSize = Size;
                _previousLocation = Location;
                Size = new Size(monitorInfo.rcWork.Width(), monitorInfo.rcWork.Height());
                Location = new Point(monitorInfo.rcWork.left, monitorInfo.rcWork.top);
            }
            else
            {
                Size = _previousSize;
                Location = _previousLocation;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (DesignMode)
                return;
            UpdateButtons(e, true);

            base.OnMouseUp(e);
            ReleaseCapture();
        }

        private void ResizeForm(ResizeDirection direction)
        {
            if (DesignMode)
                return;
            var dir = -1;
            switch (direction)
            {
                case ResizeDirection.BottomLeft:
                    dir = HTBOTTOMLEFT;
                    Cursor = Cursors.SizeNESW;
                    break;

                case ResizeDirection.Left:
                    dir = HTLEFT;
                    Cursor = Cursors.SizeWE;
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
            UpdateRects();
        }

        private void UpdateRects()
        {
            _minButtonBounds = new Rectangle((Width) - 3 * STATUS_BAR_BUTTON_WIDTH, 0, STATUS_BAR_BUTTON_WIDTH, STATUS_BAR_HEIGHT);
            _maxButtonBounds = new Rectangle((Width) - 2 * STATUS_BAR_BUTTON_WIDTH, 0, STATUS_BAR_BUTTON_WIDTH, STATUS_BAR_HEIGHT);
            _xButtonBounds = new Rectangle((Width) - STATUS_BAR_BUTTON_WIDTH, 0, STATUS_BAR_BUTTON_WIDTH, STATUS_BAR_HEIGHT);
            _statusBarBounds = new Rectangle(0, 0, Width, STATUS_BAR_HEIGHT);
            _actionBarBounds = new Rectangle(0, STATUS_BAR_HEIGHT, Width, ACTION_BAR_HEIGHT);
            _drawerButtonBounds = new Rectangle((SkinManager.FORM_PADDING / 2)+3, STATUS_BAR_HEIGHT + (ACTION_BAR_HEIGHT/2) - (ACTION_BAR_HEIGHT_DEFAULT/2), ACTION_BAR_HEIGHT_DEFAULT, ACTION_BAR_HEIGHT_DEFAULT);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var hoverBrush = SkinManager.BackgroundHoverBrush;
            var downBrush = SkinManager.BackgroundFocusBrush;
            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            g.Clear(SkinManager.BackdropColor);

            //Draw border
            using (var borderPen = new Pen(SkinManager.DividersColor, 1))
            {
                g.DrawLine(borderPen, new Point(0, _actionBarBounds.Bottom), new Point(0, Height - 2));
                g.DrawLine(borderPen, new Point(Width - 1, _actionBarBounds.Bottom), new Point(Width - 1, Height - 2));
                g.DrawLine(borderPen, new Point(0, Height - 1), new Point(Width - 1, Height - 1));
            }

            if (_formStyle != FormStyles.StatusAndActionBar_None)
            {
                if (ControlBox == true)
                {
                    g.FillRectangle(SkinManager.ColorScheme.DarkPrimaryBrush, _statusBarBounds);
                    g.FillRectangle(SkinManager.ColorScheme.PrimaryBrush, _actionBarBounds);
                }

                // Determine whether or not we even should be drawing the buttons.
                bool showMin = MinimizeBox && ControlBox;
                bool showMax = MaximizeBox && ControlBox;

                // When MaximizeButton == false, the minimize button will be painted in its place
                if (_buttonState == ButtonState.MinOver && showMin)
                    g.FillRectangle(hoverBrush, showMax ? _minButtonBounds : _maxButtonBounds);

                if (_buttonState == ButtonState.MinDown && showMin)
                    g.FillRectangle(downBrush, showMax ? _minButtonBounds : _maxButtonBounds);

                if (_buttonState == ButtonState.MaxOver && showMax)
                    g.FillRectangle(hoverBrush, _maxButtonBounds);

                if (_buttonState == ButtonState.MaxDown && showMax)
                    g.FillRectangle(downBrush, _maxButtonBounds);

                if (_buttonState == ButtonState.XOver && ControlBox)
                    g.FillRectangle(SkinManager.BackgroundHoverRedBrush, _xButtonBounds);

                if (_buttonState == ButtonState.XDown && ControlBox)
                    g.FillRectangle(SkinManager.BackgroundDownRedBrush, _xButtonBounds);

                using (var formButtonsPen = new Pen(SkinManager.ColorScheme.TextColor, 2))
                {
                    // Minimize button.
                    if (showMin)
                    {
                        int x = showMax ? _minButtonBounds.X : _maxButtonBounds.X;
                        int y = showMax ? _minButtonBounds.Y : _maxButtonBounds.Y;

                        g.DrawLine(
                            formButtonsPen,
                            x + (int)(_minButtonBounds.Width * 0.33),
                            y + (int)(_minButtonBounds.Height * 0.66),
                            x + (int)(_minButtonBounds.Width * 0.66),
                            y + (int)(_minButtonBounds.Height * 0.66)
                       );
                    }

                    // Maximize button
                    if (showMax)
                    {
                        g.DrawRectangle(
                            formButtonsPen,
                            _maxButtonBounds.X + (int)(_maxButtonBounds.Width * 0.33),
                            _maxButtonBounds.Y + (int)(_maxButtonBounds.Height * 0.36),
                            (int)(_maxButtonBounds.Width * 0.39),
                            (int)(_maxButtonBounds.Height * 0.31)
                       );
                    }

                    // Close button
                    if (ControlBox)
                    {
                        g.DrawLine(
                            formButtonsPen,
                            _xButtonBounds.X + (int)(_xButtonBounds.Width * 0.33),
                            _xButtonBounds.Y + (int)(_xButtonBounds.Height * 0.33),
                            _xButtonBounds.X + (int)(_xButtonBounds.Width * 0.66),
                            _xButtonBounds.Y + (int)(_xButtonBounds.Height * 0.66)
                       );

                        g.DrawLine(
                            formButtonsPen,
                            _xButtonBounds.X + (int)(_xButtonBounds.Width * 0.66),
                            _xButtonBounds.Y + (int)(_xButtonBounds.Height * 0.33),
                            _xButtonBounds.X + (int)(_xButtonBounds.Width * 0.33),
                            _xButtonBounds.Y + (int)(_xButtonBounds.Height * 0.66));
                    }
                }
            }

            // Drawer Icon
            if (DrawerTabControl != null && _formStyle != FormStyles.ActionBar_None && _formStyle != FormStyles.StatusAndActionBar_None)
            {
                if (_buttonState == ButtonState.DrawerOver)
                    g.FillRectangle(hoverBrush, _drawerButtonBounds);

                if (_buttonState == ButtonState.DrawerDown)
                    g.FillRectangle(downBrush, _drawerButtonBounds);

                _drawerIconRect = new Rectangle(SkinManager.FORM_PADDING / 2, STATUS_BAR_HEIGHT, ACTION_BAR_HEIGHT_DEFAULT, ACTION_BAR_HEIGHT);
                // Ripple
                if (_clickAnimManager.IsAnimating())
                {
                    var clickAnimProgress = _clickAnimManager.GetProgress();

                    var rippleBrush = new SolidBrush(Color.FromArgb((int)(51 - (clickAnimProgress * 50)), Color.White));
                    var rippleSize = (int)(clickAnimProgress * _drawerIconRect.Width * 1.75);

                    g.SetClip(_drawerIconRect);
                    g.FillEllipse(rippleBrush, new Rectangle(_animationSource.X - rippleSize / 2, _animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
                    g.ResetClip();
                    rippleBrush.Dispose();
                }

                using (var formButtonsPen = new Pen(SkinManager.ColorScheme.TextColor, 2))
                {
                    // Middle line
                    g.DrawLine(
                       formButtonsPen,
                       _drawerIconRect.X + (int)(SkinManager.FORM_PADDING),
                       _drawerIconRect.Y + (int)(ACTION_BAR_HEIGHT / 2),
                       _drawerIconRect.X + (int)(SkinManager.FORM_PADDING) + 18,
                       _drawerIconRect.Y + (int)(ACTION_BAR_HEIGHT / 2));

                    // Bottom line
                    g.DrawLine(
                       formButtonsPen,
                       _drawerIconRect.X + (int)(SkinManager.FORM_PADDING),
                       _drawerIconRect.Y + (int)(ACTION_BAR_HEIGHT / 2) - 6,
                       _drawerIconRect.X + (int)(SkinManager.FORM_PADDING) + 18,
                       _drawerIconRect.Y + (int)(ACTION_BAR_HEIGHT / 2) - 6);

                    // Top line
                    g.DrawLine(
                       formButtonsPen,
                       _drawerIconRect.X + (int)(SkinManager.FORM_PADDING),
                       _drawerIconRect.Y + (int)(ACTION_BAR_HEIGHT / 2) + 6,
                       _drawerIconRect.X + (int)(SkinManager.FORM_PADDING) + 18,
                       _drawerIconRect.Y + (int)(ACTION_BAR_HEIGHT / 2) + 6);
                }
            }

            if (ControlBox== true && _formStyle != FormStyles.ActionBar_None && _formStyle != FormStyles.StatusAndActionBar_None)
            {
                //Form title
                using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
                {
                    Rectangle textLocation = new Rectangle(DrawerTabControl != null ? TITLE_LEFT_PADDING : TITLE_LEFT_PADDING - (ICON_SIZE + (ACTION_BAR_PADDING*2)), STATUS_BAR_HEIGHT, Width, ACTION_BAR_HEIGHT);
                    NativeText.DrawTransparentText(Text, SkinManager.getLogFontByType(MaterialSkinManager.fontType.H6),
                        SkinManager.ColorScheme.TextColor,
                        textLocation.Location,
                        textLocation.Size,
                        NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
                }
            }
        }

        private readonly AnimationManager _clickAnimManager;

        private Rectangle _drawerIconRect;

        private Point _animationSource;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            //
            // MaterialForm
            //
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "MaterialForm";
            this.Padding = new System.Windows.Forms.Padding(PADDING_MINIMUM, 64, PADDING_MINIMUM, PADDING_MINIMUM);
            this.Load += new System.EventHandler(this.MaterialForm_Load);
            this.ResumeLayout(false);
        }

        private void MaterialForm_Load(object sender, EventArgs e)
        {
        }
    }

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
}
