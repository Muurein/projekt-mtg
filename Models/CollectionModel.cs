using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Projekt_mtg.Models
{
    public class Collection
    {
        public int Id { get; set; }

        //FK till CARD
        public int CardId { get; set; }

        //skapar ett objekt för att bättre kunna visa rätt data senare - CardId=personnummer, Card=hela personen
        public Card? Card { get; set; }

        //FK till USER, string för IdentityUser.Id är alltid string
        public string UserId { get; set; } = "";

        //where does the user want to place the card?
        public int OwnedQuantity { get; set; }

        public int WishlistQuantity { get; set; }

        public int InDeckQuantity { get; set; }
    }
}