using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MaterialSkin.Animations;

namespace MaterialSkin.Controls 
{
    public class MaterialComboBox : ComboBox , IMaterialControl
    {
        //Properties for managing the material design properties
        [Browsable(false)]
        public int Depth { get; set; }
        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;
        [Browsable(false)]
        public MouseState MouseState { get; set; }
        private Brush BorderBrush = new SolidBrush(SystemColors.Window);
        private Brush ArrowBrush = new SolidBrush(SystemColors.ControlText);
        private Brush DropButtonBrush = new SolidBrush(SystemColors.Control);
     
       


        public MaterialComboBox()
        {

            base.OnCreateControl();
            ResetColors();


        }


        void ResetColors()
        {
            Font = SkinManager.ROBOTO_REGULAR_11;
            BackColor = SkinManager.GetApplicationBackgroundColor();
            ForeColor = SkinManager.GetPrimaryTextColor();

            DropButtonBrush = SkinManager.GetDividersBrush();
            ArrowBrush = SkinManager.GetPrimaryTextBrush();
            BorderBrush = SkinManager.GetDividersBrush();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }


       protected void colorchange(EventArgs args)
        {
            ResetColors();
            DrawCombobox();
        }
       

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            colorchange(e);
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            colorchange(e);
        }

        protected override void WndProc(ref Message m)
        {

            base.WndProc(ref m);
            switch (m.Msg)
            {

                case 0xF:
                    {
                        DrawCombobox();
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }


        // Override mouse and focus events to draw
        // proper borders. Basically, set the color and Invalidate(),
        // In general, Invalidate causes a control to redraw itself.

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            BorderBrush = SkinManager.GetDividersBrush();
            this.Invalidate();
        }
        
        protected void DrawCombobox()
        {
            SuspendLayout();
            Graphics g = this.CreateGraphics();
            Pen p = new Pen(SkinManager.GetPrimaryTextColor(), 1);
            BorderBrush = new SolidBrush(SkinManager.GetApplicationBackgroundColor());
            g.Clear(SkinManager.GetApplicationBackgroundColor());
            g.FillRectangle(BorderBrush, this.ClientRectangle);
            g.DrawRectangle(new Pen(SkinManager.GetApplicationBackgroundColor(), 1), this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1);


            // Draw the background of the dropdown button
            Rectangle rect = new Rectangle(this.Width - 15, 3, 12, this.Height - 6);
            g.FillRectangle(DropButtonBrush, rect);

            // Create the path for the arrow
            System.Drawing.Drawing2D.GraphicsPath pth = new System.Drawing.Drawing2D.GraphicsPath();
            PointF TopLeft = new PointF(this.Width - 13, (this.Height - 5) / 2);
            PointF TopRight = new PointF(this.Width - 6, (this.Height - 5) / 2);
            PointF Bottom = new PointF(this.Width - 9, (this.Height + 2) / 2);
            pth.AddLine(TopLeft, TopRight);
            pth.AddLine(TopRight, Bottom);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            // Determine the arrow's color.
            if (this.DroppedDown)
                ArrowBrush = SkinManager.ColorScheme.AccentBrush;
            else
                ArrowBrush = SkinManager.GetPrimaryTextBrush();

            // Draw the arrow
            g.FillPath(ArrowBrush, pth);

            if (DropDownStyle == ComboBoxStyle.DropDownList)
                g.DrawString(this.Text, this.Font, SkinManager.GetPrimaryTextBrush(), ClientRectangle.X + 2, ClientRectangle.Y + 3);

            ResumeLayout();

        }

    }
}

