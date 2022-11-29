namespace WebApplication1.Models;

public class LinksCollection
{
    public int Quantity => _listOfLinks.Count;
    
    private readonly List<string> _listOfLinks;
    
    public List<string> ListOfLinks => _listOfLinks;

    public LinksCollection(List<string> list)
    {
        _listOfLinks = list.Distinct().ToList();
    }

    public override string ToString()
    {
        var str = $"{Quantity}{Environment.NewLine}";
        foreach (var link in _listOfLinks)
        {
            str += $"<h3>{link}</h3>";
        }

        return str;
    }
}