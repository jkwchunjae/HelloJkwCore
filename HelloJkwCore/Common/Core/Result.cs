using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Core
{
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public bool IsFail => !IsSuccess;
        public Exception Exception { get; private set; }
        public string FailReason { get; private set; }

        public static Result Success()
            => new Result
            {
                IsSuccess = true,
            };

        public static Result Fail(Exception ex, string reason)
            => new Result
            {
                IsSuccess = false,
                Exception = ex,
                FailReason = reason,
            };

        public static Result Fail(string reason)
            => Fail(null, reason);

        public static Result Fail()
            => Fail(null, null);
    }
}
