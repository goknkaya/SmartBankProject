using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Desktop.Win.Core
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }
        public string ResponseBody { get; }

        public ApiException(int statusCode, string body, string message)
            : base(message)
        {
            StatusCode = statusCode;
            ResponseBody = body ?? string.Empty;
        }
    }
}
