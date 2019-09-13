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
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="MaterialForm" />
    /// </summary>
    public class MaterialForm : Form, IMaterialControl
    {
        /// <summary>
        /// Gets or sets the Depth
        /// </summary>
        [Browsable(false)]
        public int Depth { get; set; }

        /// <summary>
        /// Gets the SkinManager
        /// </summary>
        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        /// <summary>
        /// Gets or sets the MouseState
        /// </summary>
        [Browsable(false)]
        public MouseState MouseState { get; set; }

        /// <summary>
        /// Gets or sets the FormBorderStyle
        /// </summary>
        public new FormBorderStyle FormBorderStyle
        {
            get { return base.FormBorderStyle; }
            set { base.FormBorderStyle = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Sizable
        /// </summary>
        [Category("Layout")]
        public bool Sizable { get; set; }

        /// <summary>
        /// The SendMessage
        /// </summary>
        /// <param name="hWnd">The hWnd<see cref="IntPtr"/></param>
        /// <param name="Msg">The Msg<see cref="int"/></param>
        /// <param name="wParam">The wParam<see cref="int"/></param>
        /// <param name="lParam">The lParam<see cref="int"/></param>
        /// <returns>The <see cref="int"/></returns>
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        /// <summary>
        /// The ReleaseCapture
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        /// <summary>
        /// The TrackPopupMenuEx
        /// </summary>
        /// <param name="hmenu">The hmenu<see cref="IntPtr"/></param>
        /// <param name="fuFlags">The fuFlags<see cref="uint"/></param>
        /// <param name="x">The x<see cref="int"/></param>
        /// <param name="y">The y<see cref="int"/></param>
        /// <param name="hwnd">The hwnd<see cref="IntPtr"/></param>
        /// <param name="lptpm">The lptpm<see cref="IntPtr"/></param>
        /// <returns>The <see cref="int"/></returns>
        [DllImport("user32.dll")]
        public static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

        /// <summary>
        /// The GetSystemMenu
        /// </summary>
        /// <param name="hWnd">The hWnd<see cref="IntPtr"/></param>
        /// <param name="bRevert">The bRevert<see cref="bool"/></param>
        /// <returns>The <see cref="IntPtr"/></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        /// <summary>
        /// The MonitorFromWindow
        /// </summary>
        /// <param name="hwnd">The hwnd<see cref="IntPtr"/></param>
        /// <param name="dwFlags">The dwFlags<see cref="uint"/></param>
        /// <returns>The <see cref="IntPtr"/></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        /// <summary>
        /// The GetMonitorInfo
        /// </summary>
        /// <param name="hmonitor">The hmonitor<see cref="HandleRef"/></param>
        /// <param name="info">The info<see cref="MONITORINFOEX"/></param>
        /// <returns>The <see cref="bool"/></returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out] MONITORINFOEX info);

        /// <summary>
        /// Defines the WM_NCLBUTTONDOWN
        /// </summary>
        public const int WM_NCLBUTTONDOWN = 0xA1;

        /// <summary>
        /// Defines the HT_CAPTION
        /// </summary>
        public const int HT_CAPTION = 0x2;

        /// <summary>
        /// Defines the WM_MOUSEMOVE
        /// </summary>
        public const int WM_MOUSEMOVE = 0x0200;

        /// <summary>
        /// Defines the WM_LBUTTONDOWN
        /// </summary>
        public const int WM_LBUTTONDOWN = 0x0201;

        /// <summary>
        /// Defines the WM_LBUTTONUP
        /// </summary>
        public const int WM_LBUTTONUP = 0x0202;

        /// <summary>
        /// Defines the WM_LBUTTONDBLCLK
        /// </summary>
        public const int WM_LBUTTONDBLCLK = 0x0203;

        /// <summary>
        /// Defines the WM_RBUTTONDOWN
        /// </summary>
        public const int WM_RBUTTONDOWN = 0x0204;

        /// <summary>
        /// Defines the HTBOTTOMLEFT
        /// </summary>
        private const int HTBOTTOMLEFT = 16;

        /// <summary>
        /// Defines the HTBOTTOMRIGHT
        /// </summary>
        private const int HTBOTTOMRIGHT = 17;

        /// <summary>
        /// Defines the HTLEFT
        /// </summary>
        private const int HTLEFT = 10;

        /// <summary>
        /// Defines the HTRIGHT
        /// </summary>
        private const int HTRIGHT = 11;

        /// <summary>
        /// Defines the HTBOTTOM
        /// </summary>
        private const int HTBOTTOM = 15;

        /// <summary>
        /// Defines the HTTOP
        /// </summary>
        private const int HTTOP = 12;

        /// <summary>
        /// Defines the HTTOPLEFT
        /// </summary>
        private const int HTTOPLEFT = 13;

        /// <summary>
        /// Defines the HTTOPRIGHT
        /// </summary>
        private const int HTTOPRIGHT = 14;

        /// <summary>
        /// Defines the BORDER_WIDTH
        /// </summary>
        private const int BORDER_WIDTH = 7;

        /// <summary>
        /// Defines the _resizeDir
        /// </summary>
        private ResizeDirection _resizeDir;

        /// <summary>
        /// Defines the _buttonState
        /// </summary>
        private ButtonState _buttonState = ButtonState.None;

        /// <summary>
        /// Defines the WMSZ_TOP
        /// </summary>
        private const int WMSZ_TOP = 3;

        /// <summary>
        /// Defines the WMSZ_TOPLEFT
        /// </summary>
        private const int WMSZ_TOPLEFT = 4;

        /// <summary>
        /// Defines the WMSZ_TOPRIGHT
        /// </summary>
        private const int WMSZ_TOPRIGHT = 5;

        /// <summary>
        /// Defines the WMSZ_LEFT
        /// </summary>
        private const int WMSZ_LEFT = 1;

        /// <summary>
        /// Defines the WMSZ_RIGHT
        /// </summary>
        private const int WMSZ_RIGHT = 2;

        /// <summary>
        /// Defines the WMSZ_BOTTOM
        /// </summary>
        private const int WMSZ_BOTTOM = 6;

        /// <summary>
        /// Defines the WMSZ_BOTTOMLEFT
        /// </summary>
        private const int WMSZ_BOTTOMLEFT = 7;

        /// <summary>
        /// Defines the WMSZ_BOTTOMRIGHT
        /// </summary>
        private const int WMSZ_BOTTOMRIGHT = 8;

        /// <summary>
        /// Defines the _resizingLocationsToCmd
        /// </summary>
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

        /// <summary>
        /// Defines the STATUS_BAR_BUTTON_WIDTH
        /// </summary>
        private const int STATUS_BAR_BUTTON_WIDTH = STATUS_BAR_HEIGHT;

        /// <summary>
        /// Defines the STATUS_BAR_HEIGHT
        /// </summary>
        private const int STATUS_BAR_HEIGHT = 24;

        /// <summary>
        /// Defines the ACTION_BAR_HEIGHT
        /// </summary>
        private const int ACTION_BAR_HEIGHT = 40;

        /// <summary>
        /// Defines the TPM_LEFTALIGN
        /// </summary>
        private const uint TPM_LEFTALIGN = 0x0000;

        /// <summary>
        /// Defines the TPM_RETURNCMD
        /// </summary>
        private const uint TPM_RETURNCMD = 0x0100;

        /// <summary>
        /// Defines the WM_SYSCOMMAND
        /// </summary>
        private const int WM_SYSCOMMAND = 0x0112;

        /// <summary>
        /// Defines the WS_MINIMIZEBOX
        /// </summary>
        private const int WS_MINIMIZEBOX = 0x20000;

        /// <summary>
        /// Defines the WS_SYSMENU
        /// </summary>
        private const int WS_SYSMENU = 0x00080000;

        /// <summary>
        /// Defines the MONITOR_DEFAULTTONEAREST
        /// </summary>
        private const int MONITOR_DEFAULTTONEAREST = 2;

        /// <summary>
        /// Defines the <see cref="MONITORINFOEX" />
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        public class MONITORINFOEX
        {
            /// <summary>
            /// Defines the cbSize
            /// </summary>
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));

            /// <summary>
            /// Defines the rcMonitor
            /// </summary>
            public RECT rcMonitor = new RECT();

            /// <summary>
            /// Defines the rcWork
            /// </summary>
            public RECT rcWork = new RECT();

            /// <summary>
            /// Defines the dwFlags
            /// </summary>
            public int dwFlags = 0;

            /// <summary>
            /// Defines the szDevice
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] szDevice = new char[32];
        }

        /// <summary>
        /// Defines the <see cref="RECT" />
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            /// <summary>
            /// Defines the left
            /// </summary>
            public int left;

            /// <summary>
            /// Defines the top
            /// </summary>
            public int top;

            /// <summary>
            /// Defines the right
            /// </summary>
            public int right;

            /// <summary>
            /// Defines the bottom
            /// </summary>
            public int bottom;

            /// <summary>
            /// The Width
            /// </summary>
            /// <returns>The <see cref="int"/></returns>
            public int Width()
            {
                return right - left;
            }

            /// <summary>
            /// The Height
            /// </summary>
            /// <returns>The <see cref="int"/></returns>
            public int Height()
            {
                return bottom - top;
            }
        }

        /// <summary>
        /// Defines the ResizeDirection
        /// </summary>
        private enum ResizeDirection
        {
            /// <summary>
            /// Defines the BottomLeft
            /// </summary>
            BottomLeft,

            /// <summary>
            /// Defines the Left
            /// </summary>
            Left,

            /// <summary>
            /// Defines the Right
            /// </summary>
            Right,

            /// <summary>
            /// Defines the BottomRight
            /// </summary>
            BottomRight,

            /// <summary>
            /// Defines the Bottom
            /// </summary>
            Bottom,

            /// <summary>
            /// Defines the None
            /// </summary>
            None
        }

        /// <summary>
        /// Defines the ButtonState
        /// </summary>
        private enum ButtonState
        {
            /// <summary>
            /// Defines the XOver
            /// </summary>
            XOver,

            /// <summary>
            /// Defines the MaxOver
            /// </summary>
            MaxOver,

            /// <summary>
            /// Defines the MinOver
            /// </summary>
            MinOver,

            /// <summary>
            /// Defines the XDown
            /// </summary>
            XDown,

            /// <summary>
            /// Defines the MaxDown
            /// </summary>
            MaxDown,

            /// <summary>
            /// Defines the MinDown
            /// </summary>
            MinDown,

            /// <summary>
            /// Defines the None
            /// </summary>
            None
        }

        /// <summary>
        /// Defines the _resizeCursors
        /// </summary>
        private readonly Cursor[] _resizeCursors = { Cursors.SizeNESW, Cursors.SizeWE, Cursors.SizeNWSE, Cursors.SizeWE, Cursors.SizeNS };

        /// <summary>
        /// Defines the _minButtonBounds
        /// </summary>
        private Rectangle _minButtonBounds;

        /// <summary>
        /// Defines the _maxButtonBounds
        /// </summary>
        private Rectangle _maxButtonBounds;

        /// <summary>
        /// Defines the _xButtonBounds
        /// </summary>
        private Rectangle _xButtonBounds;

        /// <summary>
        /// Defines the _actionBarBounds
        /// </summary>
        private Rectangle _actionBarBounds;

        public Rectangle UserArea
        {
            get
            {
                return new Rectangle(0, STATUS_BAR_HEIGHT + ACTION_BAR_HEIGHT, Width, Height - (STATUS_BAR_HEIGHT + ACTION_BAR_HEIGHT));
            }
        }

        /// <summary>
        /// Defines the _statusBarBounds
        /// </summary>
        private Rectangle _statusBarBounds;

        /// <summary>
        /// Defines the _maximized
        /// </summary>
        private bool _maximized;

        /// <summary>
        /// Defines the _previousSize
        /// </summary>
        private Size _previousSize;

        /// <summary>
        /// Defines the _previousLocation
        /// </summary>
        private Point _previousLocation;

        /// <summary>
        /// Defines the _headerMouseDown
        /// </summary>
        private bool _headerMouseDown;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialForm"/> class.
        /// </summary>
        public MaterialForm()
        {
            DrawerWidth = 200;
            DrawerIsOpen = false;
            DrawerShowIconsWhenHidden = false;
            DrawerAutoHide = true;
            DrawerIndicatorWidth = 0;

            FormBorderStyle = FormBorderStyle.None;
            Sizable = true;
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            // This enables the form to trigger the MouseMove event even when mouse is over another control
            Application.AddMessageFilter(new MouseMessageFilter());
            MouseMessageFilter.MouseMove += OnGlobalMouseMove;

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
        [Category("Drawer")]
        public bool DrawerShowIconsWhenHidden { get; set; }
        [Category("Drawer")]
        public int DrawerWidth { get; set; }
        [Category("Drawer")]
        public bool DrawerAutoHide { get; set; }
        [Category("Drawer")]
        public int DrawerIndicatorWidth { get; set; }
        private bool _isOpen;
        [Category("Drawer")]
        public bool DrawerIsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                _isOpen = value;
                if (drawerControl != null)
                {
                    if (value)
                        drawerControl.Show();
                    else
                        drawerControl.Hide();
                }
            }
        }

        MaterialDrawer drawerControl = new MaterialDrawer();
        private MaterialTabControl _drawerTabControl;
        [Category("Drawer")]
        public MaterialTabControl DrawerTabControl
        {
            get
            {
                return _drawerTabControl;
            }
            set
            {
                _drawerTabControl = value;
            }
        }

        private AnimationManager _showHideAnimManager;

        protected void AddDrawerOverlayForm()
        {
            Form drawerOverlay = new Form();
            Form drawerForm = new Form();

            if (DrawerTabControl == null)
                return;

            // Form opacity fade animation;
            _showHideAnimManager = new AnimationManager
            {
                AnimationType = AnimationType.EaseInOut,
                Increment = 0.04
            };

            _showHideAnimManager.OnAnimationProgress += (sender) =>
            {
                drawerOverlay.Opacity = (float)(_showHideAnimManager.GetProgress() * 0.55f);
            };


            int H = Size.Height - _statusBarBounds.Height - _actionBarBounds.Height;
            int Y = Location.Y + _statusBarBounds.Height + _actionBarBounds.Height;

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
            drawerControl.IndicatorWidth = DrawerIndicatorWidth;

            // Changing colors or theme
            SkinManager.ThemeChanged += sender =>
            {
                drawerForm.Refresh();
            };
            SkinManager.ColorSchemeChanged += sender =>
            {
                drawerForm.Refresh();
            };

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

            // Resize and move events
            Resize += (sender, e) =>
            {
                H = Size.Height - _statusBarBounds.Height - _actionBarBounds.Height;
                drawerForm.Size = new Size(DrawerIndicatorWidth, H);
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

            // Animation and visibility
            drawerControl.DrawerBeginOpen += (sender) =>
            {
                _showHideAnimManager.StartNewAnimation(AnimationDirection.In);
            };

            drawerControl.DrawerBeginClose += (sender) =>
            {
                _showHideAnimManager.StartNewAnimation(AnimationDirection.Out);
            };
        }


        /// <summary>
        /// The WndProc
        /// </summary>
        /// <param name="m">The m<see cref="Message"/></param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (DesignMode || IsDisposed)
                return;

            // Drawer
            if (_drawerTabControl != null && (m.Msg == WM_LBUTTONDOWN || m.Msg == WM_LBUTTONDBLCLK) && _drawerIconRect.Contains(PointToClient(Cursor.Position)))
            {
                drawerControl.Toggle();
                _clickAnimManager.SetProgress(0);
                _clickAnimManager.StartNewAnimation(AnimationDirection.In);
                _animationSource = (PointToClient(Cursor.Position));
            }
            // Double click to maximize
            else if (m.Msg == WM_LBUTTONDBLCLK)
            {
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
                if (!Sizable)
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

        /// <summary>
        /// Gets the CreateParams
        /// </summary>
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

        /// <summary>
        /// The OnMouseDown
        /// </summary>
        /// <param name="e">The e<see cref="MouseEventArgs"/></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (DesignMode)
                return;
            UpdateButtons(e);

            if (e.Button == MouseButtons.Left && !_maximized)
                ResizeForm(_resizeDir);
            base.OnMouseDown(e);
        }

        /// <summary>
        /// The OnMouseLeave
        /// </summary>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (DesignMode)
                return;
            _buttonState = ButtonState.None;
            Invalidate();
        }

        /// <summary>
        /// The OnMouseMove
        /// </summary>
        /// <param name="e">The e<see cref="MouseEventArgs"/></param>
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
                else if (e.Location.X < BORDER_WIDTH && !isChildUnderMouse && !_maximized)
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

        /// <summary>
        /// The OnGlobalMouseMove
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="MouseEventArgs"/></param>
        protected void OnGlobalMouseMove(object sender, MouseEventArgs e)
        {
            if (IsDisposed)
                return;
            // Convert to client position and pass to Form.MouseMove
            var clientCursorPos = PointToClient(e.Location);
            var newE = new MouseEventArgs(MouseButtons.None, 0, clientCursorPos.X, clientCursorPos.Y, 0);
            OnMouseMove(newE);
        }

        /// <summary>
        /// The UpdateButtons
        /// </summary>
        /// <param name="e">The e<see cref="MouseEventArgs"/></param>
        /// <param name="up">The up<see cref="bool"/></param>
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
                else
                    _buttonState = ButtonState.None;
            }

            if (oldState != _buttonState)
                Invalidate();
        }

        /// <summary>
        /// The MaximizeWindow
        /// </summary>
        /// <param name="maximize">The maximize<see cref="bool"/></param>
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

        /// <summary>
        /// The OnMouseUp
        /// </summary>
        /// <param name="e">The e<see cref="MouseEventArgs"/></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (DesignMode)
                return;
            UpdateButtons(e, true);

            base.OnMouseUp(e);
            ReleaseCapture();
        }

        /// <summary>
        /// The ResizeForm
        /// </summary>
        /// <param name="direction">The direction<see cref="ResizeDirection"/></param>
        private void ResizeForm(ResizeDirection direction)
        {
            if (DesignMode)
                return;
            var dir = -1;
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

        /// <summary>
        /// The OnResize
        /// </summary>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            _minButtonBounds = new Rectangle((Width) - 3 * STATUS_BAR_BUTTON_WIDTH, 0, STATUS_BAR_BUTTON_WIDTH, STATUS_BAR_HEIGHT);
            _maxButtonBounds = new Rectangle((Width) - 2 * STATUS_BAR_BUTTON_WIDTH, 0, STATUS_BAR_BUTTON_WIDTH, STATUS_BAR_HEIGHT);
            _xButtonBounds = new Rectangle((Width) - STATUS_BAR_BUTTON_WIDTH, 0, STATUS_BAR_BUTTON_WIDTH, STATUS_BAR_HEIGHT);
            _statusBarBounds = new Rectangle(0, 0, Width, STATUS_BAR_HEIGHT);
            _actionBarBounds = new Rectangle(0, STATUS_BAR_HEIGHT, Width, ACTION_BAR_HEIGHT);
        }

        /// <summary>
        /// The OnPaint
        /// </summary>
        /// <param name="e">The e<see cref="PaintEventArgs"/></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            g.Clear(SkinManager.GetApplicationBackgroundColor());
            g.FillRectangle(SkinManager.ColorScheme.DarkPrimaryBrush, _statusBarBounds);
            g.FillRectangle(SkinManager.ColorScheme.PrimaryBrush, _actionBarBounds);

            //Draw border
            using (var borderPen = new Pen(SkinManager.GetDividersColor(), 1))
            {
                g.DrawLine(borderPen, new Point(0, _actionBarBounds.Bottom), new Point(0, Height - 2));
                g.DrawLine(borderPen, new Point(Width - 1, _actionBarBounds.Bottom), new Point(Width - 1, Height - 2));
                g.DrawLine(borderPen, new Point(0, Height - 1), new Point(Width - 1, Height - 1));
            }

            // Determine whether or not we even should be drawing the buttons.
            bool showMin = MinimizeBox && ControlBox;
            bool showMax = MaximizeBox && ControlBox;
            var hoverBrush = SkinManager.GetButtonHoverBackgroundBrush();
            var downBrush = SkinManager.GetButtonPressedBackgroundBrush();

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
                g.FillRectangle(hoverBrush, _xButtonBounds);

            if (_buttonState == ButtonState.XDown && ControlBox)
                g.FillRectangle(downBrush, _xButtonBounds);

            using (var formButtonsPen = new Pen(SkinManager.ACTION_BAR_TEXT_SECONDARY, 2))
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

            // Drawer Icon
            if (_drawerTabControl != null)
            {
                _drawerIconRect = new Rectangle(SkinManager.FORM_PADDING / 2, STATUS_BAR_HEIGHT, 24 + SkinManager.FORM_PADDING + SkinManager.FORM_PADDING / 2, ACTION_BAR_HEIGHT);
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


                using (var formButtonsPen = new Pen(SkinManager.ACTION_BAR_TEXT_BRUSH, 2))
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

            //Form title
            g.DrawString(Text, SkinManager.ROBOTO_MEDIUM_12, SkinManager.ColorScheme.TextBrush,
                new Rectangle(SkinManager.FORM_PADDING + (_drawerTabControl != null ? 24 + (int)(SkinManager.FORM_PADDING * 1.5) : 0), STATUS_BAR_HEIGHT, Width, ACTION_BAR_HEIGHT),
                new StringFormat { LineAlignment = StringAlignment.Center });
        }

        /// <summary>
        /// Defines the _clickAnimManager
        /// </summary>
        private readonly AnimationManager _clickAnimManager;

        /// <summary>
        /// Defines the _drawerIconRect
        /// </summary>
        private Rectangle _drawerIconRect;

        /// <summary>
        /// Defines the _animationSource
        /// </summary>
        private Point _animationSource;

        /// <summary>
        /// Initializes the component
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // MaterialForm
            // 
            ClientSize = new System.Drawing.Size(284, 261);
            Name = "MaterialForm";
            Load += new System.EventHandler(MaterialForm_Load);
            ResumeLayout(false);

        }

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaterialForm_Load(object sender, EventArgs e)
        {
        }
    }

    /// <summary>
    /// Defines the <see cref="MouseMessageFilter" />
    /// </summary>
    public class MouseMessageFilter : IMessageFilter
    {
        /// <summary>
        /// Defines the WM_MOUSEMOVE
        /// </summary>
        private const int WM_MOUSEMOVE = 0x0200;

        /// <summary>
        /// Defines the MouseMove
        /// </summary>
        public static event MouseEventHandler MouseMove;

        /// <summary>
        /// The PreFilterMessage
        /// </summary>
        /// <param name="m">The m<see cref="Message"/></param>
        /// <returns>The <see cref="bool"/></returns>
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
