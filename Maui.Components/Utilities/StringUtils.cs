using Maui.Components.Interfaces;

namespace Maui.Components.Utilities;

public static class StringUtils
{
    public static List<ISearchable> FilterList(
        this List<ISearchable> list, 
        string search)
    {
        if (string.IsNullOrEmpty(search))
        {
            return list;
        }

        string[] userInput = search.Split(" ", StringSplitOptions.RemoveEmptyEntries);

        List<ISearchable> filtered = new();
        foreach(ISearchable searchable in list)
        {
            int foundCount = 0;
            foreach(string checkAgainst in userInput)
            {
                foreach (string validWord in searchable.SearchableTerms)
                {
                    if (validWord.ToLower().Contains(checkAgainst.ToLower()))
                    {
                        foundCount++;
                    }   
                }    
            }

            if (foundCount == userInput.Length)
            {
                filtered.Add(searchable);
            }
        }

        return filtered;
    }
}
