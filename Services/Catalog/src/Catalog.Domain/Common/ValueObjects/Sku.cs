using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Catalog.Domain.Common.ValueObjects;
    public  class Sku : ValueObject
    {
        public string Value { get; private init; }

        private Sku(string value)
        {
            Value = value;
        }

        public static ErrorOr<Sku> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Error.Validation("SKU.Empty", "SKU cannot be empty.");
            }

            var regex = new Regex(@"^[A-Z0-9_-]{5,20}$");
            if (!regex.IsMatch(value))
            {
                return Error.Validation("SKU.Invalid", "SKU format is invalid (A-Z, 0-9, '-', '_', length 5-20).");
            }

            return new Sku(value.ToUpperInvariant());
        }

        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }