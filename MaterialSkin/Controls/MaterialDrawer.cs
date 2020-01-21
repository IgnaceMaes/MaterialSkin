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

        private bool _showIconsWhenHidden;

        [Category("Drawer")]

        public bool ShowIconsWhenHidden
        {
            get
            {
                return _showIconsWhenHidden;
            }
            set
            {
                if (_showIconsWhenHidden != value)
                {
                    _showIconsWhenHidden = value;
                    UpdateTabRects();
                    preProcessIcons();
                    showHideAnimation();
                    Paint(new PaintEventArgs(CreateGraphics(), ClientRectangle));
                    DrawerShowIconsWhenHiddenChanged?.Invoke(this);
                }
            }
        }

        private bool _isOpen;

        [Category("Drawer")]
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

        [Category("Drawer")]
        public bool AutoHide { get; set; }

        [Category("Drawer")]
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
                Invalidate();
            }
        }

        [Category("Drawer")]
        private bool _highlightWithAccent;
        public bool HighlightWithAccent
        {
            get
            {
                return _highlightWithAccent;
            }
            set
            {
                _highlightWithAccent = value;
                preProcessIcons();
                Invalidate();
            }
        }

        [Category("Drawer")]
        private bool _backgroundWithAccent;
        public bool BackgroundWithAccent
        {
            get
            {
                return _backgroundWithAccent;
            }
            set
            {
                _backgroundWithAccent = value;
                Invalidate();
            }
        }

        [Category("Drawer")]
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
        public event DrawerStateHandler DrawerShowIconsWhenHiddenChanged;

        // icons
        private Dictionary<string, TextureBrush> iconsBrushes;
        private Dictionary<string, TextureBrush> iconsSelectedBrushes;
        private Dictionary<string, Rectangle> iconsSize;
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
            if (_baseTabControl == null || _baseTabControl.TabCount == 0 || _baseTabControl.ImageList == null || _drawerItemRects == null || _drawerItemRects.Count == 0)
                return;

            // Calculate lightness and color
            float l = UseColors ? SkinManager.ColorScheme.TextColor.R / 255 : SkinManager.Theme == MaterialSkinManager.Themes.LIGHT ? 0f : 1f;
            float r = (_highlightWithAccent ? SkinManager.ColorScheme.AccentColor.R : SkinManager.ColorScheme.PrimaryColor.R) / 255f;
            float g = (_highlightWithAccent ? SkinManager.ColorScheme.AccentColor.G : SkinManager.ColorScheme.PrimaryColor.G) / 255f;
            float b = (_highlightWithAccent ? SkinManager.ColorScheme.AccentColor.B : SkinManager.ColorScheme.PrimaryColor.B) / 255f;

            // Create matrices
            float[][] matrixGray = {
                    new float[] {   0,   0,   0,   0,  0}, // Red scale factor
                    new float[] {   0,   0,   0,   0,  0}, // Green scale factor
                    new float[] {   0,   0,   0,   0,  0}, // Blue scale factor
                    new float[] {   0,   0,   0, .7f,  0}, // alpha scale factor
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
            iconsSize = new Dictionary<string, Rectangle>(_baseTabControl.TabPages.Count);

            foreach (TabPage tabPage in _baseTabControl.TabPages)
            {
                // skip items without image
                if (String.IsNullOrEmpty(tabPage.ImageKey) || _drawerItemRects == null)
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
                   _drawerItemRects[currentTabIndex].X + (drawerItemHeight / 2) - (_baseTabControl.ImageList.Images[tabPage.ImageKey].Width / 2),
                   _drawerItemRects[currentTabIndex].Y + (drawerItemHeight / 2) - (_baseTabControl.ImageList.Images[tabPage.ImageKey].Height / 2),
                   _baseTabControl.ImageList.Images[tabPage.ImageKey].Width, _baseTabControl.ImageList.Images[tabPage.ImageKey].Height);

                textureBrushGray.TranslateTransform(iconRect.X + iconRect.Width / 2 - _baseTabControl.ImageList.Images[tabPage.ImageKey].Width / 2,
                                                    iconRect.Y + iconRect.Height / 2 - _baseTabControl.ImageList.Images[tabPage.ImageKey].Height / 2);
                textureBrushColor.TranslateTransform(iconRect.X + iconRect.Width / 2 - _baseTabControl.ImageList.Images[tabPage.ImageKey].Width / 2,
                                                     iconRect.Y + iconRect.Height / 2 - _baseTabControl.ImageList.Images[tabPage.ImageKey].Height / 2);

                // add to dictionary
                iconsBrushes.Add(tabPage.ImageKey, textureBrushGray);
                iconsSelectedBrushes.Add(tabPage.ImageKey, textureBrushColor);
                iconsSize.Add(tabPage.ImageKey, new Rectangle(0, 0, iconRect.Width, iconRect.Height));
            }
        }

        private int _previousSelectedTabIndex;

        private Point _animationSource;

        private readonly AnimationManager _clickAnimManager;

        private readonly AnimationManager _showHideAnimManager;

        private List<Rectangle> _drawerItemRects;
        private List<GraphicsPath> _drawerItemPaths;

        private const int TAB_HEADER_PADDING = 24;

        private int drawerItemHeight;

        public int MinWidth;
        public MaterialDrawer()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            Height = 120;
            Width = 250;
            IndicatorWidth = 0;
            _isOpen = true;
            ShowIconsWhenHidden = false;
            AutoHide = false;
            HighlightWithAccent = true;
            BackgroundWithAccent = false;

            _showHideAnimManager = new AnimationManager
            {
                AnimationType = AnimationType.EaseInOut,
                Increment = 0.04
            };
            _showHideAnimManager.OnAnimationProgress += sender =>
            {
                Invalidate();
                showHideAnimation();
            };
            _showHideAnimManager.OnAnimationFinished += sender =>
            {
                if (_baseTabControl != null && _drawerItemRects.Count > 0)
                    rippleSize = _drawerItemRects[_baseTabControl.SelectedIndex].Width;
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

            drawerItemHeight = TAB_HEADER_PADDING * 2 - SkinManager.FORM_PADDING / 2;
            MinWidth = (int)(SkinManager.FORM_PADDING * 1.5 + drawerItemHeight);
            _showHideAnimManager.SetProgress(_isOpen ? 0 : 1);
            showHideAnimation();
            Invalidate();

            base.InitLayout();
        }

        private void showHideAnimation()
        {
            var showHideAnimProgress = _showHideAnimManager.GetProgress();
            if (_showHideAnimManager.IsAnimating())
            {
                if (ShowIconsWhenHidden)
                {
                    Location = new Point((int)((-Width + MinWidth) * showHideAnimProgress), Location.Y);
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
                        Location = new Point((int)(-Width + MinWidth), Location.Y);
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
            Paint(e);
        }

        private new void Paint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // redraw stuff
            g.Clear(UseColors ? SkinManager.ColorScheme.PrimaryColor : SkinManager.BackdropColor);

            if (_baseTabControl == null)
                return;

            if (!_clickAnimManager.IsAnimating() || _drawerItemRects == null || _drawerItemRects.Count != _baseTabControl.TabCount)
                UpdateTabRects();

            if (_drawerItemRects == null || _drawerItemRects.Count != _baseTabControl.TabCount)
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
                var rippleBrush = new SolidBrush(Color.FromArgb((int)(70 - (clickAnimProgress * 70)),
                    UseColors ? SkinManager.ColorScheme.AccentColor : // Using colors
                    SkinManager.Theme == MaterialSkinManager.Themes.LIGHT ? SkinManager.ColorScheme.PrimaryColor : // light theme
                    SkinManager.ColorScheme.LightPrimaryColor)); // dark theme

                g.SetClip(_drawerItemPaths[_baseTabControl.SelectedIndex]);
                g.FillEllipse(rippleBrush, new Rectangle(_animationSource.X + dx - (rSize / 2), _animationSource.Y - rSize / 2, rSize, rSize));
                g.ResetClip();
                rippleBrush.Dispose();
            }

            // Draw menu items
            foreach (TabPage tabPage in _baseTabControl.TabPages)
            {
                var currentTabIndex = _baseTabControl.TabPages.IndexOf(tabPage);

                // Background
                Brush bgBrush = new SolidBrush(Color.FromArgb(CalculateAlpha(60, 0, currentTabIndex, clickAnimProgress, 1 - showHideAnimProgress),
                    UseColors ? _backgroundWithAccent ? SkinManager.ColorScheme.AccentColor : SkinManager.ColorScheme.LightPrimaryColor : // using colors
                    _backgroundWithAccent ? SkinManager.ColorScheme.AccentColor : // defaul accent
                    SkinManager.Theme == MaterialSkinManager.Themes.LIGHT ? SkinManager.ColorScheme.PrimaryColor : // default light
                    SkinManager.ColorScheme.LightPrimaryColor)); // default dark
                g.FillPath(bgBrush, _drawerItemPaths[currentTabIndex]);
                bgBrush.Dispose();

                // Text
                Color textColor = Color.FromArgb(CalculateAlphaZeroWhenClosed(SkinManager.TextHighEmphasisColor.A, UseColors ? SkinManager.TextMediumEmphasisColor.A : 255, currentTabIndex, clickAnimProgress, 1 - showHideAnimProgress), // alpha
                    UseColors ? (currentTabIndex == _baseTabControl.SelectedIndex ? (_highlightWithAccent ? SkinManager.ColorScheme.AccentColor : SkinManager.ColorScheme.PrimaryColor) // Use colors - selected
                    : SkinManager.ColorScheme.TextColor) :  // Use colors - not selected
                    (currentTabIndex == _baseTabControl.SelectedIndex ? (_highlightWithAccent ? SkinManager.ColorScheme.AccentColor : SkinManager.ColorScheme.PrimaryColor) : // selected
                    SkinManager.TextHighEmphasisColor));

                IntPtr textFont = SkinManager.getLogFontByType(MaterialSkinManager.fontType.Subtitle2);

                Rectangle textRect = _drawerItemRects[currentTabIndex];
                textRect.X += _baseTabControl.ImageList != null ? drawerItemHeight : (int)(SkinManager.FORM_PADDING * 0.75);
                textRect.Width -= SkinManager.FORM_PADDING << 2;

                using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
                {
                    NativeText.DrawTransparentText(tabPage.Text, textFont, textColor, textRect.Location, textRect.Size, NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
                }

                // Icons
                if (_baseTabControl.ImageList != null && !String.IsNullOrEmpty(tabPage.ImageKey))
                {
                    Rectangle iconRect = new Rectangle(
                        _drawerItemRects[currentTabIndex].X + (drawerItemHeight >> 1) - (iconsSize[tabPage.ImageKey].Width >> 1),
                        _drawerItemRects[currentTabIndex].Y + (drawerItemHeight >> 1) - (iconsSize[tabPage.ImageKey].Height >> 1),
                        iconsSize[tabPage.ImageKey].Width, iconsSize[tabPage.ImageKey].Height);

                    if (ShowIconsWhenHidden)
                    {
                        iconsBrushes[tabPage.ImageKey].TranslateTransform(dx, 0);
                        iconsSelectedBrushes[tabPage.ImageKey].TranslateTransform(dx, 0);
                    }

                    g.FillRectangle(currentTabIndex == _baseTabControl.SelectedIndex ? iconsSelectedBrushes[tabPage.ImageKey] : iconsBrushes[tabPage.ImageKey], iconRect);
                }
            }

            // Draw divider if not using colors
            if (!UseColors)
            {
                using (Pen dividerPen = new Pen(SkinManager.DividersColor, 1))
                {
                    g.DrawLine(dividerPen, Width - 1, 0, Width - 1, Height);
                }
            }


            // Animate tab indicator
            var previousSelectedTabIndexIfHasOne = _previousSelectedTabIndex == -1 ? _baseTabControl.SelectedIndex : _previousSelectedTabIndex;
            var previousActiveTabRect = _drawerItemRects[previousSelectedTabIndexIfHasOne];
            var activeTabPageRect = _drawerItemRects[_baseTabControl.SelectedIndex];

            var y = previousActiveTabRect.Y + (int)((activeTabPageRect.Y - previousActiveTabRect.Y) * clickAnimProgress);
            var x = ShowIconsWhenHidden ? -Location.X : 0;
            var height = drawerItemHeight;

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

            if (_drawerItemRects == null)
                UpdateTabRects();
            for (var i = 0; i < _drawerItemRects.Count; i++)
            {
                if (_drawerItemRects[i].Contains(e.Location))
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

            if (_drawerItemRects == null)
                UpdateTabRects();
            for (var i = 0; i < _drawerItemRects.Count; i++)
            {
                if (_drawerItemRects[i].Contains(e.Location))
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
            //or if there aren't tab pages in the base tab control, the list should just be empty
            if (_baseTabControl == null || _baseTabControl.TabCount == 0 || SkinManager == null || _drawerItemRects == null)
            {
                _drawerItemRects = new List<Rectangle>();
                _drawerItemPaths = new List<GraphicsPath>();
                return;
            }

            if (_drawerItemRects.Count != _baseTabControl.TabCount)
            {
                _drawerItemRects = new List<Rectangle>(_baseTabControl.TabCount);
                _drawerItemPaths = new List<GraphicsPath>(_baseTabControl.TabCount);

                for (var i = 0; i < _baseTabControl.TabCount; i++)
                {
                    _drawerItemRects.Add(new Rectangle());
                    _drawerItemPaths.Add(new GraphicsPath());
                }
            }
            
            //Calculate the bounds of each tab header specified in the base tab control
            for (int i = 0; i < _baseTabControl.TabPages.Count; i++)
            {
                _drawerItemRects[i] = (new Rectangle(
                    (int)(SkinManager.FORM_PADDING * 0.75) - (ShowIconsWhenHidden ? Location.X : 0),
                    (TAB_HEADER_PADDING * 2) * i + (int)(SkinManager.FORM_PADDING >> 1),
                    (Width + (ShowIconsWhenHidden ? Location.X : 0)) - (int)(SkinManager.FORM_PADDING * 1.5) - 1,
                    drawerItemHeight));

                _drawerItemPaths[i] = DrawHelper.CreateRoundRect(new RectangleF(_drawerItemRects[i].X - 0.5f, _drawerItemRects[i].Y - 0.5f, _drawerItemRects[i].Width, _drawerItemRects[i].Height), 4);
            }
        }
    }
}
