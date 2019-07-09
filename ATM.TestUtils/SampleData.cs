using ATM.DataAccess.Models;
using System.Collections.Generic;

namespace ATM.TestUtils
{
    public class SampleData
    {
        public static readonly List<CreditCard> CREDITCARDS = new List<CreditCard>
        {
                new CreditCard { Id = 1, Number = "1111-1111-1111-1111", Pin = 1111, Balance = 1000, IsValid = true },
                new CreditCard { Id = 2, Number = "2222-2222-2222-2222", Pin = 2222, Balance = 500, IsValid = false },
                new CreditCard { Id = 3, Number = "1234-1234-1234-1234", Pin = 1234, Balance = 2000, IsValid = true }
        };
        public static readonly CreditCard CARD_NOT_ON_THE_LIST = new CreditCard { Id = 777, Number = "7777-7777-7777-7777", Pin = 7777, Balance = 777, IsValid = true };
    }
}
