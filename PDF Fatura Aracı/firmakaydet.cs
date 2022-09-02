using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;


namespace PDF_Fatura_Aracı
{
    public partial class firmakaydet : Form
    {
        public firmakaydet()
        {
            InitializeComponent();
        }
        public SQLiteConnection dbstring;

        private void button1_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedItems.Count <1)
            {
                MessageBox.Show("Dosya içeriğinden satır seçmelisin.");
            }
            else
            {
                textBox1.Text=listBox1.SelectedItem.ToString();
                button2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count <1)
            {
                MessageBox.Show("Dosya içeriğinden satır seçmelisin.");
            }
            else
            {
                textBox2.Text = listBox1.SelectedItem.ToString();
                button3.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count <1 & listBox1.SelectedItems.ToString()=="")
            {
                MessageBox.Show("Dosya içeriğinden satır seçmelisin.");
            }
            else
            {

#pragma warning disable CS8604 // Olası null başvuru bağımsız değişkeni.
             listBox2.Items.Add(listBox1.SelectedItem.ToString());
                button4.Enabled = true;
                button6.Enabled = true;
                button5.Enabled = true;
#pragma warning restore CS8604 // Olası null başvuru bağımsız değişkeni.
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(listBox2.SelectedItems.Count <1)
            {
                MessageBox.Show("Silinecek satırı sağ kutudan seçmedin.");
            }
            {
#pragma warning disable CS8604 // Olası null başvuru bağımsız değişkeni.
                listBox2.Items.Remove(listBox2.SelectedItem.ToString());
#pragma warning restore CS8604 // Olası null başvuru bağımsız değişkeni.
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "" && textBox2.Text == "" && listBox2.Items.Count <1)
            {
                MessageBox.Show("Firma adı ve vergi no alanları ve anahtar kelimeler alanları dolu olmalıdır.");
                return;
            }
            Form1 anaform = new Form1();

            string cs = anaform.FirmalarDBFile;
            using var con = new SQLiteConnection(cs);
            con.Open();
            SQLiteCommand insertSQL = new SQLiteCommand("INSERT INTO Firmalar (firma,vergino,AnahtarSozcuk) VALUES (@Firma,@vergino,@anahtarsozcuk)", con);

            foreach (String strCol in listBox2.Items)
            {

                

                insertSQL.Parameters.AddWithValue("@Firma", @textBox1.Text.ToString());
                insertSQL.Parameters.AddWithValue("@vergino", @textBox2.Text);
                insertSQL.Parameters.AddWithValue("@anahtarsozcuk", strCol);
                insertSQL.ExecuteNonQuery();
            }
            try
            {
                
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message);
            }
            
            con.Close();
           
            this.Hide(); 
            

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                MessageBox.Show("Anahtar kelime boşluk olamaz");
                return;
            }
            else
            {
                listBox2.Items.Add(textBox3.Text);
                textBox3.Text = "";
                if (button5.Enabled == false) button5.Enabled = true;
            }
        }
    }
}
    