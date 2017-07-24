using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhatsForDinnerMVC.Models
{
    public class Recipe
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public Recipe(int recipeID)
        {
            this.ID = recipeID; 
        }

    }
}