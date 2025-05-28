using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public class Kullanici
        {
            public double Boy { get; set; } // metre cinsinden
            public double Kilo { get; set; } // kg cinsinden

            public Kullanici(double boy, double kilo)
            {
                Boy = boy;
                Kilo = kilo;
            }

            public virtual double BKIHesapla()
            {
                return Kilo / (Boy * Boy);
            }
        }

        public class SaglikliKullanici : Kullanici
        {
            public SaglikliKullanici(double boy, double kilo) : base(boy, kilo) { }

            public string DurumBelirle()
            {
                double bki = BKIHesapla();

                if (bki < 18.5)
                    return "Zayıf - Daha fazla beslenmeye dikkat et.";
                else if (bki >= 18.5 && bki < 25)
                    return "Normal - Formunu koru!";
                else if (bki >= 25 && bki < 30)
                    return "Fazla kilolu - Egzersiz yapmaya başlamalısın.";
                else
                    return "Obez - Sağlık uzmanına danış.";
            }
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            {
                // TextBox ve ComboBox verilerini al
                string isim = textBox1.Text.Trim();
                string yas = textBox2.Text.Trim();
                string sehir = textBox3.Text.Trim();
                string boy = textBox4.Text.Trim();
                string kilo = textBox5.Text.Trim();
                string cinsiyet = comboBox1.SelectedItem?.ToString();

                // Bağlantı dizesi (veritabanı adı "KullaniciLar" olarak düzeltildi)
                string constring = "Server=DESKTOP-NOHTLAO\\SQLEXPRESS;Database=KullaniciLar;Trusted_Connection=True;";

                using (SqlConnection conn = new SqlConnection(constring))
                {
                    try
                    {
                        conn.Open();
                        string query = "INSERT INTO kullanicilar1 (Isim, Yas, Sehir, Boy, Kilo, Cinsiyet) VALUES (@Isim, @Yas, @Sehir, @Boy, @Kilo, @Cinsiyet)";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Isim", isim);
                        cmd.Parameters.AddWithValue("@Yas", yas);
                        cmd.Parameters.AddWithValue("@Sehir", sehir);
                        cmd.Parameters.AddWithValue("@Boy", boy);
                        cmd.Parameters.AddWithValue("@Kilo", kilo);
                        cmd.Parameters.AddWithValue("@Cinsiyet", cinsiyet);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Kullanıcı başarıyla eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata: " + ex.Message);
                    }
                }
            }
        }

        
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel10.Visible = false;
            string constring = "Server=DESKTOP-NOHTLAO\\SQLEXPRESS;Database=KullaniciLar;Trusted_Connection=True;";

            using (SqlConnection conn = new SqlConnection(constring))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT Isim FROM kullanicilar1";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        comboBox2.Items.Add(reader["Isim"].ToString());
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex != -1)
            {
                panel10.Visible = true;
            }
            else
            {
                panel10.Visible = false;
            }
            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen bir kullanıcı seçin.");
                return;
            }

            string secilenKullanici = comboBox2.SelectedItem.ToString();

            string constring = "Server=DESKTOP-NOHTLAO\\SQLEXPRESS;Database=KullaniciLar;Trusted_Connection=True;";

            using (SqlConnection conn = new SqlConnection(constring))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT Boy, Kilo FROM kullanicilar1 WHERE Isim = @Isim";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Isim", secilenKullanici);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Boy verisini al ve metreye çevir (cm ise)
                        double boyRaw = Convert.ToDouble(reader["Boy"]);
                        double boy = boyRaw;

                        if (boy > 10) // Eğer büyükse (örneğin 175 cm), metreye çevir
                        {
                            boy = boyRaw / 100.0;
                        }

                        double kilo = Convert.ToDouble(reader["Kilo"]);

                        if (boy <= 0)
                        {
                            MessageBox.Show("Geçersiz boy değeri.");
                            return;
                        }

                        SaglikliKullanici kullanici = new SaglikliKullanici(boy, kilo);
                        double bki = kullanici.BKIHesapla();
                        string yorum = kullanici.DurumBelirle();

                        label12.Text = $"BKI: {bki:F2}\nDurum: {yorum}";
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı verisi bulunamadı.");
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }
        }
    }

        
    


