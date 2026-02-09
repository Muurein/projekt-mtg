using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;


namespace Projekt_mtg.Models
{
    public class Set
    {
        public ObjectId Id { get; set; }

        [Required(ErrorMessage = "You must provide a name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "You must provide a relase date")]
        public DateOnly Relase_date { get; set; }

        [Required(ErrorMessage = "You must provide if the set is allowed in standard format games")]
        public bool Is_standard { get; set; } 


    }
}