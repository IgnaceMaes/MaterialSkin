namespace MaterialSkin.Controls
{
    using MaterialSkin.Animations;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="MaterialDrawer" />
    /// </summary>
    public class MaterialDrawer : Control, IMaterialControl
    {
        // TODO: Invalidate when changing custom properties

        /// <summary>
        /// Define whether or not the icons should still be visible when the drawer is closed
        /// </summary>
        public bool ShowIconsWhenHidden { get; set; }

        /// <summary>
        /// Defines the _isOpen
        /// </summary>
        private bool _isOpen;

        /// <summary>
        /// Gets or Sets the IsOpen
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                _isOpen = value;
                if (value)
                    Show();
                else
                    Hide();
            }
        }

        /// <summary>
        /// Automatically hide the Drawer after a item is pressed
        /// </summary>
        public bool AutoHide { get; set; }

        /// <summary>
        /// Defines the Width of the indicator
        /// </summary>
        public int IndicatorWidth { get; set; }

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
        /// Defines the _baseTabControl
        /// </summary>
        private MaterialTabControl _baseTabControl;

        /// <summary>
        /// Gets or sets the BaseTabControl
        /// </summary>
        public MaterialTabControl BaseTabControl
        {
            get { return _baseTabControl; }
            set
            {
                _baseTabControl = value;
                if (_baseTabControl == null)
                    return;
                _previousSelectedTabIndex = _baseTabControl.SelectedIndex;
                _baseTabControl.Deselected += (sender, args) =>
                {
                    _previousSelectedTabIndex = _baseTabControl.SelectedIndex;
                };
                _baseTabControl.SelectedIndexChanged += (sender, args) =>
                {
                    _clickAnimManager.SetProgress(0);
                    _clickAnimManager.StartNewAnimation(AnimationDirection.In);
                };
                _baseTabControl.ControlAdded += delegate
                {
                    Invalidate();
                };
                _baseTabControl.ControlRemoved += delegate
                {
                    Invalidate();
                };
            }
        }

        /// <summary>
        /// Defines the _previousSelectedTabIndex
        /// </summary>
        private int _previousSelectedTabIndex;

        /// <summary>
        /// Defines the _animationSource
        /// </summary>
        private Point _animationSource;

        /// <summary>
        /// Defines the _clickAnimManager
        /// </summary>
        private readonly AnimationManager _clickAnimManager;

        /// <summary>
        /// Defines the _showHideAnimManager
        /// </summary>
        private readonly AnimationManager _showHideAnimManager;

        /// <summary>
        /// Defines the _tabRects
        /// </summary>
        private List<Rectangle> _tabRects;

        /// <summary>
        /// Defines the TAB_HEADER_PADDING
        /// </summary>
        private const int TAB_HEADER_PADDING = 24;

        /// <summary>
        ///  Defines the individual item Heigth in the drawer
        /// </summary>
        private int tabHeight;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialDrawer"/> class.
        /// </summary>
        public MaterialDrawer()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            Height = 120;
            Width = 250;
            IndicatorWidth = 0;
            _isOpen = true;
            ShowIconsWhenHidden = false;
            AutoHide = false;

            _showHideAnimManager = new AnimationManager
            {
                AnimationType = AnimationType.EaseInOut,
                Increment = 0.04
            };
            _showHideAnimManager.OnAnimationProgress += sender => Invalidate();
            _showHideAnimManager.OnAnimationProgress += showHideAnimation;


            _clickAnimManager = new AnimationManager
            {
                AnimationType = AnimationType.EaseOut,
                Increment = 0.04
            };
            _clickAnimManager.OnAnimationProgress += sender => Invalidate();
        }

        /// <summary>
        /// Called once, when the layout is first being draw
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void InitLayout()
        {

            tabHeight = TAB_HEADER_PADDING * 2 - SkinManager.FORM_PADDING / 2;
            _showHideAnimManager.SetProgress(_isOpen ? 0 : 1);
            showHideAnimation(this);
            Invalidate();

            base.InitLayout();
        }

        /// <summary>
        /// Handles the movement of the drawer
        /// </summary>
        /// <param name="sender"></param>
        private void showHideAnimation(object sender)
        {
            var showHideAnimProgress = _showHideAnimManager.GetProgress();
            if (_showHideAnimManager.IsAnimating())
            {
                if (ShowIconsWhenHidden)
                {
                    Location = new Point((int)((-Width + SkinManager.FORM_PADDING * 1.5 + tabHeight) * showHideAnimProgress), Location.Y);
                    // Width - SkinManager.FORM_PADDING
                }
                else
                {
                    Location = new Point((int)(-Width * showHideAnimProgress), Location.Y);
                }
            }
            else
            {
                if (_isOpen)
                {
                    Location = new Point(0, Location.Y);
                }
                else
                {
                    if (ShowIconsWhenHidden)
                    {
                        Location = new Point((int)((-Width + SkinManager.FORM_PADDING * 1.5 + tabHeight) * showHideAnimProgress), Location.Y);
                    }
                    else
                    {
                        Location = new Point(-Width, Location.Y);
                    }
                }
            }
            UpdateTabRects();
        }

        /// <summary>
        /// The OnPaint
        /// </summary>
        /// <param name="e">The e<see cref="PaintEventArgs"/></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;


            // redraw stuff
            g.Clear(SkinManager.ColorScheme.PrimaryColor);

            if (_baseTabControl == null)
                return;

            if (!_clickAnimManager.IsAnimating() || _tabRects == null || _tabRects.Count != _baseTabControl.TabCount)
                UpdateTabRects();

            // Click Animation
            var clickAnimProgress = _clickAnimManager.GetProgress();
            // Show/Hide Drawer Animation
            var showHideAnimProgress = _showHideAnimManager.GetProgress();

            // Ripple
            if (_clickAnimManager.IsAnimating())
            {
                var rippleBrush = new SolidBrush(Color.FromArgb((int)(51 - (clickAnimProgress * 50)), Color.White));
                var rippleSize = (int)(clickAnimProgress * _tabRects[_baseTabControl.SelectedIndex].Width * 1.75);

                g.SetClip(_tabRects[_baseTabControl.SelectedIndex]);
                g.FillEllipse(rippleBrush, new Rectangle(_animationSource.X - rippleSize / 2, _animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
                g.ResetClip();
                rippleBrush.Dispose();
            }

            // Draw menu items
            foreach (TabPage tabPage in _baseTabControl.TabPages)
            {
                var currentTabIndex = _baseTabControl.TabPages.IndexOf(tabPage);

                // Background
                Brush bgBrush = new SolidBrush(Color.FromArgb(CalculateAlpha(SkinManager.GetAccentAlpha(), 0, currentTabIndex, clickAnimProgress, 1 - showHideAnimProgress), SkinManager.ColorScheme.AccentColor));
                g.FillRectangle(bgBrush, _tabRects[currentTabIndex]);
                bgBrush.Dispose();

                // Text
                Brush textBrush = new SolidBrush(Color.FromArgb(CalculateAlphaZeroWhenClosed(SkinManager.ACTION_BAR_TEXT.A, SkinManager.ACTION_BAR_TEXT_SECONDARY.A, currentTabIndex, clickAnimProgress, 1 - showHideAnimProgress), SkinManager.ColorScheme.TextColor));
                Rectangle textRect = _tabRects[currentTabIndex];
                textRect.X += _baseTabControl.ImageList != null ? tabHeight : (int)(SkinManager.FORM_PADDING / 1.5);
                textRect.Width -= SkinManager.FORM_PADDING;
                g.DrawString(tabPage.Text, SkinManager.ROBOTO_MEDIUM_10, textBrush, textRect, new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap, Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
                textBrush.Dispose();

                // Icons
                if (_baseTabControl.ImageList != null && tabPage.ImageKey != "")
                {
                    ColorMatrix cm = new ColorMatrix();
                    cm.Matrix33 = (float)(CalculateAlpha(255, SkinManager.ACTION_BAR_TEXT_SECONDARY.A, currentTabIndex, clickAnimProgress, 1 - showHideAnimProgress) / 255f);
                    ImageAttributes ia = new ImageAttributes();
                    ia.SetColorMatrix(cm);
                    Rectangle iconRect = new Rectangle(
                        _tabRects[currentTabIndex].X + (tabHeight / 2) - (_baseTabControl.ImageList.Images[tabPage.ImageKey].Width / 2),
                        _tabRects[currentTabIndex].Y + (tabHeight / 2) - (_baseTabControl.ImageList.Images[tabPage.ImageKey].Height / 2),
                        _baseTabControl.ImageList.Images[tabPage.ImageKey].Width, _baseTabControl.ImageList.Images[tabPage.ImageKey].Height);
                    g.DrawImage(_baseTabControl.ImageList.Images[tabPage.ImageKey], iconRect, 0, 0, iconRect.Width, iconRect.Height, GraphicsUnit.Pixel, ia);

                }
            }

            // Animate tab indicator
            var previousSelectedTabIndexIfHasOne = _previousSelectedTabIndex == -1 ? _baseTabControl.SelectedIndex : _previousSelectedTabIndex;
            var previousActiveTabRect = _tabRects[previousSelectedTabIndexIfHasOne];
            var activeTabPageRect = _tabRects[_baseTabControl.SelectedIndex];

            var y = previousActiveTabRect.Y + (int)((activeTabPageRect.Y - previousActiveTabRect.Y) * clickAnimProgress);
            var x = ShowIconsWhenHidden ? -Location.X : 0;
            var height = tabHeight;

            g.FillRectangle(SkinManager.ColorScheme.AccentBrush, x, y, IndicatorWidth, height);
        }
        
        /// <summary>
        /// Show the full drawer (aka open)
        /// </summary>
        public new void Show()
        {
            _isOpen = true;
            _showHideAnimManager.StartNewAnimation(AnimationDirection.Out);
        }

        /// <summary>
        /// Hides the drawer or collapse it to just icons if the ShowIconsWhenHidden is set (aka close)
        /// </summary>
        public new void Hide()
        {
            _isOpen = false;
            _showHideAnimManager.StartNewAnimation(AnimationDirection.In);
        }

        /// <summary>
        /// Toggles between Show and Hide
        /// </summary>
        public void Toggle()
        {
            if (_isOpen)
                Hide();
            else
                Show();
        }

        /// <summary>
        /// The CalculateTextAlpha
        /// </summary>
        /// <param name="tabIndex">The tabIndex<see cref="int"/></param>
        /// <param name="clickAnimProgress">The clickAnimProgress<see cref="double"/></param>
        /// <returns>The <see cref="int"/></returns>
        private int CalculateAlphaZeroWhenClosed(int primaryA, int secondaryA, int tabIndex, double clickAnimProgress, double showHideAnimProgress)
        {
            // Drawer is closed
            if (!_isOpen && !_showHideAnimManager.IsAnimating())
            {
                return 0;
            }
            // Active menu (no change)
            if (tabIndex == _baseTabControl.SelectedIndex && (!_clickAnimManager.IsAnimating() || _showHideAnimManager.IsAnimating()))
            {
                return (int)(primaryA * showHideAnimProgress);
            }
            // Previous menu (changing)
            if (tabIndex == _previousSelectedTabIndex && !_showHideAnimManager.IsAnimating())
            {
                return primaryA - (int)((primaryA - secondaryA) * clickAnimProgress);
            }
            // Inactive menu (no change)
            if (tabIndex != _baseTabControl.SelectedIndex)
            {
                return (int)(secondaryA * showHideAnimProgress);
            }
            // Active menu (changing)
            return secondaryA + (int)((primaryA - secondaryA) * clickAnimProgress);
        }

        /// <summary>
        /// The CalculateBgAlpha
        /// </summary>
        /// <param name="tabIndex">The tabIndex<see cref="int"/></param>
        /// <param name="clickAnimProgress">The clickAnimProgress<see cref="double"/></param>
        /// <returns>The <see cref="int"/></returns>
        private int CalculateAlpha(int primaryA, int secondaryA, int tabIndex, double clickAnimProgress, double showHideAnimProgress)
        {
            if (tabIndex == _baseTabControl.SelectedIndex && !_clickAnimManager.IsAnimating())
            {
                return (int)(primaryA);
            }
            if (tabIndex != _previousSelectedTabIndex && tabIndex != _baseTabControl.SelectedIndex)
            {
                return secondaryA;
            }
            if (tabIndex == _previousSelectedTabIndex)
            {
                return primaryA - (int)((primaryA - secondaryA) * clickAnimProgress);
            }
            return secondaryA + (int)((primaryA - secondaryA) * clickAnimProgress);
        }

        /// <summary>
        /// The OnMouseUp
        /// </summary>
        /// <param name="e">The e<see cref="MouseEventArgs"/></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (_tabRects == null)
                UpdateTabRects();
            for (var i = 0; i < _tabRects.Count; i++)
            {
                if (_tabRects[i].Contains(e.Location))
                {
                    _baseTabControl.SelectedIndex = i;
                    if (AutoHide)
                        Hide();
                }
            }

            _animationSource = e.Location;
        }

        /// <summary>
        /// The OnMouseMove
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (DesignMode)
                return;

            if (_tabRects == null)
                UpdateTabRects();
            for (var i = 0; i < _tabRects.Count; i++)
            {
                if (_tabRects[i].Contains(e.Location))
                {
                    Cursor = Cursors.Hand;
                    return;
                }
            }
            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// The UpdateTabRects
        /// </summary>
        private void UpdateTabRects()
        {
            _tabRects = new List<Rectangle>();

            //If there isn't a base tab control, the rects shouldn't be calculated
            //If there aren't tab pages in the base tab control, the list should just be empty which has been set already; exit the void
            if (_baseTabControl == null || _baseTabControl.TabCount == 0)
                return;

            //Calculate the bounds of each tab header specified in the base tab control
            _tabRects.Add(new Rectangle((int)(SkinManager.FORM_PADDING / 1.5) - (ShowIconsWhenHidden ? Location.X : 0),
                SkinManager.FORM_PADDING / 2,
                (Width + (ShowIconsWhenHidden ? Location.X : 0)) - (int)(SkinManager.FORM_PADDING * 1.5) - 1,
                tabHeight));
            for (int i = 1; i < _baseTabControl.TabPages.Count; i++)
            {
                _tabRects.Add(new Rectangle((int)(SkinManager.FORM_PADDING / 1.5) - (ShowIconsWhenHidden ? Location.X : 0),
                    (TAB_HEADER_PADDING * 2) * i + SkinManager.FORM_PADDING / 2,
                    (Width + (ShowIconsWhenHidden ? Location.X : 0)) - (int)(SkinManager.FORM_PADDING * 1.5) - 1,
                    tabHeight));
            }
        }
    }
}
