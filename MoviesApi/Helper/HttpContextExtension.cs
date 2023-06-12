using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Helper
{
    //add parameter to the header of response 
    public static class HttpContextExtension
{
        //InsertPaginationParametersInResponse is an extension method so it is static method
        //defined in static class and the first parameter is prefexed  with "this"
        //contains at least one parameter of type, we want to extend its functionality
      //  extend the existing functionality and call these methods as if they are part of the framework or the external library.
      //also can be done with our own classes
        public async static Task InsertPaginationParametersInResponse<T>(this HttpContext httpContext,
            IQueryable<T> queryable, int recordsPerPage)
        {
            if (httpContext == null) { throw new ArgumentNullException(nameof(httpContext)); }

            double count = await queryable.CountAsync();
            double totalAmountPages = Math.Ceiling(count / recordsPerPage);
            httpContext.Response.Headers.Add("totalAmountPages", totalAmountPages.ToString());
        }
    }
}
