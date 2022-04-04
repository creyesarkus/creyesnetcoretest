using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryCRUD.Services.Interfaces;
using System.Text.RegularExpressions;

namespace LibraryCRUD.Services
{
    public class StringTransformer : IStringTransformer
    {
        string FilterPattern = @"[\s\-\+\[\]\\*/?!@#$%^&*()0-9]";

        public string CleanString(string value)
        {
            string result = Regex.Replace(value, FilterPattern, "");
            return result.ToLower();
        }

        public List<string> CleanStrings(List<string> values)
        {
            List<string> result = new List<string>();
            
            foreach(string value in values)
            {
                result.Add(CleanString(value));
            }

            return result;
        }
    }
}
