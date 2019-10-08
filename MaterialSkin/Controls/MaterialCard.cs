namespace MaterialSkin.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    public class MaterialCard : Panel, IMaterialControl
    {
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        public MaterialCard() {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Paint += new PaintEventHandler(paintControl);
            BackColor = SkinManager.GetButtonBackgroundColor();
            ForeColor = SkinManager.GetPrimaryTextColor();
            Margin = new Padding(SkinManager.FORM_PADDING);
            Padding = new Padding(SkinManager.FORM_PADDING);
        }

        private void drawShadowOnParent(object sender, PaintEventArgs e)
        {
            // paint shadow on parent
            Graphics gp = e.Graphics;
            Rectangle rect = new Rectangle(Location, ClientRectangle.Size);
            Console.WriteLine(rect);
            gp.SmoothingMode = SmoothingMode.AntiAlias;
            DrawHelper.DrawSquareShadow(gp, rect);
        }

        protected override void InitLayout()
        {
            Parent.Paint += drawShadowOnParent;
            ForeColor = SkinManager.GetPrimaryTextColor();
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            BackColor = SkinManager.GetButtonBackgroundColor();
        }

        private void paintControl(Object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            g.Clear(Parent.BackColor);

            // card rectangle path
            RectangleF cardRectF = new RectangleF(ClientRectangle.Location, ClientRectangle.Size);
            cardRectF.X -= 0.5f;
            cardRectF.Y -= 0.5f;
            GraphicsPath cardPath = DrawHelper.CreateRoundRect(cardRectF, 4);

            // button shadow (blend with form shadow)
            DrawHelper.DrawSquareShadow(g, ClientRectangle);

            // Draw card
            using (SolidBrush normalBrush = new SolidBrush(BackColor))
            {
                g.FillPath(normalBrush, cardPath);
            }
        }
    }
}
