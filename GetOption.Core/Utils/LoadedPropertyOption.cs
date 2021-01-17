using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetOption.Core.Utils
{
    public class LoadedPropertyOption<T>
    {
        public LoadedPropertyOption() 
        {
            this.LoadedProperties = new List<string>();
        }
        public List<string> LoadedProperties { get; set; }

        public LoadedPropertyOption<T> ToPascal()
        {
            LoadedPropertyOption<T> loadedPropertyOption = new LoadedPropertyOption<T>();
            foreach (var lp in this.LoadedProperties)
            {
                var asList = lp.Split(new char['_']).ToList();
                asList.ForEach(a => a = a.First().ToString().ToUpper() + a.Substring(1, a.Length - 1));
                loadedPropertyOption.LoadedProperties.Add(string.Join("", asList));
            }
            return loadedPropertyOption;
        }
    }
}
