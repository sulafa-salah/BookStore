using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Common.ValueObjects;
    public  class Money : ValueObject
    {
        public decimal Amount { get; private init; }
        public string Currency { get; private init; }

        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static ErrorOr<Money> Create(decimal amount, string currency = "USD")
        {
            if (amount < 0)
            {
                return Error.Validation("Money.Negative", "Amount cannot be negative.");
            }

            if (string.IsNullOrWhiteSpace(currency))
            {
                return Error.Validation("Money.Currency", "Currency must be provided.");
            }

            return new Money(decimal.Round(amount, 2), currency.ToUpper());
        }

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
            {
                throw new InvalidOperationException("Cannot add money with different currencies.");
            }

            return new Money(Amount + other.Amount, Currency);
        }

        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public override string ToString() => $"{Amount:0.00} {Currency}";
    }