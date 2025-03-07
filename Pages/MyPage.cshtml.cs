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

        // List to hold shopping lists for the logged-in user
        public IList<ShoppingList> ShoppingLists { get; set; }

        //[BindProperty]
        public ShoppingList EditList { get; set; }

        public ShoppingList AddProduct { get; set; }


        //Lista f�r att h�lla produkterna
        public IList<ShoppingList> ProductsInList { get; set; }

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

            return RedirectToPage(); // Ladda om sidan f�r att uppdatera listan
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

        // L�gg till ny produkt i vald lista
        public async Task<IActionResult> OnPostAddProductAsync(string product, string category, decimal amount, int listId)
        {
            // H�mtar listan som ska redigeras
            var editList = await _context.ShoppingLists.FindAsync(listId);

            var addNewProduct = new ShoppingList
            {
                Product = product,
                Category = category,
                Amount = amount,
                UserId = editList.UserId,
                UserList = editList.UserList
            };

            _context.ShoppingLists.Remove(editList);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        // Spara Editerad lista
        public async Task<IActionResult> OnPostSaveAsync(int listId)
        {
            return Page();
        }
    }
}
