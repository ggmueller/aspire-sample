using Acmion.CshtmlComponent;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspireSampleWeb.Pages.Components;

[HtmlTargetElement(ElementName)]
public class FieldComponent : CshtmlComponentBase
{
    private bool _required;
    private bool _requiredSet;
    public const string ElementName = "zyrrio-field";

    public FieldComponent(IHtmlHelper htmlHelper) : base(htmlHelper)
    {
    }

    public string? InputCssClasses { get; set; }
    public string? FieldCssClasses { get; set; }

    [HtmlAttributeName("asp-for")]
    public required ModelExpression For { get; set; }

    public string? AdditionalDescription { get; set; }

    public bool Required
    {
        get => _required;
        set
        {
            _required = value;
            _requiredSet = true;
        }
    }

    public bool IsRequiredSet => _requiredSet;

    public string? RequiredCssClass { get; set; }

    public string? Type { get; set; }
    
    public string? Label { get; set; }
    
    [HtmlAttributeNotBound]
    public List<ModelExpression> DisplayValidationsFor { get; } = new(0);

    public ReadOnlyTagHelperAttributeList AllAttributes { get; set; } = new TagHelperAttributeList();

    public bool IsRequiredByModel()
    {
        if (_requiredSet) return _required;
        var metadata = For?.ModelExplorer.Metadata as DefaultModelMetadata;
        return metadata?.ValidationMetadata.IsRequired ?? false;
    }

    protected override Task ProcessComponent(TagHelperContext context, TagHelperOutput output)
    {
        AllAttributes = output.Attributes;
        return Task.CompletedTask;
    }

}


