// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using SampleSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using Task.Data;

// Version Mad01

namespace SampleQueries
{
    [Title("LINQ Module")]
    [Prefix("Linq")]
    public class LinqSamples : SampleHarness
    {

        private DataSource dataSource = new DataSource();
        [Category("Restriction Operators")]
        [Title("Where - Task 1")]
        [Description("Выдайте список всех клиентов, чей суммарный оборот (сумма всех заказов) превосходит некоторую величину X. Продемонстрируйте выполнение запроса с различными X (подумайте, можно ли обойтись без копирования запроса несколько раз)")]
        public void Linq1()
        {
            decimal[] numbers = { 50000, 40000, 10000, 30000, 90000, 80000, 60000, 70000, 20000 };
            foreach (var item in numbers)
            {
                Console.WriteLine($"суммарный оборот>{item}");
                var customers = dataSource.Customers.Where(x => x.Orders.Sum(y => y.Total) > item);
                foreach (var name in customers)
                {
                    Console.WriteLine(name.CompanyName);
                }
                Console.WriteLine("-------------------------");
            }
        }
        [Category("Restriction Operators")]
        [Title("Where - Task 2")]
        [Description("Для каждого клиента составьте список поставщиков, находящихся в той же стране и том же городе. Сделайте задания с использованием группировки и без.")]

        public void Linq2()
        {
            foreach (var customer in dataSource.Customers)
            {
                Console.WriteLine($"{customer.CompanyName}: ");
                foreach (var supplier in dataSource.Suppliers.Where(x => x.Country == customer.Country && x.City == customer.City))
                {
                    Console.WriteLine(supplier.SupplierName);
                }
                Console.WriteLine("-------------------------");
            }
            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
            var result = dataSource.Customers.GroupJoin(
                dataSource.Suppliers,
                x => x.City,
                y => y.City,
                (cust, sup) => new
                {
                    NameCust = cust.CompanyName,
                    NameSup = sup.Select(z => z.SupplierName)
                }
                );
            foreach (var item in result)
            {
                Console.WriteLine($"{item.NameCust}:");
                foreach (var sup in item.NameSup)
                {
                    Console.WriteLine(sup);
                }
                Console.WriteLine("-------------------------");
            }
        }
        [Category("Restriction Operators")]
        [Title("Where - Task 3")]
        [Description("Найдите всех клиентов, у которых были заказы, превосходящие по сумме величину X")]

        public void Linq3()
        {
            decimal price = 1111;
            Console.WriteLine($"sum total>{price}");
            foreach (var item in dataSource.Customers.Where(x=>x.Orders.Max(y=>y.Total)>price))
            {
                Console.WriteLine(item.CompanyName);
            }
        }
        [Category("Restriction Operators")]
        [Title("Where - Task 4")]
        [Description("Выдайте список клиентов с указанием, начиная с какого месяца какого года они стали клиентами (принять за таковые месяц и год самого первого заказа)")]

        public void Linq4()
        {
            foreach (var item in dataSource.Customers.Where(z=>z.Orders.Length!=0))
            {
                var order =item.Orders.OrderBy(x => x.OrderDate.Year).ThenBy(z => z.OrderDate.Month).First();
                Console.WriteLine($"Name:{item.CompanyName} year:{order.OrderDate.Year} month:{order.OrderDate.Month}");
            }
        }
        [Category("Restriction Operators")]
        [Title("Where - Task 5")]
        [Description("Сделайте предыдущее задание, но выдайте список отсортированным по году, месяцу, оборотам клиента (от максимального к минимальному) и имени клиента")]

        public void Linq5()
        {

            List<dynamic> list = new List<dynamic>();
            foreach (var item in dataSource.Customers.Where(y=>y.Orders.Length!=0))
            {
                var data =  new { Name = item.CompanyName, Сirculation = item.Orders.Sum(x => x.Total), Date = item.Orders.First().OrderDate } ;
                list.Add(data);

            }
            
            foreach (var item in list.OrderBy(x=>x.Date.Year).ThenBy(y=>y.Date.Month).ThenByDescending(z=>z.Сirculation).ThenBy(t=>t.Name))
            {
                Console.WriteLine(item.Name);
            }
        }
        [Category("Restriction Operators")]
        [Title("Where - Task 6")]
        [Description("Укажите всех клиентов, у которых указан нецифровой почтовый код или не заполнен регион или в телефоне не указан код оператора (для простоты считаем, что это равнозначно «нет круглых скобочек в начале»).")]

        public void Linq6()
        {
            foreach (var item in dataSource.Customers.Where(x => string.IsNullOrEmpty(x.Region)==true || x.Phone.StartsWith("(")==false || int.TryParse(x.PostalCode, out _) != true))
            {
                Console.WriteLine(item.CompanyName);
            }
        }
        [Category("Restriction Operators")]
        [Title("Where - Task 7")]
        [Description("Сгруппируйте все продукты по категориям, внутри – по наличию на складе, внутри последней группы отсортируйте по стоимости")]

