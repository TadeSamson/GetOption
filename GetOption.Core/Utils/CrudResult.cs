using System;
using System.Collections.Generic;
using System.Text;

namespace GetOption.Core.Utils
{
    public class CrudResult<T>
    {
        public bool IsSuccessful { get; set; }
        public T Result { get; set; }
        public string ErrorMessage { get; set; }
        private CrudResult(T result)
        {
            this.Result = result;
        }

        public static CrudResult<T> Success(T result)
        {
            CrudResult<T> crudeResult= new CrudResult<T>(result);
            crudeResult.IsSuccessful = true;
            return crudeResult;
        }


        public static CrudResult<T> Failure(string error)
        {
            CrudResult<T> crudeResult = new CrudResult<T>(default(T));
            crudeResult.IsSuccessful = false;
            crudeResult.ErrorMessage = error;
            return crudeResult;
        }
    }
}
