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
    public partial class şema_düzenle : Form
    {
        public şema_düzenle()
        {
            InitializeComponent();
        }
        public List<string> paremetreler = new List<string>();
        public string dbyolu="";
        public string tablo = "";
        private void şema_düzenle_Load(object sender, EventArgs e)
        {
           
        }

        public void verileriyansit()
        {
            Form1   anaform = new Form1();
            
            paremetreler.Clear();
            paremetreler.AddRange(anaform.sqlengine(anaform.FirmalarDBFile, "SELECT AnahtarSozcuk from Firmalar", "3"));
            checkedListBox1.Items.Clear();
            // yansıt
            foreach (string anahtarkelime in paremetreler)
            {
                checkedListBox1.Items.Add(anahtarkelime);
                
            }
           


            DataSet ds = new DataSet();
            using var con = new SQLiteConnection(dbyolu);
            con.Open();
            SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM "+tablo, con);
            da.Fill(ds, tablo);
            
            dataGridView1.DataSource = ds.Tables[tablo];
            con.Close();
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            for (int i = 1; i < dataGridView1.Columns.Count; i++)
            {
                
                comboBox1.Items.Add(dataGridView1.Columns[i].HeaderText);
                comboBox2.Items.Add(dataGridView1.Columns[i].HeaderText);
            }
           
           
            comboBox3.Items.Clear();
            foreach(string firmaadi in anaform.sqlengine(anaform.FirmalarDBFile, "SELECT firma from Firmalar", "4"))
            {
                comboBox3.Items.Add(firmaadi);
  
            }

        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
            if (textBox4.Text == "")
            {
                foreach (string anahtarkelime in paremetreler)
                {
                    checkedListBox1.Items.Add(anahtarkelime);

                }
            }
            else
            {

                foreach (string str in paremetreler)
                {
                    if (str.StartsWith(textBox4.Text, StringComparison.CurrentCultureIgnoreCase))
                    {
                        checkedListBox1.Items.Add(str);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(checkedListBox1.CheckedItems.Count<1 && comboBox1.Text == "Parametre Seç")
            {
                MessageBox.Show("Anahtar kelime veya parametre doğru değil.");
                return;

            }

            Form1 anaform = new Form1();
            foreach(string anahtarkelime in checkedListBox1.CheckedItems)
            {
                
                anaform.sqlengine(dbyolu, "INSERT INTO " + tablo + " ('" + comboBox1.Text + "') VALUES ('"+anahtarkelime+"');", "0");
            }

            verileriyansit();
            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count < 1)
            {
                MessageBox.Show("Şablona kayıtlı olan seçtiğiniz bir anahtar kelime yok.\nAnahtar kelime silmek için şablon tablosundan bir anahtar kelime seçin.");
                return;

            }
            foreach (DataGridViewCell anahtarkelime in dataGridView1.SelectedCells)
            {
                Form1 anaform = new Form1();
                //MessageBox.Show("DELETE  FROM " + tablo + " WHERE " + dataGridView1.Columns[anahtarkelime.ColumnIndex].HeaderText + "='" + anahtarkelime.Value + "'");
                anaform.sqlengine(dbyolu, "DELETE  FROM " + tablo + " WHERE "+dataGridView1.Columns[anahtarkelime.ColumnIndex].HeaderText+"='"+anahtarkelime.Value+"'", "0");
                verileriyansit();
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox2.Text == "Parametre Seç")
            {
                MessageBox.Show("Silinecek parametreyi yandaki açılır menüden seçin.");
                return;

            }
            Form1 anaform = new Form1();
            //MessageBox.Show("DELETE  FROM " + tablo + " WHERE " + dataGridView1.Columns[anahtarkelime.ColumnIndex].HeaderText + "='" + anahtarkelime.Value + "'");
            anaform.sqlengine(dbyolu, "ALTER TABLE "+tablo+" DROP COLUMN '"+comboBox2.Text+"'", "0");
            verileriyansit();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (comboBox2.Text == "Parametre Seç")
            {
                MessageBox.Show("Güncellenecek parametreyi yandaki açılır menüden seçin.");
                return;

            }
            if (textBox2.Text == "")
            {
                MessageBox.Show("Güncellenecek parametrenin yeni adını yandaki alana yazın.");
                return;

            }
            Form1 anaform = new Form1();
            //MessageBox.Show("DELETE  FROM " + tablo + " WHERE " + dataGridView1.Columns[anahtarkelime.ColumnIndex].HeaderText + "='" + anahtarkelime.Value + "'");
            anaform.sqlengine(dbyolu, "ALTER TABLE " + tablo + " RENAME COLUMN " + comboBox2.Text + " TO '"+textBox2.Text+"'", "0");
            verileriyansit();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                MessageBox.Show("Eklenecek parametrenin yeni adını yandaki alana yazın.");
                return;

            }
            Form1 anaform = new Form1();
            //MessageBox.Show("DELETE  FROM " + tablo + " WHERE " + dataGridView1.Columns[anahtarkelime.ColumnIndex].HeaderText + "='" + anahtarkelime.Value + "'");
            anaform.sqlengine(dbyolu, "ALTER TABLE " + tablo + " ADD COLUMN '" + textBox3.Text + "' TEXT", "0");
            verileriyansit();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (textBox5.Text == "") {
                MessageBox.Show("Eklenecek anahtar kelimeyi üst kutucuğa yazın.");
                return; 
            }

            Form1 anaform = new Form1();
           

            anaform.sqlengine(dbyolu, "INSERT INTO " + tablo + " ('" + comboBox1.Text + "') VALUES ('" + textBox5.Text + "');", "0");

            textBox5.Clear();
            verileriyansit();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            Form1 anaform = new Form1();
            foreach (string satirokuma in anaform.sqlengine(anaform.FirmalarDBFile, "SELECT SatirOkuma from Firmalar WHERE firma='" + comboBox3.Text + "' and AnahtarSozcuk='" + comboBox4.Text + "'", "SatirOkuma"))
                if (satirokuma == "" || satirokuma == " " || satirokuma==null)
                {
                    textBox1.Text = "1";
                }
                else textBox1.Text = satirokuma;

            
            foreach (string satirokuma in anaform.sqlengine(anaform.FirmalarDBFile, "SELECT FirmaAdiCekilsin from Firmalar WHERE firma='" + comboBox3.Text + "' and AnahtarSozcuk='" + comboBox4.Text + "'", "FirmaAdiCekilsin"))
            {
               
                if (satirokuma=="0" || satirokuma=="" || satirokuma == " ")
                {
                    checkBox1.Checked = false;
                }
                else checkBox1.Checked = true;
            }
            foreach (string satirokuma in anaform.sqlengine(anaform.FirmalarDBFile, "SELECT Filtre from Firmalar WHERE firma='" + comboBox3.Text + "' and AnahtarSozcuk='" + comboBox4.Text + "'", "Filtre"))
            {

                if (satirokuma == "" || satirokuma == " ")
                {
                    textBox6.Text = "Kelime yazın..";
                }
                else textBox6.Text = satirokuma;
            }


        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Form1 anaform = new Form1();
            comboBox4.Items.Clear();
            foreach (string firmaadi in anaform.sqlengine(anaform.FirmalarDBFile, "SELECT AnahtarSozcuk from Firmalar WHERE firma='" + comboBox3.Text + "'", "3"))
            {
                comboBox4.Items.Add(firmaadi);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 anaform = new Form1();
            if(comboBox3.SelectedItem == null)
            {
                MessageBox.Show("İlk önce firma seçmen gerekir.");
                return;
            }
            if (comboBox4.SelectedItem == null)
            {
                MessageBox.Show("İlk önce anahtar kelime seçmen gerekir.");
                return;
            }
            if (checkBox1.Checked == true)
            {
                anaform.sqlengine(anaform.FirmalarDBFile, "UPDATE Firmalar SET FirmaAdiCekilsin=1 WHERE firma='" + comboBox3.Text + "' and AnahtarSozcuk='" + comboBox4.Text + "'", "0");
                anaform.sqlengine(anaform.FirmalarDBFile, "UPDATE Firmalar SET SatirOkuma="+Convert.ToInt64(textBox1.Text)+" WHERE firma='" + comboBox3.Text + "' and AnahtarSozcuk='"+comboBox4.Text+"'", "0");
            }
            else
            {
                anaform.sqlengine(anaform.FirmalarDBFile, "UPDATE Firmalar SET FirmaAdiCekilsin=0 WHERE firma='" + comboBox3.Text + "' and AnahtarSozcuk='" + comboBox4.Text + "'", "0");
                anaform.sqlengine(anaform.FirmalarDBFile, "UPDATE Firmalar SET SatirOkuma="+Convert.ToInt64(textBox1.Text)+" WHERE firma='" + comboBox3.Text + "' and AnahtarSozcuk='" + comboBox4.Text + "'", "0");
            }
            MessageBox.Show("Özel ayar eklendi.","Özel ayar ekle",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);



        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            Form1 anaform = new Form1();
            if (comboBox3.SelectedItem == null)
            {
                MessageBox.Show("İlk önce firma seçmen gerekir.");
                return;
            }
            if (comboBox4.SelectedItem == null)
            {
                MessageBox.Show("İlk önce anahtar kelime seçmen gerekir.");
                return;
            }
           
            anaform.sqlengine(anaform.FirmalarDBFile, "UPDATE Firmalar SET Filtre='"+textBox6.Text+"' WHERE firma='" + comboBox3.Text + "' and AnahtarSozcuk='" + comboBox4.Text + "'", "0");
         
            MessageBox.Show("Özel filtre ayarı eklendi.", "Özel ayar ekle", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }

}
    

