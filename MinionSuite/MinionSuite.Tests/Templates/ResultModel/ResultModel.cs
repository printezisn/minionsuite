using System;
using System.Collections.Generic;
using System.Linq;

namespace MinionSuite.Tests.Templates
{
    public class ResultModel<T>
    {
        public T Result { get; private set; }
        public IEnumerable<string> Errors { get; private set; }

        public bool IsSuccess => !Errors.Any();

        public ResultModel(T result, IEnumerable<string> errors)
        {
            Result = result;
            Errors = errors;
        }

        public ResultModel(T result)
            : this(result, new List<string>())
        {
        }

        public ResultModel(IEnumerable<string> errors)
            : this(default, errors)
        {
        }

        public ResultModel(string error)
            : this(default, new List<string>() { error })
        {
        }
    }
}
