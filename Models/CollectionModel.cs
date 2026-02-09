using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;


namespace Projekt_mtg.Models
{
    public class Collection
    {
        public ObjectId Id { get; set; }

        public ObjectId CardId { get; set; }

        public ObjectId UserId { get; set; }

        [Required(ErrorMessage = "You must provide if the card is owned, in a deck or in your wishlist")]
        public string? Status { get; set; }

        [Required(ErrorMessage = "You must provide the quantity of cards")]
        public string? Quantity { get; set; }
    }
}