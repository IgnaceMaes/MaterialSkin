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

        private readonly TextBox baseTextBox = new TextBox();
        public MaterialSingleLineTextField()
        {
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

            baseTextBox.GotFocus += (sender, args) => Invalidate();
            baseTextBox.LostFocus += (sender, args) => Invalidate();
            baseTextBox.TextChanged += BaseTextBoxOnTextChanged;
            BackColorChanged += (sender, args) =>
            {
                baseTextBox.BackColor = BackColor;
                baseTextBox.ForeColor = SkinManager.GetMainTextColor();
            };
        }

        private void BaseTextBoxOnTextChanged(object sender, EventArgs eventArgs)
        {
            if (baseTextBox.Text != Hint && baseTextBox.Text == "")
            {
                baseTextBox.Text = Hint;
                baseTextBox.ForeColor = SkinManager.GetDisabledOrHintColor();
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            g.Clear(Parent.BackColor);

            g.FillRectangle(baseTextBox.Focused ? SkinManager.PrimaryColorBrush : SkinManager.GetDividersBrush(), baseTextBox.Location.X, baseTextBox.Bottom + 1, baseTextBox.Width, 1);
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