        public void Linq7()
        {
            var data =dataSource.Products.GroupBy(x => x.Category).Select(y => y.GroupBy(z=>z.UnitsInStock).Select(q=>q.OrderBy(w=>w.UnitPrice)));
            foreach (var item in data)
            {
                foreach (var item2 in item)
                {
                    foreach (var item3 in item2)
                    {
                        Console.WriteLine($"name: {item3.ProductName} price: {item3.UnitPrice}");
                    }
                }
            }
        }
        [Category("Restriction Operators")]
        [Title("Where - Task 8")]
        [Description("Сгруппируйте товары по группам «дешевые», «средняя цена», «дорогие». Границы каждой группы задайте сами")]

        public void Linq8()
        {
            const decimal cheap = 5.0000M;
            
            const decimal expensive = 24.0000M;
            var listCheap = dataSource.Products.GroupBy(x => x.UnitPrice < cheap).Where(y=>y.Key==true);
            var listAverage = dataSource.Products.GroupBy(x => x.UnitPrice <= expensive&&x.UnitPrice>=cheap).Where(y => y.Key == true);
            var listExpensive = dataSource.Products.GroupBy(x => x.UnitPrice > expensive).Where(y => y.Key == true);
            Console.WriteLine("----cheap----");
            foreach (var item in listCheap)
            {
                foreach (var data in item)
                {
                    Console.WriteLine($"Name: {data.ProductName}");
                }
            }
            Console.WriteLine();
            Console.WriteLine("----average----");
            foreach (var item in listAverage)
            {
                foreach (var data in item)
                {
                    Console.WriteLine(data.ProductName);
                }
            }
            Console.WriteLine();
            Console.WriteLine("----expensive----");
            foreach (var item in listExpensive)
            {
                foreach (var data in item)
                {
                    Console.WriteLine(data.ProductName);
                }
            }
        }
        [Category("Restriction Operators")]
        [Title("Where - Task 9")]
        [Description("Рассчитайте среднюю прибыльность каждого города (среднюю сумму заказа по всем клиентам из данного города) и среднюю интенсивность (среднее количество заказов, приходящееся на клиента из каждого города)")]

        public void Linq9()
        {
           
            var list=dataSource.Customers.GroupBy(x => x.City)
                .Select(x => new
                {
                    City = x.Key,
                    AverageOrdersIntensity = x.Average(y => y.Orders.Length),
                    AverageOrdersProfitability= x.Average(z=>z.Orders.Sum(t=>t.Total))
                }).ToList();
            Console.WriteLine("       ");
            foreach (var item in list)
            {
                Console.WriteLine($"City:{item.City} Intensity: {item.AverageOrdersIntensity}  Profitability: {item.AverageOrdersProfitability}");
            }
        }
        [Category("Restriction Operators")]
        [Title("Where - Task 10")]
        [Description("Сделайте среднегодовую статистику активности клиентов по месяцам (без учета года), статистику по годам, по годам и месяцам (т.е. когда один месяц в разные годы имеет своё значение).")]

        public void Linq10()
        {
            var list = dataSource.Customers.Select(x=> new
            { 
               
                byMonth=x.Orders.GroupBy(m=>m.OrderDate.Month).Select(y=> new
                { 
                    Month=y.Key,
                    Profitability=y.Count()
                }
                    ),
                byYear = x.Orders.GroupBy(m => m.OrderDate.Year).Select(y => new
                {
                    Year = y.Key,
                    Profitability = y.Count()
                }
                    ),
                byMonthAndYear = x.Orders.GroupBy(m => new { m.OrderDate.Year, m.OrderDate.Month }).Select(y => new
                {
                    Month = y.Key.Month,
                    Year = y.Key.Year,
                    Profitability = y.Count()
                }
                    ),
            }
            );
            foreach (var item in list)
            {
                Console.WriteLine($"by Month");
                foreach (var itemM in item.byMonth)
                {
                    Console.WriteLine($"Month: {itemM.Month} Profitability:{itemM.Profitability} ");
                }
                Console.WriteLine($"by Year");
                foreach (var itemY in item.byYear)
                {
                    Console.WriteLine($"Year: {itemY.Year} Profitability:{itemY.Profitability} ");
                }
                Console.WriteLine($"by Month and Year");
                foreach (var itemMAY in item.byMonthAndYear)
                {
                    Console.WriteLine($"Month: {itemMAY.Month} Year: {itemMAY.Year} Profitability:{itemMAY.Profitability} ");
                }
            }
        }
        //[Category("Restriction Operators")]
        //[Title("Where - Task 1")]
        //[Description("This sample uses the where clause to find all elements of an array with a value less than 5.")]
        //public void Linq1()
        //{
        //	int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

        //	var lowNums =
        //		from num in numbers
        //		where num < 5
        //		select num;

        //	Console.WriteLine("Numbers < 5:");
        //	foreach (var x in lowNums)
        //	{
        //		Console.WriteLine(x);
        //	}
        //}

        //[Category("Restriction Operators")]
        //[Title("Where - Task 2")]
        //[Description("This sample return return all presented in market products")]

        //public void Linq2()
        //{
        //	var products =
        //		from p in dataSource.Products
        //		where p.UnitsInStock > 0
        //		select p;

        //	foreach (var p in products)
        //	{
        //		ObjectDumper.Write(p);
        //	}
        //}


    }
}
