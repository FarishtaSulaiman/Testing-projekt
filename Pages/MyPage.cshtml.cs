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

        //Properties for the sharing list functionality
        [BindProperty]
        public string UserEmail { get; set; }
        public string Message { get; set; }

        //Hold the user's lists
        public IList<UserList> UserList { get; set; }

        // List to hold shopping lists for the logged-in user
        public IList<ShoppingList> ShoppingLists { get; set; }
        public IList<ShoppingList> SharedShoppingLists { get; set; } = new List<ShoppingList>();

        public IList<Product> Products { get; set; }

        [BindProperty]
        public ShoppingList EditList { get; set; }

        [BindProperty]
        public Product AddNewProduct { get; set; }/* = new ShoppingList();*/

        // H�mtar Shoppinglistorna
        public async Task OnGetAsync()
        {
            // Get user
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {

                ////Get user's shopping lists
                ShoppingLists = await _context.ShoppingLists.Where(u => u.UserId == user.Id).ToListAsync();

                // Get user's shopping lists and include related products
                //ShoppingLists = await _context.ShoppingLists
                //    .Where(u => u.UserId == user.Id)
                //    .Include(sl => sl.Products)  // Inkludera produkter
                //    .ToListAsync();

                //Get lists that are shared with the user
                SharedShoppingLists = await _context.SharedLists
                    .Where(sl => sl.SharedWithUserId == user.Id)
                    .Include(sl => sl.ShoppingList)
                    .Select(sl => sl.ShoppingList)
                    .ToListAsync();
            }
        }

        // H�mta ShoppingId baserat p� Id
        public async Task<IActionResult> OnGetEditAsync(int? id)
        {
            if (id == null)
            {
                // Return a 404 if the id is not provided or invalid
                return NotFound();
            }

            // Fetch the shopping list by id
            EditList = await _context.ShoppingLists
                .Include(sl => sl.Products) // Include the related products
                .FirstOrDefaultAsync(sl => sl.Id == id);

            if (EditList == null)
            {
                // Return a 404 if the shopping list is not found
                return NotFound();
            }

            // If the shopping list is found, return the page with the shopping list
            return Page();
        }



        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            // Get currently logged-in user ID
            var userId = _userManager.GetUserId(User);
            var newList = new UserList
            {
                ListName = Request.Form["ListName"],
                Id = userId
            };

            // Add list to database
            _context.Users.Add(newList);
            await _context.SaveChangesAsync();


            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostShareAsync(int listId)
        {
            if (string.IsNullOrWhiteSpace(UserEmail))
            {
                Message = "Enter a valid email address";
                return RedirectToPage();
            }
            var userToShareWith = await _userManager.FindByEmailAsync(UserEmail);
            if (userToShareWith == null)
            {
                //Message for alert if user not found
                TempData["Message"] = "User not found";
                TempData["MessageType"] = "error";
                return RedirectToPage();
            }
            //Control if the list is already shared with the user
            bool alreadyShared = await _context.SharedLists
                .AnyAsync(sl => sl.ShoppingListId == listId && sl.SharedWithUserId == userToShareWith.Id);

            if (alreadyShared)
            {
                //Message for alert if list is already shared
                TempData["Message"] = "List is already shared with this user";
                TempData["MessageType"] = "error";
                return RedirectToPage();
            }

            var sharedList = new SharedList
            {
                SharedWithUserId = userToShareWith.Id,
                ShoppingListId = listId
            };

            _context.SharedLists.Add(sharedList);
            await _context.SaveChangesAsync();

            //Message for alert if list is shared successfully
            TempData["Message"] = "List shared successfully!";
            TempData["MessageType"] = "success";
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

        // POST f�r att l�gga till ny produkt i listan och spara till databasen
        public async Task<IActionResult> OnPostAddProductAsync(int shoppingListId)
        {
            // H�mta shoppinglistan fr�n databasen
            var shoppingList = await _context.ShoppingLists
                .Include(sl => sl.Products) // Se till att produkterna �r inkluderade
                .FirstOrDefaultAsync(sl => sl.Id == shoppingListId);

            if (shoppingList == null)
            {
                return NotFound();
            }

            // Skapa ny produkt och l�gg till den i listan
            var newProduct = new Product
            {
                Name = AddNewProduct.Name,
                Amount = AddNewProduct.Amount,
                Category = AddNewProduct.Category,
                ShoppingListId = shoppingListId
            };

            // L�gg till produkten i databasen
            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            // Uppdatera shoppinglistan med den nya produkten
            EditList = shoppingList; // Uppdatera EditList s� att den �terspeglar de senaste f�r�ndringarna
            EditList.Products.Add(newProduct); // L�gg till den nya produkten i listan

            return Page(); // Skicka tillbaka samma sida s� att den renderas med den nya produkten
        }





    }
}