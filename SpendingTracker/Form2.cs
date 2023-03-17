using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpendingTracker
{
    public partial class Form2 : Form
    {
        public Account account;
        string connectionString = @"URI=file:" + Application.StartupPath + "\\personal_expenditure.db";

        SQLiteConnection con;
        SQLiteDataReader dataReader;
        SQLiteCommand command;
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            txt_DateTime.Text = DateTime.Now.ToShortDateString();
            txt_DateTime.Enabled = false;
            txt_id.Enabled = false;

            comboBox_Catagory.Items.Add("Food");
            comboBox_Catagory.Items.Add("Transportation");
            comboBox_Catagory.Items.Add("Medical & Healthcare");
            comboBox_Catagory.Items.Add("Housing");
            comboBox_Catagory.Items.Add("Children");
            comboBox_Catagory.Items.Add("Entertainment");
            comboBox_Catagory.Items.Add("Personal Development");

            getDatafromDatabase();

        }
        private void getDatafromDatabase()
        {
            dataGridView1.Rows.Clear();

            var con = new SQLiteConnection(connectionString);
            con.Open();

            string sql = "SELECT * FROM spending WHERE account_id = @account_id";
            command = new SQLiteCommand(sql, con);

            command.Parameters.AddWithValue("@account_id", account.id);

            dataGridView1.ColumnCount = 4;

            dataGridView1.Columns[0].Name = "ID";
            dataGridView1.Columns[1].Name = "COST";
            dataGridView1.Columns[2].Name = "CATAGORY";
            dataGridView1.Columns[3].Name = "TIME";

            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                dataGridView1.Rows.Insert(0,
                    dataReader.GetInt32(0),
                    dataReader.GetInt32(2),
                    dataReader.GetString(3),
                    dataReader.GetString(4)
                    );
            }

            con.Close();

            int toplam = 0;
            for (int e = 0; e < dataGridView1.Rows.Count; ++e)
            {
                toplam += Convert.ToInt32(dataGridView1.Rows[e].Cells[1].Value);
            }
            lbl_sum.Text = toplam.ToString();
        }

        private void btn_create_Click(object sender, EventArgs e)
        {
            con = new SQLiteConnection(connectionString);
            con.Open();

            var command = new SQLiteCommand(con);

            try
            {
                command.CommandText = "INSERT INTO spending (account_id, catagory, cost, date_of_spending) VALUES (@account_id,@catagory,@cost, @date_of_spending)";

                command.Parameters.AddWithValue("@account_id", account.id);
                command.Parameters.AddWithValue("@catagory", comboBox_Catagory.Text);
                command.Parameters.AddWithValue("@cost", Int32.Parse(txt_cost.Text));
                command.Parameters.AddWithValue("@date_of_spending", txt_DateTime.Text);
                command.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Insert Error : " + exc.Message);
            }

            con.Close();

            getDatafromDatabase();
        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            con = new SQLiteConnection(connectionString);
            con.Open();
            command = new SQLiteCommand(con);

            try
            {
                command.CommandText = "UPDATE spending SET COST = @cost, catagory = @catagory WHERE id = @id";

                command.Parameters.AddWithValue("@cost", Int32.Parse(txt_cost.Text));
                command.Parameters.AddWithValue("@catagory", comboBox_Catagory.Text);
                command.Parameters.AddWithValue("@id", txt_id.Text);

                int updatedRowCount = command.ExecuteNonQuery();
                if (updatedRowCount == 0)
                {
                    MessageBox.Show("Person not found // You have to click that which do you want update spending line in table");
                    return;
                }

                MessageBox.Show("Person updated successfully");
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error:" + exc.Message);
            }

            con.Close();

            getDatafromDatabase();
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            con = new SQLiteConnection(connectionString);
            con.Open();
            command = new SQLiteCommand(con);

            try
            {
                command.CommandText = "DELETE FROM spending WHERE id = @id";

                command.Parameters.AddWithValue("@id", Int32.Parse(txt_id.Text));

                int deletedRowCount = command.ExecuteNonQuery();
                if (deletedRowCount == 0)
                {
                    MessageBox.Show("ID not found // You have to click that which do you want delete spending line in table");
                    return;
                }
                MessageBox.Show("Person deleted successfully");

            }
            catch (Exception exc)
            {
                MessageBox.Show("Error:" + exc.Message);
                return;
            }

            con.Close();

            getDatafromDatabase();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            String clicked = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            if (!String.IsNullOrEmpty(clicked))
            {
                txt_id.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                txt_cost.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                comboBox_Catagory.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            }
        }
    }
}
