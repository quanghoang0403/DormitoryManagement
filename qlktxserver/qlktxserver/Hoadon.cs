﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text;
using System.Configuration;

namespace qlktxserver
{
    public partial class Hoadon : UserControl
    {

        private int ID =-1;
        private string button;
        public Hoadon()
        {
            InitializeComponent();
            button3.Visible = false;
            Unable();
        }
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["QuanLyKTX"].ConnectionString);
        string UID = frmDangNhap.ID_User;
        private string id_per()
        {
            string id = "";
           // SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["QuanLyKTX"].ConnectionString);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_per_relationship WHERE id_user_rel = '" + UID + "' ", conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["suspended"].ToString() == "False")
                        {
                            id = dr["id_per_rel"].ToString();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }

            return id;
        }

        private List<string> List_per()
        {
            string idper = id_per();
            List<string> termlist = new List<string>();
            //SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["QuanLyKTX"].ConnectionString);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_permision_detail WHERE id_per = '" + idper + "' ", conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        termlist.Add(dr["code_action"].ToString());

                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Có lỗi xảy ra ", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            }
            finally
            {
                conn.Close();
            }


            return termlist;
        }

        List<string> listper = null;



        private void Hoadon_Load(object sender, EventArgs e)
        {
            listper = List_per();
            Hienthi();
            string query = string.Format("SELECT * FROM HOADON");
            dataGridView1.DataSource = DataProvider.Instance.ExecuteQuery(query);
        }
        private Boolean checkper(string code)
        {
            Boolean check = false;
            foreach (string item in listper)
            {
                if (item == code)
                {
                    check = true;
                    break;
                }
                else
                {
                    check = false;
                }
            }
            return check;
        }

        void Hienthi()
        {
            List<Phong> phongList =  PhongDAO.Instance.LoadPhongList();

            foreach (Phong item in phongList)
            {
                Button btn = new Button()
                {
                    Width = PhongDAO.PhongWidth,
                    Height = PhongDAO.PhongHeight
                };
                btn.Text = item.ID + Environment.NewLine + item.Status;
                btn.Click += btn_Click;
                btn.Tag = item;
                switch (item.Status)
                {
                    case "Dang su dung":
                        btn.BackColor = Color.Aqua;
                        break;
                    case "Trong":
                        btn.BackColor = Color.Yellow;
                        break;
                    default:
                        btn.BackColor = Color.Silver;
                        break;
                }
                
                flowLayoutPanel1.Controls.Add(btn);
            }

            if (checkper("ADD") == false)
            {
                //MessageBox.Show("Bạn có quyền thêm phòng ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Unable();
                button4.BackColor = Color.Silver;
            }

            if (checkper("EDIT") == false)
            {
                //MessageBox.Show("Bạn có quyền thêm phòng ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Unable();
                button2.BackColor = Color.Silver;
            }
        }

        void show_Hoadon(int maphong)
        {
            string query = string.Format("SELECT * FROM HOADON WHERE MAPHG= {0}", maphong);
            dataGridView1.DataSource = DataProvider.Instance.ExecuteQuery(query);
        }

