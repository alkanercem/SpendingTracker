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
using Microsoft.VisualBasic.ApplicationServices;
using System.Runtime.Intrinsics.X86;
using System.Security.Principal;

namespace SpendingTracker
{
    public partial class Form1 : Form
    {
        
        public static string dbFileName = "personal_expenditure.db";
        string connectionString = @"URI=file:" + Application.StartupPath + dbFileName;

        SQLiteConnection con;
        SQLiteCommand command;
        SQLiteDataReader dataReader;


        private void createDatabase()
        {
            if (!System.IO.File.Exists(dbFileName))
            {
                SQLiteConnection.CreateFile(dbFileName);
            }
            using (con = new SQLiteConnection(connectionString))
            {
                con.Open();

                string sql1 = "CREATE TABLE account(id INTEGER UNIQUE NOT NULL, user_name TEXT NOT NULL UNIQUE, password TEXT NOT NULL, PRIMARY KEY('id' AUTOINCREMENT))";
                command = new SQLiteCommand(sql1, con);
                command.ExecuteNonQuery();

                string sql2 = "CREATE TABLE spending(id INTEGER UNIQUE NOT NULL, account_id INTEGER NOT NULL ,cost INTEGER NOT NULL, catagory TEXT, date_of_spending TEXT, PRIMARY KEY(id AUTOINCREMENT), FOREIGN KEY(account_id) REFERENCES account(id) );";
                command = new SQLiteCommand(sql2, con);
                command.ExecuteNonQuery();

                con.Close();
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            createDatabase();
        }

        private void btn_register_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txt_username_register.Text) || txt_password_register.Text == "")
            {
                MessageBox.Show("All fields must be filled");
                return;
            }

            try
            {
                con = new SQLiteConnection(connectionString);
                con.Open();

                string sql = "INSERT into account(user_name,password) values (@username,@password)";
                command = new SQLiteCommand(sql, con);

                command.Parameters.AddWithValue("@username", txt_username_register);
                command.Parameters.AddWithValue("@password", txt_password_register);

                command.ExecuteNonQuery();

                con.Close();
                MessageBox.Show("Your account was created successfully");

            }
            catch (Exception exc)
            {
                MessageBox.Show("Register Error : " + exc.Message);
            }

        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txt_username.Text) || String.IsNullOrEmpty(txt_password.Text))
            {
                MessageBox.Show("All fields must be filled");
                return;
            }

            con = new SQLiteConnection(connectionString);
            con.Open();

            command = new SQLiteCommand(con);
            string check = "SELECT * from account where user_name = @username and password = @password";
            command.CommandText = check;

            command.Parameters.AddWithValue("@username", txt_username);
            command.Parameters.AddWithValue("@password", txt_password);

            dataReader = command.ExecuteReader();
            if (!dataReader.HasRows)
            {
                dataReader.Close();
                con.Close();
                MessageBox.Show(" Login failed : invalid credentials or you dont have an account yet ");
                return;
            }
            else
            {
                dataReader.Read();
                Account account = new Account(dataReader.GetInt32(0), dataReader.GetString(1), dataReader.GetString(2));
                dataReader.Close();
                con.Close();

                Form2 spending_records_form = new Form2();
                spending_records_form.account = account;
                this.Hide();
                spending_records_form.ShowDialog();
            }
        }
    }
}