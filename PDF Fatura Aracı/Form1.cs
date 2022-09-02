
using System.Data.SQLite;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Excel = Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using System.Data.Common;
using System.Data.SqlClient;

namespace PDF_Fatura_Aracı
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string cevrilecekdosya = "";
        public string FirmalarDBFile = "Data Source=Sql/firmalar.db;Version=3;";
        string FirmaFile = "Sql/firmalar.db";
        bool firmatanima = false;
        List<string> firmatanimaList = new List<string>(); // Firmaları tanımak için liste oluşturuyor.
        DataTable aktarilacakliste = new DataTable();
        List<string[]>Veriagacilistesi = new List<string[]>();

        List<string[]> satirlar = new List<string[]>();
        List<string> Kolonlar = new List<string>();
        bool kayitsor = true;
        private void firmatani(string satir)
        {
            string[] sayilar = Regex.Split(satir, @"\D+");
            firmatanimaList.Add(sayilar[1]);
        }
        private void xmlcevir(string cevirilecekdosya)
        {
            loadbar_label.Invoke(new Action(delegate ()
            {
                loadbar_label.Text = "Hazırlanıyor..";
                loadbar_label.Text = "Seçilen dosyalar programa yükleniyor..";
            }));
            
            string pathToPdf = cevirilecekdosya;
            if (!Directory.Exists("xmldosyalari"))
                Directory.CreateDirectory("xmldosyalari");

            string pathToXml = Path.ChangeExtension("xmldosyalari/" + Path.GetFileName(cevirilecekdosya), ".xml");
            loadbar_label.Invoke(new Action(delegate ()
            {
                loadbar_label.Text = "Dosyalar sisteme yükleniyor..";
            }));
            // PDFİ XML YE ÇEVİR.
            SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();



            // KAPLAMA LAYERLERI GEÇ
            f.XmlOptions.ConvertNonTabularDataToSpreadsheet = true;


            f.OpenPdf(pathToPdf);

            if (f.PageCount > 0)
            {
                int result = f.ToXml(pathToXml);

                //BAŞARILI DOSYAYI PROGRAMA KAYDET
                if (result == 0)
                {
                    //System.Diagnostics.Process.Start(pathToXml);

                    TabPage pdftabpage = new TabPage(Path.GetFileName(pathToPdf));
                    tabControl2.Invoke(new Action(delegate ()
                    {
                        tabControl2.TabPages.Add(pdftabpage);
                    }));
                    
                    TabControl pdftabcontrol = new TabControl();
                    pdftabpage.Invoke(new Action(delegate ()
                    {
                        pdftabpage.Controls.Add(pdftabcontrol);
                    }));

                    TabPage pdftreeviewpage = new TabPage("Veri Ağacı");
                    pdftabcontrol.TabPages.Add(pdftreeviewpage);
                    pdftreeviewpage.Invoke(new Action(delegate ()
                    {
                        pdftreeviewpage.Dock = DockStyle.Fill;
                        
                    }));

                    TreeView pdftreeview = new TreeView();
                    pdftreeviewpage.Controls.Add(pdftreeview);
                    pdftreeview.Invoke(new Action(delegate ()
                    {
                        pdftreeview.Dock = DockStyle.Fill;
                        XmlDataDocument xmldoc = new XmlDataDocument();
                        XmlNode xmlnode;
                        xmldoc.Load(pathToXml);
                        xmlnode = xmldoc.ChildNodes[1];
                        pdftreeview.Nodes.Add(new TreeNode(xmldoc.DocumentElement.Name));
                        TreeNode tNode;
                        tNode = pdftreeview.Nodes[0];
                        AddNode(xmlnode, tNode);
                        pdftreeview.ExpandAll();
                        
                        
                    }));

                    TabPage pdfxmltexttab = new TabPage("Xml Çıktısı");
                    pdftabcontrol.Invoke(new Action(delegate ()
                    {
                        pdftabcontrol.Dock = DockStyle.Fill;
                        
                        pdftabcontrol.TabPages.Add(pdfxmltexttab);
                    }));
                    

                  

                    TextBox pdfxmlviewertextbox = new TextBox();
                    pdfxmlviewertextbox.ReadOnly = true;
                    pdfxmltexttab.Invoke(new Action(delegate ()
                    {
                        pdfxmltexttab.Controls.Add(pdfxmlviewertextbox);
                    }));
                    pdfxmlviewertextbox.Invoke(new Action(delegate ()
                    {
                        pdfxmlviewertextbox.Text = File.ReadAllText(pathToXml);
                        pdfxmlviewertextbox.Dock = DockStyle.Fill;
                        pdfxmlviewertextbox.Multiline = true;
                        pdfxmlviewertextbox.ScrollBars = ScrollBars.Vertical;
                    }));

                    



                    treeView1.Invoke(new Action(delegate ()
                    {
                        treeView1.Nodes.Add(Path.GetFileName(pathToXml));
                    }));
                    
                    TabPage pdftab = new TabPage("PDF Görünüm");
                    pdftabcontrol.Invoke(new Action(delegate ()
                    {
                        pdftabcontrol.TabPages.Add(pdftab);
                    }));
                    

                    Panel webBrowser = new Panel();
                   
                    webBrowser.Dock = DockStyle.Fill;




                    pdftab.Invoke(new Action(delegate ()
                    {
                        pdftab.Controls.Add(webBrowser);
                    }));


                    


                    linereader(pathToXml, 0);
                    
                    if (firmatanima == false && kayitsor==true)
                    {
                        

                        DialogResult sorucevap = MessageBox.Show(Path.GetFileName(pathToXml) + " adlı pdf'in firma bilgileri sistemde kayıtlı değil. Kayıtlamak istiyormusun ?\nKayıt işlemi aynı firmaya bir kere yapılmaktadır. Kayıt edilmeyen firmaların faturalarında tarama sırasında kişiselleştirilmiş ayarlar uygulanamaz.", "Bilgi kaydet", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        
                        if (sorucevap == DialogResult.Yes)
                        {
                            firmakaydet firmakayıtform = new firmakaydet();

                            /*satir ="";
                            using (StreamReader readers = new StreamReader(fileName))
                            {
                                while ((satir = readers.ReadLine()) != null)
                                {
                                    string[] kaynak = satir.Split(new char[] { '.', '?', '!',' ', ';', ':', ',',
                                                                  '<', '>', '/', '$', '[', ']', '(', ')',
                                                                  '=','"' }, StringSplitOptions.RemoveEmptyEntries);
                                    satir=kaynak[0];

                                    firmakayıtform.listBox1.Items.Add(satir);

                                }
                            }
                            */
                            XmlDocument xmlDocument = new XmlDocument();
                            xmlDocument.Load(pathToXml);
#pragma warning disable CS8600 // Null sabit değeri veya olası null değeri, boş değer atanamaz türe dönüştürülüyor.
                            XmlNodeList xmlNodeList = xmlDocument.SelectNodes("//row");


                            foreach (XmlNode xmlNode in xmlNodeList)
                            {

                                firmakayıtform.listBox1.Items.Add(xmlNode.SelectSingleNode("cell").InnerText);


                            }

#pragma warning restore CS8600 // Null sabit değeri veya olası null değeri, boş değer atanamaz türe dönüştürülüyor. 

                            firmakayıtform.ShowDialog();
                            
                            firmakayıtform.Focus();
                            



                        }

                    }
                    


                }
            }


        }

        private void AddNode(XmlNode inXmlNode, TreeNode inTreeNode)
        {
            XmlNode xNode;
            TreeNode tNode;
            XmlNodeList nodeList;
            int i = 0;
            if (inXmlNode.HasChildNodes)
            {
                nodeList = inXmlNode.ChildNodes;
                for (i = 0; i <= nodeList.Count - 1; i++)
                {
                    xNode = inXmlNode.ChildNodes[i];
                    inTreeNode.Nodes.Add(new TreeNode(xNode.Name));
                    tNode = inTreeNode.Nodes[i];
                    AddNode(xNode, tNode);

                }
            }
            else
            {
                inTreeNode.Text = inXmlNode.InnerText.ToString();
            }
        }
        int okunacaksatir = 1;
        string vergino = "";
        private void verginoyakala(string satir)
        {
            for (int i = 0; i < firmatanimaList.Count; i++)
            //foreach (string satirokuma in firmatanimaList)
            {
                string satirokuma = firmatanimaList[i];
                if (satirokuma != "" || satirokuma != " " || satirokuma != null)
                {
                    int filtrenum = satir.IndexOf(satirokuma);// satırdan vergi no okunuyor.
                    if (filtrenum > 0)
                    {
                        vergino = satirokuma;
                    }
                }
            }
        }
        private void filtrele(string satir,string kolonbilgisi)
        {

        }            
        private void satirokumaverisi(string vergino,string satir,string kolonbilgisi)
        {
            
            //for (int i = 0; i < firmatanimaList.Count; i++)
            //foreach (string satirokuma in firmatanimaList)
            //{
                //string satirokuma = firmatanimaList[i];
                //if (satirokuma != "" || satirokuma != " " || satirokuma != null)
                //{
                  //  int filtrenum = satir.IndexOf(satirokuma);// satırdan vergi no okunuyor.
                   // if (filtrenum > 0)
                    //{
                        // filtre ayarları uygulanarak veri girilir.

                        //foreach (string filtre in  sqlengine(FirmalarDBFile, "SELECT SatirOkuma from Firmalar WHERE vergino=" + Convert.ToInt64(satirokuma) + " and AnahtarSozcuk='" + kolonbilgisi + "'", "SatirOkuma"))
                        for (int j = 0; j < sqlengine(FirmalarDBFile, "SELECT SatirOkuma from Firmalar WHERE vergino=" +Convert.ToInt64( vergino) + " and AnahtarSozcuk='" + kolonbilgisi + "'", "SatirOkuma").Count; j++)
                        {
                            string filtre = sqlengine(FirmalarDBFile, "SELECT SatirOkuma from Firmalar WHERE vergino=" +Convert.ToInt64( vergino) + " and AnahtarSozcuk='" + kolonbilgisi + "'", "SatirOkuma")[j];
                            if (filtre != "" || filtre != " " || filtre != null)
                            {
                                okunacaksatir = (int)Convert.ToInt64(filtre);
                            }
                        }
                    //}
                //}
                //else
                //{

                //}
            //}
        }
            int satirsayisi = 0;
        private void linereader(string dosyaadi = "", int calismamodu = 0) // calismamodu=0-firmabilgisiara | 1-listeyeaktar
        {
            if (calismamodu == 0)
            {
                string fileName = dosyaadi;
                int firma = 0;

                using (StreamReader reader = new StreamReader(fileName))
                {
                    string satir = "";
                    while ((satir = reader.ReadLine()) != null)
                    {
                        string[] kaynak = satir.Split(new char[] { '.', '?', '!', ';',' ', ':', ',',
                                                                  '<', '>', '/', '$', '[', ']', '(', ')',
                                                                 '=','"' }, StringSplitOptions.RemoveEmptyEntries);
                        //satir = kaynak[0];
                        foreach(string cevap in sqlengine(FirmalarDBFile, "", "2"))
                        {
                            firma = satir.IndexOf(cevap);
                        }
                        
                        

                        if (firma > 0) { firmatanima = true; firmatani(satir); }
                        //MessageBox.Show(satir+"Firma karşılaştırması puanı : "+firma.ToString());

                    }



                }


            }

            if (calismamodu == 1)
            {
                string fileName = dosyaadi;
                int firma = 0;
                int kolonsayisi = 0;

                
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string satir = "";
                    string[] lines = File.ReadAllLines(fileName);
                    int toplamsatir = lines.Count();
                    progressBar2.Invoke(new Action(delegate ()
                    {
                        progressBar2.Maximum = 100;
                        progressBar2.Value = 0;
                    }));
                    
                    int satirsay = 0;
                    while ((satir = reader.ReadLine()) != null) // satır= Dosyadaki çekilen ham satır
                    {
                        string[] kaynak = satir.Split(new char[] { '.', '?', '!', ';',' ', ':', ',',//kaynak = satırın yandaki karakterden temizlenmiş hali.
                                                                  '<', '>', '/', '$', '[', ']', '(', ')',
                                                                 '=','"' }, StringSplitOptions.RemoveEmptyEntries);
                        //satir = kaynak[0];
                        // veri al modu kapatılıyor
                        satirsay++;
                        verginoyakala(satir);    
                        foreach (string kolon in Kolonlar)//kolon bilgisi çekilip kolon başına sorgulamaya başlanıyor
                        {
                            if (kolon == "ID" | kolon=="") continue;

                            if (kolonsayisi >= Kolonlar.Count()-1) kolonsayisi = 0;
                            
                            kolonsayisi +=1;
                            string secilensablon = "";
                            listBox1.Invoke(new Action(delegate ()
                            {
                                secilensablon = listBox1.SelectedItem.ToString();
                            }));
                            label6.Invoke(new Action(delegate ()
                            {
                                label6.Text = "Dosya Başına İşlem : " + fileName + " okunuyor.. "+ ((satirsay * 100) / toplamsatir+"%");
                            }));
                            statusStrip1.Invoke(new Action(delegate ()
                            {
                                durumlabel.Text = "Satır okunuyor : " + satir + "...";
                            }));

                            using (SQLiteConnection connect = new SQLiteConnection("Data Source =Sql/Şablonlar/" + secilensablon + "; Version = 3"))// c0nnect= seçilen şablonun veri yolu
                            {
                                string secilensema = secilensablon.Replace(" ", "_");// boşluklar yapılandırılır
                                secilensema = secilensema.Replace(".db", "");// db eki secilen sema isminden kaldırılır
                                connect.Open();// bağlantı açılır.
                                using (SQLiteCommand fmd = connect.CreateCommand())// veritabanı komudu oluşturulur 
                                {
                                    fmd.CommandText = "SELECT * FROM "+secilensema; // veri tabanı komut satırı ile seçilen şema bulunması için komut hazırlanır
                                    fmd.CommandType = CommandType.Text;// komut tipi text olarak seçiliyor
                                    SQLiteDataReader r = fmd.ExecuteReader();// fmd ye yüklenen veritabanı komudu çalıştırılıp geri dönüş dinleniyor
                                    while (r.Read())// r satır satır okunuyor
                                    {
                                        try { if (r[kolon.ToString()] == "FaturaDosyası") continue; } catch { continue; }
                                       
                                      
                                       string kolonbilgisi = Convert.ToString(r[kolon.ToString()]);
                                        if (kolonbilgisi == "") continue;

                                        firma = satir.IndexOf(kolonbilgisi);// firma = xml dosyası satır içeriğinde veri tabanından çekilen ilgili kolonun sıradaki verisini arıyor eğer bulunur ise firma değişkeni pozitif değer alır
                                        //MessageBox.Show("Durum İzleme\n\nkolon: " + kolon.ToString() + " Kolon no: " + kolonsayisi+"\nSatır İçeriği: "+satir+"\nKolon İçeriği: "+kolonbilgisi+"\nEşleşme: "+firma.ToString()) ;
                                        if (firma > 0)// firma değişkeninin değerinin pozitif olduğu sorgulanıyor
                                        {
                                            // kolon sayısı ardışık olarak arttırılıyor
                                            
                                           
                                                if (kolonsayisi < dataGridView1.Columns.Count)//kolon sayısının belirlenen şemala kolonlarına uygunluğu sorgulanıyor
                                                {


                                                /*foreach (string satirokuma in sqlengine(FirmalarDBFile, "SELECT FirmaAdiCekilsin from Firmalar WHERE vergino='" + vergino + "' and AnahtarSozcuk='" + kolonbilgisi + "'", "FirmaAdiCekilsin"))
                                                {

                                                    if (satirokuma != "0" || satirokuma != "" || satirokuma != " ")
                                                    {
                                                        foreach (string firmaadi in sqlengine(FirmalarDBFile, "SELECT firma from Firmalar where vergino='" + vergino + "'", "4"))
                                                        {
                                                            dataGridView1.Rows[satirsayisi - 1].Cells[kolonsayisi].Value = firmaadi; // satıra veri ekleniyor
                                                        }
                                                        continue;// Filtre boş. verileni yaz geç.
                                                    }
                                                    
                                                }*/

                                                if (lines.Length > 0)
                                                    {

                                                    //int okunacaksatir = 1;
                                                        satirokumaverisi(vergino, satir, kolonbilgisi);
                                                        string gonderilensatir = lines[satirsay + okunacaksatir].Replace("<cell>", "");
                                                        if (checkBox1.Checked)
                                                        {
                                                        for(int i = 0; i < firmatanimaList.Count; i++)
                                                        //foreach (string satirokuma in firmatanimaList)
                                                        {
                                                            string satirokuma = firmatanimaList[i];
                                                            
                                                           
                                                        //}


                                                        
                                                        string[] kaynaks=gonderilensatir.Split(new char[] { '?', '!', ';', ':',//kaynak = satırın yandaki karakterden temizlenmiş hali.
                                                                  '<', '>', '/', '$', '[', ']', '(', ')',
                                                                 '=','"' }, StringSplitOptions.RemoveEmptyEntries);
                                              
                                                        gonderilensatir = kaynaks[0];

                                                            //foreach (string satirokuma in firmatanimaList)
                                                            //for(int s=0;)
                                                            //{

                                                            int filtrenumara = 0;

                                                            // filtre ayarları uygulanarak veri girilir.
                                                            foreach (string filtre in sqlengine(FirmalarDBFile, "SELECT Filtre from Firmalar WHERE vergino=" + Convert.ToInt64(satirokuma) + " and AnahtarSozcuk='" + kolonbilgisi + "'", "filtre"))
                                                                
                                                            // int filtresayisi = sqlengine(FirmalarDBFile, "SELECT Filtre from Firmalar WHERE vergino=" + Convert.ToInt64(vergino) + " and AnahtarSozcuk='" + kolonbilgisi + "'", "filtre").Count;
                                                            // for (int k=0; k<filtresayisi;k++)
                                                            {
                                                                
                                                                // string filtre = sqlengine(FirmalarDBFile, "SELECT Filtre from Firmalar WHERE vergino=" + Convert.ToInt64(vergino) + " and AnahtarSozcuk='" + kolonbilgisi + "'", "filtre")[k];
                                                                if (filtre != "" || filtre != " " || filtre != null)
                                                                        {
                                                                            int filtrelifade = gonderilensatir.IndexOf(filtre);
                                                                            if (filtrelifade > 0)
                                                                            {
                                                                                filtrenumara++;
                                                                                gonderilensatir.Replace(" ", "");
                                                                                dataGridView1.Rows[satirsayisi - 1].Cells[kolonsayisi].Value = gonderilensatir;// satıra veri ekleniyor
                                                                                continue;// verilen değeri yaz geç
                                                                            }
                                                                            else continue;// boş geç

                                                                        }
                                                                        else
                                                                        {
                                                                            gonderilensatir.Replace(" ", "");
                                                                            dataGridView1.Rows[satirsayisi - 1].Cells[kolonsayisi].Value = gonderilensatir;// satıra veri ekleniyor
                                                                            continue;// Filtre boş. verileni yaz geç.
                                                                        }

                                                                    }
                                                                    if(filtrenumara < 1)
                                                                    {
                                                                            gonderilensatir.Replace(" ", "");
                                                                            dataGridView1.Rows[satirsayisi - 1].Cells[kolonsayisi].Value = gonderilensatir;// satıra veri ekleniyor
                                                                
                                                                    }

                                                               

                                                            
                                                            

                                                        }
                                                        }
                                                    else
                                                    {
                                                        gonderilensatir.Replace(" ", "");
                                                        dataGridView1.Rows[satirsayisi - 1].Cells[kolonsayisi].Value = gonderilensatir;// satıra veri ekleniyor

                                                    }






                                                    }

                                                    


                                                }

                                                // }
                                                //catch(Exception ex)
                                                /*{
                                                    MessageBox.Show("Hata ile karşılaşıldı.\nToplam Satır sayıları\nKolonlar : " + kolon.Count() + " / " + kolonsayisi.ToString()+"\nSatırlar : "+satirlar.Count()+" / "+satirsayisi.ToString()+"\nYazılmaya çalışılan satır: "+satir+"\nHata ayrıntıları :\n\n"+ex.Message);
                                                }*/


                                           
                                        }
                                    }
                                }
                            }


                            
                        }
                        progressBar2.Invoke(new Action(delegate ()
                        {
                            if (progressBar2.Value < progressBar2.Maximum) progressBar2.Value=((satirsay*100)/toplamsatir);// ilerleme çubuğu değeri arttrılıyor
                        }));
                        




                        //MessageBox.Show(satir+"Firma karşılaştırması puanı : "+firma.ToString());

                    }



                }



            }
        }

        private void listeyiyansit()
        {
            MessageBox.Show("Kolon Sayısı : "+Kolonlar.Count()+" Satır Sayısı : "+satirlar.Count()+" olarak listeye dökülecektir.");
            int kolonsayisi = 0;
            int satirsayisi = 0;
            foreach (string kolon in Kolonlar)
            {
                kolonsayisi++;
                foreach (string satir in satirlar[kolonsayisi])
                {
                    satirsayisi+=1;
                    dataGridView1.Rows[satirsayisi].Cells[kolonsayisi].Value = satir;
                }
            }

        }


        private void faturalarıSistemeAlToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DialogResult sorucevap = MessageBox.Show("Kayıtlı olamayan faturaların firmaları için katır sorulsun istiyor musun ?\n \nKayıt işlemi aynı firmaya bir kere yapılmaktadır. Kayıt edilmeyen firmaların faturalarında tarama sırasında kişiselleştirilmiş ayarlar uygulanamaz.", "Bilgi kaydet", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (sorucevap != DialogResult.Yes)
            { 
                kayitsor = false;
            }
            dosyalariyukle.RunWorkerAsync();
        }

        private void seçerekAlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dosyalariyukle.RunWorkerAsync();

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PDF dosyası | *.pdf";
            ofd.Multiselect = true;
            ofd.FileName = "";
            Loadbar.Invoke(new Action(delegate ()
            {
                Loadbar.Value = 0;
                Loadbar.Style = ProgressBarStyle.Marquee;
            }));
            
            load_panel.Invoke(new Action(delegate ()
            {
                load_panel.Visible = true;
            }));
           
            
            var t = new Thread((ThreadStart)(() =>

            {

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Loadbar.Invoke(new Action(delegate ()
                    {
                        Loadbar.Style = ProgressBarStyle.Blocks;
                        Loadbar.Maximum = ofd.FileNames.Count();
                    }));
                   
                    durumlabel.Text = "Dosyalar okunuyor..";
                    foreach (String file in ofd.FileNames)
                    {
                        xmlcevir(file);
                        Loadbar.Invoke(new Action(delegate ()
                        {
                            Loadbar.Value++;
                        }));
                        
                        durumlabel.Text = "Okunuyor :" + Path.GetFileName(file) + "..";
                        System.Threading.Thread.Sleep(300);
                    }
                    durumlabel.Text = "Seçilen dosyalar sisteme yüklendi. Verileri listeye işlemek için Dosya Menüsü sekmesinden 'Yüklenen dosyaların içeriklerini listeye dönüştür' seçeneğini kullanın. ";
                    MessageBox.Show("Seçtiğin tüm dosyalar sisteme yüklendi.\n  Bildirim çubuğu önerilerini kullanabilirsin.", "Yükleme Başarılı"
                        , MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }));

            t.SetApartmentState(ApartmentState.STA);

            t.Start();

            t.Join();
            load_panel.Invoke(new Action(delegate ()
            {
                load_panel.Visible = false;
            }));
            
            firmatanima = false;
            tabControl1.Invoke(new Action(delegate ()
            {
                this.tabControl1.SelectedIndex = 1;
            }));
            toolStrip1.Invoke(new Action(delegate ()
            {
                yüklenenDosyalarınİçerikleriniListeyeDönüştürToolStripMenuItem.Enabled = true;
            }));
           

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists(FirmaFile))
            {
                if (Directory.Exists("Sql"))
                {
                    SQLiteConnection.CreateFile(FirmaFile);
                }
                else
                {
                    Directory.CreateDirectory("Sql");
                    File.Create(FirmaFile);
                }

            }

            // db test ediliyor..



            //sqlengine(FirmalarDBFile, "CREATE TABLE Firmalar(id INTEGER PRIMARY KEY, firma TEXT, vergino INT, AnahtarSozcuk MEMO");
            sqlengine(FirmalarDBFile, "CREATE TABLE IF NOT EXISTS Firmalar (ID INTEGER PRIMARY KEY AUTOINCREMENT,firma TEXT, vergino INTEGER, AnahtarSozcuk TEXT, SatirOkuma INTEGER, FirmaAdiCekilsin INTEGER)", "0");
            


            sqlengine(FirmalarDBFile, "ALTER TABLE  Firmalar ADD COLUMN SatirOkuma INTEGER", "update");
            sqlengine(FirmalarDBFile, "ALTER TABLE  Firmalar ADD COLUMN FirmaAdiCekilsin INTEGER", "update");
            sqlengine(FirmalarDBFile, "ALTER TABLE  Firmalar ADD COLUMN Filtre TEXT", "update");







        }

        public List<string> sqlengine(string dbyolu, string komut = "", string mod = "1")//0- input,1-firmara-2-firmaara(vkn)-3 veri çek
        {
            List<string> cevap = new List<string>();
            string cs = dbyolu;
            using var con = new SQLiteConnection(cs);

            if (con.State != ConnectionState.Open)
            {
                con.Open();

            }
            try
            {


                if (mod == "update")
                {
                    SQLiteCommand sqlite_cmd;
                    string Createsql = komut;

                    sqlite_cmd = new SQLiteCommand(komut, con);
                    sqlite_cmd.ExecuteNonQuery();

                    con.Close();
                    //cevap.Add("GUNCELLENDI");
                }
            }
            catch (Exception ex)
            {
            }  

            try
            {


               
                if (mod == "0")
                {
                    SQLiteCommand sqlite_cmd;
                    string Createsql = komut;

                    sqlite_cmd = new SQLiteCommand(komut, con);
                    sqlite_cmd.ExecuteNonQuery();

                    con.Close();
                    cevap.Add("basarili");
                }
                if (mod == "SatirOkuma")
                {
                    using (SQLiteCommand fmd = con.CreateCommand())
                    {

                        fmd.CommandText = komut;
                        fmd.CommandType = CommandType.Text;
                        SQLiteDataReader r = fmd.ExecuteReader();
                        while (r.Read())
                        {

#pragma warning disable CS8603 // Olası null başvuru dönüşü.
                            cevap.Add(r["SatirOkuma"].ToString());
#pragma warning restore CS8603 // Olası null başvuru dönüşü.
                        }
                    }
                }
                if (mod == "FirmaAdiCekilsin")
                {
                    using (SQLiteCommand fmd = con.CreateCommand())
                    {

                        fmd.CommandText = komut;
                        fmd.CommandType = CommandType.Text;
                        SQLiteDataReader r = fmd.ExecuteReader();
                        while (r.Read())
                        {

#pragma warning disable CS8603 // Olası null başvuru dönüşü.
                            cevap.Add(r["FirmaAdiCekilsin"].ToString());
#pragma warning restore CS8603 // Olası null başvuru dönüşü.
                        }
                    }
                }
                if (mod == "Filtre")
                {
                    using (SQLiteCommand fmd = con.CreateCommand())
                    {

                        fmd.CommandText = komut;
                        fmd.CommandType = CommandType.Text;
                        SQLiteDataReader r = fmd.ExecuteReader();
                        while (r.Read())
                        {

#pragma warning disable CS8603 // Olası null başvuru dönüşü.
                            cevap.Add(r["Filtre"].ToString());
#pragma warning restore CS8603 // Olası null başvuru dönüşü.
                        }
                    }
                }

                if (mod == "1")
                {
                    using (SQLiteCommand fmd = con.CreateCommand())
                    {

                        fmd.CommandText = "SELECT * FROM Firmalar";
                        fmd.CommandType = CommandType.Text;
                        SQLiteDataReader r = fmd.ExecuteReader();
                        while (r.Read())
                        {

#pragma warning disable CS8603 // Olası null başvuru dönüşü.
                            cevap.Add(r["firma"].ToString());
#pragma warning restore CS8603 // Olası null başvuru dönüşü.
                        }
                    }
                }
                if (mod == "2")
                {
                    /*using (SQLiteCommand fmd = con.CreateCommand())
                    {

                            fmd.CommandText = "SELECT * FROM Firmalar";
                            fmd.CommandType = CommandType.Text;
                            SQLiteDataReader r = fmd.ExecuteReader();
                            while (r.Read())
                            {

    #pragma warning disable CS8603 // Olası null başvuru dönüşü.
                                cevap= r["vergino"].ToString();
    #pragma warning restore CS8603 // Olası null başvuru dönüşü.
                            }
                    }
                    */
                    string sql = "select * from Firmalar";
                    SQLiteCommand command = new SQLiteCommand(sql, con);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        cevap.Add(reader["vergino"].ToString());
                    }


                }
                if (mod == "3")
                {
                    using (SQLiteCommand fmd = con.CreateCommand())
                    {

                        fmd.CommandText = komut;
                        fmd.CommandType = CommandType.Text;
                        SQLiteDataReader r = fmd.ExecuteReader();
                        while (r.Read())
                        {

#pragma warning disable CS8603 // Olası null başvuru dönüşü.
                            cevap.Add(r["AnahtarSozcuk"].ToString());

#pragma warning restore CS8603 // Olası null başvuru dönüşü.
                        }
                    }
                }
                if (mod == "4")
                {
                    using (SQLiteCommand fmd = con.CreateCommand())
                    {

                        fmd.CommandText = komut;
                        fmd.CommandType = CommandType.Text;
                        SQLiteDataReader r = fmd.ExecuteReader();
                        while (r.Read())
                        {

#pragma warning disable CS8603 // Olası null başvuru dönüşü.
                            cevap.Add(r["firma"].ToString());

#pragma warning restore CS8603 // Olası null başvuru dönüşü.
                        }
                    }
                }
                con.Close();

            }
            catch (Exception ex)
            {
                cevap.Add(MessageBox.Show("Veritabanı görev hatası... " + ex.Message).ToString());
                return cevap;

            }
