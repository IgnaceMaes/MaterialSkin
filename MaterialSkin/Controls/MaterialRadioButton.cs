using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using MaterialSkin.Animations;

namespace MaterialSkin.Controls
{
    public class MaterialRadioButton : RadioButton, IMaterialControl
    {
        public int Depth { get; set; }
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
        public MouseState MouseState { get; set; }

        private readonly AnimationManager animationManager;

        public MaterialRadioButton()
        {
            animationManager = new AnimationManager()
            {
                AnimationType = AnimationType.EaseInOut,
                Increment = 0.06,
                InterruptAnimation = false
            };
            animationManager.OnAnimationProgress += sender => Invalidate();

            CheckedChanged += (sender, args) => animationManager.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out);
        }

        private const int RADIOBUTTON_SIZE = 16;
        private const int RADIOBUTTON_OUTER_CIRCLE_WIDTH = 2;
        private const int RADIOBUTTON_INNER_CIRCLE_SIZE = RADIOBUTTON_SIZE - 2 * RADIOBUTTON_OUTER_CIRCLE_WIDTH;
        private const int RADIOBUTTON_X = 0;
        private const int RADIOBUTTON_Y = 0;
        private const int RADIOBUTTON_CENTER_X = RADIOBUTTON_X + RADIOBUTTON_SIZE / 2;
        private const int RADIOBUTTON_CENTER_Y = RADIOBUTTON_Y + RADIOBUTTON_SIZE / 2;
        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.Clear(Parent.BackColor);


            var animationProgress = animationManager.GetProgress();

            int colorAlpha = Enabled ? (int)(animationProgress * 255.0) : SkinManager.GetCheckBoxOffDisabledColor().A;
            int backgroundAlpha = Enabled ? (int)(SkinManager.GetCheckboxOffColor().A * (1.0 - animationProgress)) : SkinManager.GetCheckBoxOffDisabledColor().A;
            float animationSize = (float)(animationProgress * 8.0);
            float animationSizeHalf = animationSize / 2;
            var transition = new RectangleF(RADIOBUTTON_CENTER_X - animationSizeHalf, RADIOBUTTON_CENTER_Y - animationSizeHalf, animationSize, animationSize);

            using (var brush = new SolidBrush(Color.FromArgb(colorAlpha, Enabled ? SkinManager.AccentColor : SkinManager.GetCheckBoxOffDisabledColor())))
            {
                g.FillEllipse(new SolidBrush(Color.FromArgb(backgroundAlpha, Enabled ? SkinManager.GetCheckboxOffColor() : SkinManager.GetCheckBoxOffDisabledColor())), RADIOBUTTON_X, RADIOBUTTON_Y, RADIOBUTTON_SIZE, RADIOBUTTON_SIZE);
                
                if (Enabled)
                g.FillEllipse(brush, RADIOBUTTON_X, RADIOBUTTON_Y, RADIOBUTTON_SIZE, RADIOBUTTON_SIZE);

                g.FillEllipse(new SolidBrush(Parent.BackColor), RADIOBUTTON_OUTER_CIRCLE_WIDTH, RADIOBUTTON_OUTER_CIRCLE_WIDTH, RADIOBUTTON_INNER_CIRCLE_SIZE, RADIOBUTTON_INNER_CIRCLE_SIZE);
                
                g.FillEllipse(brush, transition);
            }

            g.DrawString(Text, SkinManager.ROBOTO_MEDIUM_10, Enabled ? SkinManager.GetMainTextBrush() : SkinManager.GetDisabledOrHintBrush(), 20, 0);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            Font = SkinManager.ROBOTO_MEDIUM_10;
        }
    }
}
