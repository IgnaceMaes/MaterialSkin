using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using MaterialSkin.Animations;

namespace MaterialSkin.Controls
{
    public class MaterialSingleLineTextField : Control, IMaterialControl
    {
        //Properties for managing the material design properties
        public int Depth { get; set; }
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
        public MouseState MouseState { get; set; }

        public override string Text { get { return baseTextBox.Text; } set { baseTextBox.Text = value; } }
        public string Hint { get { return baseTextBox.Hint; } set { baseTextBox.Hint = value; } }

        private readonly AnimationManager animationManager;

        private readonly BaseTextBox baseTextBox;
        public MaterialSingleLineTextField()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer, true);

            animationManager = new AnimationManager()
            {
                Increment = 0.06,
                AnimationType = AnimationType.EaseInOut,
                InterruptAnimation = false
            };
            animationManager.OnAnimationProgress += sender => Invalidate();

            baseTextBox = new BaseTextBox
            {
                BorderStyle = BorderStyle.None,
                Font = SkinManager.ROBOTO_REGULAR_11,
                ForeColor = SkinManager.GetMainTextColor(),
                Location = new Point(0, 0),
                Width = Width,
                Height = Height - 5,
            };

            if (!Controls.Contains(baseTextBox) && !DesignMode)
            {
                Controls.Add(baseTextBox);
            }

            baseTextBox.GotFocus += (sender, args) => animationManager.StartNewAnimation(AnimationDirection.In);
            baseTextBox.LostFocus += (sender, args) => animationManager.StartNewAnimation(AnimationDirection.Out);
            BackColorChanged += (sender, args) =>
            {
                baseTextBox.BackColor = BackColor;
                baseTextBox.ForeColor = SkinManager.GetMainTextColor();
            };
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.Clear(Parent.BackColor);

            int lineY = baseTextBox.Bottom + 3;

            if (!animationManager.IsAnimating())
            {
                //No animation
                g.FillRectangle(baseTextBox.Focused ? SkinManager.PrimaryColorBrush : SkinManager.GetDividersBrush(), baseTextBox.Location.X, lineY, baseTextBox.Width, baseTextBox.Focused ? 2 : 1);
            }
            else
            {
                //Animate
                int animationWidth = (int) (baseTextBox.Width * animationManager.GetProgress());
                int halfAnimationWidth = animationWidth / 2;
                int animationStart = baseTextBox.Location.X + baseTextBox.Width / 2;

                //Unfocused background
                g.FillRectangle(SkinManager.GetDividersBrush(), baseTextBox.Location.X, lineY, baseTextBox.Width, 1);
            
                //Animated focus transition
                g.FillRectangle(SkinManager.PrimaryColorBrush, animationStart - halfAnimationWidth, lineY, animationWidth, 2);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            baseTextBox.Location = new Point(0, 0);
            baseTextBox.Width = Width;
            
            Height = baseTextBox.Height + 5;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            baseTextBox.BackColor = Parent.BackColor;
            baseTextBox.ForeColor = SkinManager.GetMainTextColor();
        }

        private class BaseTextBox : TextBox, IMaterialControl
        {
            //Properties for managing the material design properties
            public int Depth { get; set; }
            public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
            public MouseState MouseState { get; set; }

            public string Hint { get; set; }

            public BaseTextBox()
            {
                SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
                KeyDown += OnKeyDown;
                KeyUp += (sender, args) => CheckToDrawHint();
                GotFocus += (sender, args) => CheckToDrawHint();
                LostFocus += (sender, args) => CheckToDrawHint();
            }

            private void OnKeyDown(object sender, KeyEventArgs keyEventArgs)
            {
                if (keyEventArgs.KeyCode != Keys.Back)
                {
                    //This fixes an issue when the Text is empty and the user holds a key, the background will flash cause otherwise the userpaint would still be on
                    SetStyle(ControlStyles.UserPaint, false);
                }
            }

            protected override void OnCreateControl()
            {
                base.OnCreateControl();

                CheckToDrawHint();
            }

            private bool ShouldDrawHint()
            {
                return Text == "" && !string.IsNullOrEmpty(Hint);
            }

            private void CheckToDrawHint()
            {
                //Enabled the userpaint if needed when the hint should be drawn
                SetStyle(ControlStyles.UserPaint, ShouldDrawHint());

                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                base.OnPaint(e);

                if (ShouldDrawHint())
                {
                    //The text is empty, so the hint should be drawn if there is one
                    g.DrawString(Hint, Font, SkinManager.GetDisabledOrHintBrush(), ClientRectangle, new StringFormat() {LineAlignment = StringAlignment.Center});
                }
            }
        }
    }
}
