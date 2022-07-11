using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Invoice.Application.Common.Models
{
    public class Result
    {
        internal Result(bool succeeded, object extra, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Extra = extra;
            Errors = errors.ToArray();
        }
        public bool Succeeded { get; set; }
        public string[] Errors { get; set; }
        public object Extra { get; set; }
        public static Result Success(object extra = null)
        {
            return new Result(true, extra, new string[] { });
        }

        public static Result Failed(IEnumerable<string> errors)
        {
            return new Result(false, null, errors);
        }

        public static Result Failed()
        {
            return new Result(false, null, new string[] { });
        }
    }

    public class Result<T>
    {
        internal Result(bool succeeded, object extra, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Extra = extra;
            Errors = errors.ToArray();
        }
        public bool Succeeded { get; set; }
        public string[] Errors { get; set; }
        public object Extra { get; set; }
        public T Data { get; set; }

        public static Result<T> Success(T data, object extra = null)
        {
            var result = new Result<T>(true, extra, new string[] { });
            result.Data = data;
            return result;
        }

        public static Result<T> Failed(IEnumerable<string> errors = null)
        {
            return new Result<T>(false, null, errors);
        }

        public static Result<T> Failed(T data, IEnumerable<string> errors = null)
        {
            var result = new Result<T>(false, null, new string[] { });
            result.Data = data;
            return result;
        }
    }
}
