namespace MaterialSkin.Controls
{
    using MaterialSkin.Animations;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="MaterialButton" />
    /// </summary>
    public class MaterialButton : Button, IMaterialControl
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

        public enum MaterialButtonType
        {
            Text,
            Outlined,
            Contained
        }

        public bool UseAccentColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HighEmphasis
        /// </summary>
        public bool HighEmphasis { get; set; }

        public bool DrawShadows { get; set; }

        private MaterialButtonType _type;

        /// <summary>
        /// Gets or sets a value indicating whether HighEmphasis
        /// </summary>
        public MaterialButtonType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                if (DrawShadows && Type == MaterialButtonType.Contained && Parent != null)
                {
                    Parent.Paint += new PaintEventHandler(drawShadowOnParent);
                }
            }
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            Invalidate();
        }


        /// <summary>
        /// Defines the _animationManager
        /// </summary>
        private readonly AnimationManager _animationManager;

        /// <summary>
        /// Defines the _hoverAnimationManager
        /// </summary>
        private readonly AnimationManager _hoverAnimationManager;

        /// <summary>
        /// Defines the _textSize
        /// </summary>
        private SizeF _textSize;

        /// <summary>
        /// Defines the _icon
        /// </summary>
        private Image _icon;

        /// <summary>
        /// Gets or sets the Icon
        /// </summary>
        public Image Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                if (AutoSize)
                {
                    Size = GetPreferredSize();
                }

                Invalidate();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialButton"/> class.
        /// </summary>
        public MaterialButton()
        {
            DrawShadows = true;
            HighEmphasis = true;
            UseAccentColor = false;
            Type = MaterialButtonType.Contained;

            _animationManager = new AnimationManager(false)
            {
                Increment = 0.03,
                AnimationType = AnimationType.EaseOut
            };
            _hoverAnimationManager = new AnimationManager
            {
                Increment = 0.07,
                AnimationType = AnimationType.Linear
            };

            _hoverAnimationManager.OnAnimationProgress += sender => Invalidate();
            _animationManager.OnAnimationProgress += sender => Invalidate();

            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AutoSize = true;
            Margin = new Padding(4, 6, 4, 6);
            Padding = new Padding(0);
        }

        /// <summary>
        /// Gets or sets the Text
        /// </summary>
        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                _textSize = CreateGraphics().MeasureString(value.ToUpper(), SkinManager.ROBOTO_MEDIUM_10);
                if (AutoSize)
                {
                    Size = GetPreferredSize();
                }

                Invalidate();
            }
        }

        private void drawShadowOnParent(object sender, PaintEventArgs e)
        {
            // paint shadow on parent
            Graphics gp = e.Graphics;
            Matrix mx = new Matrix(1F, 0, 0, 1F, Location.X, Location.Y);
            gp.Transform = mx;
            gp.SmoothingMode = SmoothingMode.AntiAlias;
            drawShadow(gp, ClientRectangle);
        }

        void drawShadow(Graphics g, Rectangle bounds)
        {
            SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(12, 0, 0, 0));
            g.FillRectangle(shadowBrush, new Rectangle(bounds.X - 2, bounds.Y - 1, bounds.Width + 4, bounds.Height + 6));
            g.FillRectangle(shadowBrush, new Rectangle(bounds.X - 1, bounds.Y - 1, bounds.Width + 2, bounds.Height + 4));
            g.FillRectangle(shadowBrush, new Rectangle(bounds.X - 0, bounds.Y - 0, bounds.Width + 0, bounds.Height + 2));
            g.FillRectangle(shadowBrush, new Rectangle(bounds.X - 0, bounds.Y + 2, bounds.Width + 0, bounds.Height + 0));
            g.FillRectangle(shadowBrush, new Rectangle(bounds.X - 0, bounds.Y + 1, bounds.Width + 0, bounds.Height + 0));
        }

        /// <summary>
        /// The OnPaint
        /// </summary>
        /// <param name="pevent">The pevent<see cref="PaintEventArgs"/></param>
        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            double hoverAnimProgress = _hoverAnimationManager.GetProgress();

            if (Type == MaterialButtonType.Contained)
            {
                g.Clear(Parent.BackColor);

                // draw button rect
                if (!Enabled)
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(30, SkinManager.GetDisabledOrHintColor().RemoveAlpha())), ClientRectangle);
                }
                else if (HighEmphasis)
                {

                    g.FillRectangle(UseAccentColor ? SkinManager.ColorScheme.AccentBrush : SkinManager.ColorScheme.PrimaryBrush, ClientRectangle);
                }
                else
                {
                    g.FillRectangle(new SolidBrush(Parent.BackColor), ClientRectangle);
                }
            }
            else
            {
                g.Clear(Parent.BackColor);
            }

            //Hover
            Color c = SkinManager.GetButtonHoverBackgroundColor();

            g.FillRectangle(new SolidBrush(
                Color.FromArgb(
                    (int)(hoverAnimProgress * c.A), (UseAccentColor ? (HighEmphasis && Type == MaterialButtonType.Contained ?
                    SkinManager.ColorScheme.AccentColor.Lighten(0.5f) : // Contained with Emphasis - with accent
                    SkinManager.ColorScheme.AccentColor) : // Not Contained Or Low Emphasis - with accent
                    (Type == MaterialButtonType.Contained && HighEmphasis ? SkinManager.ColorScheme.LightPrimaryColor : // Contained with Emphasis without accent
                    SkinManager.ColorScheme.PrimaryColor)).RemoveAlpha())), // Normal or Emphasis without accent
                ClientRectangle);

            if (Type == MaterialButtonType.Outlined)
            {
                Point[] ClientRectPoints = new Point[] {
                    new Point(ClientRectangle.X+1, ClientRectangle.Y), new Point(ClientRectangle.Width-2, ClientRectangle.Y),
                    new Point(ClientRectangle.Width-1, ClientRectangle.Y+1), new Point(ClientRectangle.Width-1, ClientRectangle.Height-2),
                    new Point(ClientRectangle.Width-2, ClientRectangle.Height-1), new Point(ClientRectangle.X+1, ClientRectangle.Height-1),
                    new Point(ClientRectangle.X, ClientRectangle.Height-2), new Point(ClientRectangle.X, ClientRectangle.Y+1)
                };
                g.DrawLines(new Pen(SkinManager.GetDisabledOrHintColor(), 1), ClientRectPoints);
            }

            //Ripple
            if (_animationManager.IsAnimating())
            {
                // g.SmoothingMode = SmoothingMode.AntiAlias;
                for (var i = 0; i < _animationManager.GetAnimationCount(); i++)
                {
                    var animationValue = _animationManager.GetProgress(i);
                    var animationSource = _animationManager.GetSource(i);

                    using (Brush rippleBrush = new SolidBrush(
                        Color.FromArgb((int)(100 - (animationValue * 100)),
                        (Type == MaterialButtonType.Contained && HighEmphasis ? (UseAccentColor ?
                            SkinManager.ColorScheme.AccentColor.Lighten(0.5f) : // Emphasis with accent
                            SkinManager.ColorScheme.LightPrimaryColor) : // Emphasis
                            (UseAccentColor ? SkinManager.ColorScheme.AccentColor : // Normal with accent
                            SkinManager.ColorScheme.PrimaryColor))))) // Normal
                    {
                        var rippleSize = (int)(animationValue * Width * 2);
                        g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
                    }
                }
                g.SmoothingMode = SmoothingMode.None;
            }

            //Icon
            var iconRect = new Rectangle(8, 6, 24, 24);

            if (string.IsNullOrEmpty(Text))
            {
                // Center Icon
                iconRect.X += 2;
            }

            if (Icon != null)
            {
                g.DrawImage(Icon, iconRect);
            }

            //Text
            var textRect = ClientRectangle;
            if (Icon != null)
            {
                textRect.Width -= 8 + 24 + 4 + 8; // left padding + icon width + space between Icon and Text + right padding
                textRect.X += 8 + 24 + 4; // left padding + icon width + space between Icon and Text
            }

            g.DrawString(
                Text.ToUpper(),
                SkinManager.ROBOTO_MEDIUM_10,
                Enabled ? (HighEmphasis ? (Type == MaterialButtonType.Text || Type == MaterialButtonType.Outlined) ?
                (UseAccentColor ? SkinManager.ColorScheme.AccentBrush : SkinManager.ColorScheme.PrimaryBrush) : // Outline and text primary
                SkinManager.ColorScheme.TextBrush : // Contained HighEmphasis
                SkinManager.GetPrimaryTextBrush()) : // Secondary
                SkinManager.GetButtonDisabledTextBrush(), // Disabled
                textRect,
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center }
                );
        }

        /// <summary>
        /// The GetPreferredSize
        /// </summary>
        /// <returns>The <see cref="Size"/></returns>
        private Size GetPreferredSize()
        {
            return GetPreferredSize(new Size(0, 0));
        }

        /// <summary>
        /// The GetPreferredSize
        /// </summary>
        /// <param name="proposedSize">The proposedSize<see cref="Size"/></param>
        /// <returns>The <see cref="Size"/></returns>
        public override Size GetPreferredSize(Size proposedSize)
        {
            // Provides extra space for proper padding for content
            var extra = 16;

            if (Icon != null)
            {
                // 24 is for icon size
                // 4 is for the space between icon & text
                extra += 24 + 4;
            }

            return new Size((int)Math.Ceiling(_textSize.Width) + extra, 36);
        }

        /// <summary>
        /// The OnCreateControl
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (DesignMode)
            {
                return;
            }

            MouseState = MouseState.OUT;
            MouseEnter += (sender, args) =>
            {
                MouseState = MouseState.HOVER;
                _hoverAnimationManager.StartNewAnimation(AnimationDirection.In);
                Invalidate();
            };
            MouseLeave += (sender, args) =>
            {
                MouseState = MouseState.OUT;
                _hoverAnimationManager.StartNewAnimation(AnimationDirection.Out);
                Invalidate();
            };
            MouseDown += (sender, args) =>
            {
                if (args.Button == MouseButtons.Left)
                {
                    MouseState = MouseState.DOWN;

                    _animationManager.StartNewAnimation(AnimationDirection.In, args.Location);
                    Invalidate();
                }
            };
            MouseUp += (sender, args) =>
            {
                MouseState = MouseState.HOVER;

                Invalidate();
            };
        }
    }
}
