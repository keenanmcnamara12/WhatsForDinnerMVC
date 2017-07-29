using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Data;

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
		public List<Menu> Menus { get; set; }

		/// <summary>
		/// Indicates if this instance has a valid UserID/Password combination.
		/// </summary>
		public bool IsValid { get; private set; }

        /// <summary>
        /// The menu ID of the currently selected menu for displaying the recipes 
        /// </summary>
        public Menu SelectedMenu{ private set; get; }

		#endregion

		#region Constructors
		public User(string UserID, string Name, string Password, bool ShouldValidateInDB)
		{
			this.UserID = UserID;
			this.Password = Password;
			this.Name = Name;
			this.Menus = new List<Menu>();

			if (ShouldValidateInDB == false)
			{
				IsValid = true;
			}

			using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
			{
				using (SqlCommand sqlCommand = new SqlCommand()) //MySqlCommand("SELECT COUNT(*) FROM User WHERE UserID like @user AND Password like @password;", conn))
				{
					sqlCommand.CommandText = "spIsValidUser";
					sqlCommand.Parameters.AddWithValue("@userId", UserID);
					sqlCommand.Parameters.AddWithValue("@password", Password);
					sqlCommand.CommandType = CommandType.StoredProcedure;
					sqlCommand.Connection = conn;

					conn.Open();
					SqlDataReader reader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
					if (reader.HasRows)
					{
						while (reader.Read())
						{
							int userCount;
							Int32.TryParse(reader["ValidUser"].ToString(), out userCount);
							if (userCount > 0)
							{
								IsValid = true; //TODO need to check if valid
							}
						}
					}
				}

				// Let's also load the user's name and menu ID while we're here rather than adding handling to the property to check for an "is null"
			}

			using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
			{
				using (SqlCommand sqlCommand = new SqlCommand()) //MySqlCommand("SELECT Name FROM User WHERE UserID like @user;", conn))
				{
					sqlCommand.CommandText = "spGetUserDisplayName";
					sqlCommand.Parameters.AddWithValue("@userId", UserID);
					sqlCommand.CommandType = CommandType.StoredProcedure;
					sqlCommand.Connection = conn;
					conn.Open();

					SqlDataReader reader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);

					if (reader.HasRows)
					{
						while (reader.Read())
						{
							this.Name = reader["name"].ToString();
						}
					}
				}
			}

			using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
			{
				using (SqlCommand sqlCommand = new SqlCommand())
				{
					sqlCommand.CommandText = "spGetMenusForUser";
					sqlCommand.Parameters.AddWithValue("@userId", UserID);
					sqlCommand.CommandType = CommandType.StoredProcedure;
					sqlCommand.Connection = conn;

					conn.Open();

					SqlDataReader reader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);

					if (reader.HasRows)
					{
						while (reader.Read())
						{
							int menuID;
							if (Int32.TryParse(reader["menuId"].ToString(), out menuID))
							{

                                bool duplicate = false;
                                // First make sure user doesn't already have this menu in the list.
                                foreach(Menu menu in Menus)
                                {
                                    
                                    if (menu.MenuID == menuID)
                                    {
                                        duplicate = true;
                                        break;
                                    }
                                }
                                if (duplicate == false)
                                {
								    Menus.Add(new Menu(menuID));
                                }
							}
						}
					}
				}
			}
            // Default the first menu as the selected menu to avoid null references
            if (Menus.Count > 0)
            {
                SelectedMenu = Menus[0];
            }   
		}

		/// <summary>
		/// Constructor for the login page (don't actually need the user's name).
		/// </summary>
		/// <param name="UserID"></param>
		/// <param name="Password"></param>
		public User(string UserID, string Password) : this(UserID, "", Password, false)
		{ }

		public User(string UserID, string Name, string Password) : this(UserID, Name, Password, false)
		{ }
		#endregion

		#region Methods
		public bool CreateNewUser()
		{
			// Check if the user exists in the database already.
			using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
			{
				using (SqlCommand sqlCommand = new SqlCommand())
				{
					sqlCommand.CommandText = "spIsUserIdUnique";
					sqlCommand.Parameters.AddWithValue("@userId", UserID);
					sqlCommand.CommandType = CommandType.StoredProcedure;
					sqlCommand.Connection = conn;

					conn.Open();

					SqlDataReader reader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);

					if (reader.HasRows)
					{
						while (reader.Read())
						{
							int userCount;
							Int32.TryParse(reader["userCount"].ToString(), out userCount);
							if (userCount > 0)
							{
								return false;
							}
						}
					}
				}
			}
			// Cool, let's add the UserID/Password to the database.
			using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
			{
				using (SqlCommand sqlCommand = new SqlCommand())
				{
					sqlCommand.CommandText = "spInsertNewUser";
					sqlCommand.Parameters.AddWithValue("@userId", UserID);
					sqlCommand.Parameters.AddWithValue("@name", Name);
					sqlCommand.Parameters.AddWithValue("@password", Password);
					sqlCommand.CommandType = CommandType.StoredProcedure;
					sqlCommand.Connection = conn;

					conn.Open();

					sqlCommand.ExecuteNonQuery();
					sqlCommand.Connection.Close();

					return true;
				}
			}
		}

        /// <summary>
        /// Creates a new menu record and returns the ID of the new menu
        /// </summary>
        /// <param name="MenuName">Name of the menu that the user entered.</param>
        /// <returns></returns>
        public int AddNewMenu(string MenuName)
        {
            // Call the constructor that creates a new menu in the DB.
            Menu newMenu = new Menu(MenuName, UserID);
            Menus.Add(newMenu);
            return newMenu.MenuID;
        }

        /// <summary>
        /// Update the selected menu ID.
        /// </summary>
        /// <param name="menuID">The newly selected menu ID</param>
        public void UpdateSelectedMenu(int menuID)
        {
            foreach(Menu menu in Menus)
            {
                if(menu.MenuID == menuID)
                {
                    SelectedMenu = menu;
                    return;
                }
            }
        }

        #endregion
    }
}

