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
    public partial class yenisema : Form
    {
        public yenisema()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox2.Text == "")
            {
                MessageBox.Show("Parametre bilgileri boş olamaz.", "Parametre Ekle");
            }
           
          
            listBox1.Items.Add(textBox2.Text);
            textBox2.Clear ();

        }

        private void yenisema_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedItems.Count > 0)
            {
                
#pragma warning disable CS8601 // Olası null başvuru ataması.
                listBox1.Items[listBox1.SelectedIndex] = textBox2.Text;
#pragma warning restore CS8601 // Olası null başvuru ataması.
            }
            else
            {
                MessageBox.Show("Boş parametre atayamazsın.","Parametre düzenle");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                listBox1.Items.Remove(listBox1.SelectedItem);
            }
            else
            {
                MessageBox.Show("Silme işlemi için parametre seçmelisin..", "Parametre Sil");

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox1.Text=="" )
            {
                MessageBox.Show("Şablon adı veya parametrelerini girmeden şablon oluşturamazsınız.", "Şablon oluştur.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (listBox1.Items.Count < 1)
            {
                MessageBox.Show("Şablon adı veya parametrelerini girmeden şablon oluşturamazsınız.", "Şablon oluştur.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DirectoryInfo di = new DirectoryInfo("Sql");
            FileInfo[] files = di.GetFiles("*.db");
           
            foreach (FileInfo fi in files)
            {
                if (textBox1.Text == fi.Name)
                {
                    MessageBox.Show("Aynı isimle şablon bulunuyor.", "Şablon oluştur.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (!File.Exists("Şablonlar/"+textBox1.Text+".db"))
            {
                if (Directory.Exists("Sql/Şablonlar"))
                {
                    SQLiteConnection.CreateFile(textBox1.Text);
                }
                else
                {
                    Directory.CreateDirectory("Sql/Şablonlar");
                    File.Create(textBox1.Text);
                }

            }

            // Sql dosyası oluşturulduktan sonra şablon veritabanı dosyasına db yapısını yazma işlemi.
            Form1 anaform=new Form1();
            string şablondbfile = "Data Source=Sql/Şablonlar/"+textBox1.Text+".db;Version=3;";
            
            string tablo=textBox1.Text.Replace(' ', '_');// boşlukları _ belirteci ile degistirme
            string parametreler = "";
            foreach(string parametre in listBox1.Items)
            {
                parametreler = parametreler + ", " + parametre + " TEXT";
            }
            string context = "CREATE TABLE IF NOT EXISTS " + tablo + " (ID INTEGER PRIMARY KEY AUTOINCREMENT" + parametreler + ")";
            //MessageBox.Show(context);
            anaform.sqlengine(şablondbfile, context, "0");

            şema_düzenle şemaduzenleform = new şema_düzenle();

            şemaduzenleform.dbyolu = şablondbfile;
            şemaduzenleform.tablo = tablo;
            şemaduzenleform.verileriyansit();
            şemaduzenleform.Show();
            this.Hide();

        }
    }
}
