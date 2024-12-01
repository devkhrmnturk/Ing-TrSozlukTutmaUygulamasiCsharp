using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SozlukForm
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }


        static string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "/Ceviri.accdb";
        OleDbConnection baglanti = new OleDbConnection(connectionString);
        DataSet dataSet = new DataSet();

        string kayitSay(DataSet dataSet)
        {
            int kayitSayisi = dataSet.Tables["ingilizceTurkce"].Rows.Count;
            if (kayitSayisi > 0)
                return "Kayıt Sayısı: " + kayitSayisi;
            else
                return "Kayıt Bulunamadı..";
        }

        void render(string sorgu="select * from ingilizceTurkce ORDER BY ingilizce ASC")
        {
            dataSet.Clear();
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(sorgu, baglanti);
            dataAdapter.Fill(dataSet, "ingilizceTurkce");
            dataGridView1.DataSource = dataSet.Tables["ingilizceTurkce"];
            label4.Text = kayitSay(dataSet);
        }
        string ingsecilen;
        string turksecilen;

        void seciliSatirRender()
        {
            int seciliSatir = dataGridView1.CurrentRow.Index;
            ingsecilen = dataGridView1.Rows[seciliSatir].Cells[0].Value.ToString();
            turksecilen = dataGridView1.Rows[seciliSatir].Cells[1].Value.ToString();
            secilenIng.Text = ingsecilen;
            secilenTurk.Text = turksecilen;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            araTxt.Text = "İngilizceye göre ara!";
            araTxt.ForeColor = Color.Gray;
            render();
            dataGridView1.Rows[0].Selected = true;
            seciliSatirRender();
            secilenIng.AutoEllipsis = true;
            secilenTurk.AutoEllipsis = true;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {

            if (dataGridView1.CurrentRow != null)
            {
                seciliSatirRender();
            }

        }

        Boolean karakterKontrol(string text)
        {
            string istenmeyen = "İÜÖĞŞÇıüöğşç";
            int i = 0;
            while (i < text.Length)
            {
                int j = 0;
                while (j < istenmeyen.Length)
                {
                    if (text[i].Equals(istenmeyen[j]))
                    {
                        MessageBox.Show("istenmeyen karakter bulundu: " + istenmeyen[j]);
                        return false;
                    }
                        
                    j++;
                }
                i++;
            }
            return true;
        }

        Boolean kayitVarmi(string ingtext,string turktext)
        {
            string sorgu = $"select count(*) from ingilizceTurkce where ingilizce='{ingtext}' and turkce='{turktext}'";
            OleDbCommand komut = new OleDbCommand(sorgu, baglanti);
            if(baglanti.State == ConnectionState.Closed) baglanti.Open();
            int i = (int)komut.ExecuteScalar();
            baglanti.Close();
            if (i > 0)
                return true;
            else
                return false;
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            string ingilizcetext = ingtxt.Text;
            string turkcetext = turktxt.Text;

            if (karakterKontrol(ingilizcetext))
            {
                //işlemler
                ingilizcetext = ingilizcetext.ToLower();
                turkcetext = turkcetext.ToLower();

                if (kayitVarmi(ingilizcetext, turkcetext))
                    MessageBox.Show("Kayıt zaten mevcut!");
                else
                {
                    string sorgu = "insert into ingilizceTurkce(ingilizce,turkce) values('"+ingilizcetext+"','"+turkcetext+"')";
                    OleDbCommand komut = new OleDbCommand(sorgu, baglanti);
                    if (baglanti.State == ConnectionState.Closed) baglanti.Open();
                    komut.ExecuteNonQuery();
                    baglanti.Close();
                    MessageBox.Show("kayıt eklendi..");
                    render();
                }
            }
            else
            {
                MessageBox.Show("ingilizce alanına turkce karakter girmeyiniz!");
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            string sorgu = "delete from ingilizceTurkce where ingilizce='"+ingsecilen+ "' and turkce='"+turksecilen+"'"; 
            OleDbCommand komut = new OleDbCommand(sorgu, baglanti);
            if (baglanti.State == ConnectionState.Closed) baglanti.Open();
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("kayıt silindi..");
            render();
        }

        private void btnGuncel_Click(object sender, EventArgs e)
        {
            string ingilizcetext = ingtxt.Text;
            string turkcetext = turktxt.Text;

            if (ingilizcetext.Equals("") || turkcetext.Equals(""))
                MessageBox.Show("Seçili kaydı güncellemek için ingilizce ve turkce satırlarını doldurun.");
            else
            {
                if (karakterKontrol(ingilizcetext))
                {
                    ingilizcetext = ingilizcetext.ToLower();
                    turkcetext = turkcetext.ToLower();
                    string sorgu = "update ingilizceTurkce set " +
                        "ingilizce='"+ingilizcetext+"',turkce='"+turkcetext+"'" +
                        "where ingilizce='"+ingsecilen+"' and turkce='"+turksecilen+"'";
                    OleDbCommand komut = new OleDbCommand(sorgu, baglanti);
                    if (baglanti.State == ConnectionState.Closed) baglanti.Open();
                    komut.ExecuteNonQuery();
                    baglanti.Close();
                    MessageBox.Show("kayıt güncellendi..");
                    render();
                }
                else
                {
                    MessageBox.Show("ingilizce alanına turkce karakter girmeyiniz!");
                }
            }

        }

        private void btnIptal_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void araTxt_TextChanged(object sender, EventArgs e)
        {
            string aramaSorgu = "select * from ingilizceTurkce where ingilizce like '" + araTxt.Text + "%'";
            render(aramaSorgu);
        }

        private void araTxt_Click(object sender, EventArgs e)
        {
            araTxt.Text = "";
            araTxt.ForeColor = Color.Black;
        }
    }
}
