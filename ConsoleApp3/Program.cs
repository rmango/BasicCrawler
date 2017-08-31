using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace ConsoleApp3
{

    class Program
    {
        private static List<string> toSearch = new List<string> {"https://help.yahoo.com/kb/SLN4556.html", "https://myaccount.smud.org/browser", "https://myaccount.smud.org/browser/unsupported", "https://www.t-mobile.com/nonsupportedbrowser.aspx" };
        static void Main(string[] args)
        {
            for (int i = 0; i < toSearch.Count; i++)
            {
                string site = GetSiteString(toSearch[i]);
                while (site.Equals(""))
                {
                    Console.WriteLine("That website could not be found");
                    i++;
                    site = GetSiteString(toSearch[i]);
                }
                Console.WriteLine("Site: " + GetTitle(site));
                List<string> results = recipe(site);
                for(int k = 0; k < results.Count; k++)
                {
                    Console.WriteLine("Regex match found: " + results[k].ToString());
                }
                if (Console.KeyAvailable)
                {
                    Console.ReadLine();
                }
            }
            Console.ReadLine();
        }

        public static string GetTitle(string str)
        {
            try
            {
                return str.Substring(str.IndexOf(@"<title>") + 7, str.IndexOf("</title>") - str.IndexOf("<title>") - 7);

            }
            catch
            {
                return "Could not find title";
            }
        }

        public static string SearchForPhrase(string str)
        {
            /*Console.WriteLine("What string would you like to search for?");
            string toFind = Console.ReadLine();*/
            string toFind = "(Switch to Chrome|Look for this phrase|Edge not supported|Try a different browser|Google recommends using Chrome)";
            if (Regex.IsMatch(str, toFind) || (str.Contains("Internet Explorer") && str.Contains("Chrome") && !str.Contains("Edge")))
            {
                return "There is a match!";
            }
            else
            {
                return "No match :(";
            }
        }

        public static void SearchForSites(string str)
        {
            string pattern = @"\w+://\w+\.[^<>()"",;\s]+";

            int counter = 0;
            while (counter != -1)
            {
                if (Regex.IsMatch(str, pattern))
                {
                    Match match = Regex.Match(str, pattern);
                    Console.WriteLine("Website # " + counter + ": " + match.Value);
                    toSearch.Add(match.Value);
                    counter++;
                    str = str.Substring(str.IndexOf(match.Value) + match.Value.Length);
                }
                else
                {
                    Console.WriteLine("End of page");
                    counter = -1;
                }
            }
        }


        public static string GetSiteString(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //read
                Stream s = response.GetResponseStream();
                StreamReader reader = new StreamReader(s);
                string str = reader.ReadToEnd();
                //Console.Write(str);
                response.Close();

                return str;
            }
            catch
            {
                return "";
            }
        }

        public static List<String> recipe(string str)
        {
            //tests for phrases
            string switchPhraseString = @"(Switch to|Get|Use|Download|Install|Upgrad)\\s(\\w+\\s){0,5}(Chrome|Safari|firefox|Opera|Internet Explorer|IE)(\\r\\n|\\n|\\W|\\s)";
            string supportedPhraseString = @"(browser|Edge)\\s(\\w+\\s){0,5}(isn['’]t|not|no longer)(\\w|\\s)+(supported|compatible|up to date)(\\r\\n|\\n|\\W|\\s)";
            string upgradeBrowserString = @"Upgrade\\s(\\w+\\s){0,5}(browser)";
            string outdatedBrowserString = @"(browser|Edge)\\s(\\w+\\s){0,5}(incompatible|outdated|unsupported)(\\r\\n|\\n|\\W|\\s)";
            string[] needles = {switchPhraseString, supportedPhraseString, upgradeBrowserString, outdatedBrowserString };

            List<String> results = new List<String>();

            for (int i = 0; i < needles.Length; i++)
            {
                Console.WriteLine("has match? " + Regex.IsMatch(str, needles.ElementAt(i)));
                Console.WriteLine(str.IndexOf("browser"));
                if (Regex.IsMatch(str, needles.ElementAt(i)))
                {
                    Match match = Regex.Match(str, needles.ElementAt(i));
                    Console.WriteLine(match);
                    results.Add(Regex.Match(str, needles.ElementAt(i)).ToString());
                    str = str.Substring(str.IndexOf(match.Value) + match.Value.Length);

                }
            }
            return results;
        }
    }
}


