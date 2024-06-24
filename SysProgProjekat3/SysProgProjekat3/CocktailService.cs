using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SysProgProjekat3
{
    public class CocktailService
    {
        private readonly HttpClient client = new HttpClient();
        private const string Url = "https://www.thecocktaildb.com/api/json/v1/1/search.php?f=";

        public async Task<IEnumerable<Cocktail>> FetchCocktailsAsync(char firstLetter)
        {
            string UrlForSend = Url + firstLetter;

            var response = await client.GetAsync(UrlForSend); 
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var jsonResponse = JObject.Parse(content);
            var cocktailsJson = jsonResponse["drinks"];

            if (cocktailsJson == null || !cocktailsJson.HasValues)
            {
                throw new Exception($"Nema koktela sa pocetnim slovom '{firstLetter}'.");
            }

            List<Cocktail> cocktails = new List<Cocktail>();
            foreach (var cocktail in cocktailsJson)
            {
                var ingredients = new List<string>();
                for (int i = 1; i <= 15; i++)
                {
                    var ingredient = (string)cocktail[$"strIngredient{i}"];
                    if (!string.IsNullOrWhiteSpace(ingredient))
                    {
                        ingredients.Add(ingredient.Trim());
                    }
                }

                cocktails.Add(new Cocktail
                {
                    Name = (string)cocktail["strDrink"],
                    Ingredients = ingredients
                });
            }

            return cocktails;
        }
    }
}
