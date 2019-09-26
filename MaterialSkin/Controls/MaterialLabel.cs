namespace MaterialSkin.Controls
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="MaterialLabel" />
    /// </summary>
    public class MaterialLabel : Label, IMaterialControl
    {
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

        public enum EHorizontalAlign
        {
            Left,
            Center,
            Right
        }

        public enum EVerticalAlign
        {
            Top,
            Middle,
            Bottom
        }


        private EHorizontalAlign _hAlign;
        [Category("Material Skin")]
        public EHorizontalAlign HorizontalAlign
        {
            get
            {
                return _hAlign;
            }
            set
            {
                _hAlign = value;
                updateAligment();
            }
        }

        private EVerticalAlign _vAlign;
        [Category("Material Skin")]
        public EVerticalAlign VerticalAlign
        {
            get
            {
                return _vAlign;
            }
            set
            {
                _vAlign = value;
                updateAligment();
            }
        }

        [Category("Material Skin")]
        public bool HighEmphasis { get; set; }

        [Category("Material Skin")]
        public bool UseAccent { get; set; }

        MaterialSkinManager.fontType _fontType;
        [Category("Material Skin"),
        DefaultValue(typeof(MaterialSkinManager.fontType), "Body1")]
        public MaterialSkinManager.fontType FontType
        {
            get
            {
                return _fontType;
            }
            set
            {
                _fontType = value;
                Font = SkinManager.getFontByType(_fontType);
                Refresh();
            }
        }

        public MaterialLabel()
        {
            FontType = MaterialSkinManager.fontType.Body1;
        }

        /// <summary>
        /// The GetPreferredSize
        /// </summary>
        /// <param name="proposedSize">The proposedSize<see cref="Size"/></param>
        /// <returns>The <see cref="Size"/></returns>
        public override Size GetPreferredSize(Size proposedSize)
        {
            if (AutoSize)
            {
                Size strSize;
                using (NativeTextRenderer NativeText = new NativeTextRenderer(CreateGraphics()))
                {
                    strSize = NativeText.MeasureLogString(Text, SkinManager.getLogFontByType(_fontType));
                }
                return strSize;
            }
            else
            {
                return proposedSize;
            }
        }

        private NativeTextRenderer.TextAlignFlags Alignment;
        private void updateAligment()
        {
            // Alignment
            Alignment =
                (_hAlign == EHorizontalAlign.Right ? NativeTextRenderer.TextAlignFlags.Right :
                _hAlign == EHorizontalAlign.Center ? NativeTextRenderer.TextAlignFlags.Center :
                NativeTextRenderer.TextAlignFlags.Left)
                |
                (_vAlign == EVerticalAlign.Bottom ? NativeTextRenderer.TextAlignFlags.Bottom :
                _vAlign == EVerticalAlign.Middle ? NativeTextRenderer.TextAlignFlags.Middle :
                NativeTextRenderer.TextAlignFlags.Top);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Parent.BackColor);


            // Draw Text
            using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            {
                NativeText.DrawMultilineTransparentText(
                    Text,
                    SkinManager.getLogFontByType(_fontType),
                    Enabled ? SkinManager.GetPrimaryTextColor() : SkinManager.GetDisabledOrHintColor(),
                    ClientRectangle.Location,
                    ClientRectangle.Size,
                    Alignment);
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            Font = SkinManager.getFontByType(_fontType);
            BackColorChanged += (sender, args) => Refresh();
        }
    }
}
