using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspireSampleWeb.Infrastructure.Web;

[HtmlTargetElement(Attributes = "is-active-page")]
[Localizable(false)]
public class ActivePageTagHelper : TagHelper
{
    /// <summary>The name of the action method.</summary>
    /// <remarks>Must be <c>null</c> if <see cref="P:Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper.Route" /> is non-<c>null</c>.</remarks>
    [HtmlAttributeName(name: "asp-page")]
    public string? Page { get; set; }

    [HtmlAttributeName(name: "page-active-class")]
    public string? ActiveClass { get; set; }

    [HtmlAttributeName(name: "page-inactive-class")]
    public string? InActiveClass { get; set; }

    /// <summary>
    ///     Will show this page only as active, when the current page matches exactly. When false, subpages are also shown as
    ///     active
    /// </summary>
    [HtmlAttributeName("only-exact-matches")]
    public bool OnlyExactMatches { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="T:Microsoft.AspNetCore.Mvc.Rendering.ViewContext" /> for the current request.
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; } = null!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        MarkAs(output, ShouldBeActive());

        output.Attributes.RemoveAll(name: "is-active-page");
    }

    private bool ShouldBeActive()
    {
        var currentPage = ViewContext.RouteData.Values[key: "Page"]?.ToString();

        var linkedPage = OnlyExactMatches ? Page : RemoveIndex(Page);
        return string.IsNullOrWhiteSpace(linkedPage) ||
                currentPage is null ||
                currentPage.StartsWith(linkedPage, StringComparison.OrdinalIgnoreCase);
    }

    private string? RemoveIndex(string? page)
    {
        if (page == null) return null;

        if (page.EndsWith("/Index", StringComparison.OrdinalIgnoreCase)) return page.Substring(0, page.Length - 5);
        return page;
    }

    private void MarkAs(TagHelperOutput output, bool isActive)
    {
        var cssClassValue = isActive ? ActiveClass ?? "active" : InActiveClass ?? "";
        var classAttr = output.Attributes.FirstOrDefault(a => a.Name == "class");

        if (classAttr == null)
        {
            classAttr = new TagHelperAttribute(name: "class", cssClassValue);
            output.Attributes.Add(classAttr);
        }
        else if (cssClassValue == "" || classAttr.Value.ToString()!.IndexOf(cssClassValue, StringComparison.Ordinal) < 0)
        {
            output.Attributes.SetAttribute(name: "class", classAttr.Value == null
                    ? cssClassValue
                    : cssClassValue + " " + classAttr.Value);
        }

        if (isActive)
            output.Attributes.Add(new TagHelperAttribute(name: "aria-current", value: "page"));
    }
}
