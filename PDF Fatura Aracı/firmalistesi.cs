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
using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;
using Application = System.Windows.Forms.Application;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PDF_Fatura_Aracı
{
    public partial class firmalistesi : Form
    {
#pragma warning disable CS8618 // Null atanamaz alan, oluşturucudan çıkış yaparken null olmayan bir değer içermelidir. Alanı null atanabilir olarak bildirmeyi düşünün.
        public firmalistesi()
#pragma warning restore CS8618 // Null atanamaz alan, oluşturucudan çıkış yaparken null olmayan bir değer içermelidir. Alanı null atanabilir olarak bildirmeyi düşünün.
        {
            InitializeComponent();
        }

        public SQLiteDataAdapter da;
        public DataTable dt;
        public DataSet ds;
        public SQLiteConnection con;
      


        private void button1_Click(object sender, EventArgs e)
        {
            // Eğer satır seçilmediyse
            if(dataGridView1.SelectedRows.Count <1)
            {
                MessageBox.Show("Silmek için satırı seçmelisin.\nSilmek istediğiniz satırı ilk önce satırı sağ kutucuğundan işaretleyin", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            dataGridView1.Rows.Remove(dataGridView1.SelectedRows[0]);
            MessageBox.Show("Seçtiğin satırlar listeden kaldırıldı. Listeyi şuanki durumda kaydetmek için kaydet ve güncelle butonuna basın.",Application.ProductName,MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void firmalistesi_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
           
            Form1 anaform = new Form1();
            using var con = new SQLiteConnection(anaform.FirmalarDBFile);
            using (da= new SQLiteDataAdapter(@"select * from Firmalar", con))
            {
                SQLiteCommandBuilder commandBuilder = new SQLiteCommandBuilder(da);
                da.Update(dt);
            }


           
            MessageBox.Show("Tablo güncellendi.", "Anahtar Kelime Listesi", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

        }
    }
}
