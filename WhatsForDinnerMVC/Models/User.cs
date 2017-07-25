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
								Menus.Add(new Menu(menuID));
							}




						}

					}



				}
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
		#endregion
	}
}

