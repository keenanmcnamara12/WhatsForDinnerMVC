using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WhatsForDinnerMVC.Models
{
    [Serializable]
    public class Menu
    {
        private string myConnectionString = "server=127.0.0.1;uid=root;pwd=password;database=whatsfordinnerdb;";
        public int MenuID;

        public List<Recipe> Recipes {get; set;}
        
        public Menu(int MenuID)
        {
            this.MenuID = MenuID;
            Recipes = new List<Recipe>();

            try
            {
                MySqlConnection conn = new MySqlConnection(myConnectionString);
                conn.Open();
                using (MySqlCommand sqlCommand = new MySqlCommand("SELECT RecipeID FROM usermenus WHERE MenuID = @menuID;", conn))
                {
                    sqlCommand.Parameters.AddWithValue("@menuID", MenuID);
                    MySqlDataReader reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        Recipes.Add(new Recipe((int)reader[0]));
                    }
                    conn.Close();
                }
            }
            catch (MySqlException ex)
            { }

        }
    }
}