using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Requests
{
    public record PaginationRequest(int Page, int PageSize = 10, string SearchTerm = "", string SortOrder = "asc", string ColumnOrder = "");
}
