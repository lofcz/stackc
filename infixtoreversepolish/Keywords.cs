using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace infixtoreversepolish
{
    class Keywords
    {
        public List<Keyword> keywords = new List<Keyword>();

        public Keywords()
        {
            keywords.Add(new Keyword {Name = "print"});
        }

        public Keyword GetKeyword(string name)
        {
            return keywords.FirstOrDefault(x => x.Name == name);
        }
    }

    class Keyword
    {
        public string Name {get; set;}
    }
}
