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
    public partial class Gamma : Form
    {
        public int value { get; set; }
        public Gamma()
        {
            InitializeComponent();
        }
        public Gamma(int min, int max,string formname)
        {
            InitializeComponent();
            trackBar1.Minimum = min;
            trackBar1.Value = min;
            trackBar1.Maximum = max;
            this.Text = formname;
        }

            private void Gamma_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            value = trackBar1.Value;
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text =$"Значення: {trackBar1.Value}";
        }
    }
}
