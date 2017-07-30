using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;

namespace WhatsForDinnerMVC.Models
{


	/// <summary>
	/// This class holds the details about ingredients for a recipe. 
	/// </summary>
	public class Ingredient
	{

		public string quantity { get; set; }
		public string unit { get; set; }
		public string ingredientName { get; set; }
		public string preparation { get; set; }


		public Ingredient(string quantity, string unit, string ingredientName, string preparation)
		{
			this.quantity = quantity;
			this.unit = unit;
			this.ingredientName = ingredientName;
			this.preparation = preparation;


		}

		public Ingredient(string quantity, string unit, string IngredientName)
		{
			this.quantity = quantity;
			this.unit = unit;
			this.ingredientName = IngredientName;
		}

		public override string ToString()
		{
			if (!String.IsNullOrWhiteSpace(preparation))
			{
				return quantity + " " + unit + " " + ingredientName + " " + preparation;
			}

			return quantity + " " + unit + " " + ingredientName;

		}
	}
}