using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ListLife.Data;
using ListLife.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace ListLife.Pages
{
    [Authorize]
    public class MyPageModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserList> _userManager;

        public MyPageModel(ApplicationDbContext context, UserManager<UserList> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Hold the user's lists
        public IList<UserList> UserList { get; set; }

        public ShoppingList ShoppingList { get; set; }


        // List to hold shopping lists for the logged-in user
        public IList<ShoppingList> ShoppingLists { get; set; }

        //public IList<Product> Products { get; set; }

        [BindProperty]
        public ShoppingList EditList { get; set; }

        [BindProperty]
        public ShoppingList AddNewProduct { get; set; }/* = new ShoppingList();*/

        // H�mtar Shoppinglistorna
        public async Task OnGetAsync()
        {
            //Get user
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                
                //Get user's shopping lists
                ShoppingLists = await _context.ShoppingLists.Where(u => u.UserId == user.Id).ToListAsync();
            }

        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            //Get currently logged in user id
            var userId = _userManager.GetUserId(User);
            var newList = new UserList
            {
                ListName = Request.Form["ListName"],
                Id = userId
            };

            //Add list to database
            _context.Users.Add(newList);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int listId)
        {
            // H�mta shoppinglistan fr�n databasen baserat p� listId
            var deleteList = await _context.ShoppingLists.FindAsync(listId);

            if (deleteList != null)
            {
                _context.ShoppingLists.Remove(deleteList);
                await _context.SaveChangesAsync(); 
            }

            return RedirectToPage(); 
        }


        public async Task<IActionResult> OnPostEditAsync(int listId)
        {
            // H�mtar listan som ska redigeras
            var editList = await _context.ShoppingLists.FindAsync(listId);

            if (editList == null)
            {
                return NotFound(); 
            }

            EditList = editList;

            return Page();
        }



        public async Task<IActionResult> OnPostSaveChangesAsync(int shoppingListId)
        {
            var shoppingList = await _context.ShoppingLists
                .Include(sl => sl.Products)  // Inkludera produkterna relaterade till shoppinglistan
                .FirstOrDefaultAsync(sl => sl.Id == shoppingListId && sl.UserId == _userManager.GetUserId(User));

            if (shoppingList == null)
            {
                return NotFound();  
            }

            shoppingList.Title = EditList.Title;
            shoppingList.Category ??= "Other";  // Om kategorin �r null, s�tt den till "Other"
            shoppingList.Product = EditList.Product;
            shoppingList.Amount = EditList.Amount;

            _context.ShoppingLists.Update(shoppingList);
            await _context.SaveChangesAsync();

            return Page();
        }


        // POST f�r att l�gga till ny produkt
        public async Task<IActionResult> OnPostAddProductAsync(int shoppingListId, string productName, int amount, string category)
        {
            // H�mta shoppinglistan fr�n databasen baserat p� shoppingListId
            var shoppingList = await _context.ShoppingLists
                .FirstOrDefaultAsync(sl => sl.Id == shoppingListId);  // Hitta shoppinglistan med r�tt Id

            // Skapa en ny produkt och koppla den till shoppinglistan
            var newProduct = new Product
            {
                Name = productName,
                Amount = amount,
                Category = string.IsNullOrWhiteSpace(category) ? "Other" : category,
                ShoppingListId = shoppingList.Id
            };

            shoppingList.Products.Add(newProduct);

            _context.ShoppingLists.Update(shoppingList);
            await _context.SaveChangesAsync();

            return Page();
        }

    }
}
