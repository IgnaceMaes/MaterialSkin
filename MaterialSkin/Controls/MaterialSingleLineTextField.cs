using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MaterialSkin.Controls
{
    public class MaterialSingleLineTextField : Control, IMaterialControl
    {
        //Properties for managing the material design properties
        public int Depth { get; set; }
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
        public MouseState MouseState { get; set; }

        public override string Text { get { return baseTextBox.Text; } set { baseTextBox.Text = value;  } }
        public string Hint { get; set; }

        private readonly Timer animationTimer = new Timer { Interval = 5, Enabled = false };
        private double animationValue;

        private readonly TextBox baseTextBox = new TextBox();
        public MaterialSingleLineTextField()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer, true);

            baseTextBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = SkinManager.FONT_BODY1,
                ForeColor = SkinManager.GetMainTextColor(),
                Location = new Point(0, 0),
                Text = Text,
                Width = Width,
                Height = Height - 3
            };

            if (!Controls.Contains(baseTextBox) && !DesignMode)
            {
                Controls.Add(baseTextBox);
            }

            baseTextBox.GotFocus += BaseTextBoxOnGotFocus;
            baseTextBox.LostFocus += (sender, args) => Invalidate();
            baseTextBox.TextChanged += BaseTextBoxOnTextChanged;
            BackColorChanged += (sender, args) =>
            {
                baseTextBox.BackColor = BackColor;
                baseTextBox.ForeColor = SkinManager.GetMainTextColor();
            };

            animationTimer.Tick += animationTimer_Tick;
        }

        private void BaseTextBoxOnGotFocus(object sender, EventArgs eventArgs)
        {
            animationValue = 0;
            animationTimer.Start();
        }

        private void BaseTextBoxOnTextChanged(object sender, EventArgs eventArgs)
        {
            if (baseTextBox.Text != Hint && baseTextBox.Text == "")
            {
                baseTextBox.Text = Hint;
                baseTextBox.ForeColor = SkinManager.GetDisabledOrHintColor();
            }
        }

        void animationTimer_Tick(object sender, EventArgs e)
        {
            if (animationValue < 1.00)
            {
                animationValue += 0.06;
                Invalidate();
            }
            else
            {
                animationValue = 0;
                animationTimer.Stop();
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.Clear(Parent.BackColor);

            int lineY = baseTextBox.Bottom + 1;

            if (animationValue < 0.03)
            {
                //No animation
                g.FillRectangle(baseTextBox.Focused ? SkinManager.PrimaryColorBrush : SkinManager.GetDividersBrush(), baseTextBox.Location.X, lineY, baseTextBox.Width, baseTextBox.Focused ? 2 : 1);
            }
            else
            {
                //Animate
                int animationWidth = (int) (baseTextBox.Width * animationValue);
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
            
            Height = baseTextBox.Height + 3;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            baseTextBox.BackColor = Parent.BackColor;
            baseTextBox.ForeColor = SkinManager.GetMainTextColor();
        }
    }
}
