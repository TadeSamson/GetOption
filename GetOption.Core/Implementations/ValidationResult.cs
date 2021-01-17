using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetOption.Core.Implementations
{
    public class ValidationResult
    {
        public ValidationResult()
        {
            ValidatorErrors = new List<string>();
        }
        public bool IsSucceeded { get; set; }
        public List<string> ValidatorErrors { get; set; }
    }
}
