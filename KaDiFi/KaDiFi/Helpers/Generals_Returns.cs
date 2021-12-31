using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace KaDiFi.Entities
{
    [NotMapped]
    public class General_Result
    {
        public General_Result(bool hasError = false)
        {
            HasError = hasError;
        }

        public bool HasError { get; set; }
        [DefaultValue("")]
        public string ErrorMessage { get; set; }
        public Dictionary<string, string> ErrorsDictionary { get; set; }
        public dynamic Data { get; set; }
    }

    [NotMapped]
    public class General_Status
    {
        public General_Status(bool isSuccess = true, string errorMessage = "")
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}
