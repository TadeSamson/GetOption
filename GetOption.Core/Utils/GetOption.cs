using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetOption.Core.Utils
{



    /// represents a class passed as parameter to all respository GET method to specify request settings
    /// </summary>
    public class GetOption<T>
    {
        /// <summary>
        /// Gets or sets the pagination.
        /// </summary>
        /// <value>
        /// The pagination option for api parameter.
        /// </value>
        public PaginationOption<T> PaginationOption { get; set; }
        /// <summary>
        /// Gets or sets the search_terms.
        /// </summary>
        /// <value>
        /// The search_terms option for api parameters. a string combination of key value pair seperated by =. e.g. title=startup entrepreneurship
        /// </value>
        public SearchOption<T> SearchOption { get; set; }
        /// <summary>
        /// Gets or sets the sort.
        /// </summary>
        /// <value>
        /// The sort.
        /// </value>
        public SortOption<T> SortOption { get; set; }

        /// <summary>
        /// Gets or sets the properties that is intended to load with the request response.
        /// </summary>
        /// <value>
        /// The properties to load.
        /// </value>
        public LoadedPropertyOption<T> LoadedPropertyOption { get; set; }

        public GetOption<T> ToPascal()
        {
            GetOption<T> getOption = new GetOption<T>();
            getOption.PaginationOption = this.PaginationOption==null?null: this.PaginationOption.ToPascal();
            getOption.LoadedPropertyOption = this.LoadedPropertyOption==null?null: this.LoadedPropertyOption.ToPascal();
            getOption.SearchOption = this.SearchOption==null?null: this.SearchOption.ToPascal();
            getOption.SortOption = this.SortOption==null?null: this.SortOption.ToPascal();
            return getOption;
        }
    }



   
}
