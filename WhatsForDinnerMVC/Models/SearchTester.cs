
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WhatsForDinnerMVC.Models;
using System.Web.Mvc;
//using MySql.Data.MySqlClient;
//
namespace WhatsForDinnerMVC.Models
{
	[Serializable]
	public class SearchTester
	{

		//private string myConnectionString = "server=127.0.0.1;uid=root;pwd=password;database=whatsfordinnerdb;";

		public List<User> Users { get; set; }

		public SearchTester()
		{
			Users = new List<User>();
		}

		public void PopulateAllUsers()
		{
			/*
			try
			{
				MySqlConnection conn = new MySqlConnection(myConnectionString);
				conn.Open();
				using (MySqlCommand sqlCommand = new MySqlCommand("SELECT * FROM User;", conn))
				{
					MySqlDataReader reader = sqlCommand.ExecuteReader();
					// Build up a user list object!
					while (reader.Read())
					{
						// TODO - not sure how to handle the null MenuID yet.
						//Users.Add(new User((string)reader[0], (string)reader[1], (string)reader[2], (int?)reader[3], false));
						Users.Add(new User((string)reader[0], (string)reader[1], (string)reader[2], false));
					}
				}
				conn.Close();
			}
			catch (MySqlException ex)
			{ }
			*/

		}

		public void PerformSearch(string searchString)
		{
			/*
			try
			{
				MySqlConnection conn = new MySqlConnection(myConnectionString);
				conn.Open();
				using (MySqlCommand sqlCommand = new MySqlCommand("SELECT * FROM User where Name LIKE @name;", conn))
				{
					sqlCommand.Parameters.AddWithValue("@name", "%" + searchString + "%");
					MySqlDataReader reader = sqlCommand.ExecuteReader();
					// Build up a user list object!
					while (reader.Read())
					{
						// TODO - not sure how to handle the null MenuID yet.
						//Users.Add(new User((string)reader[0], (string)reader[1], (string)reader[2], (int?)reader[3], false));
						Users.Add(new User((string)reader[0], (string)reader[1], (string)reader[2], false));
					}
				}
				conn.Close();
			}
			catch (MySqlException ex)
			{ }
			*/
		}



	}
}
