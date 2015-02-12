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
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.PrimaryPalette = ColorManager.PrimaryColors.Indigo;
            materialSkinManager.AccentPalette = ColorManager.AccentColors.Pink;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            materialSkinManager.Theme = materialSkinManager.Theme == MaterialSkinManager.Themes.DARK ? MaterialSkinManager.Themes.LIGHT : MaterialSkinManager.Themes.DARK;
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            FieldInfo[] fieldInfos = typeof(ColorManager.PrimaryColors).GetFields(BindingFlags.Static | BindingFlags.Public);
            var defaultValue = default(ColorManager.PrimaryColors);
            List<ColorManager.PrimaryColors> colors = new List<ColorManager.PrimaryColors>();
            foreach (FieldInfo info in fieldInfos)
            {
                colors.Add((ColorManager.PrimaryColors)info.GetValue(defaultValue));
            }
            int currentIndex = colors.IndexOf(materialSkinManager.PrimaryPalette);
            if (currentIndex == colors.Count - 1)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }
            materialSkinManager.PrimaryPalette = colors[currentIndex];
        }

        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            FieldInfo[] fieldInfos = typeof(ColorManager.AccentColors).GetFields(BindingFlags.Static | BindingFlags.Public);
            var defaultValue = default(ColorManager.AccentColors);
            List<ColorManager.AccentColors> colors = new List<ColorManager.AccentColors>();
            foreach (FieldInfo info in fieldInfos)
            {
                colors.Add((ColorManager.AccentColors)info.GetValue(defaultValue));
            }
            int currentIndex = colors.IndexOf(materialSkinManager.AccentPalette);
            if (currentIndex == colors.Count - 1)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }
            materialSkinManager.AccentPalette = colors[currentIndex];
        }

        private void materialFlatButton2_Click(object sender, EventArgs e)
        {

        }
    }
}
