using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MaterialSkin.Animations;

namespace MaterialSkin.Controls
{
    public class MaterialFlatButton : Button, IMaterialControl
    {
        public int Depth { get; set; }
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
        public MouseState MouseState { get; set; }

        public bool Primary { get; set; }

        private readonly AnimationManager animationManager;
        private readonly AnimationManager hoverAnimationManager;
        private readonly AnimationManager pressAnimationManager;
        private readonly AnimationManager holdRippleAnimationManager;

        private SizeF textSize;

        public MaterialFlatButton()
        {
            Primary = false;

            animationManager = new AnimationManager(false)
            {
                Increment = 0.04,
                AnimationType = AnimationType.EaseOut,
            };
            hoverAnimationManager = new AnimationManager()
            {
                Increment = 0.07,
                AnimationType = AnimationType.Linear,
            };
            pressAnimationManager = new AnimationManager()
            {
                Increment = 0.07,
                AnimationType = AnimationType.Linear,
            };
            holdRippleAnimationManager = new AnimationManager()
            {
                Increment = 0.01,
                AnimationType = AnimationType.CustomQuadratic,
            };

            hoverAnimationManager.OnAnimationProgress += sender => Invalidate();
            pressAnimationManager.OnAnimationProgress += sender => Invalidate();
            animationManager.OnAnimationProgress += sender => Invalidate();
            holdRippleAnimationManager.OnAnimationProgress += sender => Invalidate();

            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AutoSize = true;
            Margin = new Padding(4, 6, 4, 6);
            Padding = new Padding(0);
        }

        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                textSize = CreateGraphics().MeasureString(value.ToUpper(), SkinManager.ROBOTO_MEDIUM_10);
                if (AutoSize)
                    Size = GetPreferredSize();
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            //Hold Ripple Animation Management
            if (holdRippleAnimationManager.GetDirection() == AnimationDirection.In && holdRippleAnimationManager.GetProgress() > 0.99d)
            {
                holdRippleAnimationManager.SecondaryIncrement = 0.01;
                holdRippleAnimationManager.Increment = 0.01;
                holdRippleAnimationManager.AnimationType = AnimationType.EaseInOut;
                holdRippleAnimationManager.StartNewAnimation(AnimationDirection.InOutRepeatingOut, new object[] { true });
            }

            var g = pevent.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.Clear(Parent.BackColor);

            //Hover
            Color c = SkinManager.GetFlatButtonHoverBackgroundColor();
            using (Brush b = new SolidBrush(Color.FromArgb((int)(hoverAnimationManager.GetProgress() * c.A), c.RemoveAlpha())))
                g.FillRectangle(b, ClientRectangle);

            //Down
            Color c2 = SkinManager.GetFlatButtonPressedBackgroundColor();
            using (Brush b = new SolidBrush(Color.FromArgb((int)(pressAnimationManager.GetProgress() * c2.A), c2.RemoveAlpha())))
                g.FillRectangle(b, ClientRectangle);

            //Ripple
            if (animationManager.IsAnimating())
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                for (int i = 0; i < animationManager.GetAnimationCount(); i++)
                {
                    var animationValue = animationManager.GetProgress(i);
                    var animationSource = animationManager.GetSource(i);

                    using (Brush rippleBrush = new SolidBrush(Color.FromArgb((int)(101 - (animationValue * 100)), Color.Black)))
                    {
                        var rippleSize = (int)(animationValue * Width);
                        g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
                    }
                }
                g.SmoothingMode = SmoothingMode.None;
            }

            //Hold ripple
            if (holdRippleAnimationManager.IsAnimating() && holdRippleAnimationManager.GetAnimationCount() > 0)
            {
                var animationValue = holdRippleAnimationManager.GetProgress();
                var animationSource = new Point(Width / 2, Height / 2);

                var artificalAlphaColor = DrawHelper.BlendColor(Parent.BackColor, SkinManager.GetFlatButtonHoverBackgroundColor());
                artificalAlphaColor = DrawHelper.BlendColor(artificalAlphaColor, Parent.BackColor, 0.PercentageToColorComponent());
                using (Brush rippleBrush = new SolidBrush(artificalAlphaColor))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    double rippleSize = (bool)holdRippleAnimationManager.GetData()[0] ? ((double)Width - 20) - (animationValue * 6d) : (animationValue * ((double)Width - 20));
                    int rippleOffset = (int)Math.Ceiling(rippleSize / 2d);
                    g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleOffset, animationSource.Y - rippleOffset, rippleOffset * 2, rippleOffset * 2));
                    g.SmoothingMode = SmoothingMode.None;
                }
            }

            g.DrawString(Text.ToUpper(), SkinManager.ROBOTO_MEDIUM_10, Enabled ? (Primary ? SkinManager.PrimaryColorPair.PrimaryBrush : SkinManager.GetMainTextBrush()) : SkinManager.GetFlatButtonDisabledTextBrush(), ClientRectangle, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }

        private Size GetPreferredSize()
        {
            return GetPreferredSize(new Size(0, 0));
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            return new Size((int)textSize.Width + 8, 36);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (DesignMode) return;

            MouseState = MouseState.OUT;
            MouseEnter += (sender, args) =>
            {
                MouseState = MouseState.HOVER;
                hoverAnimationManager.StartNewAnimation(AnimationDirection.In);
                Invalidate();
            };
            MouseLeave += (sender, args) =>
            {
                MouseState = MouseState.OUT;
                hoverAnimationManager.StartNewAnimation(AnimationDirection.Out);
                Invalidate();
            };
            MouseDown += (sender, args) =>
            {
                if (args.Button == MouseButtons.Left)
                {
                    MouseState = MouseState.DOWN;
                    pressAnimationManager.StartNewAnimation(AnimationDirection.In);

                    holdRippleAnimationManager.SetProgress(0);
                    holdRippleAnimationManager.SecondaryIncrement = 0d;
                    holdRippleAnimationManager.AnimationType = AnimationType.CustomQuadratic;
                    holdRippleAnimationManager.StartNewAnimation(AnimationDirection.In, new object[] { false });
                    Invalidate();
                }
            };
            MouseUp += (sender, args) =>
            {
                MouseState = MouseState.HOVER;

                if (args.Button == MouseButtons.Left)
                {
                    animationManager.StartNewAnimation(AnimationDirection.In, args.Location);
                    pressAnimationManager.StartNewAnimation(AnimationDirection.Out);

                    holdRippleAnimationManager.AnimationType = AnimationType.Linear;
                    holdRippleAnimationManager.SetDirection(AnimationDirection.InOutOut);
                }
                holdRippleAnimationManager.SecondaryIncrement = 0.1d;
                Invalidate();
            };
        }
    }
}
