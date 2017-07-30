using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;

namespace WhatsForDinnerMVC.Models
{
	[Serializable]
	public class Recipe
	{
		public int ID { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Calories { get; set; }
		public Decimal Fat { get; set; }
		public Decimal Sodium { get; set; }
		public Decimal Rating { get; set; }
		public Decimal Protein { get; set; }

		public List<string> DirectionsList { get; set; }

		public List<Ingredient> IngredientsList { get; set; }

		#region constructor
		public Recipe(int recipeID)
		{
			this.ID = recipeID;

			// Get info about the menuitself
			using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					//cmd.CommandText = "SELECT title, calories, description, fat, protein, rating, sodium FROM Recipes WHERE recipeId = @recipeId";
					cmd.CommandText = "spGetRecipeInfo";
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Connection = connection;
					cmd.Parameters.AddWithValue("@recipeId", ID);
					connection.Open();

					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

					if (reader.HasRows)
					{
						reader.Read();
						Title = (string)reader[0];
						Calories = (string)reader[1];
						Description = (string)reader[2];
						Fat = (Decimal)reader[3];
						Protein = (Decimal)reader[4];
						Rating = (Decimal)reader[5];
						Sodium = (Decimal)reader[6];
					}
				}
			}
			//Get Directions
			using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
			{
				using (SqlCommand cmd = new SqlCommand())
				{

					cmd.CommandText = "spGetRecipeDirections";
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Connection = connection;
					cmd.Parameters.AddWithValue("@recipeId", ID);

					connection.Open();

					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

					if (reader.HasRows)
					{

						this.DirectionsList = new List<string>();

						while (reader.Read())
						{
							string newDirection = reader["direction"].ToString();

							DirectionsList.Add(newDirection);
						}

					}

				}
			}
			//Ingredients
			using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
			{
				using (SqlCommand cmd = new SqlCommand())
				{

					cmd.CommandText = "spGetRecipeIngredients";
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Connection = connection;
					cmd.Parameters.AddWithValue("@recipeId", ID);

					connection.Open();

					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

					if (reader.HasRows)
					{
						this.IngredientsList = new List<Ingredient>();
						while (reader.Read())
						{
							Ingredient newIngredient = new Ingredient(reader["quantity"].ToString(), reader["unit"].ToString(), reader["ingredient"].ToString(), reader["preparation"].ToString());

							IngredientsList.Add(newIngredient);

						}

					}

				}
			}
		}
		#endregion constructor

		#region methods

		#endregion


	}
}