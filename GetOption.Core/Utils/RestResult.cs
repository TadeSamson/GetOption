using System;
using System.Collections.Generic;
using System.Text;

namespace GetOption.Core.Utils
{
    public class HttpResult<T>
    {
        public bool IsSuccessful { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
        public int StatusCode { get; set; }
        private HttpResult(T data)
        {
            this.Data = data;
        }

        public static HttpResult<T> Success(T data)
        {
            HttpResult<T> restResult = new HttpResult<T>(data);
            restResult.IsSuccessful = true;
            restResult.StatusCode = 200;
            return restResult;
        }


        public static HttpResult<T> Failure(string error,int statusCode)
        {
            HttpResult<T> restResult = new HttpResult<T>(default(T));
            restResult.IsSuccessful = false;
            restResult.ErrorMessage = error;
            restResult.StatusCode = statusCode;
            return restResult;
        }
    }
}
