using AspireSampleWeb.Crm;
using AspireSampleWeb.Infrastructure.Web;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddCshtmlComponents();
builder.Services.AddRazorPages();

// builder.Services.AddDbContext<CrmContext>(o => o.UseSqlite("Data Source=Crm.db", dbContextOptions => 
//     dbContextOptions
//     .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

// builder.AddNpgsqlDbContext<CrmContext>("crmdb", configureDbContextOptions: options => options
//         .UseNpgsql(dbContextOptions =>
//         dbContextOptions
//             .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
//             .EnableRetryOnFailure()));

#region DatabaseWithoutAspire
builder.Services.AddDbContext<CrmContext>((sp, options) => options
    .UseNpgsql(GetConnectionString(sp), dbContextOptions =>
        dbContextOptions
            .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
#endregion

builder.Services.AddHostedService<CreateDatabaseBackgroundService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
var api = app.MapGroup("api");
api.MapGet("deals", async (CrmContext dbContext) => await dbContext.Deals.AsNoTracking().ToListAsync());
api.MapPost("deals", async (CrmContext dbContext, Deal deal ) =>
{
    await dbContext.Deals.AddAsync(deal);
    await dbContext.SaveChangesAsync();
    return deal;
});
    

app.Run();

string GetConnectionString(IServiceProvider serviceProvider) => serviceProvider.GetRequiredService<IConfiguration>()
    .GetConnectionString("sampledb") ?? throw new Exception("Could not get connection string 'sampledb'");
