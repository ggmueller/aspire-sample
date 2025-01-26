using System.ComponentModel.DataAnnotations;
using AspireSampleWeb.Crm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AspireSampleWeb.Pages;

public class DealPage(CrmContext dbContext) : PageModel
{
    public class DealInput
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public double Price { get; set; }
    }
    
    [BindProperty]
    public required DealInput Input { get; set; }
    
    public List<Deal> Deals { get; set; } = [];

    public async Task<IActionResult> OnGet(CancellationToken cancellationToken)
    {
        Deals = await dbContext.Deals.ToListAsync(cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        var deal = new Deal()
        {
            Name = Input.Name,
            Price = Input.Price
        };

        dbContext.Add(deal);
        await dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToPage();
    }
}