        void btn_Click(object sender, EventArgs e)
        {
            int phongID = ((sender as Button).Tag as Phong).ID;
            show_Hoadon(phongID);
            ClearData();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (checkper("ADD") == true)
            {
                //MessageBox.Show("Bạn có quyền thêm hóa đơn ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearData();
                Enable();
                button1.BringToFront();
                button = "add";
                button2.Enabled = false;
                dataGridView1.Enabled = false;
                button3.Visible = true;
            }
            else
            {
                MessageBox.Show("Bạn không có quyền thêm hóa đơn ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Unable();
            }

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Unable();
            if(e.RowIndex>=0)
            {
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
                textBox1.Text = row.Cells["MAPHG"].Value.ToString();
                textBox2.Text = row.Cells["CHISONUOC"].Value.ToString();
                textBox5.Text = row.Cells["CHISODIEN"].Value.ToString();
                comboBox4.Text =row.Cells["GIANUOC"].Value.ToString();
                comboBox5.Text = row.Cells["GIADIEN"].Value.ToString();
                ID = Convert.ToInt32(row.Cells["MAHD"].Value.ToString());
                comboBox1.Text = row.Cells["THANG"].Value.ToString();
                comboBox2.Text = row.Cells["TINHTRANGHD"].Value.ToString();
                comboBox3.Text = row.Cells["NAM"].Value.ToString();
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
               
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (checkper("EDIT") == true)
            {
                //MessageBox.Show("Bạn có quyền sửa hóa đơn ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (ID == -1)
                {
                    MessageBox.Show("Vui lòng chọn hóa đơn để sửa");
                }
                else
                {
                    dataGridView1.Enabled = false;
                    Enable();
                    button = "edit";
                    button1.BringToFront();
                    button3.Visible = true;
                    button2.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("Bạn không có quyền sửa hóa đơn ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Unable();
            }

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string oldcsNuoc;
            string oldcsDien;
            if (textBox2.Text == "" || textBox5.Text == "" || comboBox4.Text == "" || comboBox5.Text == "" || comboBox1.Text == "" || comboBox3.Text == "" || comboBox3.Text == "" || textBox1.Text == "")
                MessageBox.Show("Vui lòng điền đầy đủ thông tin");
            else
            {
                int thangmoi = Convert.ToInt32(comboBox1.Text);
                int thangcu = thangmoi - 1;
                int yd = Convert.ToInt32(comboBox3.Text);

                if (thangmoi == 1)
                {
                    thangcu = 12;
                    yd = yd - 1;
                }
                SqlCommand cmddd = new SqlCommand("SELECT * FROM HOADON WHERE MAPHG = '" + textBox1.Text + "' AND THANG = '" + thangcu + "' AND NAM = '" + yd + "'", conn);
                SqlDataAdapter da = new SqlDataAdapter(cmddd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                int csNuoc = Int32.Parse(textBox2.Text);
                int csDien = Int32.Parse(textBox5.Text);
                int giaNuoc = Int32.Parse(comboBox4.Text);
                int giaDien = Int32.Parse(comboBox5.Text);
                int tongtien = csNuoc * giaNuoc + csDien * giaDien;
                int idphong = Int32.Parse(textBox1.Text);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        oldcsNuoc = dr["CHISONUOC"].ToString();
                        oldcsDien = dr["CHISODIEN"].ToString();
                        int csNuocc = Convert.ToInt32(oldcsNuoc);
                        csNuoc = Int32.Parse(textBox2.Text) - csNuocc;
                        int csDienn = Convert.ToInt32(oldcsDien);
                        csDien = Int32.Parse(textBox5.Text) - csDienn;
                        giaNuoc = Int32.Parse(comboBox4.Text);
                        giaDien = Int32.Parse(comboBox5.Text);
                        tongtien = csNuoc * giaNuoc + csDien * giaDien;
                        idphong = Int32.Parse(textBox1.Text);
                    }
                }
                if (tongtien < 0)
                {
                    MessageBox.Show("Nhập lại chỉ số điện, nước ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    switch (button)
                    {
                        case "add":

                            string query = "INSERT into HOADON VALUES ('" + textBox2.Text + "', '" + textBox5.Text + "', '" + comboBox1.Text + "', '" + comboBox3.Text + "', '" + tongtien + "', '" + textBox1.Text + "', '" + comboBox2.Text + "', '" + comboBox4.Text + "', '" + comboBox5.Text + "')";
                            dataGridView1.DataSource = DataProvider.Instance.ExecuteQuery(query);
                            MessageBox.Show("Thêm thành công");

                            show_Hoadon(idphong);
                            Unable();
                            button2.Enabled = true;
                            button3.Visible = false;
                            break;
                        case "edit":

                            query = "UPDATE HOADON SET CHISONUOC=" + textBox2.Text + ", CHISODIEN=" + textBox5.Text + " , THANG=" + comboBox1.Text + ", NAM=" + comboBox3.Text + " , TONGTIEN=" + tongtien + " , MAPHG=" + textBox1.Text + ", TINHTRANGHD='" + comboBox2.Text + "', GIANUOC=" + comboBox4.Text + ", GIADIEN=" + comboBox5.Text + " WHERE MAHD=" + ID;
                            dataGridView1.DataSource = DataProvider.Instance.ExecuteQuery(query);
                            dataGridView1.Enabled = true;
                            show_Hoadon(idphong);
                            Unable();
                            button2.Enabled = true;
                            button3.Visible = false;
                            break;

                    }
                    button4.BringToFront();
                    dataGridView1.Enabled = true;
                }
                conn.Close();
            }

        }
        public void ClearData()
        {
            textBox1.Text = textBox2.Text = textBox5.Text = "";
            comboBox1.Text = comboBox2.Text = comboBox3.Text = comboBox4.Text = comboBox5.Text = "";
            ID = -1;
        }
        public void Unable()
        {
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox5.Enabled = false;
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            comboBox4.Enabled = false;
            comboBox3.Enabled = false;
            comboBox5.Enabled = false;
        }
        public void Enable()
        {
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            textBox5.Enabled = true;
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            comboBox4.Enabled = true;
            comboBox3.Enabled = true;
            comboBox5.Enabled = true;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            button4.BringToFront();
            button2.Enabled = true;
            dataGridView1.Enabled = true;
            button3.Visible = false;
            Unable();
        }

        private void TextBox1_Validating(object sender, CancelEventArgs e)
        {
            int num;
            if (!int.TryParse(textBox1.Text, out num))
            {
                errorProviderHD.SetError(textBox1, "Vui lòng nhập phòng");
            }
            else
            {
                errorProviderHD.SetError(textBox1, "");
            }

        }

        private void TextBox2_Validating(object sender, CancelEventArgs e)
        {
            int num;
            if (!int.TryParse(textBox2.Text, out num))
            {
                errorProviderHD.SetError(textBox2, "Vui lòng nhập chỉ số nước");
            }
            else
            {
                errorProviderHD.SetError(textBox2, "");
            }

        }

        private void TextBox5_Validating(object sender, CancelEventArgs e)
        {
            int num;
            if (!int.TryParse(textBox5.Text, out num))
            {
                errorProviderHD.SetError(textBox5, "Vui lòng nhập chỉ số điện");
            }
            else
            {
                errorProviderHD.SetError(textBox5, "");
            }

        }

        private void ComboBox1_Validating(object sender, CancelEventArgs e)
        {
            if (comboBox1.Text == string.Empty)
            {
                errorProviderHD.SetError(comboBox1, "Vui lòng chọn tháng");
            }
            else
            {
                errorProviderHD.SetError(comboBox1, "");
            }

        }

        private void ComboBox3_Validating(object sender, CancelEventArgs e)
        {
            if (comboBox3.Text == string.Empty)
            {
                errorProviderHD.SetError(comboBox3, "Vui lòng chọn năm");
            }
            else
            {
                errorProviderHD.SetError(comboBox3, "");
            }

        }

        private void ComboBox2_Validating(object sender, CancelEventArgs e)
        {
            if (comboBox2.Text == string.Empty)
            {
                errorProviderHD.SetError(comboBox2, "Vui lòng chọn tình trạng");
            }
            else
            {
                errorProviderHD.SetError(comboBox2, "");
            }

        }

        private void ComboBox4_Validating(object sender, CancelEventArgs e)
        {
            if (comboBox4.Text == string.Empty)
            {
                errorProviderHD.SetError(comboBox4, "Vui lòng chọn giá nước");
            }
            else
            {
                errorProviderHD.SetError(comboBox4, "");
            }

        }

        private void ComboBox5_Validating(object sender, CancelEventArgs e)
        {
            if (comboBox5.Text == string.Empty)
            {
                errorProviderHD.SetError(comboBox5, "Vui lòng chọn giá điện");
            }
            else
            {
                errorProviderHD.SetError(comboBox5, "");
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
           
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //public void exportgridtopdf(DataGridView dgw, string filename)
        //{
        //    BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, BaseFont.EMBEDDED);
        //    PdfPTable pdftable = new PdfPTable(dgw.Columns.Count);
        //    pdftable.DefaultCell.Padding = 3;
        //    pdftable.WidthPercentage = 100;
        //    pdftable.HorizontalAlignment = Element.ALIGN_LEFT;
        //    pdftable.DefaultCell.BorderWidth = 1;

        //    iTextSharp.text.Font text = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL);
        //    foreach (DataGridViewColumn column in dgw.Columns)
        //    {
        //        PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText, text));
        //        cell.BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240);
        //        pdftable.AddCell(cell);
        //    }

        //    foreach(DataGridViewRow row in dgw.Rows)
        //    {
        //        foreach (DataGridViewCell cell in row.Cells)
        //        {
        //            pdftable.AddCell(new Phrase(cell.Value.ToString(), text));
        //        }
        //    }
        //    var savefiledialoge = new SaveFileDialog();
        //    savefiledialoge.FileName = filename;
        //    savefiledialoge.DefaultExt = ".pdf";
        //    if (savefiledialoge.ShowDialog() == DialogResult.OK)
        //    {
        //        using (FileStream stream = new FileStream(savefiledialoge.FileName, FileMode.Create))
        //        {
        //            Document pdfdoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
        //            PdfWriter.GetInstance(pdfdoc, stream);
        //            pdfdoc.Open();
        //            pdfdoc.Add(pdftable);
        //            stream.Close();
        //        }
        //    }

        //}

        private void button5_Click_1(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application oexcel = new Microsoft.Office.Interop.Excel.Application();
            var connectionString = ConfigurationManager.ConnectionStrings["QuanLyKTX"].ConnectionString;

            SqlConnection conn = new SqlConnection(connectionString);
            string query = "SELECT * FROM HOADON WHERE TINHTRANGHD='Da thanh toan'";
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable db = new DataTable();
            adapter.SelectCommand = cmd;
            adapter.Fill(db);
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                object misValue = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Excel.Workbook obook = oexcel.Workbooks.Add(misValue);
                Microsoft.Office.Interop.Excel.Worksheet osheet = new Microsoft.Office.Interop.Excel.Worksheet();


                //  obook.Worksheets.Add(misValue);

                osheet = (Microsoft.Office.Interop.Excel.Worksheet)obook.Sheets["Sheet1"];
                int colIndex = 0;
                int rowIndex = 1;

                foreach (DataColumn dc in db.Columns)
                {
                    colIndex++;
                    osheet.Cells[1, colIndex] = dc.ColumnName;
                }
                foreach (DataRow dr in db.Rows)
                {
                    rowIndex++;
                    colIndex = 0;

                    foreach (DataColumn dc in db.Columns)
                    {
                        colIndex++;
                        osheet.Cells[rowIndex, colIndex] = dr[dc.ColumnName];
                    }
                }

                osheet.Columns.AutoFit();
                string filepath = "D:\\hoc\\Book1";

                //Release and terminate excel

                obook.SaveAs(filepath);
                obook.Close();
                oexcel.Quit();

                GC.Collect();
            }
            catch (Exception ex)
            {
                oexcel.Quit();

            }
        }
    }
}
