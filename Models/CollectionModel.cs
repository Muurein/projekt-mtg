using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Projekt_mtg.Models
{
    public class Collection
    {
        public int Id { get; set; }

        // FK till CARD
        public int CardId { get; set; }

        //skapar ett objekt för att bättre kunna visa rätt data senare - CardId=personnummer, Card=hela personen
        public Card? Card { get; set; }

        // FK till USER, tring för IdentityUser.Id är alltid string
        public string UserId { get; set; }

        // ska kunna vara owned, in deck, wishlist
        [Required(ErrorMessage = "You must provide if the card is owned, in a deck or in your wishlist")]
        public string Status { get; set; }

        [Required(ErrorMessage = "You must provide the quantity of cards")]
        public int Quantity { get; set; }
    }
}