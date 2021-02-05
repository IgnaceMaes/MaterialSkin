namespace MaterialSkin.Controls
{
    using MaterialSkin.Animations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Text;
    using System.Windows.Forms;

    public class MaterialTabSelector : Control, IMaterialControl
    {
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        private MaterialTabControl _baseTabControl;

        [Category("Material Skin"), Browsable(true)]
        public MaterialTabControl BaseTabControl
        {
            get { return _baseTabControl; }
            set
            {
                _baseTabControl = value;
                if (_baseTabControl == null) return;

                UpdateTabRects();

                _previousSelectedTabIndex = _baseTabControl.SelectedIndex;
                _baseTabControl.Deselected += (sender, args) =>
                {
                    _previousSelectedTabIndex = _baseTabControl.SelectedIndex;
                };
                _baseTabControl.SelectedIndexChanged += (sender, args) =>
                {
                    _animationManager.SetProgress(0);
                    _animationManager.StartNewAnimation(AnimationDirection.In);
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

        private int _previousSelectedTabIndex;

        private Point _animationSource;

        private readonly AnimationManager _animationManager;

        private List<Rectangle> _tabRects;

        private const int TAB_HEADER_PADDING = 24;

        private int _tab_over_index = -1;

        private int _tab_indicator_height;

        [Category("Material Skin"), Browsable(true), DisplayName("Tab Indicator Height"), DefaultValue(2)]
        public int TabIndicatorHeight 
        {
            get { return _tab_indicator_height; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("Tab Indicator Height", value, "Value should be > 0");
                else
                {
                    _tab_indicator_height = value;
                    Refresh();
                }
            }
        }

        public MaterialTabSelector()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);
            Height = 48;
            TabIndicatorHeight = 2;

            _animationManager = new AnimationManager
            {
                AnimationType = AnimationType.EaseOut,
                Increment = 0.04
            };
            _animationManager.OnAnimationProgress += sender => Invalidate();
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            Font = SkinManager.getFontByType(MaterialSkinManager.fontType.Body1);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            g.Clear(SkinManager.ColorScheme.PrimaryColor);

            if (_baseTabControl == null) return;

            if (!_animationManager.IsAnimating() || _tabRects == null || _tabRects.Count != _baseTabControl.TabCount)
                UpdateTabRects();

            var animationProgress = _animationManager.GetProgress();

            //Click feedback
            if (_animationManager.IsAnimating())
            {
                var rippleBrush = new SolidBrush(Color.FromArgb((int)(51 - (animationProgress * 50)), Color.White));
                var rippleSize = (int)(animationProgress * _tabRects[_baseTabControl.SelectedIndex].Width * 1.75);

                g.SetClip(_tabRects[_baseTabControl.SelectedIndex]);
                g.FillEllipse(rippleBrush, new Rectangle(_animationSource.X - rippleSize / 2, _animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
                g.ResetClip();
                rippleBrush.Dispose();
            }

            //Draw tab headers
            if (_tab_over_index >= 0)
            { 
                //Change mouse over tab background color
                g.FillRectangle(SkinManager.BackgroundHoverBrush , _tabRects[_tab_over_index].X, _tabRects[_tab_over_index].Y , _tabRects[_tab_over_index].Width, _tabRects[_tab_over_index].Height - _tab_indicator_height);
            }

            foreach (TabPage tabPage in _baseTabControl.TabPages)
            {
                var currentTabIndex = _baseTabControl.TabPages.IndexOf(tabPage);

                // Text
                using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
                {
                    var IconWidth = String.IsNullOrEmpty(tabPage.ImageKey) && tabPage.ImageIndex == -1 ? 0 : _baseTabControl.ImageList.ImageSize.Width/2;
                    Rectangle textLocation = new Rectangle(_tabRects[currentTabIndex].X + IconWidth, _tabRects[currentTabIndex].Y, _tabRects[currentTabIndex].Width, _tabRects[currentTabIndex].Height);
                    NativeText.DrawTransparentText(
                        tabPage.Text.ToUpper(),
                        SkinManager.getLogFontByType(MaterialSkinManager.fontType.Button),
                        Color.FromArgb(CalculateTextAlpha(currentTabIndex, animationProgress), SkinManager.ColorScheme.TextColor),
                        textLocation.Location,
                        textLocation.Size,
                        NativeTextRenderer.TextAlignFlags.Center | NativeTextRenderer.TextAlignFlags.Middle);
                }
 
                // Icons
                if (_baseTabControl.ImageList != null && (!String.IsNullOrEmpty(tabPage.ImageKey) | tabPage.ImageIndex > -1))
                {
                    Size ImageSize = !String.IsNullOrEmpty(tabPage.ImageKey) ? _baseTabControl.ImageList.Images[tabPage.ImageKey].Size : _baseTabControl.ImageList.Images[tabPage.ImageIndex].Size ;
                    Rectangle iconRect = new Rectangle(
                        _tabRects[currentTabIndex].X + (Height / 2) - (ImageSize.Width / 2),
                        _tabRects[currentTabIndex].Y + (Height / 2) - (ImageSize.Height / 2),
                        ImageSize.Width, ImageSize.Height);
                    g.DrawImage(!String.IsNullOrEmpty(tabPage.ImageKey) ? _baseTabControl.ImageList.Images[tabPage.ImageKey]: _baseTabControl.ImageList.Images[tabPage.ImageIndex], iconRect);
                }
                    
           }

            //Animate tab indicator
            var previousSelectedTabIndexIfHasOne = _previousSelectedTabIndex == -1 ? _baseTabControl.SelectedIndex : _previousSelectedTabIndex;
            var previousActiveTabRect = _tabRects[previousSelectedTabIndexIfHasOne];
            var activeTabPageRect = _tabRects[_baseTabControl.SelectedIndex];

            var y = activeTabPageRect.Bottom - _tab_indicator_height;
            var x = previousActiveTabRect.X + (int)((activeTabPageRect.X - previousActiveTabRect.X) * animationProgress);
            var width = previousActiveTabRect.Width + (int)((activeTabPageRect.Width - previousActiveTabRect.Width) * animationProgress);

            g.FillRectangle(SkinManager.ColorScheme.AccentBrush, x, y, width, _tab_indicator_height);
        }

        private int CalculateTextAlpha(int tabIndex, double animationProgress)
        {
            int primaryA = SkinManager.TextHighEmphasisColor.A;
            int secondaryA = SkinManager.TextMediumEmphasisColor.A;

            if (tabIndex == _baseTabControl.SelectedIndex && !_animationManager.IsAnimating())
            {
                return primaryA;
            }
            if (tabIndex != _previousSelectedTabIndex && tabIndex != _baseTabControl.SelectedIndex)
            {
                return secondaryA;
            }
            if (tabIndex == _previousSelectedTabIndex)
            {
                return primaryA - (int)((primaryA - secondaryA) * animationProgress);
            }
            return secondaryA + (int)((primaryA - secondaryA) * animationProgress);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_tabRects == null) UpdateTabRects();
            for (var i = 0; i < _tabRects.Count; i++)
            {
                if (_tabRects[i].Contains(e.Location))
                {
                    _baseTabControl.SelectedIndex = i;
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

            int old_tab_over_index = _tab_over_index;
            _tab_over_index = -1;
            for (var i = 0; i < _tabRects.Count; i++)
            {
                if (_tabRects[i].Contains(e.Location))
                {
                    Cursor = Cursors.Hand;
                    _tab_over_index = i;
                    break;
                }
            }
            if (_tab_over_index == -1)
                Cursor = Cursors.Arrow;
            if (old_tab_over_index != _tab_over_index)
                Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (DesignMode)
                return;

            if (_tabRects == null)
                UpdateTabRects();

            Cursor = Cursors.Arrow;
            _tab_over_index = -1;
            Invalidate();
        }

        private void UpdateTabRects()
        {
            _tabRects = new List<Rectangle>();

            //If there isn't a base tab control, the rects shouldn't be calculated
            //If there aren't tab pages in the base tab control, the list should just be empty which has been set already; exit the void
            if (_baseTabControl == null || _baseTabControl.TabCount == 0) return;

            //Calculate the bounds of each tab header specified in the base tab control
            using (var b = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(b))
                {
                    int ImgWidth=0;
                    if (_baseTabControl.ImageList != null && (!String.IsNullOrEmpty(_baseTabControl.TabPages[0].ImageKey) | _baseTabControl.TabPages[0].ImageIndex > -1)) ImgWidth = _baseTabControl.ImageList.ImageSize.Width;
                    Size textSize = TextRenderer.MeasureText(_baseTabControl.TabPages[0].Text, Font);
                    _tabRects.Add(new Rectangle(SkinManager.FORM_PADDING, 0, (TAB_HEADER_PADDING * 2) + textSize.Width + ImgWidth, Height));
                    for (int i = 1; i < _baseTabControl.TabPages.Count; i++)
                    {
                        if (_baseTabControl.ImageList != null && (!String.IsNullOrEmpty(_baseTabControl.TabPages[i].ImageKey) | _baseTabControl.TabPages[i].ImageIndex > -1)) { ImgWidth = _baseTabControl.ImageList.ImageSize.Width; } else { ImgWidth = 0; };
                        textSize = TextRenderer.MeasureText(_baseTabControl.TabPages[i].Text, Font);
                        _tabRects.Add(new Rectangle(_tabRects[i - 1].Right, 0, (TAB_HEADER_PADDING * 2) + textSize.Width + ImgWidth, Height));
                    }
                }
            }
        }
    }
}
