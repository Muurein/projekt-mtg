using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Projekt_mtg.Models
{
    public class Collection
    {
        public int Id { get; set; }

        //FK to CARD
        public int CardId { get; set; }

        //creates an object to better show data later - CardId=social security number, Card=the whole person
        public Card? Card { get; set; }

        //FK to USER, string forIdentityUser.Id is alway a sstring
        public string UserId { get; set; } = "";

        //where does the user want to place the card?
        public int OwnedQuantity { get; set; }

        public int WishlistQuantity { get; set; }

        public int InDeckQuantity { get; set; }
    }
}