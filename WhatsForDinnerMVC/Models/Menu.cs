using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;

namespace WhatsForDinnerMVC.Models
{
	[System.Web.Script.Services.ScriptService]
	public class Menu
	{

		public int MenuID;

		public List<Recipe> Recipes { get; set; }

		public Menu(int MenuID)
		{
			this.MenuID = MenuID;
			Recipes = new List<Recipe>();
			SqlDataReader reader;
			using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					cmd.CommandText = "spGetRecipesFromMenu";
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Connection = connection;
					cmd.Parameters.AddWithValue("@menuId", MenuID);
					connection.Open();

					reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

					if (reader.HasRows)
					{

						while (reader.Read())
						{
							int recipeId;

							if (Int32.TryParse(reader["recipeId"].ToString(), out recipeId))
							{

								Recipes.Add(new Recipe(recipeId));
							}

						}

					}

				}

			}

		}
	}
}