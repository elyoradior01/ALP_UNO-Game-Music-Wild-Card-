using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ALP_UNO_Game
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static string Warna { get; set; }
        
        private void BlueButton_Click(object sender, EventArgs e)
        {
            Warna = "blue";

            this.Close();
            MessageBox.Show("The Color has been changed to Blue!!");
        }

        private void YellowButton_Click(object sender, EventArgs e)
        {
            Warna = "yellow";
            this.Close();
            MessageBox.Show("The Color has been changed to Yellow!!");
        }

        private void GreenButton_Click(object sender, EventArgs e) 
        {
            Warna = "green";
            this.Close();
            MessageBox.Show("The Color has been changed to Green!!");
        }

        private void button1_Click(object sender, EventArgs e) //merah
        {
            Warna = "red";
            this.Close();
            MessageBox.Show("The Color has been changed to Red!!");
        }
    }
}
