using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecipeBox.Models
{
  public class Recipe
  {
    public int RecipeId { get; set; }
    [Required(ErrorMessage = "The recipe's name can't be empty!")]
    public string RecipeName { get; set; }
    
    [Required(ErrorMessage = "The recipe's ingredients can't be empty!")]
    public string Ingredients { get; set; }

    [Required(ErrorMessage = "The recipe's instructions can't be empty!")]
    public string Instructions { get; set; }
    public Rating Ranking { get; set; }

    public List<RecipeTag> JoinEntities { get;}
    public ApplicationUser User { get; set; }
    public string UserId { get; set; }
    
  }

  public enum Rating
  {
    excellent,
    good,
    alright
  }

}