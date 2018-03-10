using System;
using System.Collections.Generic;
using System.Linq;

namespace NexPayApi.Filters
{
    internal sealed class ValidationFailure
    {
        public string Message { get; set; }
        public IDictionary<string, IEnumerable<string>> ModelState { get; set; } = new Dictionary<string, IEnumerable<string>>();

        public override string ToString()
        {
            return $"{Message}{Environment.NewLine}{string.Join(Environment.NewLine, ModelState.Select(m => string.Join(Environment.NewLine, m.Value.Select(v => $"{m.Key}: {v}"))))}";
        }
    }
}