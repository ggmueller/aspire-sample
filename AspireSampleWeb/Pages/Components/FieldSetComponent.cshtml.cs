using System.Diagnostics.CodeAnalysis;
using Acmion.CshtmlComponent;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspireSampleWeb.Pages.Components;

[HtmlTargetElement(tag: "zyrrio-field-set")]
public class FieldSetComponent : CshtmlComponentBase
{
    public FieldSetComponent([NotNull] IHtmlHelper htmlHelper) : base(htmlHelper)
    {
    }

    public string Header { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool Disabled { get; set; } = false;
}
