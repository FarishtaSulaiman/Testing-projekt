using ListLife.Data;
using ListLife.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ListLife.Pages
{
    public class CreateNewShoppingList : PageModel
    {
        // instans av databasen f�r att lagra listor
        private readonly ApplicationDbContext Dbcontext;

        // Hanterar anv�ndare via ASP.NET Identity (UserManager) f�r att h�mta och hantera inloggade anv�ndare
        private readonly UserManager<UserList> userManager;

        // Konstruktor
        public CreateNewShoppingList(ApplicationDbContext context, UserManager<UserList> userManager)
        {
            this.Dbcontext = context;
            this.userManager = userManager;
        }

        [BindProperty]
        // Property f�r att h�lla anv�ndarens shoppinglistor
        public ShoppingList ShoppingList { get; set; }

        public string UserListName { get; set; }

        public async Task OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User); // H�mta den inloggade anv�ndaren
            if (user != null)
            {
                UserListName = user.ListName; 
            }
        }

        public async Task<IActionResult> OnPostAsync() 
        {
            var user = await userManager.GetUserAsync(User);

            if (user != null)
            {
                // Koppla shoppinglistan till anv�ndaren och s�tt anv�ndarens ID
                ShoppingList.UserId = user.Id;
                ShoppingList.UserList = user;
                ShoppingList.Category ??= "Other";  // Om kategorin �r null, s�tt den till "�vrigt"
                ShoppingList.Title ??= "New List";  // Om titeln �r null, s�tt den till "Ny lista"


                // L�gg till shoppinglistan i databasen
                Dbcontext.ShoppingLists.Add(ShoppingList);
                await Dbcontext.SaveChangesAsync();  // Spara �ndringarna i databasen
            }

            return RedirectToPage("/MyPage");  // Omdirigera till annan sida efter att ha sparat listan
        }
    }
}
