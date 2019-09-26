﻿namespace MaterialSkin.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="MaterialComboBox" />
    /// </summary>
    public class MaterialComboBox : ComboBox, IMaterialControl
    {

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Description("Auto adjust the width of the combobox to the longest item text"), Category("Data")]
        public override bool AutoSize { get; set; }
        //Properties for managing the material design properties
        /// <summary>
        /// Gets or sets the Depth
        /// </summary>
        [Browsable(false)]
        public int Depth { get; set; }

        /// <summary>
        /// Gets the SkinManager
        /// </summary>
        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        /// <summary>
        /// Gets or sets the MouseState
        /// </summary>
        [Browsable(false)]
        public MouseState MouseState { get; set; }

        /// <summary>
        /// Defines the BorderBrush
        /// </summary>
        private Brush BorderBrush = new SolidBrush(SystemColors.Window);

        /// <summary>
        /// Defines the ArrowBrush
        /// </summary>
        private Brush ArrowBrush = new SolidBrush(SystemColors.ControlText);

        /// <summary>
        /// Defines the DropButtonBrush
        /// </summary>
        private Brush DropButtonBrush = new SolidBrush(SystemColors.Control);

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialComboBox"/> class.
        /// </summary>
        public MaterialComboBox()
        {

            base.OnCreateControl();
            ResetColors();

        }

        /// <summary>
        /// The ResetColors
        /// </summary>
        internal void ResetColors()
        {
            Brush br = new SolidBrush(SkinManager.GetControlBackgroundColor());
            Font = SkinManager.getFontByType(MaterialSkinManager.fontType.Body1);
            BackColor = SkinManager.GetControlBackgroundColor();
            ForeColor = SkinManager.GetPrimaryTextColor();

            DropButtonBrush = br;
            ArrowBrush = SkinManager.GetPrimaryTextBrush();
            BorderBrush = br;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        /// <summary>
        /// The colorchange
        /// </summary>
        /// <param name="args">The args<see cref="EventArgs"/></param>
        protected void colorchange(EventArgs args)
        {
            ResetColors();
            DrawCombobox();
        }

        /// <summary>
        /// The OnBackColorChanged
        /// </summary>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            colorchange(e);
        }

        /// <summary>
        /// The OnForeColorChanged
        /// </summary>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            colorchange(e);
        }

        /// <summary>
        /// The WndProc
        /// </summary>
        /// <param name="m">The m<see cref="Message"/></param>
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
        /// <summary>
        /// The OnSelectedIndexChanged
        /// </summary>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            BorderBrush = SkinManager.GetDividersBrush();
            this.Invalidate();

        }

        /// <summary>
        /// The DrawCombobox
        /// </summary>
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
            {
                ArrowBrush = SkinManager.ColorScheme.AccentBrush;
            }
            else
            {
                ArrowBrush = SkinManager.GetPrimaryTextBrush();
            }

            // Draw the arrow
            g.FillPath(ArrowBrush, pth);

            // Text
            if (DropDownStyle == ComboBoxStyle.DropDownList)
            {
                using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
                {
                    NativeText.DrawTransparentText(Text, SkinManager.getLogFontByType(MaterialSkinManager.fontType.Body1),
                        SkinManager.GetPrimaryTextColor(),
                        ClientRectangle.Location,
                        ClientRectangle.Size,
                        NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
                }
            }

            ResumeLayout();
        }



        protected override void OnDropDown(EventArgs e)
        {
            base.OnDropDown(e);
            ;
            if (AutoSize == false) { return; }


            int width = this.DropDownWidth;
            Graphics g = this.CreateGraphics();
            Font font = this.Font;

            int vertScrollBarWidth = (this.Items.Count > this.MaxDropDownItems)
                    ? SystemInformation.VerticalScrollBarWidth : 0;

            var itemsList = this.Items.Cast<object>().Select(item => item.ToString());

            foreach (string s in itemsList)
            {
                int newWidth = (int)g.MeasureString(s, font).Width + vertScrollBarWidth;

                if (width < newWidth)
                {
                    width = newWidth;
                }
            }

            this.DropDownWidth = width;
            this.Width = width + 15;
        }
    }
}
