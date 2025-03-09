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

        // List to hold shopping lists for the logged-in user
        public IList<ShoppingList> ShoppingLists { get; set; }

        [BindProperty]
        public ShoppingList EditList { get; set; }

        [BindProperty]
        public ShoppingList AddNewProduct { get; set; } = new ShoppingList();


        ////Lista f�r att h�lla produkterna
        //public IList<ShoppingList> ProductsInList { get; set; }

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

        public async Task<IActionResult> OnPostSaveChangesAsync(int listId)
        {
            // H�mta shoppinglistan
            var shoppingList = await _context.ShoppingLists
                .FirstOrDefaultAsync(x => x.Id == listId);

            if (shoppingList == null)
            {
                return NotFound("TEST");
            }

            // Uppdatera shoppinglistan med de nya v�rdena
            shoppingList.Title = EditList.Title;
            shoppingList.Category = EditList.Category;

            // Spara �ndringar till databasen
            _context.ShoppingLists.Update(shoppingList);
            await _context.SaveChangesAsync();

            return RedirectToPage(); 
        }


        // POST f�r att l�gga till ny produkt
        public async Task<IActionResult> OnPostAddProductAsync(int listId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // H�mta inloggad anv�ndare
            var user = await _userManager.GetUserAsync(User);

            // H�mta shoppinglistan fr�n databasen
            var shoppingList = await _context.ShoppingLists
                .FirstOrDefaultAsync(x => x.Id == listId && x.UserId == user.Id);

            if (shoppingList == null)
            {
                return NotFound();
            }

            // Hantera att l�gga till den nya produkten i listan
            if (!string.IsNullOrEmpty(shoppingList.Product))
            {
                // Om det finns redan produkter, l�gg till den nya produkten med komma-separation
                shoppingList.Product += ", " + AddNewProduct.Product;
            }
            else
            {
                // Om inga produkter finns, s�tt den f�rsta produkten
                shoppingList.Product = AddNewProduct.Product;
            }

            // L�gg till m�ngden och uppdatera kategori (om det �r n�dv�ndigt)
            shoppingList.Amount += AddNewProduct.Amount;
            shoppingList.Category = AddNewProduct.Category;

            // Uppdatera shoppinglistan i databasen
            _context.ShoppingLists.Update(shoppingList);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
