namespace MaterialSkin.Controls
{
    using MaterialSkin.Animations;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Text;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class MaterialForm : Form, IMaterialControl
    {
        #region Public Properties
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        [Category("Layout")]
        public bool Sizable { get; set; }

        [Category("Material Skin"), Browsable(true), DisplayName("Form Style"), DefaultValue(FormStyles.ActionBar_40)]
        public FormStyles FormStyle
        {
            get => _formStyle;
            set
            {
                if (_formStyle == value) return;

                _formStyle = value;
                RecalculateFormBoundaries();
            }
        }

        [Category("Drawer")]
        public bool DrawerShowIconsWhenHidden
        {
            get => _drawerShowIconsWhenHidden;
            set
            {
                if (_drawerShowIconsWhenHidden == value) return;

                _drawerShowIconsWhenHidden = value;

                if (drawerControl == null) return;

                drawerControl.ShowIconsWhenHidden = _drawerShowIconsWhenHidden;
                drawerControl.Refresh();
            }
        }

        [Category("Drawer")]
        public int DrawerWidth { get; set; }

        [Category("Drawer")]
        public bool DrawerAutoHide
        {
            get => _drawerAutoHide;
            set => drawerControl.AutoHide = _drawerAutoHide = value;
        }

        [Category("Drawer")]
        public bool DrawerAutoShow
        {
            get => _drawerAutoShow;
            set => drawerControl.AutoShow = _drawerAutoShow = value;
        }

        [Category("Drawer")]
        public int DrawerIndicatorWidth { get; set; }

        [Category("Drawer")]
        public bool DrawerIsOpen
        {
            get => _drawerIsOpen;
            set
            {
                if (_drawerIsOpen == value) return;

                _drawerIsOpen = value;

                if (value)
                    drawerControl?.Show();
                else
                    drawerControl?.Hide();
            }
        }

        [Category("Drawer")]
        public bool DrawerUseColors
        {
            get => _drawerUseColors;
            set
            {
                if (_drawerUseColors == value) return;

                _drawerUseColors = value;

                if (drawerControl == null) return;

                drawerControl.UseColors = value;
                drawerControl.Refresh();
            }
        }

        [Category("Drawer")]
        public bool DrawerHighlightWithAccent
        {
            get => _drawerHighlightWithAccent;
            set
            {
                if (_drawerHighlightWithAccent == value) return;

                _drawerHighlightWithAccent = value;

                if (drawerControl == null) return;

                drawerControl.HighlightWithAccent = value;
                drawerControl.Refresh();
            }
        }

        [Category("Drawer")]
        public bool DrawerBackgroundWithAccent
        {
            get => _backgroundWithAccent;
            set
            {
                if (_backgroundWithAccent == value) return;

                _backgroundWithAccent = value;

                if (drawerControl == null) return;

                drawerControl.BackgroundWithAccent = value;
                drawerControl.Refresh();
            }
        }

        [Category("Drawer")]
        public MaterialTabControl DrawerTabControl { get; set; }

        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; Invalidate(); }
        }

        public new FormWindowState WindowState
        {
            get { return base.WindowState; }
            set { base.WindowState = value; }
        }

        public new FormBorderStyle FormBorderStyle
        {
            get { return base.FormBorderStyle; }
            set { base.FormBorderStyle = value; }
        }

        public Rectangle UserArea
        {
            get
            {
                return new Rectangle(ClientRectangle.X, ClientRectangle.Y + STATUS_BAR_HEIGHT + ACTION_BAR_HEIGHT, ClientSize.Width, ClientSize.Height - (STATUS_BAR_HEIGHT + ACTION_BAR_HEIGHT));
            }
        }
        #endregion

        #region Enums
        /// <summary>
        /// Various options to control the top caption of a window
        /// </summary>
        public enum FormStyles
        {
            StatusAndActionBar_None,
            ActionBar_None,
            ActionBar_40,
            ActionBar_48,
            ActionBar_56,
            ActionBar_64,
        }

        /// <summary>
        /// Various directions the form can be resized in
        /// </summary>
        private enum ResizeDirection
        {
            BottomLeft,
            Left,
            Right,
            BottomRight,
            Bottom,
            None
        }

        /// <summary>
        /// The states a button can be in
        /// </summary>
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

        /// <summary>
        /// Window Messages
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/winmsg/about-messages-and-message-queues"/>
        /// </summary>
        private enum WM
        {
            /// <summary>
            /// WM_NCCALCSIZE
            /// </summary>
            NonClientCalcSize = 0x0083,
            /// <summary>
            /// WM_NCACTIVATE
            /// </summary>
            NonClientActivate = 0x0086,
            /// <summary>
            /// WM_NCLBUTTONDOWN
            /// </summary>
            NonClientLeftButtonDown = 0x00A1,
            /// <summary>
            /// WM_SYSCOMMAND
            /// </summary>
            SystemCommand = 0x0112,
            /// <summary>
            /// WM_MOUSEMOVE
            /// </summary>
            MouseMove = 0x0200,
            /// <summary>
            /// WM_LBUTTONDOWN
            /// </summary>
            LeftButtonDown = 0x0201,
            /// <summary>
            /// WM_LBUTTONUP
            /// </summary>
            LeftButtonUp = 0x0202,
            /// <summary>
            /// WM_LBUTTONDBLCLK
            /// </summary>
            LeftButtonDoubleClick = 0x0203,
            /// <summary>
            /// WM_RBUTTONDOWN
            /// </summary>
            RightButtonDown = 0x0204,
        }

        /// <summary>
        /// Hit Test Results
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-nchittest"/>
        /// </summary>
        private enum HT
        {
            /// <summary>
            /// HTNOWHERE - Nothing under cursor
            /// </summary>
            None = 0,
            /// <summary>
            /// HTCAPTION - Titlebar
            /// </summary>
            Caption = 2,
            /// <summary>
            /// HTLEFT - Left border
            /// </summary>
            Left = 10,
            /// <summary>
            /// HTRIGHT - Right border
            /// </summary>
            Right = 11,
            /// <summary>
            /// HTTOP - Top border
            /// </summary>
            Top = 12,
            /// <summary>
            /// HTTOPLEFT - Top left corner
            /// </summary>
            TopLeft = 13,
            /// <summary>
            /// HTTOPRIGHT - Top right corner
            /// </summary>
            TopRight = 14,
            /// <summary>
            /// HTBOTTOM - Bottom border
            /// </summary>
            Bottom = 15,
            /// <summary>
            /// HTBOTTOMLEFT - Bottom left corner
            /// </summary>
            BottomLeft = 16,
            /// <summary>
            /// HTBOTTOMRIGHT - Bottom right corner
            /// </summary>
            BottomRight = 17,
        }

        /// <summary>
        /// Window Styles
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/winmsg/window-styles"/>
        /// </summary>
        private enum WS
        {
            /// <summary>
            /// WS_MINIMIZEBOX - Allow minimizing from taskbar
            /// </summary>
            MinimizeBox = 0x20000,
            /// <summary>
            /// WS_SIZEFRAME - Required for Aero Snapping
            /// </summary>
            SizeFrame = 0x40000,
            /// <summary>
            /// WS_SYSMENU - Trigger the creation of the system menu
            /// </summary>
            SysMenu = 0x80000,
        }

        /// <summary>
        /// Track Popup Menu Flags
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-trackpopupmenu"/>
        /// </summary>
        private enum TPM
        {
            /// <summary>
            /// TPM_LEFTALIGN
            /// </summary>
            LeftAlign = 0x0000,
            /// <summary>
            /// TPM_RETURNCMD
            /// </summary>
            ReturnCommand = 0x0100,
        }
        #endregion

        #region Constants
        // Form Constants
        private const int BORDER_WIDTH = 7;
        private const int STATUS_BAR_BUTTON_WIDTH = 24;
        private const int STATUS_BAR_HEIGHT_DEFAULT = 24;
        private const int ICON_SIZE = 24;
        private const int PADDING_MINIMUM = 3;
        private const int TITLE_LEFT_PADDING = 72;
        private const int ACTION_BAR_PADDING = 16;
        private const int ACTION_BAR_HEIGHT_DEFAULT = 40;
        #endregion

        #region Private Fields
        private readonly Cursor[] _resizeCursors = { Cursors.SizeNESW, Cursors.SizeWE, Cursors.SizeNWSE, Cursors.SizeWE, Cursors.SizeNS };

        private ResizeDirection _resizeDir;
        private ButtonState _buttonState = ButtonState.None;
        private FormStyles _formStyle;
        private Rectangle _minButtonBounds => new Rectangle(ClientSize.Width - 3 * STATUS_BAR_BUTTON_WIDTH, ClientRectangle.Y, STATUS_BAR_BUTTON_WIDTH, STATUS_BAR_HEIGHT);
        private Rectangle _maxButtonBounds => new Rectangle(ClientSize.Width - 2 * STATUS_BAR_BUTTON_WIDTH, ClientRectangle.Y, STATUS_BAR_BUTTON_WIDTH, STATUS_BAR_HEIGHT);
        private Rectangle _xButtonBounds => new Rectangle(ClientSize.Width - STATUS_BAR_BUTTON_WIDTH, ClientRectangle.Y, STATUS_BAR_BUTTON_WIDTH, STATUS_BAR_HEIGHT);
        private Rectangle _actionBarBounds => new Rectangle(ClientRectangle.X, ClientRectangle.Y + STATUS_BAR_HEIGHT, ClientSize.Width, ACTION_BAR_HEIGHT);
        private Rectangle _drawerButtonBounds => new Rectangle(ClientRectangle.X + (SkinManager.FORM_PADDING / 2) + 3, STATUS_BAR_HEIGHT + (ACTION_BAR_HEIGHT / 2) - (ACTION_BAR_HEIGHT_DEFAULT / 2), ACTION_BAR_HEIGHT_DEFAULT, ACTION_BAR_HEIGHT_DEFAULT);
        private Rectangle _statusBarBounds => new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientSize.Width, STATUS_BAR_HEIGHT);
        private Rectangle _drawerIconRect;

        private bool Maximized
        {
            get => WindowState == FormWindowState.Maximized;
            set
            {
                if (!MaximizeBox || !ControlBox) return;

                if (value)
                    WindowState = FormWindowState.Maximized;
                else
                    WindowState = FormWindowState.Normal;
            }
        }
        private Point _animationSource;
        private Padding originalPadding;

        private Form drawerOverlay = new Form();
        private Form drawerForm = new Form();

        // Drawer overlay and speed improvements
        private bool _drawerShowIconsWhenHidden;
        private bool _drawerAutoHide;
        private bool _drawerAutoShow;
        private bool _drawerIsOpen;
        private bool _drawerUseColors;
        private bool _drawerHighlightWithAccent;
        private bool _backgroundWithAccent;
        private MaterialDrawer drawerControl = new MaterialDrawer();
        private AnimationManager _drawerShowHideAnimManager;
        private readonly AnimationManager _clickAnimManager;

        private int STATUS_BAR_HEIGHT = 24;
        private int ACTION_BAR_HEIGHT = 40;
        #endregion

        public MaterialForm()
        {
            MaximizedBounds = Screen.GetWorkingArea(Location);
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

        #region Private Methods
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

            int H = ClientSize.Height - _statusBarBounds.Height - _actionBarBounds.Height;
            int Y = PointToScreen(Point.Empty).Y + _statusBarBounds.Height + _actionBarBounds.Height;

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
            drawerOverlay.Size = new Size(ClientSize.Width, H);
            drawerOverlay.Location = new Point(PointToScreen(Point.Empty).X, Y);
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
            drawerForm.Location = new Point(PointToScreen(Point.Empty).X, Y);
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
                H = ClientSize.Height - _statusBarBounds.Height - _actionBarBounds.Height;
                drawerForm.Size = new Size(DrawerWidth, H);
                drawerOverlay.Size = new Size(ClientSize.Width, H);
            };

            Move += (sender, e) =>
            {
                Point pos = new Point(PointToScreen(Point.Empty).X, PointToScreen(Point.Empty).Y + _statusBarBounds.Height + _actionBarBounds.Height);
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
                if (Sizable && !Maximized)
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

        private void UpdateButtons(MouseButtons button, Point location, bool up = false)
        {
            if (DesignMode) return;

            var oldState = _buttonState;
            bool showMin = MinimizeBox && ControlBox;
            bool showMax = MaximizeBox && ControlBox;

            if (button == MouseButtons.Left && !up)
            {
                if (showMin && !showMax && _maxButtonBounds.Contains(location))
                    _buttonState = ButtonState.MinDown;
                else if (showMin && showMax && _minButtonBounds.Contains(location))
                    _buttonState = ButtonState.MinDown;
                else if (showMax && _maxButtonBounds.Contains(location))
                    _buttonState = ButtonState.MaxDown;
                else if (ControlBox && _xButtonBounds.Contains(location))
                    _buttonState = ButtonState.XDown;
                else if (_drawerButtonBounds.Contains(location))
                    _buttonState = ButtonState.DrawerDown;
                else
                    _buttonState = ButtonState.None;
            }
            else
            {
                if (showMin && !showMax && _maxButtonBounds.Contains(location))
                {
                    _buttonState = ButtonState.MinOver;

                    if (oldState == ButtonState.MinDown && up)
                        WindowState = FormWindowState.Minimized;
                }
                else if (showMin && showMax && _minButtonBounds.Contains(location))
                {
                    _buttonState = ButtonState.MinOver;

                    if (oldState == ButtonState.MinDown && up)
                        WindowState = FormWindowState.Minimized;
                }
                else if (showMax && _maxButtonBounds.Contains(location))
                {
                    _buttonState = ButtonState.MaxOver;

                    if (oldState == ButtonState.MaxDown && up)
                        Maximized = !Maximized;
                }
                else if (ControlBox && _xButtonBounds.Contains(location))
                {
                    _buttonState = ButtonState.XOver;

                    if (oldState == ButtonState.XDown && up)
                        Close();
                }
                else if (_drawerButtonBounds.Contains(location))
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

        private void ResizeForm(ResizeDirection direction)
        {
            if (DesignMode)
                return;
            var dir = -1;
            switch (direction)
            {
                case ResizeDirection.BottomLeft:
                    dir = (int)HT.BottomLeft;
                    Cursor = Cursors.SizeNESW;
                    break;

                case ResizeDirection.Left:
                    dir = (int)HT.Left;
                    Cursor = Cursors.SizeWE;
                    break;

                case ResizeDirection.Right:
                    dir = (int)HT.Right;
                    break;

                case ResizeDirection.BottomRight:
                    dir = (int)HT.BottomRight;
                    break;

                case ResizeDirection.Bottom:
                    dir = (int)HT.Bottom;
                    break;
            }

            ReleaseCapture();
            if (dir != -1)
            {
                SendMessage(Handle, (int)WM.NonClientLeftButtonDown, dir, 0);
            }
        }

        private void RecalculateFormBoundaries()
        {
            switch (_formStyle)
            {
                case FormStyles.StatusAndActionBar_None:
                    ACTION_BAR_HEIGHT = 0;
                    STATUS_BAR_HEIGHT = 0;
                    break;
                case FormStyles.ActionBar_None:
                    ACTION_BAR_HEIGHT = 0;
                    STATUS_BAR_HEIGHT = STATUS_BAR_HEIGHT_DEFAULT;
                    break;
                case FormStyles.ActionBar_40:
                    ACTION_BAR_HEIGHT = ACTION_BAR_HEIGHT_DEFAULT;
                    STATUS_BAR_HEIGHT = STATUS_BAR_HEIGHT_DEFAULT;
                    break;
                case FormStyles.ActionBar_48:
                    ACTION_BAR_HEIGHT = 48;
                    STATUS_BAR_HEIGHT = STATUS_BAR_HEIGHT_DEFAULT;
                    break;
                case FormStyles.ActionBar_56:
                    ACTION_BAR_HEIGHT = 56;
                    STATUS_BAR_HEIGHT = STATUS_BAR_HEIGHT_DEFAULT;
                    break;
                case FormStyles.ActionBar_64:
                    ACTION_BAR_HEIGHT = 64;
                    STATUS_BAR_HEIGHT = STATUS_BAR_HEIGHT_DEFAULT;
                    break;
                default:
                    ACTION_BAR_HEIGHT = ACTION_BAR_HEIGHT_DEFAULT;
                    STATUS_BAR_HEIGHT = STATUS_BAR_HEIGHT_DEFAULT;
                    break;
            }

            Padding = new Padding(_drawerShowIconsWhenHidden ? drawerControl.MinWidth : PADDING_MINIMUM, STATUS_BAR_HEIGHT + ACTION_BAR_HEIGHT, Padding.Right, Padding.Bottom);
            originalPadding = Padding;

            if (DrawerTabControl != null)
            {
                var height = ClientSize.Height - (STATUS_BAR_HEIGHT + ACTION_BAR_HEIGHT);
                var location = Point.Add(Location, new Size(0, STATUS_BAR_HEIGHT + ACTION_BAR_HEIGHT));
                drawerOverlay.Size = new Size(ClientSize.Width, height);
                drawerOverlay.Location = location;
                drawerForm.Size = new Size(DrawerWidth, height);
                drawerForm.Location = location;
            }

            Invalidate();
        }
        #endregion

        #region WinForms Methods
        protected override CreateParams CreateParams
        {
            get
            {
                var par = base.CreateParams;
                par.Style |= (int)WS.MinimizeBox | (int)WS.SysMenu;
                return par;
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            // Sets the Window Style for having a Size Frame after the form is created
            // This prevents unexpected sizing while still allowing for Aero Snapping
            var flags = GetWindowLongPtr(Handle, -16).ToInt64();
            SetWindowLongPtr(Handle, -16, (IntPtr)(flags | (int)WS.SizeFrame));
        }

        protected override void WndProc(ref Message m)
        {
            var message = (WM)m.Msg;
            // Prevent the base class from receiving the message
            if (message == WM.NonClientCalcSize) return;

            // https://docs.microsoft.com/en-us/windows/win32/winmsg/wm-ncactivate?redirectedfrom=MSDN#parameters
            // "If this parameter is set to -1, DefWindowProc does not repaint the nonclient area to reflect the state change."
            if (message == WM.NonClientActivate)
            {
                m.Result = new IntPtr(-1);
                return;
            }

            base.WndProc(ref m);
            if (DesignMode || IsDisposed)
                return;

            var cursorPos = PointToClient(Cursor.Position);
            var isOverCaption = (_statusBarBounds.Contains(cursorPos) || _actionBarBounds.Contains(cursorPos)) &&
                !(_minButtonBounds.Contains(cursorPos) || _maxButtonBounds.Contains(cursorPos) || _xButtonBounds.Contains(cursorPos));

            // Drawer
            if (DrawerTabControl != null && (message == WM.LeftButtonDown || message == WM.LeftButtonDoubleClick) && _drawerIconRect.Contains(cursorPos))
            {
                drawerControl.Toggle();
                _clickAnimManager.SetProgress(0);
                _clickAnimManager.StartNewAnimation(AnimationDirection.In);
                _animationSource = cursorPos;
            }
            // Double click to maximize
            else if (message == WM.LeftButtonDoubleClick && isOverCaption)
            { 
                Maximized = !Maximized;
            }
            // Treat the Caption as if it was Non-Client
            else if (message == WM.LeftButtonDown && isOverCaption)
            {
                ReleaseCapture();
                SendMessage(Handle, (int)WM.NonClientLeftButtonDown, (int)HT.Caption, 0);
            }
            // Default context menu
            else if (message == WM.RightButtonDown)
            {
                if (_statusBarBounds.Contains(cursorPos) && !_minButtonBounds.Contains(cursorPos) &&
                    !_maxButtonBounds.Contains(cursorPos) && !_xButtonBounds.Contains(cursorPos))
                {
                    // Temporary disable user defined ContextMenuStrip
                    var user_cms = base.ContextMenuStrip;
                    base.ContextMenuStrip = null;

                    // Show default system menu when right clicking titlebar
                    var id = TrackPopupMenuEx(GetSystemMenu(Handle, false), (int)TPM.LeftAlign | (int)TPM.ReturnCommand, Cursor.Position.X, Cursor.Position.Y, Handle, IntPtr.Zero);

                    // Pass the command as a WM_SYSCOMMAND message
                    SendMessage(Handle, (int)WM.SystemCommand, id, 0);
                    
                    // restore user defined ContextMenuStrip
                    base.ContextMenuStrip = user_cms;
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (DesignMode)
                return;
            UpdateButtons(e.Button, e.Location);

            if (e.Button == MouseButtons.Left && !Maximized && _resizeCursors.Contains(Cursor))
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

            if (DesignMode) return;

            var coords = e.Location;

            UpdateButtons(e.Button, coords);

            if (!Sizable) return;

            //True if the mouse is hovering over a child control
            var isChildUnderMouse = GetChildAtPoint(coords) != null;

            if (!isChildUnderMouse && !Maximized && coords.X <= BORDER_WIDTH && coords.Y >= ClientSize.Height - BORDER_WIDTH)
            {
                _resizeDir = ResizeDirection.BottomLeft;
                Cursor = Cursors.SizeNESW;
            }
            else if ((!isChildUnderMouse || DrawerTabControl != null) && !Maximized && coords.X <= BORDER_WIDTH)
            {
                _resizeDir = ResizeDirection.Left;
                Cursor = Cursors.SizeWE;
            }
            else if (!isChildUnderMouse && !Maximized && coords.X >= ClientSize.Width - BORDER_WIDTH && coords.Y >= ClientSize.Height - BORDER_WIDTH)
            {
                _resizeDir = ResizeDirection.BottomRight;
                Cursor = Cursors.SizeNWSE;
            }
            else if (!isChildUnderMouse && !Maximized && coords.X >= ClientSize.Width - BORDER_WIDTH)
            {
                _resizeDir = ResizeDirection.Right;
                Cursor = Cursors.SizeWE;
            }
            else if (!isChildUnderMouse && !Maximized && coords.Y >= ClientSize.Height - BORDER_WIDTH)
            {
                _resizeDir = ResizeDirection.Bottom;
                Cursor = Cursors.SizeNS;
            }
            else
            {
                _resizeDir = ResizeDirection.None;

                //Only reset the cursor when needed, this prevents it from flickering when a child control changes the cursor to its own needs
                if (_resizeCursors.Contains(Cursor))
                    Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (DesignMode)
                return;
            UpdateButtons(e.Button, e.Location, true);

            base.OnMouseUp(e);
            ReleaseCapture();
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
                g.DrawLine(borderPen, new Point(0, _actionBarBounds.Bottom), new Point(0, ClientSize.Height - 2));
                g.DrawLine(borderPen, new Point(ClientSize.Width - 1, _actionBarBounds.Bottom), new Point(ClientSize.Width - 1, ClientSize.Height - 2));
                g.DrawLine(borderPen, new Point(0, ClientSize.Height - 1), new Point(ClientSize.Width - 1, ClientSize.Height - 1));
            }

            if (_formStyle != FormStyles.StatusAndActionBar_None)
            {
                if (ControlBox)
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
                        if (WindowState != FormWindowState.Maximized)
                        {
                            g.DrawRectangle(
                                formButtonsPen,
                                _maxButtonBounds.X + (int)(_maxButtonBounds.Width * 0.33),
                                _maxButtonBounds.Y + (int)(_maxButtonBounds.Height * 0.36),
                                (int)(_maxButtonBounds.Width * 0.39),
                                (int)(_maxButtonBounds.Height * 0.31)
                            );
                        }
                        else
                        {
                            // Change position of square
                            g.DrawRectangle(
                                formButtonsPen,
                                _maxButtonBounds.X + (int)(_maxButtonBounds.Width * 0.30),
                                _maxButtonBounds.Y + (int)(_maxButtonBounds.Height * 0.42),
                                (int)(_maxButtonBounds.Width * 0.40),
                                (int)(_maxButtonBounds.Height * 0.32)
                            );
                            // Draw lines for background square
                            g.DrawLine(formButtonsPen,
                                _maxButtonBounds.X + (int)(_maxButtonBounds.Width * 0.42),
                                _maxButtonBounds.Y + (int)(_maxButtonBounds.Height * 0.30),
                                _maxButtonBounds.X + (int)(_maxButtonBounds.Width * 0.42),
                                _maxButtonBounds.Y + (int)(_maxButtonBounds.Height * 0.38)
                            );
                            g.DrawLine(formButtonsPen,
                                _maxButtonBounds.X + (int)(_maxButtonBounds.Width * 0.40),
                                _maxButtonBounds.Y + (int)(_maxButtonBounds.Height * 0.30),
                                _maxButtonBounds.X + (int)(_maxButtonBounds.Width * 0.86),
                                _maxButtonBounds.Y + (int)(_maxButtonBounds.Width * 0.30)
                            );
                            g.DrawLine(formButtonsPen,
                                _maxButtonBounds.X + (int)(_maxButtonBounds.Width * 0.82),
                                _maxButtonBounds.Y + (int)(_maxButtonBounds.Height * 0.28),
                                _maxButtonBounds.X + (int)(_maxButtonBounds.Width * 0.82),
                                _maxButtonBounds.Y + (int)(_maxButtonBounds.Width * 0.64)
                            );
                            g.DrawLine(formButtonsPen,
                                _maxButtonBounds.X + (int)(_maxButtonBounds.Width * 0.70),
                                _maxButtonBounds.Y + (int)(_maxButtonBounds.Height * 0.62),
                                _maxButtonBounds.X + (int)(_maxButtonBounds.Width * 0.84),
                                _maxButtonBounds.Y + (int)(_maxButtonBounds.Width * 0.62)
                            );
                        }
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

            if (ControlBox == true && _formStyle != FormStyles.ActionBar_None && _formStyle != FormStyles.StatusAndActionBar_None)
            {
                //Form title
                using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
                {
                    Rectangle textLocation = new Rectangle(DrawerTabControl != null ? TITLE_LEFT_PADDING : TITLE_LEFT_PADDING - (ICON_SIZE + (ACTION_BAR_PADDING*2)), STATUS_BAR_HEIGHT, ClientSize.Width, ACTION_BAR_HEIGHT);
                    NativeText.DrawTransparentText(Text, SkinManager.getLogFontByType(MaterialSkinManager.fontType.H6),
                        SkinManager.ColorScheme.TextColor,
                        textLocation.Location,
                        textLocation.Size,
                        NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
                }
            }
        }
        #endregion

        #region Low Level Windows Methods
        /// <summary>
        ///     Provides a single method to call either the 32-bit or 64-bit method based on the size of an <see cref="IntPtr"/> for getting the
        ///     Window Style flags.<br/>
        ///     <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowlongptra">GetWindowLongPtr</see>
        /// </summary>
        private static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 8)
                return GetWindowLongPtr64(hWnd, nIndex);
            else
                return GetWindowLong(hWnd, nIndex);
        }

        /// <summary>
        ///     Provides a single method to call either the 32-bit or 64-bit method based on the size of an <see cref="IntPtr"/> for setting the
        ///     Window Style flags.<br/>
        ///     <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowlongptra">SetWindowLongPtr</see>
        /// </summary>
        private static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return SetWindowLong(hWnd, nIndex, dwNewLong.ToInt32());
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        #endregion
    }
}
