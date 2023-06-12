using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WhoisNET.Models
{
    public class WhoisModel
    {
        private Dictionary<string, string> WhoisData { get; set; }
        
        public void Load(string Whois)
        {
            //Regex Reg = new(@"(.*):\s+(.*?)\s+", RegexOptions.Singleline);
            var WhoisMatch = Regex.Matches(Whois, @"(.*):\s+(.+)\s+", RegexOptions.Multiline);


            foreach (Match Item in WhoisMatch)
            {
                string Name = Item.Groups[1].Value.Trim();
                string Value = Item.Groups[2].Value.Trim();

                Console.WriteLine($"{Name} ==== {Value}");
            }

            /*
                         if (WhoisMatch.Success)
            {
                while (WhoisMatch.NextMatch().)
                {
                    string Name = WhoisMatch.Groups[1].Value.Trim();
                    string Value = WhoisMatch.Groups[2].Value.Trim();

                    Console.Write($"{Name} ==== {Value}");
                }
                

              //  foreach (var Item in WhoisMatch.Captures)
              //  {
              //      Console.WriteLine($"{Item}");
              //  }

              //  foreach (var Item in WhoisMatch.Groups)
              //  {
              //      Console.WriteLine($"Name: {Item}");
              //  }
            }
             */
        }

    }
}
