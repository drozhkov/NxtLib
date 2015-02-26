﻿using System;
using System.Linq;
using System.Threading.Tasks;
using NxtLib;
using NxtLib.MonetarySystem;

namespace ReservableCurrenciesDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.WaitAll(PrintReservableCurrencies());
        }

        private async static Task PrintReservableCurrencies()
        {
            var service = new MonetarySystemService();
            var allCurrencies = await service.GetAllCurrencies();
            var reservableCurrencies = allCurrencies.Currencies
                .Where(c => c.Types.Contains(CurrencyType.Reservable))
                .OrderBy(c => c.Name)
                .ToList();

            Console.WriteLine("** Listing of all reservable currencies **");
            Console.WriteLine("");
            var total = 0M;

            foreach (var currency in reservableCurrencies)
            {
                var founders = await service.GetCurrencyFounders(currency.CurrencyId);

                Console.WriteLine("----------------------------");
                Console.WriteLine("Name: {0}", currency.Name);
                Console.WriteLine("Founders: {0}", founders.Founders.Count);
                if (founders.Founders.Count > 0)
                {
                    // Can't really get my head around how this really works, code below does not show correct values
                    var reserverdPerUnitNxt = founders.Founders.Sum(f => f.AmountPerUnit.Nxt);
                    var reserveRatio = reserverdPerUnitNxt/currency.MinReservePerUnit.Nxt/(decimal)Math.Pow(10, currency.Decimals);
                    var minimumReserveGoalNxt = currency.ReserveSupply*currency.MinReservePerUnit.Nxt;
                    var reservedNxt = reserveRatio*minimumReserveGoalNxt;

                    total += reservedNxt;
                    Console.WriteLine("Amount: {0} NXT", reservedNxt);
                }
                Console.WriteLine("");
            }

            Console.WriteLine("----------------------------");
            Console.WriteLine("Total reserved: {0} NXT", total);
            Console.WriteLine("");
            Console.WriteLine("Press any key to quit");
            Console.ReadLine();
        }
    }
}
