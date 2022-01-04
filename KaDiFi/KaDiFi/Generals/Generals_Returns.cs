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
            ErrorsDictionary = new Dictionary<string, string>();
        }

        public bool HasError { get; set; }
        [DefaultValue("")]
        public string ErrorMessage { get; set; }
        public Dictionary<string, string> ErrorsDictionary { get; set; }
    }

    public class General_ResultWithData : General_Result
    {
        public General_ResultWithData() : base() { }
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

    [NotMapped]
    public class General_StatusWithData : General_Status
    {
        public General_StatusWithData() : base() { }
        public dynamic Data { get; set; }
    }
}
