using System.ComponentModel.DataAnnotations;

namespace Projekt_mtg.Models
{
    public class Card
    {
        //properties - fields in database
        public int Id { get; set; }

        public int SetId { get; set; }

        [Required(ErrorMessage = "You must provide a title")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "You must provide at least one color")]
        public string? Color { get; set; }

        [Required(ErrorMessage = "You must provide a mana value")]
        public string? ManaValue { get; set; }

        [Required(ErrorMessage = "You must provide a type")]
        public string? Type { get; set; }

        [Required(ErrorMessage = "You must provide a description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "You must provide a level of rarity")]
        public string? Rarity { get; set; }

    }
}