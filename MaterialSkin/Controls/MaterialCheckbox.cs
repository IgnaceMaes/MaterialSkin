using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Security.Principal;
using System.Windows.Forms;
using MaterialSkin.Animations;

namespace MaterialSkin.Controls
{
    public class MaterialCheckBox : CheckBox, IMaterialControl
    {
        public int Depth { get; set; }
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
        public MouseState MouseState { get; set; }
        public Point MouseLocation { get; set; }

        private bool ripple;
        public bool Ripple
        {
            get { return ripple; }
            set
            {
                ripple = value;
                AutoSize = AutoSize; //Make AutoSize directly set the bounds.

                if (value)
                {
                    Margin = new Padding(0);
                }

                Invalidate();
            }
        }

        private readonly AnimationManager animationManager;
        private readonly AnimationManager rippleAnimationManager;

        public MaterialCheckBox()
        {
            animationManager = new AnimationManager()
            {
                AnimationType = AnimationType.EaseInOut,
                Increment = 0.05
            };
            rippleAnimationManager = new AnimationManager(false)
            {
                AnimationType = AnimationType.Linear,
                Increment = 0.10,
                SecondaryIncrement = 0.08
            };
            animationManager.OnAnimationProgress += sender => Invalidate();
            rippleAnimationManager.OnAnimationProgress += sender => Invalidate();

            CheckedChanged += (sender, args) =>
            {
                animationManager.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out);
            };

            SizeChanged += OnSizeChanged;
            MouseMove += (sender, args) =>
            {
                Cursor = checkBoxBounds.Contains(args.Location) ? Cursors.Hand : Cursors.Default;
            };

            Ripple = true;
            MouseLocation = new Point(-1, -1);
        }

        private int boxOffset; // the offset, this is needed because of the ripple. The checkbox itself doesn't start at (0,0)
        private Rectangle checkBoxBounds; // contains the bounds of the checkbox
        private const double CHECKBOX_SIZE = 17.0; // the size of the checkbox
        private void OnSizeChanged(object sender, EventArgs eventArgs)
        {
            boxOffset = Height / 2 - 9;
            checkBoxBounds = new Rectangle(boxOffset, boxOffset, (int)CHECKBOX_SIZE, (int)CHECKBOX_SIZE);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            int w = boxOffset + TEXT_OFFSET + (int)CreateGraphics().MeasureString(Text, SkinManager.ROBOTO_MEDIUM_10).Width;
            return Ripple ? new Size(w, 30) : new Size(w, 20);
        }

        private static readonly Point[] CHECKMARK_LINE = { new Point(3, 8), new Point(7, 12), new Point(14, 5) };
        private const int TEXT_OFFSET = 22;
        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            // clear the control
            g.Clear(Parent.BackColor);

            double animationProgress = animationManager.GetProgress();

            int colorAlpha = Enabled ? (int)(animationProgress * 255.0) : SkinManager.GetCheckBoxOffDisabledColor().A;
            int backgroundAlpha = Enabled ? (int)(SkinManager.GetCheckboxOffColor().A * (1.0 - animationProgress)) : SkinManager.GetCheckBoxOffDisabledColor().A;

            var brush = new SolidBrush(Color.FromArgb(colorAlpha, Enabled ? SkinManager.AccentColor : SkinManager.GetCheckBoxOffDisabledColor()));

            // draw ripple animation
            if (Ripple && rippleAnimationManager.IsAnimating())
            {
                for (int i = 0; i < rippleAnimationManager.GetAnimationCount(); i++)
                {
                    var animationValue = rippleAnimationManager.GetProgress(i);
                    var animationSource = new Point((int)(boxOffset + Math.Floor(CHECKBOX_SIZE / 2)), (int)(boxOffset + Math.Floor(CHECKBOX_SIZE / 2)));
                    var rippleBrush = new SolidBrush(Color.FromArgb((int)((animationValue * 40)), ((bool)rippleAnimationManager.GetData(i)[0]) ? Color.Black : brush.Color));
                    var rippleSize = (rippleAnimationManager.GetDirection(i) == AnimationDirection.InOutIn) ? (int)(Height * (0.8d + (0.2d * animationValue))) : Height;
                    g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
                }
            }

            // draw checkbox
            using (var checkmarkPath = DrawHelper.CreateRoundRect(boxOffset, boxOffset, (int)CHECKBOX_SIZE, (int)CHECKBOX_SIZE, 2f))
            {
                g.FillPath(new SolidBrush(Color.FromArgb(backgroundAlpha, Enabled ? SkinManager.GetCheckboxOffColor() : SkinManager.GetCheckBoxOffDisabledColor())), checkmarkPath);
                g.FillRectangle(new SolidBrush(Parent.BackColor), boxOffset + 2, boxOffset + 2, 13, 13);

                if (Enabled)
                {
                    g.FillPath(brush, checkmarkPath);
                }
                else if (Checked)
                {
                    g.FillRectangle(brush, boxOffset + 2, boxOffset + 2, 13, 13);
                }

                var checkMarkLineFill = new Rectangle(boxOffset, boxOffset, (int)(CHECKBOX_SIZE * animationProgress), (int)CHECKBOX_SIZE);
                g.DrawImageUnscaledAndClipped(DrawCheckMarkBitmap(), checkMarkLineFill);
            }

            // draw checkbox text
            SizeF stringSize = g.MeasureString(Text, SkinManager.ROBOTO_MEDIUM_10);
            g.DrawString(
                Text, 
                SkinManager.ROBOTO_MEDIUM_10,
                Enabled ? SkinManager.GetMainTextBrush() : SkinManager.GetDisabledOrHintBrush(),
                boxOffset + TEXT_OFFSET, Height / 2 - stringSize.Height / 2);

            // dispose used paint objects
            brush.Dispose();
        }

        private Bitmap DrawCheckMarkBitmap()
        {
            var checkMark = new Bitmap((int)CHECKBOX_SIZE, (int)CHECKBOX_SIZE);
            var g = Graphics.FromImage(checkMark);

            // clear everything, transparant
            g.Clear(Color.Transparent);

            // draw the checkmark lines
            using (var pen = new Pen(Parent.BackColor, 2))
            {
                g.DrawLines(pen, CHECKMARK_LINE);
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

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            Font = SkinManager.ROBOTO_MEDIUM_10;

            if (DesignMode) return;

            MouseState = MouseState.OUT;
            MouseEnter += (sender, args) =>
            {
                MouseState = MouseState.HOVER;
                Invalidate();
            };
            MouseLeave += (sender, args) =>
            {
                MouseLocation = new Point(-1, -1);
                MouseState = MouseState.OUT;
                Invalidate();
            };
            MouseDown += (sender, args) =>
            {
                MouseState = MouseState.DOWN;

                if (Ripple && args.Button == MouseButtons.Left)
                {
                    if (checkBoxBounds.Contains(args.Location))
                    {
                        rippleAnimationManager.SecondaryIncrement = 0;
                        rippleAnimationManager.StartNewAnimation(AnimationDirection.InOutIn, data: new object[] { Checked });
                    }
                }

                Invalidate();
            };
            MouseUp += (sender, args) =>
            {
                MouseState = MouseState.HOVER;

                rippleAnimationManager.SecondaryIncrement = 0.08;

                Invalidate();
            };
            MouseMove += (sender, args) =>
            {
                MouseLocation = args.Location;
                Invalidate();
            };
        }

    }
}
