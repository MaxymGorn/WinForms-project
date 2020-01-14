using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maxs_Gorn
{
    public partial class NewArtwork : Form
    {
        private Button button2;
        private Button button1;
        private Label label2;
        private Label label1;
        public NumericUpDown bmHeight;
        public NumericUpDown bmWidth;

        public NewArtwork()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewArtwork));
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.bmHeight = new System.Windows.Forms.NumericUpDown();
            this.bmWidth = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.bmHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bmWidth)).BeginInit();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(35, 92);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 41);
            this.button2.TabIndex = 11;
            this.button2.Text = "&Відміна";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_2);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(184, 92);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 41);
            this.button1.TabIndex = 10;
            this.button1.Text = "&OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 45);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 28);
            this.label2.TabIndex = 9;
            this.label2.Text = "&Довжина:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 28);
            this.label1.TabIndex = 8;
            this.label1.Text = "&Ширина:";
            // 
            // bmHeight
            // 
            this.bmHeight.Location = new System.Drawing.Point(147, 45);
            this.bmHeight.Margin = new System.Windows.Forms.Padding(4);
            this.bmHeight.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.bmHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.bmHeight.Name = "bmHeight";
            this.bmHeight.Size = new System.Drawing.Size(161, 33);
            this.bmHeight.TabIndex = 7;
            this.bmHeight.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // bmWidth
            // 
            this.bmWidth.Location = new System.Drawing.Point(147, 9);
            this.bmWidth.Margin = new System.Windows.Forms.Padding(4);
            this.bmWidth.Maximum = new decimal(new int[] {
            320,
            0,
            0,
            0});
            this.bmWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.bmWidth.Name = "bmWidth";
            this.bmWidth.Size = new System.Drawing.Size(161, 33);
            this.bmWidth.TabIndex = 6;
            this.bmWidth.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // NewArtwork
            // 
            this.ClientSize = new System.Drawing.Size(335, 146);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bmHeight);
            this.Controls.Add(this.bmWidth);
            this.Font = new System.Drawing.Font("Monotype Corsiva", 13.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NewArtwork";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Нова картинка";
            this.Load += new System.EventHandler(this.NewArtwork_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bmHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bmWidth)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void NewArtwork_Load(object sender, EventArgs e)
        {

        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
