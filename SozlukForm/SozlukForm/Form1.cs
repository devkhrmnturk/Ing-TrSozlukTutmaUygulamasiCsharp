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

namespace SozlukForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+ Application.StartupPath+"/Ceviri.accdb";
        OleDbConnection baglanti = new OleDbConnection(connectionString);
        DataSet dataSet = new DataSet();

        string kayitSay(DataSet dataSet)
        {
            int kayitSayisi= dataSet.Tables["ingilizceTurkce"].Rows.Count;
            if (kayitSayisi > 0)
                return "Kayıt Sayısı: " + kayitSayisi;
            else
                return "Kayıt Bulunamadı.";
        }

        void render(string sorgu="select * from ingilizceTurkce ORDER BY ingilizce ASC")
        {
            dataSet.Clear();
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(sorgu,baglanti);
            dataAdapter.Fill(dataSet,"ingilizceTurkce");
            listBox1.DataSource = dataSet.Tables["ingilizceTurkce"];
            listBox2.DataSource = dataSet.Tables["ingilizceTurkce"];
            listBox1.DisplayMember = "ingilizce";
            listBox2.DisplayMember = "turkce";
            label3.Text = kayitSay(dataSet);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            render();
            //MessageBox.Show(Application.StartupPath);

        }
        string AramaSorgu;
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked)
                AramaSorgu = "select * from ingilizceTurkce where ingilizce like '" + textBox1.Text + "%'";
            else
                AramaSorgu = "select * from ingilizceTurkce where turkce like '" + textBox1.Text + "%'";

            render(AramaSorgu);

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                listBox2.ForeColor = Color.Black;
                AramaSorgu = "select * from ingilizceTurkce where ingilizce like '" + textBox1.Text + "%'";
                listBox1.ForeColor = Color.Red;
            }

            else
            {
                listBox1.ForeColor = Color.Black;
                AramaSorgu = "select * from ingilizceTurkce where turkce like '" + textBox1.Text + "%'";
                listBox2.ForeColor = Color.Red;
            }
                

            render(AramaSorgu);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 frm2 = new Form2();
            frm2.ShowDialog();
            if(frm2.DialogResult==DialogResult.OK)
            {
                render();
            }
        }
    }
}
