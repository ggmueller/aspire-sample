using AspireSampleWeb.Crm;
using AspireSampleWeb.Infrastructure.Web;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddCshtmlComponents();
builder.Services.AddRazorPages();

// builder.Services.AddDbContext<CrmContext>(o => o.UseSqlite("Data Source=Crm.db", dbContextOptions => 
//     dbContextOptions
//     .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

builder.AddNpgsqlDbContext<CrmContext>("db", configureDbContextOptions: options => options
        .UseNpgsql(dbContextOptions =>
        dbContextOptions
            .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            .EnableRetryOnFailure()));

builder.Services.AddHostedService<CreateDatabaseBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.Run();