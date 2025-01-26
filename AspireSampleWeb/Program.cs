using Acmion.CshtmlComponent;
using AspireSampleWeb.Crm;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<ICshtmlComponentTracker, CshtmlComponentTracker>();
builder.Services.AddScoped<ITagHelperComponent, CshtmlComponentInjectionContentHandler>();
builder.Services.AddScoped<ICshtmlComponentInjectionContentStore, CshtmlComponentInjectionContentStore>();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<CrmContext>(o => o.UseSqlite("Data Source=Crm.db", sqlite => 
    sqlite
    .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
builder.Services.AddHostedService<CreateDatabaseService>();

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