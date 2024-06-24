using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Concurrency;

namespace SysProgProjekat3
{
    public class Program
    {
        static HttpListener listener = new HttpListener();

        static async Task Main(string[] args)
        {
            listener.Prefixes.Add($"http://localhost:5050/");
            Console.WriteLine("Server je startovan...\n\n");
            listener.Start();

            while (true)
            {
                try
                {
                    HttpListenerContext context = await listener.GetContextAsync();
                    Task.Run(() => ProcessRequest(context)); //nova nit za svaki zahtev
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred: {ex.Message}");
                }
            }
        }

        static async Task ProcessRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            string input = request.Url.Segments.Last().Trim(); //poslednji segment URLa koji se koristi kao ulaz za pretragu koktela

            // Provera da li je input jedno slovo pomocu regularnog izraza
            if (!Regex.IsMatch(input, @"^[a-zA-Z]$"))
            {
                string errorMessage = "Nevalidan zahtev. Molimo unesite samo jedno slovo.";
                byte[] errorBytes = System.Text.Encoding.UTF8.GetBytes(errorMessage);
                response.StatusCode = (int)HttpStatusCode.BadRequest; 
                response.OutputStream.Write(errorBytes, 0, errorBytes.Length);
                response.Close();
                return;
            }

            Console.WriteLine($"Primljen zahtev za koktele koji pocinju na slovo: {input}");
            Console.WriteLine();

            var cocktailStream = new CocktailStream(); //kreiranje streama za dobijanje koktela na osnovu pocetnog slova
            var observer = new CocktailObserver(); //dodavanje observera za prikaz i obradu svakog koktela koji odgovara kriterijumu

            //filtriranje po prvom slovu(bez obzira da li je veliko ili malo)
            var filteredStream = cocktailStream.Where(cocktail => cocktail.Name.StartsWith(input, StringComparison.OrdinalIgnoreCase));

            var subscription = filteredStream  //zapocinje subscription na stream podataka koji dolazi iz filteredStream
                .SubscribeOn(Scheduler.NewThread) // operacije koje su nakon subscr. na filteredStream (obrada pod.) ce se izvrsavati na novoj niti a ne na glavnoj da bi se izbeglo blokiranje
                .ObserveOn(Scheduler.CurrentThread) // Emisija podataka na trenutnoj niti(gde je pozvan main) 
                .Subscribe(observer); //zavrsava subscription na stream, observer reaguje na podatke koje stream emituje

            await cocktailStream.GetCocktailsAsync(input); //pribavljanje koktela

            // Prekidamo subscription jer vise nije potreban za ovaj zahtev
            subscription.Dispose();

            // uspesna obrada zahteva
            string successMessage = "Zahtev je uspesno obradjen.";
            byte[] successBytes = System.Text.Encoding.UTF8.GetBytes(successMessage);
            response.StatusCode = (int)HttpStatusCode.OK;
            response.OutputStream.Write(successBytes, 0, successBytes.Length);
            response.Close();
        }
    }
}
