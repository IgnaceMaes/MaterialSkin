using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace MaterialSkinExample
{
    public partial class MainForm : MaterialForm
    {
        private MaterialSkinManager materialSkinManager;
        public MainForm()
        {
            InitializeComponent();

            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            /*materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.PrimaryColor = Color.FromArgb(63, 81, 181);
            materialSkinManager.PrimaryColorDark = Color.FromArgb(48, 63, 159);
            materialSkinManager.AccentColor = Color.FromArgb(255, 64, 129);*/
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.Palette = ColorManager.Colors.Indigo;
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

        private void MainForm_Load(object sender, EventArgs e)
        {

        }


        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            FieldInfo[] fieldInfos = typeof(ColorManager.Colors).GetFields(BindingFlags.Static | BindingFlags.Public);
            var defaultValue = default(ColorManager.Colors);
            List<ColorManager.Colors> colors = new List<ColorManager.Colors>();
            foreach (FieldInfo info in fieldInfos)
            {
                colors.Add((ColorManager.Colors)info.GetValue(defaultValue));
            }
            int currentIndex = colors.IndexOf(materialSkinManager.Palette);
            if (currentIndex == colors.Count - 1)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }
            materialSkinManager.Palette = colors[currentIndex];
        }
    }
}
