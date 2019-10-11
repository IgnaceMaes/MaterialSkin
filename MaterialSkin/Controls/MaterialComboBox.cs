namespace MaterialSkin.Controls
{
    using MaterialSkin.Animations;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    public class MaterialComboBox : ComboBox, IMaterialControl
    {
        [EditorBrowsable(EditorBrowsableState.Always), Description("Auto adjust the width of the combobox to the longest item text"), Category("Layout")]
        bool _AutoSize;
        public override bool AutoSize
        {
            get { return _AutoSize; }
            set
            {
                _AutoSize = value;
                recalculateAutoSize();
            }
        }

        //Properties for managing the material design properties
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        private bool _UseTallSize;
        [Category("Material Skin"), DefaultValue(true), Description("Using a larger size enables the hint to always be visible")]
        public bool UseTallSize
        {
            get { return _UseTallSize; }
            set
            {
                _UseTallSize = value;
                setHeightVars();
                Invalidate();
            }
        }

        [Category("Material Skin"), DefaultValue(true)]
        public bool UseAccent { get; set; }

        private string _hint = string.Empty;
        [Category("Material Skin"), DefaultValue("")]
        public string Hint
        {
            get { return _hint; }
            set
            {
                _hint = value;
                hasHint = !String.IsNullOrEmpty(Hint);
                Invalidate();
            }
        }

        private const int TEXT_SMALL_SIZE = 18;
        private const int TEXT_SMALL_Y = 4;
        private const int BOTTOM_PADDING = 3;
        private int HEIGHT = 50;
        private int LINE_Y;

        private bool hasHint;

        private readonly AnimationManager _animationManager;


        public MaterialComboBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            // Material Properties
            Hint = "";
            UseAccent = true;
            UseTallSize = true;
            MaxDropDownItems = 4;

            Font = SkinManager.getFontByType(MaterialSkinManager.fontType.Subtitle2);
            BackColor = SkinManager.BackgroundColor;
            ForeColor = SkinManager.TextHighEmphasisColor;
            DrawMode = DrawMode.OwnerDrawVariable;
            DropDownStyle = ComboBoxStyle.DropDownList;
            DropDownWidth = Width;

            // Animations
            _animationManager = new AnimationManager(true)
            {
                Increment = 0.08,
                AnimationType = AnimationType.EaseInOut
            };
            _animationManager.OnAnimationProgress += sender => Invalidate();

            DropDownClosed += (sender, args) =>
            {
                MouseState = MouseState.OUT;
                if (SelectedIndex < 0 && !Focused) _animationManager.StartNewAnimation(AnimationDirection.Out);
            };
            LostFocus += (sender, args) =>
            {
                MouseState = MouseState.OUT;
                if (SelectedIndex < 0) _animationManager.StartNewAnimation(AnimationDirection.Out);
            };
            DropDown += (sender, args) =>
            {
                _animationManager.StartNewAnimation(AnimationDirection.In);
            };
            GotFocus += (sender, args) =>
            {
                _animationManager.StartNewAnimation(AnimationDirection.In);
            };
            MouseEnter += (sender, args) =>
            {
                MouseState = MouseState.HOVER;
                Invalidate();
            };
            MouseLeave += (sender, args) =>
            {
                MouseState = MouseState.OUT;
                Invalidate();
            };
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;

            g.Clear(Parent.BackColor);
            g.FillRectangle(Enabled ? Focused ?
                SkinManager.BackgroundFocusBrush : // Focused
                MouseState == MouseState.HOVER ?
                SkinManager.BackgroundHoverBrush : // Hover
                SkinManager.BackgroundAlternativeBrush : // normal
                SkinManager.BackgroundDisabledBrush // Disabled
                , ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, LINE_Y);

            // Create and Draw the arrow
            System.Drawing.Drawing2D.GraphicsPath pth = new System.Drawing.Drawing2D.GraphicsPath();
            PointF TopRight = new PointF(this.Width - 0.5f - SkinManager.FORM_PADDING, (this.Height >> 1) - 2.5f);
            PointF MidBottom = new PointF(this.Width - 4.5f - SkinManager.FORM_PADDING, (this.Height >> 1) + 2.5f);
            PointF TopLeft = new PointF(this.Width - 8.5f - SkinManager.FORM_PADDING, (this.Height >> 1) - 2.5f);
            pth.AddLine(TopLeft, TopRight);
            pth.AddLine(TopRight, MidBottom);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.FillPath((SolidBrush)(DroppedDown || Focused ? UseAccent ? SkinManager.ColorScheme.AccentBrush : SkinManager.ColorScheme.PrimaryBrush : SkinManager.TextHighEmphasisBrush), pth);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            // HintText
            bool userTextPresent = SelectedIndex >= 0 && !_animationManager.IsAnimating();
            Color textColor = Enabled ? DroppedDown || Focused ?
                            UseAccent ? SkinManager.ColorScheme.AccentColor : SkinManager.ColorScheme.PrimaryColor : // DroppedDown / Focus
                            SkinManager.TextHighEmphasisColor : // Inactive
                            SkinManager.TextDisabledOrHintColor; // Disabled
            Rectangle hintRect = new Rectangle(SkinManager.FORM_PADDING, ClientRectangle.Y, Width, LINE_Y);
            int hintTextSize = 16;

            // bottom line base
            g.FillRectangle(SkinManager.DividersAlternativeBrush, 0, LINE_Y, Width, 1);

            if (!_animationManager.IsAnimating())
            {
                // No animation
                if (hasHint && UseTallSize && (DroppedDown || Focused || SelectedIndex >= 0))
                {
                    // hint text
                    hintRect = new Rectangle(SkinManager.FORM_PADDING, TEXT_SMALL_Y, Width, TEXT_SMALL_SIZE);
                    hintTextSize = 12;
                }

                // bottom line
                if (DroppedDown || Focused)
                {
                    g.FillRectangle(UseAccent ? SkinManager.ColorScheme.AccentBrush : SkinManager.ColorScheme.PrimaryBrush, 0, LINE_Y, Width, 2);
                }
            }
            else
            {
                // Animate - Focus got/lost
                double animationProgress = _animationManager.GetProgress();

                // hint Animation
                if (hasHint && UseTallSize)
                {
                    hintRect = new Rectangle(
                        SkinManager.FORM_PADDING,
                        userTextPresent ? (TEXT_SMALL_Y) : ClientRectangle.Y + (int)((TEXT_SMALL_Y - ClientRectangle.Y) * animationProgress),
                        Width,
                        userTextPresent ? (TEXT_SMALL_SIZE) : (int)(LINE_Y + (TEXT_SMALL_SIZE - LINE_Y) * animationProgress));
                    hintTextSize = userTextPresent ? 12 : (int)(16 + (12 - 16) * animationProgress);
                }

                // Line Animation
                int LineAnimationWidth = (int)(Width * animationProgress);
                int LineAnimationX = (Width / 2) - (LineAnimationWidth / 2);
                g.FillRectangle(UseAccent ? SkinManager.ColorScheme.AccentBrush : SkinManager.ColorScheme.PrimaryBrush, LineAnimationX, LINE_Y, LineAnimationWidth, 2);
            }

            // Text stuff:
            string textToDisplay = Text;
            string textSelected;
            Rectangle textSelectRect;

            // Calc text Rect
            Rectangle textRect = new Rectangle(
                SkinManager.FORM_PADDING,
                hasHint && UseTallSize ? (hintRect.Y + hintRect.Height) - 2 : ClientRectangle.Y,
                ClientRectangle.Width - SkinManager.FORM_PADDING * 3 - 8,
                hasHint && UseTallSize ? LINE_Y - (hintRect.Y + hintRect.Height) : LINE_Y);

            g.Clip = new Region(textRect);

            using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            {
                // Selection rects calc
                string textBeforeSelection = textToDisplay.Substring(0, SelectionStart);
                textSelected = textToDisplay.Substring(SelectionStart, SelectionLength);

                int selectX = NativeText.MeasureLogString(textBeforeSelection, SkinManager.getLogFontByType(MaterialSkinManager.fontType.Subtitle1)).Width;
                int selectWidth = NativeText.MeasureLogString(textSelected, SkinManager.getLogFontByType(MaterialSkinManager.fontType.Subtitle1)).Width;

                textSelectRect = new Rectangle(
                    textRect.X + selectX, UseTallSize ? hasHint ?
                     textRect.Y + BOTTOM_PADDING : // tall and hint
                     LINE_Y / 3 - BOTTOM_PADDING : // tall and no hint
                     BOTTOM_PADDING, // not tall
                    selectWidth,
                    UseTallSize ? hasHint ?
                    textRect.Height - BOTTOM_PADDING * 2 : // tall and hint
                    (int)(LINE_Y / 2) : // tall and no hint
                    LINE_Y - BOTTOM_PADDING * 2); // not tall

                // Draw user text
                NativeText.DrawTransparentText(
                    textToDisplay,
                    SkinManager.getLogFontByType(MaterialSkinManager.fontType.Subtitle1),
                    Enabled ? SkinManager.TextHighEmphasisColor : SkinManager.TextDisabledOrHintColor,
                    textRect.Location,
                    textRect.Size,
                    NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
            }

            g.Clip = new Region(ClientRectangle);

            // Draw hint text
            if (hasHint && (UseTallSize || String.IsNullOrEmpty(Text)))
            {
                using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
                {
                    NativeText.DrawTransparentText(
                    Hint,
                    SkinManager.getTextBoxFontBySize(hintTextSize),
                    Enabled ? DroppedDown || Focused ? UseAccent ?
                    SkinManager.ColorScheme.AccentColor : // Focus Accent
                    SkinManager.ColorScheme.PrimaryColor : // Focus Primary
                    SkinManager.TextMediumEmphasisColor : // not focused
                    SkinManager.TextDisabledOrHintColor, // Disabled
                    hintRect.Location,
                    hintRect.Size,
                    NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
                }
            }
        }


        private void CustomMeasureItem(object sender, System.Windows.Forms.MeasureItemEventArgs e)
        {
            e.ItemHeight = HEIGHT - 7;
        }

        private void CustomDrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index > Items.Count || !Focused) return;

            Graphics g = e.Graphics;

            // Draw the background of the item.
            g.FillRectangle(SkinManager.BackgroundBrush, e.Bounds);

            // Hover
            if (e.State.HasFlag(DrawItemState.Focus)) // Focus == hover
            {
                g.FillRectangle(SkinManager.BackgroundHoverBrush, e.Bounds);
            }

            using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            {
                NativeText.DrawTransparentText(
                Items[e.Index].ToString(),
                SkinManager.getFontByType(MaterialSkinManager.fontType.Subtitle1),
                SkinManager.TextHighEmphasisColor,
                new Point(e.Bounds.Location.X + SkinManager.FORM_PADDING, e.Bounds.Location.Y),
                new Size(e.Bounds.Size.Width - SkinManager.FORM_PADDING * 2, e.Bounds.Size.Height),
                NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle); ;
            }

        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            MouseState = MouseState.OUT;
            MeasureItem += CustomMeasureItem;
            DrawItem += CustomDrawItem;
            DropDownStyle = ComboBoxStyle.DropDownList;
            DrawMode = DrawMode.OwnerDrawVariable;
            recalculateAutoSize();
            setHeightVars();
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            recalculateAutoSize();
            setHeightVars();
        }

        private void setHeightVars()
        {
            HEIGHT = UseTallSize ? 50 : 36;
            Size = new Size(Size.Width, HEIGHT);
            LINE_Y = HEIGHT - BOTTOM_PADDING;
            ItemHeight = HEIGHT - 7;
            DropDownHeight = ItemHeight * MaxDropDownItems + 2;
        }

        public void recalculateAutoSize()
        {
            Console.WriteLine(AutoSize);
            if (!AutoSize) return;

            int w = DropDownWidth;
            int padding = SkinManager.FORM_PADDING * 3;
            int vertScrollBarWidth = (Items.Count > MaxDropDownItems) ? SystemInformation.VerticalScrollBarWidth : 0;

            Graphics g = CreateGraphics();
            using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            {

                var itemsList = this.Items.Cast<object>().Select(item => item.ToString());
                foreach (string s in itemsList)
                {
                    int newWidth = NativeText.MeasureLogString(s, SkinManager.getLogFontByType(MaterialSkinManager.fontType.Subtitle1)).Width + vertScrollBarWidth + padding;
                    if (w < newWidth) w = newWidth;
                }
            }

            Console.WriteLine(w);

            DropDownWidth = w;
            Width = w;
        }

        // Remove dropdown border
    }
}
