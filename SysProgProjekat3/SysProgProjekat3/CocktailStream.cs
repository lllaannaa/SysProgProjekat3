using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace SysProgProjekat3
{
    public class CocktailStream : IObservable<Cocktail>
    {
        private readonly Subject<Cocktail> cocktailsSubject = new Subject<Cocktail>();
        private readonly CocktailService cocktailService = new CocktailService();

        public async Task GetCocktailsAsync(string firstLetter)
        {
            try
            {
                var cocktails = await cocktailService.FetchCocktailsAsync(firstLetter[0]);
                foreach (var cocktail in cocktails)
                {
                    cocktailsSubject.OnNext(cocktail); //emituje svaki koktel u stream
                }
                cocktailsSubject.OnCompleted(); //obavetava da su svi podaci emitovani
            }
            catch (Exception ex)
            {
                cocktailsSubject.OnError(ex);
            }
        }

        public IDisposable Subscribe(IObserver<Cocktail> observer)
        {
            return cocktailsSubject.Subscribe(observer); //pretplacuje observer na stream
        }
    }
}
