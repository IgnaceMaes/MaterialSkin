namespace MaterialSkin.Controls
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class MaterialLabel : Label, IMaterialControl
    {
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

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
