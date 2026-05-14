using Projekt_mtg.Models;
using Projekt_mtg.Models.ViewModels;

//för att kunna lägga till Card i Collection, med andra ord för att kunna lägga till data från båda modellerna
namespace Projekt_mtg.Models.ViewModels
{
    public class CardAndCollectionVM
{
    //Card-data
    //public Card Card { get; set; } = new Card();
    public string CardName { get; set; } = "";


    //Collection-data
    //status = owned/wishlist/in deck
    public string Status { get; set; } = "";

    //antal kort
    public int Quantity { get; set; }
}
}
