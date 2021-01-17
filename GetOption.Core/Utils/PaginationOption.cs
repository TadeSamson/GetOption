using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetOption.Core.Utils
{
    /// <summary>
    /// represent a class passed into an api instance which represents how the current api request should be paginated
    /// </summary>
    public class PaginationOption<T>
    {
        /// <summary>
        /// Gets or sets the page_no of the current pagination request.
        /// </summary>
        /// <value>
        /// The page_no.
        /// </value>
        public int PageNo { get; set; }
        /// <summary>
        /// Gets or sets the page_size of the pagination option.
        /// </summary>
        /// <value>
        /// The page_size.
        /// </value>
        public int PageSize { get; set; }

        public PaginationOption<T> ToPascal()
        {
            PaginationOption<T> paginationOption = new PaginationOption<T>();
            paginationOption.PageNo = this.PageNo;
            paginationOption.PageSize = this.PageSize;
            return paginationOption;
        }

       public List<T> Paginate<T>(List<T> TList)
        {

                var startIndex = (PageNo - 1) * PageSize;

                if (startIndex >= TList.Count)
                {
                    return new List<T>();
                }
                if (startIndex + PageSize <= TList.Count)
                {
                    return TList.GetRange(startIndex, PageSize);
                }

                else
                {
                    return TList.GetRange(startIndex, TList.Count - startIndex);
                }


        }
    }
}
