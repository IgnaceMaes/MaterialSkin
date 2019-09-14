namespace MaterialSkin.Controls
{
    using MaterialSkin.Animations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.Windows.Forms;

    public class MaterialDrawer : Control, IMaterialControl
    {
        // TODO: Invalidate when changing custom properties

        [Category("Appearance")]
        public bool ShowIconsWhenHidden { get; set; }

        private bool _isOpen;

        [Category("Behavior")]
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

        [Category("Behavior")]
        public bool AutoHide { get; set; }

        [Category("Appearance")]
        private bool _useColors;
        public bool UseColors
        {
            get
            {
                return _useColors;
            }
            set
            {
                _useColors = value;
                preProcessIcons();
            }
        }

        [Category("Appearance")]
        public int IndicatorWidth { get; set; }

        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        public delegate void DrawerStateHandler(object sender);
        public event DrawerStateHandler DrawerStateChanged;
        public event DrawerStateHandler DrawerBeginOpen;
        public event DrawerStateHandler DrawerEndOpen;
        public event DrawerStateHandler DrawerBeginClose;
        public event DrawerStateHandler DrawerEndClose;

        // icons
        private Dictionary<string, TextureBrush> iconsBrushes;
        private Dictionary<string, TextureBrush> iconsSelectedBrushes;
        private int prevLocation;

        private int rippleSize = 0;

        private MaterialTabControl _baseTabControl;

        [Category("Behavior")]
        public MaterialTabControl BaseTabControl
        {
            get { return _baseTabControl; }
            set
            {
                _baseTabControl = value;
                if (_baseTabControl == null)
                    return;

                UpdateTabRects();
                preProcessIcons();

                // Other helpers

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

        private void preProcessIcons()
        {
            // pre-process and pre-allocate texture brushes (icons)
            if (_baseTabControl == null || _baseTabControl.TabCount == 0 || _baseTabControl.ImageList == null || _tabRects == null || _tabRects.Count == 0)
                return;

            // Calculate lightness and color
            float l = SkinManager.Theme == MaterialSkinManager.Themes.LIGHT ? 0f : 1f;
            float r = (SkinManager.Theme == MaterialSkinManager.Themes.LIGHT && !_useColors ? SkinManager.ColorScheme.PrimaryColor.R : SkinManager.ColorScheme.AccentColor.R) / 255f;
            float g = (SkinManager.Theme == MaterialSkinManager.Themes.LIGHT && !_useColors ? SkinManager.ColorScheme.PrimaryColor.G : SkinManager.ColorScheme.AccentColor.G) / 255f;
            float b = (SkinManager.Theme == MaterialSkinManager.Themes.LIGHT && !_useColors ? SkinManager.ColorScheme.PrimaryColor.B : SkinManager.ColorScheme.AccentColor.B) / 255f;

            // Create matrices
            float[][] matrixGray = {
                    new float[] {   0,   0,   0,   0,  0}, // Red scale factor
                    new float[] {   0,   0,   0,   0,  0}, // Green scale factor
                    new float[] {   0,   0,   0,   0,  0}, // Blue scale factor
                    new float[] {   0,   0,   0, .5f,  0}, // alpha scale factor
                    new float[] {   l,   l,   l,   0,  1}};// offset

            float[][] matrixColor = {
                    new float[] {   0,   0,   0,   0,  0}, // Red scale factor
                    new float[] {   0,   0,   0,   0,  0}, // Green scale factor
                    new float[] {   0,   0,   0,   0,  0}, // Blue scale factor
                    new float[] {   0,   0,   0,   1,  0}, // alpha scale factor
                    new float[] {   r,   g,   b,   0,  1}};// offset

            ColorMatrix colorMatrixGray = new ColorMatrix(matrixGray);
            ColorMatrix colorMatrixColor = new ColorMatrix(matrixColor);

            ImageAttributes grayImageAttributes = new ImageAttributes();
            ImageAttributes colorImageAttributes = new ImageAttributes();

            // Set color matrices
            grayImageAttributes.SetColorMatrix(colorMatrixGray, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            colorImageAttributes.SetColorMatrix(colorMatrixColor, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            // Create brushes
            iconsBrushes = new Dictionary<string, TextureBrush>(_baseTabControl.TabPages.Count);
            iconsSelectedBrushes = new Dictionary<string, TextureBrush>(_baseTabControl.TabPages.Count);

            foreach (TabPage tabPage in _baseTabControl.TabPages)
            {
                // skip items without image
                if (String.IsNullOrEmpty(tabPage.ImageKey) || _tabRects == null)
                    continue;

                // Image Rect
                Rectangle destRect = new Rectangle(0, 0, _baseTabControl.ImageList.Images[tabPage.ImageKey].Width, _baseTabControl.ImageList.Images[tabPage.ImageKey].Height);

                // Create a pre-processed copy of the image (GRAY)
                Bitmap bgray = new Bitmap(destRect.Width, destRect.Height);
                using (Graphics gGray = Graphics.FromImage(bgray))
                {
                    gGray.DrawImage(_baseTabControl.ImageList.Images[tabPage.ImageKey],
                        new Point[] {
                                new Point(0, 0),
                                new Point(destRect.Width, 0),
                                new Point(0, destRect.Height),
                        },
                        destRect, GraphicsUnit.Pixel, grayImageAttributes);
                }

                // Create a pre-processed copy of the image (PRIMARY COLOR)
                Bitmap bcolor = new Bitmap(destRect.Width, destRect.Height);
                using (Graphics gColor = Graphics.FromImage(bcolor))
                {
                    gColor.DrawImage(_baseTabControl.ImageList.Images[tabPage.ImageKey],
                        new Point[] {
                                new Point(0, 0),
                                new Point(destRect.Width, 0),
                                new Point(0, destRect.Height),
                        },
                        destRect, GraphicsUnit.Pixel, colorImageAttributes);
                }

                // added processed image to brush for drawing
                TextureBrush textureBrushGray = new TextureBrush(bgray);
                TextureBrush textureBrushColor = new TextureBrush(bcolor);

                textureBrushGray.WrapMode = System.Drawing.Drawing2D.WrapMode.Clamp;
                textureBrushColor.WrapMode = System.Drawing.Drawing2D.WrapMode.Clamp;

                // Translate the brushes to the correct positions
                var currentTabIndex = _baseTabControl.TabPages.IndexOf(tabPage);

                Rectangle iconRect = new Rectangle(
                   _tabRects[currentTabIndex].X + (tabHeight / 2) - (_baseTabControl.ImageList.Images[tabPage.ImageKey].Width / 2),
                   _tabRects[currentTabIndex].Y + (tabHeight / 2) - (_baseTabControl.ImageList.Images[tabPage.ImageKey].Height / 2),
                   _baseTabControl.ImageList.Images[tabPage.ImageKey].Width, _baseTabControl.ImageList.Images[tabPage.ImageKey].Height);

                textureBrushGray.TranslateTransform(iconRect.X + iconRect.Width / 2 - _baseTabControl.ImageList.Images[tabPage.ImageKey].Width / 2, iconRect.Y + iconRect.Height / 2 - _baseTabControl.ImageList.Images[tabPage.ImageKey].Height / 2);
                textureBrushColor.TranslateTransform(iconRect.X + iconRect.Width / 2 - _baseTabControl.ImageList.Images[tabPage.ImageKey].Width / 2, iconRect.Y + iconRect.Height / 2 - _baseTabControl.ImageList.Images[tabPage.ImageKey].Height / 2);

                // add to dictionary
                iconsBrushes.Add(tabPage.ImageKey, textureBrushGray);
                iconsSelectedBrushes.Add(tabPage.ImageKey, textureBrushColor);
            }
        }

        private int _previousSelectedTabIndex;

        private Point _animationSource;

        private readonly AnimationManager _clickAnimManager;

        private readonly AnimationManager _showHideAnimManager;

        private List<Rectangle> _tabRects;
        private List<GraphicsPath> _tabPaths;

        private const int TAB_HEADER_PADDING = 24;

        private int tabHeight;

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
                Increment = 0.08
            };
            _showHideAnimManager.OnAnimationProgress += sender =>
            {
                Invalidate();
                showHideAnimation(sender);
            };
            _showHideAnimManager.OnAnimationFinished += sender =>
            {
                if (_baseTabControl != null && _tabRects.Count > 0)
                    rippleSize = _tabRects[_baseTabControl.SelectedIndex].Width;
                if (_isOpen)
                {
                    DrawerEndOpen?.Invoke(this);
                }
                else
                {
                    DrawerEndClose?.Invoke(this);
                }
            };

            SkinManager.ColorSchemeChanged += sender =>
            {
                preProcessIcons();
            };

            SkinManager.ThemeChanged += sender =>
            {
                preProcessIcons();
            };

            _clickAnimManager = new AnimationManager
            {
                AnimationType = AnimationType.EaseOut,
                Increment = 0.04
            };
            _clickAnimManager.OnAnimationProgress += sender => Invalidate();
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void InitLayout()
        {

            tabHeight = TAB_HEADER_PADDING * 2 - SkinManager.FORM_PADDING / 2;
            _showHideAnimManager.SetProgress(_isOpen ? 0 : 1);
            showHideAnimation(this);
            Invalidate();

            base.InitLayout();
        }

        private void showHideAnimation(object sender)
        {
            var showHideAnimProgress = _showHideAnimManager.GetProgress();
            if (_showHideAnimManager.IsAnimating())
            {
                if (ShowIconsWhenHidden)
                {
                    Location = new Point((int)((-Width + SkinManager.FORM_PADDING * 1.5 + tabHeight) * showHideAnimProgress), Location.Y);
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
                        Location = new Point((int)(-Width + SkinManager.FORM_PADDING * 1.5 + tabHeight), Location.Y);
                    }
                    else
                    {
                        Location = new Point(-Width, Location.Y);
                    }
                }
            }
            UpdateTabRects();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;


            // redraw stuff
            g.Clear(UseColors ? SkinManager.ColorScheme.PrimaryColor : SkinManager.GetApplicationBackgroundColor());

            if (_baseTabControl == null)
                return;

            if (!_clickAnimManager.IsAnimating() || _tabRects == null || _tabRects.Count != _baseTabControl.TabCount)
                UpdateTabRects();

            if (_tabRects == null || _tabRects.Count != _baseTabControl.TabCount)
                return;


            // Click Animation
            var clickAnimProgress = _clickAnimManager.GetProgress();
            // Show/Hide Drawer Animation
            var showHideAnimProgress = _showHideAnimManager.GetProgress();
            var rSize = (int)(clickAnimProgress * rippleSize * 1.75);

            int dx = prevLocation - Location.X;
            prevLocation = Location.X;

            // Ripple
            if (_clickAnimManager.IsAnimating())
            {
                var rippleBrush = new SolidBrush(Color.FromArgb((int)(50 - (clickAnimProgress * 50)),
                    UseColors ? SkinManager.ColorScheme.AccentColor : // Using colors
                    SkinManager.Theme == MaterialSkinManager.Themes.LIGHT ? SkinManager.ColorScheme.PrimaryColor : // light theme
                    SkinManager.ColorScheme.LightPrimaryColor)); // dark theme

                g.SetClip(_tabPaths[_baseTabControl.SelectedIndex]);
                g.FillEllipse(rippleBrush, new Rectangle(_animationSource.X + dx - rSize / 2, _animationSource.Y - rSize / 2, rSize, rSize));
                g.ResetClip();
                rippleBrush.Dispose();
            }

            // Draw menu items
            foreach (TabPage tabPage in _baseTabControl.TabPages)
            {
                var currentTabIndex = _baseTabControl.TabPages.IndexOf(tabPage);

                // Background
                Brush bgBrush = new SolidBrush(Color.FromArgb(CalculateAlpha(100, 0, currentTabIndex, clickAnimProgress, 1 - showHideAnimProgress),
                    UseColors ? SkinManager.ColorScheme.LightPrimaryColor : // Using colors
                    SkinManager.Theme == MaterialSkinManager.Themes.LIGHT ? SkinManager.ColorScheme.PrimaryColor : // light theme
                    SkinManager.ColorScheme.PrimaryColor)); // dark theme
                g.FillPath(bgBrush, _tabPaths[currentTabIndex]);
                bgBrush.Dispose();

                // Text
                Brush textBrush = new SolidBrush(Color.FromArgb(CalculateAlphaZeroWhenClosed(SkinManager.ACTION_BAR_TEXT.A, UseColors ? SkinManager.ACTION_BAR_TEXT_SECONDARY.A : 255, currentTabIndex, clickAnimProgress, 1 - showHideAnimProgress), // alpha
                    UseColors ? (currentTabIndex == _baseTabControl.SelectedIndex ? SkinManager.ColorScheme.AccentColor : SkinManager.GetPrimaryTextColor()) :  // Use colors
                    (currentTabIndex == _baseTabControl.SelectedIndex ? SkinManager.Theme == MaterialSkinManager.Themes.LIGHT ? SkinManager.ColorScheme.DarkPrimaryColor : // selected on light theme
                    SkinManager.ColorScheme.AccentColor : // selected on dark theme
                    SkinManager.GetPrimaryTextColor()))); // not selected

                Rectangle textRect = _tabRects[currentTabIndex];
                textRect.X += _baseTabControl.ImageList != null ? tabHeight : (int)(SkinManager.FORM_PADDING / 1.5);
                textRect.Width -= SkinManager.FORM_PADDING;
                g.DrawString(tabPage.Text, SkinManager.ROBOTO_BOLD_10, textBrush, textRect, new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap, Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
                textBrush.Dispose();

                // Icons
                if (_baseTabControl.ImageList != null && !String.IsNullOrEmpty(tabPage.ImageKey))
                {
                    Rectangle iconRect = new Rectangle(
                        _tabRects[currentTabIndex].X + (tabHeight / 2) - (_baseTabControl.ImageList.Images[tabPage.ImageKey].Width / 2),
                        _tabRects[currentTabIndex].Y + (tabHeight / 2) - (_baseTabControl.ImageList.Images[tabPage.ImageKey].Height / 2),
                        _baseTabControl.ImageList.Images[tabPage.ImageKey].Width, _baseTabControl.ImageList.Images[tabPage.ImageKey].Height);

                    if (ShowIconsWhenHidden)
                        iconsBrushes[tabPage.ImageKey].TranslateTransform(dx, 0);
                    iconsSelectedBrushes[tabPage.ImageKey].TranslateTransform(dx, 0);

                    g.FillRectangle(currentTabIndex == _baseTabControl.SelectedIndex ? iconsSelectedBrushes[tabPage.ImageKey] : iconsBrushes[tabPage.ImageKey], iconRect);

                }
            }

            // Draw divider if not using colors
            if (!UseColors)
            {
                using (Pen dividerPen = new Pen(SkinManager.GetDividersColor(), 1))
                {
                    g.DrawLine(dividerPen, Width - 1, 0, Width - 1, Height);
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

        public new void Show()
        {
            _isOpen = true;
            DrawerStateChanged?.Invoke(this);
            DrawerBeginOpen?.Invoke(this);
            _showHideAnimManager.StartNewAnimation(AnimationDirection.Out);
        }

        public new void Hide()
        {
            _isOpen = false;
            DrawerStateChanged?.Invoke(this);
            DrawerBeginClose?.Invoke(this);
            _showHideAnimManager.StartNewAnimation(AnimationDirection.In);
        }

        public void Toggle()
        {
            if (_isOpen)
                Hide();
            else
                Show();
        }

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

        private void UpdateTabRects()
        {
            //If there isn't a base tab control, the rects shouldn't be calculated
            //If there aren't tab pages in the base tab control, the list should just be empty; exit the void
            if (_baseTabControl == null || _baseTabControl.TabCount == 0 || SkinManager == null || _tabRects == null)
            {
                _tabRects = new List<Rectangle>();
                _tabPaths = new List<GraphicsPath>();
                return;
            }

            if (_tabRects.Count != _baseTabControl.TabCount)
            {
                _tabRects = new List<Rectangle>(_baseTabControl.TabCount);
                _tabPaths = new List<GraphicsPath>(_baseTabControl.TabCount);

                for (var i = 0; i < _baseTabControl.TabCount; i++)
                {
                    _tabRects.Add(new Rectangle());
                    _tabPaths.Add(new GraphicsPath());
                }
            }

            //Calculate the bounds of each tab header specified in the base tab control
            for (int i = 0; i < _baseTabControl.TabPages.Count; i++)
            {
                _tabRects[i] = (new Rectangle((int)(SkinManager.FORM_PADDING / 1.5) - (ShowIconsWhenHidden ? Location.X : 0),
                    (TAB_HEADER_PADDING * 2) * i + SkinManager.FORM_PADDING / 2,
                    (Width + (ShowIconsWhenHidden ? Location.X : 0)) - (int)(SkinManager.FORM_PADDING * 1.5) - 1,
                    tabHeight));

                _tabPaths[i] = DrawHelper.CreateRoundRect(new RectangleF(_tabRects[i].X - 0.5f, _tabRects[i].Y - 0.5f, _tabRects[i].Width, _tabRects[i].Height), 4);
            }
        }
    }
}
