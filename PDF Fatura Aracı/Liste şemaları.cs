using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDF_Fatura_Aracı
{
    public partial class Liste_şemaları : Form
    {
        public Liste_şemaları()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedItems.Count > 0)
            {
                System.IO.File.Delete("Sql/Şablonlar/"+listBox1.SelectedItem.ToString());
                listBox1.Items.Remove(listBox1.SelectedItem);
            }
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            yenisema yenisema = new yenisema();
            yenisema.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedItems.Count > 0 &&listBox1.SelectedItems.Count<2)
            {
                şema_düzenle şemaduzenleform = new şema_düzenle();
                string şablondbfile = "Data Source=Sql/Şablonlar/"+listBox1.SelectedItem.ToString()+";Version=3;";
                string tablo = listBox1.SelectedItem.ToString().Replace(' ', '_');// boşlukları _ belirteci ile degistirme
                tablo= tablo.Replace(".db", "");
                şemaduzenleform.dbyolu = şablondbfile;
                şemaduzenleform.tablo = tablo;
               
                şemaduzenleform.verileriyansit();
                şemaduzenleform.Show();
                this.Hide();
            }
        }
    }
}
