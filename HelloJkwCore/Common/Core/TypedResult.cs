using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Core
{
    public class TypedResult<T>
    {
        public bool IsSuccess { get; private set; }
        public bool IsFail => !IsSuccess;
        public Exception Exception { get; private set; }
        public string FailReason { get; private set; }
        public T Result { get; private set; }

        public static TypedResult<T> Success(T result)
            => new TypedResult<T>
            {
                IsSuccess = true,
                Result = result,
            };

        public static TypedResult<T> Fail(Exception ex, string reason)
            => new TypedResult<T>
            {
                IsSuccess = false,
                Exception = ex,
                FailReason = reason,
            };

        public static TypedResult<T> Fail(string reason)
            => Fail(null, reason);

        public static TypedResult<T> Fail()
            => Fail(null, null);

    }
}
