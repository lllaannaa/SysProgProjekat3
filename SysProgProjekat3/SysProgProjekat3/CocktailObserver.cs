using System;
using System.Collections.Generic;
using System.Linq;

namespace SysProgProjekat3
{
    public class CocktailObserver : IObserver<Cocktail>
    {
        private List<string> ingredientsList = new List<string>(); // Lista za cuvanje sastojaka, svaki put kad se primi novi koktel u onNext sastojci se dodaju u ovu listu

        public CocktailObserver()
        {
        }


        //poziva se kad se zavrsi tok podataka
        public void OnCompleted() 
        {
            GenerateWordCloud();
            Console.WriteLine();
            Console.WriteLine("Svi kokteli su uspesno pribavljeni");
            Console.WriteLine();
        }


        //poziva se ako dodje do greske prilikom primanja podataka o koktelima
        public void OnError(Exception error)
        {
            Console.WriteLine();
            Console.WriteLine($"Doslo je do greske!: {error.Message}");
            Console.WriteLine();
        }


        //poziva se svaki put kad CocktailObserver primi novi koktel od CocktailStream
        public void OnNext(Cocktail cocktail)
        {
            // Dodavanje sastojaka u listu
            ingredientsList.AddRange(cocktail.Ingredients.Select(ingredient => ingredient.ToLower()));

            Console.WriteLine($"Ime koktela: {cocktail.Name}");
            Console.WriteLine($"Sastojci: {string.Join(", ", cocktail.Ingredients)}"); // Prikazuje sastojke
            Console.WriteLine();
        }

        private void GenerateWordCloud()
        {
            Dictionary<string, int> wordCloud = new Dictionary<string, int>(); //kljuc=sastojak, vrednost=broj pojavljivanja

            foreach (string ingredient in ingredientsList)
            {
                if (wordCloud.ContainsKey(ingredient))
                {
                    wordCloud[ingredient]++;
                }
                else
                {
                    wordCloud[ingredient] = 1;
                }
            }

            var sortedWordCloud = wordCloud.OrderByDescending(entry => entry.Value);

            foreach (KeyValuePair<string, int> entry in sortedWordCloud)
            {
                Console.WriteLine($"{entry.Key} : {entry.Value}");
            }
        }
    }
}
