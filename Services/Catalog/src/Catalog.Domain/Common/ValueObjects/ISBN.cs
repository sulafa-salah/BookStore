using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Catalog.Domain.Common.ValueObjects;
    public class ISBN : ValueObject
    {
        public string Value { get; private init; }

        private ISBN(string value)
        {
            Value = value;
        }

        public static ErrorOr<ISBN> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Error.Validation("ISBN.Empty", "ISBN cannot be empty.");
            }

            // ISBN-13 validation (starts with 978 or 979 and 13 digits)
            var regex = new Regex(@"^(97(8|9))?\d{9}(\d|X)$");
            if (!regex.IsMatch(value.Replace("-", "")))
            {
                return Error.Validation("ISBN.Invalid", "ISBN format is invalid.");
            }

            return new ISBN(value.Replace("-", ""));
        }

        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }