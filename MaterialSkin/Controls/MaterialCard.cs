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
            BackColor = SkinManager.GetButtonBackgroundColor();
            ForeColor = SkinManager.GetPrimaryTextColor();
            Margin = new Padding(SkinManager.FORM_PADDING);
            Padding = new Padding(SkinManager.FORM_PADDING);
        }

        private void drawShadowOnParent(object sender, PaintEventArgs e)
        {
            // paint shadow on parent
            Graphics gp = e.Graphics;
            Matrix mx = new Matrix(1F, 0, 0, 1F, Location.X, Location.Y);
            gp.Transform = mx;
            gp.SmoothingMode = SmoothingMode.AntiAlias;
            DrawHelper.DrawSquareShadow(gp, ClientRectangle);
        }
        protected override void InitLayout()
        {
            Parent.Paint += new PaintEventHandler(drawShadowOnParent);
            ForeColor = SkinManager.GetPrimaryTextColor();
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            BackColor = SkinManager.GetButtonBackgroundColor();
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // card rectangle path
            RectangleF cardRectF = new RectangleF(ClientRectangle.Location, ClientRectangle.Size);
            cardRectF.X -= 0.5f;
            cardRectF.Y -= 0.5f;
            GraphicsPath cardPath = DrawHelper.CreateRoundRect(cardRectF, 4);

            // button shadow (blend with form shadow)
            DrawHelper.DrawSquareShadow(g, ClientRectangle);
            using (SolidBrush normalBrush = new SolidBrush(BackColor))
            {
                g.FillPath(normalBrush, cardPath);
            }
        }
    }
}
