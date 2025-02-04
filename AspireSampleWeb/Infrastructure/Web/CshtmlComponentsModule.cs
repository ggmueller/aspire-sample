using Acmion.CshtmlComponent;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspireSampleWeb.Infrastructure.Web;

public static class CshtmlComponentsModule
{
    public static void AddCshtmlComponents(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICshtmlComponentTracker, CshtmlComponentTracker>();
        builder.Services.AddScoped<ITagHelperComponent, CshtmlComponentInjectionContentHandler>();
        builder.Services.AddScoped<ICshtmlComponentInjectionContentStore, CshtmlComponentInjectionContentStore>();
    }
}