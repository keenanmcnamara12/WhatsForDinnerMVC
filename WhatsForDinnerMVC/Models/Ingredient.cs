using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;

namespace WhatsForDinnerMVC.Models
{

	//This is scott's comment that we don't want overriding keenan's comment

	public class Ingredient
	{

		string quantity { get; set; }
		string unit { get; set; }
		string ingredientName { get; set; }
		string preparation { get; set; }


		public Ingredient(string quantity, string unit, string ingredientName, string preparation)
		{
			this.quantity = quantity;
			this.unit = unit;
			this.ingredientName = ingredientName;
			this.preparation = preparation;


		}


		public override string ToString()
		{
			return quantity + " " + unit + " " + ingredientName + " " + preparation;
		}
	}
}