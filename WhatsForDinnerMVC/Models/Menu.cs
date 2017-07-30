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
	[Serializable]
	public class Menu
	{
		public int MenuID { get; set; }
		public string MenuName { get; set; }
		//public DateTime WhenCreated { get; set; }
		public List<Recipe> Recipes { get; set; }
		public Recipe SelectedDeleteRecipe { get; private set; }
		public Recipe SelectedAddRecipe { get; private set; }
		public List<Recipe> SearchRecipeResults { get; private set; }
        public bool ShowModel { get; set; }

		#region constructor(s)
		/// <summary>
		/// Given a name, create a menu in the database.  
		/// </summary>
		/// <param name="MenuName">Name of the new menu</param>
		public Menu(string MenuName, string UserName)
		{
			if (String.IsNullOrWhiteSpace(MenuName))
			{
				return;
			}

			using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					cmd.CommandText = "spCreateMenu";
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Connection = connection;
					cmd.Parameters.AddWithValue("@displayName", MenuName);
					cmd.Parameters.AddWithValue("@userId", UserName);
					connection.Open();

					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

					if (reader.HasRows)
					{
						while (reader.Read())
						{
							int newMenuId;
							if (Int32.TryParse(reader["menuId"].ToString(), out newMenuId))
							{
								this.MenuID = newMenuId;
								this.MenuName = MenuName;
								Recipes = new List<Recipe>();
								//WhenCreated = new DateTime();
								SearchRecipeResults = new List<Recipe>();
                                SelectedAddRecipe = new Recipe();
                                ShowModel = false;
							}
						}
					}
				}
			}
		}

		public Menu(int MenuID)
		{
			this.MenuID = MenuID;
			Recipes = new List<Recipe>();
			//WhenCreated = new DateTime();
			SearchRecipeResults = new List<Recipe>();
            SelectedAddRecipe = new Recipe();
            ShowModel = false;

			// Get info about the menuitself
			using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					//cmd.CommandText = "SELECT displayName FROM Menus WHERE menuId = @menuId";
					cmd.CommandText = "spGetMenuInfo";
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Connection = connection;
					cmd.Parameters.AddWithValue("@menuId", MenuID);
					connection.Open();

					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

					if (reader.HasRows)
					{
						while (reader.Read())
						{
							MenuName = (string)reader[0];
						}
					}
				}
			}

			using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					cmd.CommandText = "spGetRecipesFromMenu";
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Connection = connection;
					cmd.Parameters.AddWithValue("@menuId", MenuID);
					connection.Open();

					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

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
			// Default the first recipe as the selected recipe to avoid null references
			if (Recipes.Count > 0)
			{
				SelectedDeleteRecipe = Recipes[0];
			}
		}
		#endregion

		#region methods
		/// <summary>
		/// Update the selected Recipe so on the menu (so deletetion can leverage).
		/// </summary>
		/// <param name="id"></param>
		public void UpdateDeleteSelectedRecipe(int id)
		{
			foreach (Recipe recipe in Recipes)
			{
				if (recipe.ID == id)
				{
					SelectedDeleteRecipe = recipe;
					return;
				}
			}
		}

		/// <summary>
		/// Update the selected Recipe so on the menu (so add can leverage).
		/// </summary>
		/// <param name="id"></param>
		public void UpdateAddSelectedRecipe(int id)
		{
			foreach (Recipe recipe in SearchRecipeResults)
			{
				if (recipe.ID == id)
				{
					SelectedAddRecipe = recipe;
					return;
				}
			}
		}

		/// <summary>
		/// Perform a search and load the list of recipes to the search
		/// </summary>
		public void PerformSearch(string searchString)
		{
			// First clear past search results
			SearchRecipeResults.Clear();
			using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					//sqlCommand.CommandText = "SELECT recipeId FROM Recipes WHERE title like @searchString";
					cmd.CommandText = "spSearchDatabase";
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@searchTerms", searchString);
					cmd.Connection = conn;

					conn.Open();
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					if (reader.HasRows)
					{
						while (reader.Read())
						{
							int recipeId;
							if (Int32.TryParse(reader["recipeId"].ToString(), out recipeId))
							{
								SearchRecipeResults.Add(new Recipe(recipeId));
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// If there is a selected recipe to add, it will be added to the database
		/// and the local Recipe list.
		/// </summary>
		public void AddSelectedRecipeToMenu()
		{
			// In case a menu hasn't been selected, don't attempt to add it to the list.
			if (SelectedAddRecipe != null & SelectedAddRecipe.IsValid == true)
			{
				// Ensure not a duplicate (or you'll get a primary key duplicate error in SQL and throw exception)
				foreach (Recipe recipe in Recipes)
				{
					if (recipe.ID == SelectedAddRecipe.ID)
					{
						return;
					}
				}

				// Update local object
				Recipes.Add(SelectedAddRecipe);

				// Update DB
				using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
				{
					using (SqlCommand cmd = new SqlCommand())
					{
						cmd.CommandText = "spInsertRecipeIntoMenu";
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@recipeId", SelectedAddRecipe.ID);
						cmd.Parameters.AddWithValue("@menuId", MenuID);
						cmd.Connection = conn;

						conn.Open();
						cmd.ExecuteNonQuery();
						cmd.Connection.Close();
					}
				}
			}
		}

		/// <summary>
		/// If there is a selected recipe to delete, it will be deleted from the database
		/// and the local Recipe list.
		/// </summary>
		public void DeleteSelectedRecipeFromMenu()
		{
			// In case a menu hasn't been selected, don't attempt to add it to the list.
			if (SelectedDeleteRecipe != null && SelectedAddRecipe.IsValid == true)
			{
				// Update local object
				Recipes.Remove(SelectedDeleteRecipe);

				// Update DB
				using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
				{
					using (SqlCommand cmd = new SqlCommand())
					{
						cmd.CommandText = "spRemoveRecipeFromMenu";
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@recipeId", SelectedDeleteRecipe.ID);
						cmd.Parameters.AddWithValue("@menuId", MenuID);
						cmd.Connection = conn;

						conn.Open();
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		#endregion
	}
}