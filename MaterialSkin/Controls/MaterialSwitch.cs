namespace MaterialSkin.Controls
{
    using MaterialSkin.Animations;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    public class MaterialSwitch : CheckBox, IMaterialControl
    {
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        [Browsable(false)]
        public Point MouseLocation { get; set; }

        private bool _ripple;

        [Category("Appearance")]
        public bool Ripple
        {
            get { return _ripple; }
            set
            {
                _ripple = value;
                AutoSize = AutoSize; //Make AutoSize directly set the bounds.

                if (value)
                {
                    Margin = new Padding(0);
                }

                Invalidate();
            }
        }

        private readonly AnimationManager _checkAM;
        private readonly AnimationManager _hoverAM;
        private readonly AnimationManager _rippleAM;

        private const int THUMB_SIZE = 22;

        private const int THUMB_SIZE_HALF = THUMB_SIZE / 2;

        private const int TRACK_SIZE_HEIGHT = (int)(14);
        private const int TRACK_SIZE_WIDTH = (int)(36);
        private const int TRACK_RADIUS = (int)(TRACK_SIZE_HEIGHT / 2);

        private int TRACK_CENTER_Y;
        private int TRACK_CENTER_X_BEGIN;
        private int TRACK_CENTER_X_END;
        private int TRACK_CENTER_X_DELTA;

        private const int RIPPLE_DIAMETER = 37;

        private bool justChanged;
        private int _trackOffsetY;

        public MaterialSwitch()
        {
            _checkAM = new AnimationManager
            {
                AnimationType = AnimationType.EaseInOut,
                Increment = 0.05
            };
            _hoverAM = new AnimationManager(true)
            {
                AnimationType = AnimationType.Linear,
                Increment = 0.10
            };
            _rippleAM = new AnimationManager(false)
            {
                AnimationType = AnimationType.Linear,
                Increment = 0.10,
                SecondaryIncrement = 0.08
            };
            _checkAM.OnAnimationProgress += sender => Invalidate();
            _rippleAM.OnAnimationProgress += sender => Invalidate();
            _hoverAM.OnAnimationProgress += sender => Invalidate();

            CheckedChanged += (sender, args) =>
            {
                if (Ripple)
                    _checkAM.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out);
            };

            Ripple = true;
            MouseLocation = new Point(-1, -1);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            _trackOffsetY = Height / 2 - THUMB_SIZE_HALF;

            TRACK_CENTER_Y = _trackOffsetY + THUMB_SIZE_HALF - 1;
            TRACK_CENTER_X_BEGIN = TRACK_CENTER_Y;
            TRACK_CENTER_X_END = TRACK_CENTER_X_BEGIN + TRACK_SIZE_WIDTH - (TRACK_RADIUS * 2);
            TRACK_CENTER_X_DELTA = TRACK_CENTER_X_END - TRACK_CENTER_X_BEGIN;
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size strSize;
            using (NativeTextRenderer NativeText = new NativeTextRenderer(CreateGraphics()))
            {
                strSize = NativeText.MeasureLogString(Text, SkinManager.getLogFontByType(MaterialSkinManager.fontType.Body1));
            }
            var w = TRACK_SIZE_WIDTH + THUMB_SIZE + strSize.Width;
            return Ripple ? new Size(w, RIPPLE_DIAMETER) : new Size(w, THUMB_SIZE);
        }

        private static readonly Point[] CheckmarkLine = { new Point(3, 8), new Point(7, 12), new Point(14, 5) };

        private const int TEXT_OFFSET = THUMB_SIZE;

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            g.Clear(Parent.BackColor);

            var animationProgress = _checkAM.GetProgress();

            // Draw Track
            Color thumbColor = DrawHelper.BlendColor(
                        (Enabled ? SkinManager.GetSwitchOffThumbColor() : SkinManager.GetSwitchOffDisabledThumbColor()), // Off color
                        (Enabled ? SkinManager.ColorScheme.AccentColor : DrawHelper.BlendColor(SkinManager.ColorScheme.AccentColor, SkinManager.GetSwitchOffDisabledThumbColor(), 197)), // On color
                        animationProgress * 255); // Blend amount

            using (var path = DrawHelper.CreateRoundRect(new Rectangle(TRACK_CENTER_X_BEGIN - TRACK_RADIUS, TRACK_CENTER_Y - TRACK_SIZE_HEIGHT / 2, TRACK_SIZE_WIDTH, TRACK_SIZE_HEIGHT), TRACK_RADIUS))
            {
                using (SolidBrush trackBrush = new SolidBrush(
                    Color.FromArgb(Enabled ? SkinManager.GetSwitchOffTrackColor().A : SkinManager.GetSwitchOffDisabledTrackColor().A, // Track alpha
                    DrawHelper.BlendColor( // animate color
                        (Enabled ? SkinManager.GetSwitchOffTrackColor() : SkinManager.GetSwitchOffDisabledTrackColor()), // Off color
                        SkinManager.ColorScheme.AccentColor, // On color
                        animationProgress * 255) // Blend amount
                        .RemoveAlpha())))
                {
                    g.FillPath(trackBrush, path);
                }

            }

            // Calculate animation movement X position
            int OffsetX = (int)(TRACK_CENTER_X_DELTA * animationProgress);

            // Ripple
            int rippleSize = (Height % 2 == 0) ? Height - 2 : Height - 3;

            Color rippleColor = Color.FromArgb(40, // color alpha
                Checked ? SkinManager.ColorScheme.AccentColor : // On color
                (SkinManager.Theme == MaterialSkinManager.Themes.LIGHT ? Color.Black : Color.White)); // Off color

            if (Ripple && _rippleAM.IsAnimating())
            {
                for (int i = 0; i < _rippleAM.GetAnimationCount(); i++)
                {
                    double rippleAnimProgress = _rippleAM.GetProgress(i);
                    int rippleAnimatedDiameter = (_rippleAM.GetDirection(i) == AnimationDirection.InOutIn) ? (int)(rippleSize * (0.7 + (0.3 * rippleAnimProgress))) : rippleSize;

                    using (SolidBrush rippleBrush = new SolidBrush(Color.FromArgb((int)(40 * rippleAnimProgress), rippleColor.RemoveAlpha())))
                    {
                        g.FillEllipse(rippleBrush, new Rectangle(TRACK_CENTER_X_BEGIN + OffsetX - rippleAnimatedDiameter / 2, TRACK_CENTER_Y - rippleAnimatedDiameter / 2, rippleAnimatedDiameter, rippleAnimatedDiameter));
                    }
                }
            }

            // Hover
            if (Ripple)
            {
                double rippleAnimProgress = _hoverAM.GetProgress();
                int rippleAnimatedDiameter = (int)(rippleSize * (0.7 + (0.3 * rippleAnimProgress)));

                using (SolidBrush rippleBrush = new SolidBrush(Color.FromArgb((int)(40 * rippleAnimProgress), rippleColor.RemoveAlpha())))
                {
                    g.FillEllipse(rippleBrush, new Rectangle(TRACK_CENTER_X_BEGIN + OffsetX - rippleAnimatedDiameter / 2, TRACK_CENTER_Y - rippleAnimatedDiameter / 2, rippleAnimatedDiameter, rippleAnimatedDiameter));
                }
            }


            // draw Thumb Shadow
            RectangleF thumbBounds = new RectangleF(TRACK_CENTER_X_BEGIN + OffsetX - THUMB_SIZE_HALF, TRACK_CENTER_Y - THUMB_SIZE_HALF, THUMB_SIZE, THUMB_SIZE);
            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(12, 0, 0, 0)))
            {
                g.FillEllipse(shadowBrush, new RectangleF(thumbBounds.X - 2, thumbBounds.Y - 1, thumbBounds.Width + 4, thumbBounds.Height + 6));
                g.FillEllipse(shadowBrush, new RectangleF(thumbBounds.X - 1, thumbBounds.Y - 1, thumbBounds.Width + 2, thumbBounds.Height + 4));
                g.FillEllipse(shadowBrush, new RectangleF(thumbBounds.X - 0, thumbBounds.Y - 0, thumbBounds.Width + 0, thumbBounds.Height + 2));
                g.FillEllipse(shadowBrush, new RectangleF(thumbBounds.X - 0, thumbBounds.Y + 2, thumbBounds.Width + 0, thumbBounds.Height + 0));
                g.FillEllipse(shadowBrush, new RectangleF(thumbBounds.X - 0, thumbBounds.Y + 1, thumbBounds.Width + 0, thumbBounds.Height + 0));
            }

            // draw Thumb
            using (SolidBrush thumbBrush = new SolidBrush(thumbColor))
            {
                g.FillEllipse(thumbBrush, thumbBounds);
            }

            // draw text
            using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            {
                Rectangle textLocation = new Rectangle(TEXT_OFFSET + TRACK_SIZE_WIDTH, 0, Width - (TEXT_OFFSET + TRACK_SIZE_WIDTH), Height);
                NativeText.DrawTransparentText(
                    Text,
                    SkinManager.getLogFontByType(MaterialSkinManager.fontType.Body1),
                    Enabled ? SkinManager.GetPrimaryTextColor() : SkinManager.GetDisabledOrHintColor(),
                    textLocation.Location,
                    textLocation.Size,
                    NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
            }
        }

        private Bitmap DrawCheckMarkBitmap()
        {
            var checkMark = new Bitmap(THUMB_SIZE, THUMB_SIZE);
            var g = Graphics.FromImage(checkMark);

            // clear everything, transparent
            g.Clear(Color.Transparent);

            // draw the checkmark lines
            using (var pen = new Pen(Parent.BackColor, 2))
            {
                g.DrawLines(pen, CheckmarkLine);
            }

            return checkMark;
        }

        public override bool AutoSize
        {
            get { return base.AutoSize; }
            set
            {
                base.AutoSize = value;
                if (value)
                {
                    Size = new Size(10, 10);
                }
            }
        }
        private bool IsMouseInCheckArea()
        {
            return ClientRectangle.Contains(MouseLocation);
        }

        private bool hovered = false;

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (DesignMode) return;

            MouseState = MouseState.OUT;

            GotFocus += (sender, AddingNewEventArgs) =>
            {
                if (Ripple && !hovered)
                {
                    _hoverAM.StartNewAnimation(AnimationDirection.In, new object[] { Checked });
                    hovered = true;
                }
            };

            LostFocus += (sender, args) =>
            {
                if (Ripple && hovered)
                {
                    _hoverAM.StartNewAnimation(AnimationDirection.Out, new object[] { Checked });
                    hovered = false;
                }
            };

            MouseEnter += (sender, args) =>
            {
                MouseState = MouseState.HOVER;
                //if (Ripple && !hovered)
                //{
                //    _hoverAM.StartNewAnimation(AnimationDirection.In, new object[] { Checked });
                //    hovered = true;
                //}
            };

            MouseLeave += (sender, args) =>
            {
                MouseLocation = new Point(-1, -1);
                MouseState = MouseState.OUT;
                //if (Ripple && hovered)
                //{
                //    _hoverAM.StartNewAnimation(AnimationDirection.Out, new object[] { Checked });
                //    hovered = false;
                //}
            };

            MouseDown += (sender, args) =>
            {
                MouseState = MouseState.DOWN;
                if (Ripple)
                {
                    _rippleAM.SecondaryIncrement = 0;
                    _rippleAM.StartNewAnimation(AnimationDirection.InOutIn, new object[] { Checked });
                }
            };

            KeyDown += (sender, args) =>
            {
                if (Ripple && (args.KeyCode == Keys.Space) && _rippleAM.GetAnimationCount() == 0)
                {
                    _rippleAM.SecondaryIncrement = 0;
                    _rippleAM.StartNewAnimation(AnimationDirection.InOutIn, new object[] { Checked });
                }
            };

            MouseUp += (sender, args) =>
            {
                if (Ripple)
                {
                    MouseState = MouseState.HOVER;
                    _rippleAM.SecondaryIncrement = 0.08;
                    _hoverAM.StartNewAnimation(AnimationDirection.Out, new object[] { Checked });
                    hovered = false;
                }
            };

            KeyUp += (sender, args) =>
            {
                if (Ripple && (args.KeyCode == Keys.Space))
                {
                    MouseState = MouseState.HOVER;
                    _rippleAM.SecondaryIncrement = 0.08;
                }
            };

            MouseMove += (sender, args) =>
            {
                MouseLocation = args.Location;
                Cursor = IsMouseInCheckArea() ? Cursors.Hand : Cursors.Default;
            };
        }
    }
}
