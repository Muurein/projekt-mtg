using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;


namespace Projekt_mtg.Models
{
    public class Set
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You must provide a name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "You must provide a relase date")]
        public DateOnly RelaseDate { get; set; }

        [Required(ErrorMessage = "You must provide if the set is allowed in standard format games")]
        public bool IsStandard { get; set; } = true;
        //sätter som true då många nya set man lägger till kommer vara standard

    }
}