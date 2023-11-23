namespace Maui.Components.Interfaces;

public interface ISearchable
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ImageSource Icon { get; set; }
    public Color IconBackgroundColor { get; set; }
    public string[] SearchableTerms { get; }
}
