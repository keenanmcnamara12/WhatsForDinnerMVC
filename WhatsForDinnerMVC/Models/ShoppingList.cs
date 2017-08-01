using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;
using System.Collections;

namespace WhatsForDinnerMVC.Models
{
	public class ShoppingList : IEnumerable
	{
		List<Ingredient> IngredientList;
		List<string> FinalShoppingList;

		public ShoppingList(int menuId)
		{

			FinalShoppingList = new List<string>();
			IngredientList = new List<Ingredient>();

			using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					cmd.CommandText = "spCreateShoppingList";
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Connection = connection;
					cmd.Parameters.AddWithValue("@menuId", menuId);
					connection.Open();

					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

					if (reader.HasRows)
					{
						while (reader.Read())
						{

							Ingredient newIngredient = new Ingredient(reader["quantity"].ToString(), reader["unit"].ToString(), reader["ingredient"].ToString());

							addOrMergeToIngredientList(newIngredient);

						}
					}
				}
			}
			List<Ingredient>.Enumerator IngredientListEnum = IngredientList.GetEnumerator();

			IngredientListEnum.MoveNext();
			Ingredient nextIngredient = IngredientListEnum.Current;

			while (nextIngredient != null)
			{


				addToShoppingList(nextIngredient);
				IngredientListEnum.MoveNext();
				nextIngredient = IngredientListEnum.Current;

			}


		}


		private void addOrMergeToIngredientList(Ingredient ingredient)
		{

			bool done = false;

			//Handle if ingredient list is empty, add to ingredient list and return
			if (!IngredientList.Any())
			{
				IngredientList.Add(ingredient);
			}

			//otherwise search through the ingredient list and see if any ingredients match
			else
			{
				List<Ingredient>.Enumerator IngredientListEnum = IngredientList.GetEnumerator();
				IngredientListEnum.MoveNext();
				Ingredient nextIngredient = IngredientListEnum.Current;

				while (nextIngredient != null && done != true)
				{
					//check to see if ingredient name matches 
					if (ingredient.ingredientName.Equals(nextIngredient.ingredientName))
					{

						mergeIngredient(ingredient, nextIngredient);
						done = true;


					}
					else
					{
						IngredientListEnum.MoveNext();
						nextIngredient = IngredientListEnum.Current;
					}
				}
				if (done != true)
				{
					IngredientList.Add(ingredient);
				}

			}

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="oldIngredient"></param>
		/// <param name="newIngredient"></param>
		private void mergeIngredient(Ingredient oldIngredient, Ingredient newIngredient)
		{

			//if units match merge quantity
			if (doUnitsMatch(oldIngredient.unit, newIngredient.unit))
			{
				double quantity1 = FractionToDouble(oldIngredient.quantity);
				double quantity2 = FractionToDouble(newIngredient.quantity);

				double newQuantity = quantity1 + quantity2;

				Ingredient combinedIngredient = new Ingredient(newQuantity.ToString(), oldIngredient.unit, oldIngredient.ingredientName);

				IngredientList.Add(combinedIngredient);

				//remove both old and new ingredients from ingredient list
				IngredientList.Remove(oldIngredient);
				IngredientList.Remove(newIngredient);


			}
		}


		private void addToShoppingList(Ingredient ingredient)
		{

			string newShoppingListLine = ingredient.ToString();

			FinalShoppingList.Add(newShoppingListLine);


		}

		double FractionToDouble(string fraction)
		{
			double result;

			if (double.TryParse(fraction, out result))
			{
				return result;
			}

			string[] split = fraction.Split(new char[] { ' ', '/' });

			if (split.Length == 2 || split.Length == 3)
			{
				int a, b;

				if (int.TryParse(split[0], out a) && int.TryParse(split[1], out b))
				{
					if (split.Length == 2)
					{
						return (double)a / b;
					}

					int c;

					if (int.TryParse(split[2], out c))
					{
						return a + (double)b / c;
					}
				}
			}

			throw new FormatException("Not a valid fraction.");
		}

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable)FinalShoppingList).GetEnumerator();
		}


		#region Unit Matching Methods

		private bool doUnitsMatch(string unit1, string unit2)
		{

			if (unit1 != null && unit2 != null)
			{
				//if the units are the same string (e.g. cup + cup)
				if (unit1.Equals(unit2))
				{
					return true;
				}

				List<string> unit1List = getAlternateUnitList(unit1);

				//if the units are two related units return true (e.g. cup + cups)
				if (unit1List.Contains(unit2))
				{
					return true;
				}
			}

			return false;

		}





		private List<string> getAlternateUnitList(string unit)
		{

			List<string> alternateUnitList = new List<string>();

			if (unit == "teaspoon" || unit == "teaspoons" || unit == "tsp")
			{

				alternateUnitList.Add("teaspoon");
				alternateUnitList.Add("teaspoons");
				alternateUnitList.Add("tsp");

			}
			else if (unit == "tablespoon" || unit == "tablespoons" || unit == "tbsp")
			{
				alternateUnitList.Add("tablespoon");
				alternateUnitList.Add("tablespoons");
				alternateUnitList.Add("tbsp");
			}
			else if (unit == "cup" || unit == "cups")
			{
				alternateUnitList.Add("cup");
				alternateUnitList.Add("cups");
			}
			else if (unit == "pound" || unit == "pounds" || unit == "lb" || unit == "lbs")
			{
				alternateUnitList.Add("pound");
				alternateUnitList.Add("pounds");
				alternateUnitList.Add("lb");
				alternateUnitList.Add("lbs");


			}
			else if (unit == "pint" || unit == "pints")
			{
				alternateUnitList.Add("pint");
				alternateUnitList.Add("pints");
			}

			else if (unit == "ounce" || unit == "ounces" || unit == "oz")
			{
				alternateUnitList.Add("ounce");
				alternateUnitList.Add("ounces");
				alternateUnitList.Add("oz");
			}
			else if (unit == "quart" || unit == "quarts" || unit == "qt")
			{
				alternateUnitList.Add("quart");
				alternateUnitList.Add("quarts");
				alternateUnitList.Add("qt");
			}
			else if (unit == "clove" || unit == "cloves")
			{
				alternateUnitList.Add("clove");
				alternateUnitList.Add("cloves");
			}

			else if (unit == "slice" || unit == "slices")
			{
				alternateUnitList.Add("slice");
				alternateUnitList.Add("slices");
			}

			else if (unit == "sprig" || unit == "sprigs")
			{
				alternateUnitList.Add("sprig");
				alternateUnitList.Add("sprigs");
			}

			else if (unit == "stalk" || unit == "stalks")
			{
				alternateUnitList.Add("stalk");
				alternateUnitList.Add("stalks");
			}

			else if (unit == "sheet" || unit == "sheets")
			{
				alternateUnitList.Add("sheet");
				alternateUnitList.Add("sheets");
			}

			else if (unit == "head" || unit == "heads")
			{
				alternateUnitList.Add("head");
				alternateUnitList.Add("heads");
			}

			else if (unit == "can" || unit == "cans")
			{
				alternateUnitList.Add("can");
				alternateUnitList.Add("cans");
			}

			else if (unit == "jar" || unit == "jars")
			{
				alternateUnitList.Add("jar");
				alternateUnitList.Add("jars");
			}

			else if (unit == "gram" || unit == "grams" || unit == "g")
			{
				alternateUnitList.Add("gram");
				alternateUnitList.Add("grams");
				alternateUnitList.Add("g");
			}

			else if (unit == "liter" || unit == "litre" || unit == "liters" || unit == "litres" || unit == "l")
			{
				alternateUnitList.Add("liter");
				alternateUnitList.Add("litre");
				alternateUnitList.Add("liters");
				alternateUnitList.Add("litres");
				alternateUnitList.Add("l");
			}

			else if (unit == "milliliter" || unit == "millilitre" || unit == "milliters" || unit == "millilitres" || unit == "mL" || unit == "ml")
			{
				alternateUnitList.Add("milliliter");
				alternateUnitList.Add("millilitre");
				alternateUnitList.Add("milliliters");
				alternateUnitList.Add("millilitres");
				alternateUnitList.Add("mL");
				alternateUnitList.Add("ml");
			}

			else if (unit == "bottle" || unit == "bottles")
			{
				alternateUnitList.Add("bottle");
				alternateUnitList.Add("bottles");
			}

			else if (unit == "slab" || unit == "slabs")
			{
				alternateUnitList.Add("slab");
				alternateUnitList.Add("slabs");
			}

			else if (unit == "envelope" || unit == "envelopes")
			{
				alternateUnitList.Add("envelope");
				alternateUnitList.Add("envelopes");
			}

			else if (unit == "package" || unit == "packages")
			{
				alternateUnitList.Add("package");
				alternateUnitList.Add("packages");
			}

			else if (unit == "stick" || unit == "sticks")
			{
				alternateUnitList.Add("stick");
				alternateUnitList.Add("sticks");
			}

			else if (unit == "strip" || unit == "strips")
			{
				alternateUnitList.Add("strip");
				alternateUnitList.Add("strips");
			}

			else if (unit == "piece" || unit == "pieces")
			{
				alternateUnitList.Add("piece");
				alternateUnitList.Add("pieces");

			}

			else if (unit == "basket" || unit == "baskets")
			{
				alternateUnitList.Add("basket");
				alternateUnitList.Add("baskets");
			}

			else if (unit == "bag" || unit == "bags")
			{
				alternateUnitList.Add("bag");
				alternateUnitList.Add("bags");
			}

			else if (unit == "ear" || unit == "ears")
			{
				alternateUnitList.Add("ear");
				alternateUnitList.Add("ears");
			}

			return alternateUnitList;


		}


		#endregion

	}
}