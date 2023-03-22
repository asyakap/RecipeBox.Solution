using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using System;

namespace RecipeBox.Controllers
{
  [Authorize]
  public class RecipesController : Controller
  {
    private readonly RecipeBoxContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public RecipesController(UserManager<ApplicationUser> userManager, RecipeBoxContext db)
    {
      _userManager = userManager;
      _db = db;
    }
    
    public async Task<ActionResult> Index()
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);
      List<Recipe> model = _db.Recipes.OrderBy(recipe => recipe.Ranking).ToList();
      return View(model);
    }

    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Recipe recipe)
    {
      if (!ModelState.IsValid)
      {
        return View(recipe);
      }
      else
      {
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);
        recipe.User = currentUser;
        _db.Recipes.Add(recipe);
        _db.SaveChanges();
        return RedirectToAction("Index");
      }
    }


// WIP -- branching pages???
    // public async Task<ActionResult> Details(int id, Recipe recipe)
    // {    
    //     string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //     ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);
        
    //     //if(currentUser){
    //     //  currentuser = currentUser
    //     //}
    //     //else{
    //     //  currentUser = 0 (set to null so we can )
    //     //}

    //     ViewBag.CurrentUser = currentUser;

    //   Recipe thisRecipe = _db.Recipes
    //         .Include(recipe => recipe.JoinEntities)
    //         .ThenInclude(join => join.Tag)
    //         .FirstOrDefault(recipe => recipe.RecipeId == id);
    //   return View(thisRecipe);
    // }

// This one works, do not delete
    public ActionResult Details(int id)
    {
      string userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      ViewBag.CurrentUser = userId;

      Recipe thisRecipe = _db.Recipes
            .Include(recipe => recipe.JoinEntities)
            .ThenInclude(join => join.Tag)
            .FirstOrDefault(recipe => recipe.RecipeId == id);
      return View(thisRecipe);
    }

    public ActionResult Edit(int id)
    {
      Recipe thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
      return View(thisRecipe);
    }

    [HttpPost]
    public ActionResult Edit(Recipe recipe)
    {
      _db.Recipes.Update(recipe);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      Recipe thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
      return View(thisRecipe);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      Recipe thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
      _db.Recipes.Remove(thisRecipe);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult AddTag(int id)
    {
      Recipe thisRecipe = _db.Recipes.FirstOrDefault(recipes => recipes.RecipeId == id);
      ViewBag.TagId = new SelectList(_db.Tags, "TagId", "Cuisine");
      return View(thisRecipe);
    }

    [HttpPost]
    public ActionResult AddTag(Recipe recipe, int tagId)
    {
      #nullable enable
      RecipeTag? joinEntity = _db.RecipeTags.FirstOrDefault(join => (join.TagId == tagId && join.RecipeId == recipe.RecipeId));
      #nullable disable
      if (joinEntity == null && tagId != 0)
      {
        _db.RecipeTags.Add(new RecipeTag() { TagId = tagId, RecipeId = recipe.RecipeId });
        _db.SaveChanges();
      }
      return RedirectToAction("Details", new { id = recipe.RecipeId });
    }

    [HttpPost]
    public ActionResult DeleteJoin(int joinId)
    {
      RecipeTag joinEntry = _db.RecipeTags.FirstOrDefault(entry => entry.RecipeTagId == joinId);
      _db.RecipeTags.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }



    public async Task<ActionResult> Ranking()
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);
      List<Recipe> model = _db.Recipes.OrderBy(recipe => recipe.Ranking).ToList();
      return View(model);
    }

// This one works! Do not delete! -- Search by recipe title
    // [HttpPost, ActionName("Search")]
    // public ActionResult Search(string search)
    // {
    //   List<Recipe> model = _db.Recipes.Where(recipe => recipe.RecipeName == search).ToList();
    //   return View(model);
    // }

// This is what we want: searching through ingredients
    [HttpPost, ActionName("Search")]
    public ActionResult Search(string search)
    {
      List<Recipe> model = _db.Recipes.Where(recipe => recipe.Ingredients.ToLower()
                              .Contains(search.ToLower())).ToList();
      return View(model);
    }
  }
}