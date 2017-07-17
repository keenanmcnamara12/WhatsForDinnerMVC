using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WhatsForDinnerMVC.Models
{
    public class User
    {

        private MySqlConnection connection;

        /// <summary>
        /// User's ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// User's name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// User's password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// List of menus associated with this user. 
        /// </summary>
        public List<int> MenuID { get; set; }

        public bool IsValid { get; private set; }

        public User(string UserID, string password)
        {
            this.UserID = UserID;
            this.Password = password;

            MySqlConnection conn;
            string myConnectionString;

            myConnectionString = "server=localhost;uid=root;" +
                "pwd=password;database=WhatsForDinner;";

            try
            {
                conn = new MySqlConnection();
                conn.ConnectionString = myConnectionString;
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            { }




            //Initialize values
            string server;
            string database;
            string uid;
            string sqlPassword;
            server = "localhost";
            database = "WhatsForDinnerDB";
            uid = "root";
            sqlPassword = "";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + sqlPassword + ";";

            connection = new MySqlConnection(connectionString);

            string query = "SELECT * FROM User";

            connection.Open();





            //Create Command
            MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                //while (dataReader.Read())
                //{
                //    list[0].Add(dataReader["id"] + "");
                //    list[1].Add(dataReader["name"] + "");
                //    list[2].Add(dataReader["age"] + "");
                //}

                //close Data Reader
                dataReader.Close();

                //close Connection
                connection.Close();

            

            // TODO - check for the user in the DB and matching password
            if (true)
            {
                IsValid = true;
            }
            else
            {
                IsValid = false;
            }
        }
    }
}