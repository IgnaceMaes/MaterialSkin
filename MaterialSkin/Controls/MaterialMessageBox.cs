using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MaterialSkin.Controls
{
    ///TODO: Break this out into a MaterialDialog then extend into the MaterialMsgBox

    ///Adapted from http://www.codeproject.com/Articles/601900/FlexibleMessageBox
    public class MaterialMessageBox : IMaterialControl
    {
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        [Browsable(false)]
        public Point MouseLocation { get; set; }

        public static DialogResult Show(string text, bool UseRichTextBox = true, FlexibleMaterialForm.ButtonsPosition buttonsPosition = FlexibleMaterialForm.ButtonsPosition.Right)
        {
            return FlexibleMaterialForm.Show(null, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, UseRichTextBox, buttonsPosition);
        }

        public static DialogResult Show(IWin32Window owner, string text, bool UseRichTextBox = true, FlexibleMaterialForm.ButtonsPosition buttonsPosition = FlexibleMaterialForm.ButtonsPosition.Right)
        {
            return FlexibleMaterialForm.Show(owner, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, UseRichTextBox, buttonsPosition);
        }

        public static DialogResult Show(string text, string caption, bool UseRichTextBox = true, FlexibleMaterialForm.ButtonsPosition buttonsPosition = FlexibleMaterialForm.ButtonsPosition.Right)
        {
            return FlexibleMaterialForm.Show(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, UseRichTextBox, buttonsPosition);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, bool UseRichTextBox = true, FlexibleMaterialForm.ButtonsPosition buttonsPosition = FlexibleMaterialForm.ButtonsPosition.Right)
        {
            return FlexibleMaterialForm.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, UseRichTextBox, buttonsPosition);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, bool UseRichTextBox = true, FlexibleMaterialForm.ButtonsPosition buttonsPosition = FlexibleMaterialForm.ButtonsPosition.Right)
        {
            return FlexibleMaterialForm.Show(null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, UseRichTextBox, buttonsPosition);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, bool UseRichTextBox = true, FlexibleMaterialForm.ButtonsPosition buttonsPosition = FlexibleMaterialForm.ButtonsPosition.Right)
        {
            return FlexibleMaterialForm.Show(owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, UseRichTextBox, buttonsPosition);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, bool UseRichTextBox = true, FlexibleMaterialForm.ButtonsPosition buttonsPosition = FlexibleMaterialForm.ButtonsPosition.Right)
        {
            return FlexibleMaterialForm.Show(null, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, UseRichTextBox, buttonsPosition);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, bool UseRichTextBox = true, FlexibleMaterialForm.ButtonsPosition buttonsPosition = FlexibleMaterialForm.ButtonsPosition.Right)
        {
            return FlexibleMaterialForm.Show(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, UseRichTextBox, buttonsPosition);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, bool UseRichTextBox = true, FlexibleMaterialForm.ButtonsPosition buttonsPosition = FlexibleMaterialForm.ButtonsPosition.Right)
        {
            return FlexibleMaterialForm.Show(null, text, caption, buttons, icon, defaultButton, UseRichTextBox, buttonsPosition);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, bool UseRichTextBox = true, FlexibleMaterialForm.ButtonsPosition buttonsPosition = FlexibleMaterialForm.ButtonsPosition.Right)
        {
            return FlexibleMaterialForm.Show(owner, text, caption, buttons, icon, defaultButton, UseRichTextBox, buttonsPosition);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons messageBoxButtons, FlexibleMaterialForm.ButtonsPosition buttonsPosition = FlexibleMaterialForm.ButtonsPosition.Right)
        {
            return FlexibleMaterialForm.Show(null, text, caption, messageBoxButtons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, true, buttonsPosition);
        }
    }
}
