using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;


namespace Mtg.Models
{
    public class User
    {
        public ObjectId Id { get; set; }

        [Required(ErrorMessage = "You must provide a username")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "You must provide an email address")]
        public string? Email { get; set; }
    }
}