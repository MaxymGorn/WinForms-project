using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaxsGorn
{
    public partial class Crop : Form
    {
        public Crop()
        {
            InitializeComponent();
        }
        public int value1 { get; set; }
        public int value2 { get; set; }
        public int value3 { get; set; }
        public int value4 { get; set; }
        public Crop(int maxX, int maxY)
        {
            InitializeComponent();
            numericUpDown1.Maximum = maxX;
            numericUpDown2.Maximum = maxY;
            numericUpDown3.Maximum = maxX;
            numericUpDown4.Maximum = maxY;

        }

        private void Crop_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            value1 = int.Parse(numericUpDown1.Value.ToString());
            value2 = int.Parse(numericUpDown2.Value.ToString());
            value3 = int.Parse(numericUpDown3.Value.ToString());
            value4 = int.Parse(numericUpDown4.Value.ToString());
            this.DialogResult = DialogResult.OK;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
