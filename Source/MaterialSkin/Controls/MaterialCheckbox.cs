using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using MaterialSkin.Animations;

namespace MaterialSkin.Controls
{
    public class MaterialCheckBox : CheckBox, IMaterialControl
    {
        private const int CHECKBOX_SIZE = 18;
        private const int CHECKBOX_SIZE_HALF = CHECKBOX_SIZE / 2;
        private const int CHECKBOX_INNER_BOX_SIZE = CHECKBOX_SIZE - 4;
        private const int TEXT_OFFSET = 22;

        private static readonly Point[] CheckmarkLine =
        {
            new Point(3, 8),
            new Point(7, 12),
            new Point(14, 5)
        };

        private readonly AnimationManager _animationManager;
        private readonly AnimationManager _rippleAnimationManager;
        private int _boxOffset;
        private Rectangle _boxRectangle;
        private bool _ripple;

        public override bool AutoSize
        {
            get => base.AutoSize;
            set
            {
                base.AutoSize = value;
                if (value)
                {
                    Size = new Size(10, 10);
                }
            }
        }

        [Browsable(false)]
        public Point MouseLocation { get; set; }

        [Category("Behavior")]
        public bool Ripple
        {
            get => _ripple;
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

        public MaterialCheckBox()
        {
            _animationManager = new AnimationManager
            {
                AnimationType = AnimationType.EaseInOut,
                Increment = 0.05
            };
            _rippleAnimationManager = new AnimationManager(false)
            {
                AnimationType = AnimationType.Linear,
                Increment = 0.10,
                SecondaryIncrement = 0.08
            };
            _animationManager.OnAnimationProgress += sender => Invalidate();
            _rippleAnimationManager.OnAnimationProgress += sender => Invalidate();

            CheckedChanged += (sender, args) => { _animationManager.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out); };

            Ripple = true;
            MouseLocation = new Point(-1, -1);
        }

        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        public override Size GetPreferredSize(Size proposedSize)
        {
            int w = _boxOffset + CHECKBOX_SIZE + 2 + (int) CreateGraphics().MeasureString(Text, SkinManager.ROBOTO_MEDIUM_10).Width;
            return Ripple ? new Size(w, 30) : new Size(w, 20);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            Font = SkinManager.ROBOTO_MEDIUM_10;

            if (DesignMode)
            {
                return;
            }

            MouseState = MouseState.OUT;
            MouseEnter += (sender, args) => { MouseState = MouseState.HOVER; };
            MouseLeave += (sender, args) =>
            {
                MouseLocation = new Point(-1, -1);
                MouseState = MouseState.OUT;
            };
            MouseDown += (sender, args) =>
            {
                MouseState = MouseState.DOWN;

                if (Ripple && args.Button == MouseButtons.Left && IsMouseInCheckArea())
                {
                    _rippleAnimationManager.SecondaryIncrement = 0;
                    _rippleAnimationManager.StartNewAnimation(AnimationDirection.InOutIn, new object[]
                    {
                        Checked
                    });
                }
            };
            MouseUp += (sender, args) =>
            {
                MouseState = MouseState.HOVER;
                _rippleAnimationManager.SecondaryIncrement = 0.08;
            };
            MouseMove += (sender, args) =>
            {
                MouseLocation = args.Location;
                Cursor = IsMouseInCheckArea() ? Cursors.Hand : Cursors.Default;
            };
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            // clear the control
            g.Clear(Parent.BackColor);

            int CHECKBOX_CENTER = _boxOffset + CHECKBOX_SIZE_HALF - 1;

            double animationProgress = _animationManager.GetProgress();

            int colorAlpha = Enabled ? (int) (animationProgress * 255.0) : SkinManager.GetCheckBoxOffDisabledColor().A;
            int backgroundAlpha = Enabled ? (int) (SkinManager.GetCheckboxOffColor().A * (1.0 - animationProgress)) : SkinManager.GetCheckBoxOffDisabledColor().A;

            SolidBrush brush = new SolidBrush(Color.FromArgb(colorAlpha, Enabled ? SkinManager.ColorScheme.AccentColor : SkinManager.GetCheckBoxOffDisabledColor()));
            SolidBrush brush3 = new SolidBrush(Enabled ? SkinManager.ColorScheme.AccentColor : SkinManager.GetCheckBoxOffDisabledColor());
            Pen pen = new Pen(brush.Color);

            // draw ripple animation
            if (Ripple && _rippleAnimationManager.IsAnimating())
            {
                for (int i = 0; i < _rippleAnimationManager.GetAnimationCount(); i++)
                {
                    double animationValue = _rippleAnimationManager.GetProgress(i);
                    Point animationSource = new Point(CHECKBOX_CENTER, CHECKBOX_CENTER);
                    SolidBrush rippleBrush = new SolidBrush(Color.FromArgb((int) (animationValue * 40), (bool) _rippleAnimationManager.GetData(i)[0] ? Color.Black : brush.Color));
                    int rippleHeight = Height % 2 == 0 ? Height - 3 : Height - 2;
                    int rippleSize = _rippleAnimationManager.GetDirection(i) == AnimationDirection.InOutIn ? (int) (rippleHeight * (0.8d + 0.2d * animationValue)) : rippleHeight;
                    using (GraphicsPath path = DrawHelper.CreateRoundRect(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize, rippleSize / 2))
                    {
                        g.FillPath(rippleBrush, path);
                    }

                    rippleBrush.Dispose();
                }
            }

            brush3.Dispose();

            Rectangle checkMarkLineFill = new Rectangle(_boxOffset, _boxOffset, (int) (17.0 * animationProgress), 17);
            using (GraphicsPath checkmarkPath = DrawHelper.CreateRoundRect(_boxOffset, _boxOffset, 17, 17, 1f))
            {
                SolidBrush brush2 = new SolidBrush(DrawHelper.BlendColor(Parent.BackColor, Enabled ? SkinManager.GetCheckboxOffColor() : SkinManager.GetCheckBoxOffDisabledColor(), backgroundAlpha));
                Pen pen2 = new Pen(brush2.Color);
                g.FillPath(brush2, checkmarkPath);
                g.DrawPath(pen2, checkmarkPath);

                g.FillRectangle(new SolidBrush(Parent.BackColor), _boxOffset + 2, _boxOffset + 2, CHECKBOX_INNER_BOX_SIZE - 1, CHECKBOX_INNER_BOX_SIZE - 1);
                g.DrawRectangle(new Pen(Parent.BackColor), _boxOffset + 2, _boxOffset + 2, CHECKBOX_INNER_BOX_SIZE - 1, CHECKBOX_INNER_BOX_SIZE - 1);

                brush2.Dispose();
                pen2.Dispose();

                if (Enabled)
                {
                    g.FillPath(brush, checkmarkPath);
                    g.DrawPath(pen, checkmarkPath);
                }
                else if (Checked)
                {
                    g.SmoothingMode = SmoothingMode.None;
                    g.FillRectangle(brush, _boxOffset + 2, _boxOffset + 2, CHECKBOX_INNER_BOX_SIZE, CHECKBOX_INNER_BOX_SIZE);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                }

                g.DrawImageUnscaledAndClipped(DrawCheckMarkBitmap(), checkMarkLineFill);
            }

            // draw checkbox text
            SizeF stringSize = g.MeasureString(Text, SkinManager.ROBOTO_MEDIUM_10);
            g.DrawString(Text, SkinManager.ROBOTO_MEDIUM_10, Enabled ? SkinManager.GetPrimaryTextBrush() : SkinManager.GetDisabledOrHintBrush(), _boxOffset + TEXT_OFFSET, Height / 2 - stringSize.Height / 2);

            // dispose used paint objects
            pen.Dispose();
            brush.Dispose();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            _boxOffset = Height / 2 - 9;
            _boxRectangle = new Rectangle(_boxOffset, _boxOffset, CHECKBOX_SIZE - 1, CHECKBOX_SIZE - 1);
        }

        private Bitmap DrawCheckMarkBitmap()
        {
            Bitmap checkMark = new Bitmap(CHECKBOX_SIZE, CHECKBOX_SIZE);
            Graphics g = Graphics.FromImage(checkMark);

            // clear everything, transparent
            g.Clear(Color.Transparent);

            // draw the checkmark lines
            using (Pen pen = new Pen(Parent.BackColor, 2))
            {
                g.DrawLines(pen, CheckmarkLine);
            }

            return checkMark;
        }

        private bool IsMouseInCheckArea()
        {
            return _boxRectangle.Contains(MouseLocation);
        }
    }
}
