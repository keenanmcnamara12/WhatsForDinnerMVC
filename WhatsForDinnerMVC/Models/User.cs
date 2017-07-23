using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WhatsForDinnerMVC.Models
{
    /// <summary>
    /// User object to be the main method for interacting with the user level DB.
    /// Serializable so the user object can be saved to the session so the data 
    /// will persist from login to search.
    /// </summary>
    [Serializable]
    public class User
    {
        private string myConnectionString = "server=127.0.0.1;uid=root;pwd=password;database=whatsfordinnerdb;";

        #region Properties
        /// <summary>
        /// User's ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// User's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// User's password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// List of menus associated with this user. 
        /// </summary>
        public int? MenuID { get; set; }

        /// <summary>
        /// Indicates if this instance has a valid UserID/Password combination.
        /// </summary>
        public bool IsValid { get; private set; }
        #endregion

        #region Constructors
        public User(string UserID, string Name, string Password, int? MenuID,bool ShouldValidateInDB)
        {
            this.UserID = UserID;
            this.Password = Password;
            this.Name = Name;
            this.MenuID = MenuID;

            if (ShouldValidateInDB == false)
            {
                IsValid = true;
            }

            try
            {
                MySqlConnection conn = new MySqlConnection(myConnectionString);
                conn.Open();
                using (MySqlCommand sqlCommand = new MySqlCommand("SELECT COUNT(*) FROM User WHERE UserID like @user AND Password like @password;", conn))
                {
                    sqlCommand.Parameters.AddWithValue("@user", UserID);
                    sqlCommand.Parameters.AddWithValue("@password", Password);
                    long userCount = (long)sqlCommand.ExecuteScalar();
                    if (userCount > 0)
                    {
                        IsValid = true;
                    }
                }
                conn.Close();

                // Let's also load the user's name and menu ID while we're here rather than adding handling to the property to check for an "is null"
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                using (MySqlCommand sqlCommand = new MySqlCommand("SELECT Name, MenuID FROM User WHERE UserID like @user;", conn))
                {
                    sqlCommand.Parameters.AddWithValue("@user", UserID);
                    MySqlDataReader reader = sqlCommand.ExecuteReader();
                    reader.Read();
                    this.Name = (string)reader[0];
                    if (String.IsNullOrEmpty(reader[1].ToString()))
                    {
                        this.MenuID = null;
                    }
                    else
                    {
                        this.MenuID = (int?)reader[1];
                    } 
                }
                conn.Close();
                
            }
            catch (MySqlException ex)
            { }
        }

        /// <summary>
        /// Constructor for the login page (don't actually need the user's name).
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="Password"></param>
        public User(string UserID, string Password) : this(UserID, "", Password, null, false)
        { }

        public User(string UserID, string Name, string Password) : this(UserID, "", Password, null, false)
        { }
        #endregion

        #region Methods
        public bool CreateNewUser()
        {
            // This user already valid in DB, return
            if (IsValid == true)
            {
                return false;
            }

            try
            {
                // Check if the user exists in the database already.
                MySqlConnection conn = new MySqlConnection(myConnectionString);
                conn.Open();
                using (MySqlCommand sqlCommand = new MySqlCommand("SELECT COUNT(*) FROM User WHERE UserID like @user;", conn))
                {
                    sqlCommand.Parameters.AddWithValue("@user", UserID);
                    long userCount = (long)sqlCommand.ExecuteScalar();
                    if (userCount > 0)
                    {
                        conn.Close();
                        return false;
                    }
                }
                conn.Close();

                // Cool, let's add the UserID/Password to the database.
                string sqlString = "insert into user (UserID, Name, Password) "
                                 + "values(@user, @name, @password);";
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                using (MySqlCommand sqlCommand = new MySqlCommand(sqlString, conn))
                {
                    sqlCommand.Parameters.AddWithValue("@user", UserID);
                    sqlCommand.Parameters.AddWithValue("@name", Name);
                    sqlCommand.Parameters.AddWithValue("@password", Password);
                    int rowAffected = sqlCommand.ExecuteNonQuery();
                    if (rowAffected > 0)
                    {
                        IsValid = true;
                        conn.Close();
                        return true;
                    }
                }
                conn.Close();
            }
            catch (MySqlException ex)
            { }
            return false;
        }
        #endregion

    }
}