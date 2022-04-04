using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryCRUD.Services.Interfaces
{
    public interface IStringTransformer
    {
        string CleanString(string value);
        List<string> CleanStrings(List<string> values); 
    }
}