#pragma warning disable CS8603 // Olası null başvuru dönüşü.
            return cevap;
#pragma warning restore CS8603 // Olası null başvuru dönüşü.


        }
        public DataTable dt = new DataTable();
        public SQLiteDataAdapter da = new SQLiteDataAdapter();
        private void kayıtlıFirmalarToolStripMenuItem_Click(object sender, EventArgs e)
        {


            
            using var con = new SQLiteConnection(FirmalarDBFile);
            
            con.Open();
            da = new SQLiteDataAdapter("SELECT * FROM Firmalar", con);
            DataSet ds = new DataSet();
            da.Fill(ds, "Firmalar");

            SQLiteCommandBuilder builder = new SQLiteCommandBuilder(da);

            con.Close();
            SQLiteCommand delete = new SQLiteCommand("DELETE FROM Firmalar WHERE ID = @ID", con);
            SQLiteCommand insert = new SQLiteCommand("INSERT INTO Firmalar (firma, vergino, AnahtarSozcuk,Filtre) VALUES (@firma, @vergino, @AnahtarSozcuk,@Filtre)", con);
            SQLiteCommand update = new SQLiteCommand("UPDATE Firmalar SET firma = @firma, vergino = @vergino, AnahtarSozcuk = @AnahtarSozcuk, Filtre=@Filtre WHERE ID = @ID", con);

            delete.Parameters.Add("@ID", (DbType)SqlDbType.Int, 4, "ID");

            insert.Parameters.Add("@firma", (DbType)SqlDbType.Text, 100, "firma");
            insert.Parameters.Add("@vergino", (DbType)SqlDbType.Int, 8, "vergino");
            insert.Parameters.Add("@AnahtarSozcuk", (DbType)SqlDbType.Text, 10, "AnahtarSozcuk");
            insert.Parameters.Add("@Filtre", (DbType)SqlDbType.Text, 10, "Filtre");

            update.Parameters.Add("@firma", (DbType)SqlDbType.Text, 100, "firma");
            update.Parameters.Add("@vergino", (DbType)SqlDbType.Int, 8, "vergino");
            update.Parameters.Add("@AnahtarSozcuk", (DbType)SqlDbType.Text, 10, "AnahtarSozcuk");
            update.Parameters.Add("@Filtre", (DbType)SqlDbType.Text, 10, "Filtre");
            update.Parameters.Add("@ID", (DbType)SqlDbType.Int, 4, "ID");

            da.DeleteCommand = delete;
            da.InsertCommand = insert;
            da.UpdateCommand = update;
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            dt= ds.Tables["Firmalar"];
            firmalistesi firmalistesiform = new firmalistesi();
            firmalistesiform.dataGridView1.DataSource = dt;
            firmalistesiform.con = con;
            firmalistesiform.da = da;
            firmalistesiform.dt = dt;
            firmalistesiform.ds = ds;
            firmalistesiform.Show();



        }

        private void listeŞemalarıToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Liste_şemaları listesemaform = new Liste_şemaları();
            if (Directory.Exists("Sql/Şablonlar"))
            {
                DirectoryInfo di = new DirectoryInfo("Sql/Şablonlar/");
                FileInfo[] files = di.GetFiles("*.db");
                listesemaform.listBox1.Items.Clear();
                foreach (FileInfo fi in files)
                {
                    listesemaform.listBox1.Items.Add(fi.Name);
                }
            }
            listesemaform.Show();
        }

        private void yeniOluşturToolStripMenuItem_Click(object sender, EventArgs e)
        {
            yenisema yenisema = new yenisema();
            yenisema.Show();
        }

        private void yüklenenDosyalarınİçerikleriniListeyeDönüştürToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool verivar = false;
            if (Directory.Exists("Sql/Şablonlar"))
            {
                DirectoryInfo di = new DirectoryInfo("Sql/Şablonlar/");
                FileInfo[] files = di.GetFiles("*.db");

                foreach (FileInfo fi in files)
                {
                    verivar = true;
                }


            }

            if (verivar == false)
            {
                MessageBox.Show("Hazırlanmış bir liste şeması bulunmuyor. Liste şeması hazırlamak için yönlendirileceksin.", "Liste hazırla", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                yenisema yenisema = new yenisema();
                yenisema.Show();
            }
            this.tabControl1.SelectedIndex = 0;
            if (Directory.Exists("Sql/Şablonlar"))
            {
                DirectoryInfo di = new DirectoryInfo("Sql/Şablonlar/");
                FileInfo[] files = di.GetFiles("*.db");
                listBox1.Items.Clear();
                foreach (FileInfo fi in files)
                {
                    listBox1.Items.Add(fi.Name);
                }
            }
            panel_şemasec.Visible = true;


        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel_şemasec.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count < 1)
            {
                MessageBox.Show("Listeden Şema Seçilmelidir.", "Liste Oluştur");
                return;
            }
            progressBar1.Maximum = treeView1.Nodes.Count;
            progressBar1.Value = 0;
            progressBar2.Value = 0;
            panel_listeolustur.Visible = true;
            listeyap.RunWorkerAsync();
        }


        private void listeolustur()
        {
            string secilensema = listBox1.SelectedItem.ToString();
           
            DataSet ds = new DataSet();
            using var con = new SQLiteConnection("Data Source =Sql/Şablonlar/" + secilensema + "; Version = 3");
            {
                secilensema = secilensema.Replace(" ", "_");
                secilensema = secilensema.Replace(".db", "");
                con.Open();
                using (SQLiteCommand fmd = con.CreateCommand())
                {
                    fmd.CommandText = "SELECT name FROM pragma_table_info('" + secilensema + "') ORDER BY cid ";
                    fmd.CommandType = CommandType.Text;
                    SQLiteDataReader r = fmd.ExecuteReader();
                    while (r.Read())
                    {
                        dataGridView1.Columns.Add(Convert.ToString(r["name"]), Convert.ToString(r["name"]));
                    }
                }

            }

            

            //dataGridView1.DataSource = aktarilacakliste;


            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                Kolonlar.Add(dataGridView1.Columns[i].HeaderText);
            }

            con.Close();
            satirsayisi = 0;
            foreach (TreeNode dosya in treeView1.Nodes)
            {
                satirsayisi++;

                DataGridViewRow row=new DataGridViewRow();// yeni satır açılıyor
                dataGridView1.Rows.Add(row);
                linereader("xmldosyalari/" + dosya.Text,1);
                progressBar1.Value++;
                
            }
            //listeyiyansit(); // oluşturulan veriler listeye ekleniyor.
            panel_listeolustur.Visible = false;
            panel_şemasec.Visible = false;
            MessageBox.Show("Liste oluşturuldu.\nListeyi dışarı aktarmak için dosya menüsünü kullanın.","Liste Oluştur.");
            listeyiDışarıAktarToolStripMenuItem.Enabled = true;
            durumlabel.Text = "Dosya menüsü>Listeyi Dışarı aktar seçeneğini kullanarak listeyi dışarı aktarabilirsiniz.";

        }

        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            string secilensema = "";
            listBox1.Invoke(new Action(delegate ()
            {
                secilensema = listBox1.SelectedItem.ToString();
            }));
            

            DataSet ds = new DataSet();
            using var con = new SQLiteConnection("Data Source =Sql/Şablonlar/" + secilensema + "; Version = 3");
            {
                secilensema = secilensema.Replace(" ", "_");
                secilensema = secilensema.Replace(".db", "");
                con.Open();
                using (SQLiteCommand fmd = con.CreateCommand())
                {
                    fmd.CommandText = "SELECT name FROM pragma_table_info('" + secilensema + "') ORDER BY cid ";
                    fmd.CommandType = CommandType.Text;
                    SQLiteDataReader r = fmd.ExecuteReader();
                    while (r.Read())
                    {
                        dataGridView1.Invoke(new Action(delegate ()
                        {
                            dataGridView1.Columns.Add(Convert.ToString(r["name"]), Convert.ToString(r["name"]));
                        }));
                        
                    }
                }

            }
            dataGridView1.Invoke(new Action(delegate ()
            {
                dataGridView1.Columns.Add("Fatura Dosyası", "FaturaDosyasi");
            }));



            //dataGridView1.DataSource = aktarilacakliste;


            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                Kolonlar.Add(dataGridView1.Columns[i].HeaderText);
            }

            con.Close();
            satirsayisi = 0;
            foreach (TreeNode dosya in treeView1.Nodes)
            {
                satirsayisi++;

                DataGridViewRow row = new DataGridViewRow();// yeni satır açılıyor
                dataGridView1.Invoke(new Action(delegate ()
                {
                    dataGridView1.Rows.Add(row);
                    dataGridView1.Rows[satirsayisi - 1].Cells[dataGridView1.Columns.Count-1].Value = dosya.Text;
                }));
               
                linereader("xmldosyalari/" + dosya.Text, 1);
               // Thread thread = new Thread(() => linereader("xmldosyalari/" + dosya.Text, 1));
                //thread.Start();




                progressBar1.Invoke(new Action(delegate ()
                {
                    progressBar1.Value++;
                    yuzdelabel.Text = "%" + ((progressBar1.Value * 100) / progressBar1.Maximum).ToString();
                }));
                yuzdelabel.Invoke(new Action(delegate ()
                {    
                    yuzdelabel.Text = "%" + ((progressBar1.Value * 100) / progressBar1.Maximum).ToString();
                }));

            }
            //listeyiyansit(); // oluşturulan veriler listeye ekleniyor.
            panel_listeolustur.Invoke(new Action(delegate ()
            {
                panel_listeolustur.Visible = false;
            }));
            panel_şemasec.Invoke(new Action(delegate ()
            {
                panel_şemasec.Visible = false;
            }));

            firmatanimaList.Clear();
            MessageBox.Show("Liste oluşturuldu.\nListeyi dışarı aktarmak için dosya menüsünü kullanın.", "Liste Oluştur.");
            toolStrip1.Invoke(new Action(delegate ()
            {
                listeyiDışarıAktarToolStripMenuItem.Enabled = true;
            }));
            statusStrip1.Invoke(new Action(delegate ()
            {
                durumlabel.Text = "Dosya menüsü>Listeyi Dışarı aktar seçeneğini kullanarak listeyi dışarı aktarabilirsiniz.";
            }));
            

        }

        private void listeyiDışarıAktarToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void excelDosyasıOlarakAktarToolStripMenuItem_Click(object sender, EventArgs e)
        {
             SaveFileDialog save = new SaveFileDialog();

             save.Filter = "Excel Dosyası |*.xls";
             save.OverwritePrompt = true;
             save.CreatePrompt = true;
             save.FileName = "Fatura Listesi";

             if (save.ShowDialog() == DialogResult.OK)
             {
                 exceldosyasiolustur(save.FileName);
             }
            
     
        }
        private void copyAlltoClipboard()
        {
            dataGridView1.SelectAll();
            DataObject dataObj = dataGridView1.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }
        private void exceldosyasiolustur(string kayitdosyasi="")
        {
            try
            {
                Excel.Application xlApp;
                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;

                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                int i = 0;
                int j = 0;
                int StartCol = 1;
                int StartRow = 1;

                for (j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    Excel.Range myRange = (Excel.Range)xlWorkSheet.Cells[StartRow, StartCol + j];
                    myRange.Value2 = dataGridView1.Columns[j].HeaderText;
                }
                StartRow++;
                for (i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    for (j = 0; j < dataGridView1.Columns.Count; j++)
                    {

                        Excel.Range myRange = (Excel.Range)xlWorkSheet.Cells[StartRow + i, StartCol + j];
                        myRange.Value2 = dataGridView1[j, i].Value == null ? "" : dataGridView1[j, i].Value;
                        myRange.Select();
                    }
                }

                xlWorkBook.SaveAs(kayitdosyasi, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);

               
                MessageBox.Show("Excel dosyasına raporlama başarı ile tamamlandı.", "Excele Aktar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Excel dosyasına raporlama işlemi tamamlanamadı..\n|nHata Ayrıntıları:\n"+ex.Message, "Excele Aktar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Nesne bırakılırken istisna meydana geldi. " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        private void pdfiaktar_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                DialogResult sorucevap = MessageBox.Show(treeView1.SelectedNode.Text + " adlı pdf'i işlem sırasından kaldırmak istiyor musun?\n\nBu işlem seçilen pdf dosyasını işlem listesinden kaldıracak ve pdf dosyası oluşturulacak listlerde yer almayacaktır. Fakat dosya içeriklerinde göreceksin.", "Seçili Pdfi kaldır.", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (sorucevap == DialogResult.Yes)
                {
                    treeView1.Nodes.Remove(treeView1.SelectedNode);
                    MessageBox.Show("Seçilen pdf dosyası kaldırıldı.\n\nArtık işlem listesinde olmadığından dolayı tekrardan eklenmediği sürece listeleme işleminde bulunmayacak.", "İşlem listesinden kaldır.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void klasörüAlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog Klasor = new FolderBrowserDialog();
            if (Klasor.ShowDialog() == DialogResult.OK)
            {
              
            }
            
        }

        private void panel_listeolustur_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}