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
        public int MenuID { get; set; }
        public string MenuName { get; set; }
        public DateTime WhenCreated { get; set; }
        public List<Recipe> Recipes { get; set; }
        public Recipe SelectedDeleteRecipe { get; private set; }
        public Recipe SelectedAddRecipe { get; private set; }
        public List<Recipe> SearchRecipeResults { get; private set; }

        #region constructor(s)
        public Menu(int MenuID)
		{
			this.MenuID = MenuID;
			Recipes = new List<Recipe>();
            WhenCreated = new DateTime();
            SearchRecipeResults = new List<Recipe>();

            // Get info about the menuitself
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT displayName,timeOfLastUpdate FROM Menus WHERE menuId = @menuId";
                    //cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@menuId", MenuID);
                    connection.Open();

                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            MenuName = (string)reader[0];
                            WhenCreated = reader[1] as DateTime? ?? default(DateTime);
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
            SelectedDeleteRecipe = Recipes[0];
        }
        #endregion

        #region methods
        /// <summary>
        /// Update the selected Recipe so on the menu (so deletetion can leverage).
        /// </summary>
        /// <param name="id"></param>
        public void UpdateDeleteSelectedRecipe(int id)
        {
            foreach(Recipe recipe in Recipes)
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
            foreach (Recipe recipe in Recipes)
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
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.CommandText = "SELECT recipeId FROM Recipes WHERE title like @searchString";
                    sqlCommand.Parameters.AddWithValue("@searchString", "%" + searchString + "%");
                    sqlCommand.Connection = conn;

                    conn.Open();
                    SqlDataReader reader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int id = (int)reader[0];
                            Recipe recipe = new Recipe((int)reader[0]);
                            SearchRecipeResults.Add(recipe);
                        }
                    }
                }
            }
        }

        #endregion
    }
}