using System.Globalization;
using System.Text.RegularExpressions;
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

    // from microsoft: https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format?redirectedfrom=MSDN
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }    

        try
        {
            // Normalize the domain
            email = Regex.Replace(
                email, 
                @"(@)(.+)$", 
                DomainMapper,
                RegexOptions.None, 
                TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                string domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException e)
        {
            return false;
        }
        catch (ArgumentException e)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase, 
                TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }
}
