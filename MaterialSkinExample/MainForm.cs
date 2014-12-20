using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;

namespace MaterialSkinExample
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            // Initialize MaterialSkinManager
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.PrimaryColor = Color.FromArgb(63, 81, 181);
            materialSkinManager.PrimaryColorDark = Color.FromArgb(48, 63, 159);
            materialSkinManager.AccentColor = Color.FromArgb(255, 64, 129);
            BackColor = materialSkinManager.GetApplicationBackgroundColor();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void materialButton1_Click(object sender, EventArgs e)
        {

        }

    }
}
