using ListLife.Data;
using ListLife.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ListLife.Pages
{
    public class CreateNewShoppingList : PageModel
    {
        // instans av databasen f�r att lagra listor
        private readonly ApplicationDbContext context;

        // Hanterar anv�ndare via ASP.NET Identity (UserManager) f�r att h�mta och hantera inloggade anv�ndare
        private readonly UserManager<IdentityUser> userManager;


        // Property f�r att h�lla anv�ndarens shoppinglistor
        public IList<ShoppingList> ShoppingLists { get; set; }

        // Konstruktor
        public CreateNewShoppingList(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // H�mta den inloggade anv�ndaren asynkront
            var user = await userManager.GetUserAsync(User); // User �r en inbyggd property i PageModel som inneh�ller information om den inloggade anv�ndaren

            if (user != null)
            {
                // H�mta alla shoppinglistor f�r den inloggade anv�ndaren
                ShoppingLists = context.ShoppingLists
                    .Where(shoppingList => shoppingList.UserId == user.Id)  // Filtrera baserat p� anv�ndarens ID
                    .ToList();

                context.Add(ShoppingLists);
                await context.SaveChangesAsync();                
            }
            return RedirectToPage("/MyPage");
        }
    }
}
