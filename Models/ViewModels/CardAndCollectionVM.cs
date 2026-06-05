
//so user can add Card to Collection, eg to be able to add data from both CardModel and CollectionModel
using System.ComponentModel.DataAnnotations;

namespace Projekt_mtg.Models.ViewModels
{
    public class CardAndCollectionVM
{
    //Card-data
    [Required]
    public string CardName { get; set; } = "";

    //Collection-data
    [Required]
    public int OwnedQuantity { get; set; }

    [Required]
    public int WishlistQuantity { get; set; }

    [Required]
    public int InDeckQuantity { get; set; }
}
}
