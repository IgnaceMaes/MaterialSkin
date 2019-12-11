using System.ComponentModel;
using System.Windows.Forms;

namespace MaterialSkin.Controls
{
    public class MaterialLabel : Label, IMaterialControl
    {
        [Browsable(false)]
        public int Depth { get; set; }
        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;
        [Browsable(false)]
        public MouseState MouseState { get; set; }

        public MaterialLabel() : base()
        {
            ForeColor = SkinManager.GetPrimaryTextColor();
            Font = SkinManager.ROBOTO_REGULAR_11;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            BackColorChanged += (sender, args) => ForeColor = SkinManager.GetPrimaryTextColor();
        }

        [Browsable(true)]
        [Category("Material")]
        [Description("Chenge the Font.Size Property")]
        public float FontSize
        {
            get { return this.Font.Size; }
            set
            {
                base.Font = SkinManager.SetRoboFontSize(value);
                this.Refresh();
            }
        }
    }
}
