using System.Windows.Forms;

namespace MaterialSkin.Controls
{
    public class MaterialLabel : Label, IMaterialControl
    {
        public int Depth { get; set; }
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
        public MouseState MouseState { get; set; }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            ForeColor = SkinManager.GetMainTextColor();
            Font = SkinManager.FONT_BODY1;

            BackColorChanged += (sender, args) => ForeColor = SkinManager.GetMainTextColor();
        }
    }
}
