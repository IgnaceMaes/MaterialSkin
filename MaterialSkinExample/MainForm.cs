using System;
using System.Drawing;
using MaterialSkin;
using MaterialSkin.Controls;

namespace MaterialSkinExample
{
    public partial class MainForm : MaterialForm
    {
        public MainForm()
        {
            InitializeComponent();

            // Initialize MaterialSkinManager
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            /*materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.PrimaryColor = Color.FromArgb(63, 81, 181);
            materialSkinManager.PrimaryColorDark = Color.FromArgb(48, 63, 159);
            materialSkinManager.AccentColor = Color.FromArgb(255, 64, 129);*/
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.PrimaryColor = Color.FromArgb(55, 71, 79);
            materialSkinManager.PrimaryColorDark = Color.FromArgb(38, 50, 56);
            materialSkinManager.AccentColor = Color.FromArgb(64, 196, 255);
            /*materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.PrimaryColor = Color.FromArgb(32, 149, 242);
            materialSkinManager.PrimaryColorDark = Color.FromArgb(24, 117, 209);
            materialSkinManager.AccentColor = Color.FromArgb(254, 86, 33);*/
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.Theme = materialSkinManager.Theme == MaterialSkinManager.Themes.DARK ? MaterialSkinManager.Themes.LIGHT : MaterialSkinManager.Themes.DARK;
        }
    }
}
