// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using SampleSupport;
using System;
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
            foreach (var item in dataSource.Customers.Where(x => x.Orders.Sum(y => y.Total) > price))
            {
                Console.WriteLine(item.CompanyName);
            }
        }
        [Category("Restriction Operators")]
        [Title("Where - Task 4")]
        [Description("Выдайте список клиентов с указанием, начиная с какого месяца какого года они стали клиентами (принять за таковые месяц и год самого первого заказа)")]

        public void Linq4()
        {
            foreach (var item in dataSource.Customers)
            {
                string text = null;
                if (item.Orders.Length == 0)
                {
                    text = "No orders";
                }
                else
                {
                    text = $"Year:{item.Orders.OrderBy(x => x.OrderDate.Year).ThenBy(z => z.OrderDate.Month).First().OrderDate.Year} Month:{item.Orders.OrderBy(x => x.OrderDate.Year).ThenBy(z => z.OrderDate.Month).First().OrderDate.Month}";
                }
                Console.WriteLine($"Name:{item.CompanyName} {text}");
            }
        }
        [Category("Restriction Operators")]
        [Title("Where - Task 5")]
        [Description("Сделайте предыдущее задание, но выдайте список отсортированным по году, месяцу, оборотам клиента (от максимального к минимальному) и имени клиента")]

        public void Linq5()
        {
            foreach (var item in dataSource.Customers.Where(x=>x.Orders.Length!=0).OrderBy(x => x.Orders.FirstOrDefault().OrderDate.Year).ThenBy(x => x.Orders.FirstOrDefault().OrderDate.Month).ThenByDescending(x => x.Orders.Sum(y=>y.Total)).ThenBy(x => x.CompanyName))
            {
                Console.WriteLine(item.CompanyName);
            }
            foreach (var item in dataSource.Customers.Where(x => x.Orders.Length == 0).OrderBy(x => x.CompanyName))
            {
                Console.WriteLine(item.CompanyName);
            }
        }
        [Category("Restriction Operators")]
        [Title("Where - Task 6")]
        [Description("Укажите всех клиентов, у которых указан нецифровой почтовый код или не заполнен регион или в телефоне не указан код оператора (для простоты считаем, что это равнозначно «нет круглых скобочек в начале»).")]

        public void Linq6()
        {
            foreach (var item in dataSource.Customers.Where(x => x.Region == null || x.Phone.ToArray().FirstOrDefault() != '(' || int.TryParse(x.PostalCode, out _) != true))
            {
                Console.WriteLine(item.CompanyName);
            }
        }
        [Category("Restriction Operators")]
        [Title("Where - Task 7")]
        [Description("Сгруппируйте все продукты по категориям, внутри – по наличию на складе, внутри последней группы отсортируйте по стоимости")]

        public void Linq7()
        {
            foreach (var category in dataSource.Products.GroupBy(x => x.Category))
            {
                Console.WriteLine($"Category: {category.Key}");
                foreach (var stock in category.GroupBy(y => y.UnitsInStock))
                {
                    Console.WriteLine($"Units In Stock: {stock.Key}");
                    foreach (var item in stock.OrderBy(z => z.UnitPrice))
                    {
                        Console.WriteLine($"Name: {item.ProductName} Price: {item.UnitPrice}");
                    }
                }
            }
        }
        [Category("Restriction Operators")]
        [Title("Where - Task 8")]
        [Description("Сгруппируйте товары по группам «дешевые», «средняя цена», «дорогие». Границы каждой группы задайте сами")]

        public void Linq8()
        {
            var middlePrice = dataSource.Products.Sum(x => x.UnitPrice) / (dataSource.Products.Count());
            Console.WriteLine("Low");
            foreach (var item in dataSource.Products.Where(z => z.UnitPrice < middlePrice - 5.0000M).GroupBy(x => x.UnitPrice))
            {

                foreach (var unit in item)
                {
                    Console.WriteLine(unit.ProductName);
                }
            }
            Console.WriteLine("-------------------------");
            Console.WriteLine("Midlle");
            foreach (var item in dataSource.Products.Where(z => z.UnitPrice >= middlePrice - 5.0000M && z.UnitPrice <= middlePrice + 5.0000M).GroupBy(x => x.UnitPrice))
            {

                foreach (var unit in item)
                {
                    Console.WriteLine(unit.ProductName);
                }
            }
            Console.WriteLine("-------------------------");
            Console.WriteLine("Big");
            foreach (var item in dataSource.Products.GroupBy(x => x.UnitPrice).Where(x => x.Key > middlePrice + 5.0000M))
            {

                foreach (var unit in item)
                {
                    Console.WriteLine(unit.ProductName);
                }
            }
        }
        [Category("Restriction Operators")]
        [Title("Where - Task 9")]
        [Description("Рассчитайте среднюю прибыльность каждого города (среднюю сумму заказа по всем клиентам из данного города) и среднюю интенсивность (среднее количество заказов, приходящееся на клиента из каждого города)")]

        public void Linq9()
        {
            foreach (var city in dataSource.Customers.GroupBy(x => x.City))
            {
                Console.WriteLine($"Name: {city.Key}");
                Console.WriteLine($"Medium profitability:{(city.Sum(y => y.Orders.Sum(z => z.Total)) / dataSource.Customers.Count()).ToString("0.00")} ");
                Console.WriteLine($"Medium intensity: {(city.Sum(y => y.Orders.Count()) / dataSource.Customers.Count()).ToString("0.00") }");
                Console.WriteLine(city.Count());
            }
        }
        [Category("Restriction Operators")]
        [Title("Where - Task 10")]
        [Description("Сделайте среднегодовую статистику активности клиентов по месяцам (без учета года), статистику по годам, по годам и месяцам (т.е. когда один месяц в разные годы имеет своё значение).")]

        public void Linq10()
        {
            Console.WriteLine("By Month");
            for (int i = 1; i < 13; i++)
            {
                decimal count=0;
                foreach (var item in dataSource.Customers.Where(x=>x.Orders.Length!=0))
                {
                    count += item.Orders.Where(y => y.OrderDate.Month == i).Count();
                }
                Console.WriteLine($"month:{i} medium activity:{(count/dataSource.Customers.Count()).ToString("0.00")}");
            }

            Console.WriteLine("-------------------------");
            Console.WriteLine("By Year");

            for (int i = dataSource.Customers.Where(y => y.Orders.Length != 0).Min(z => z.Orders.Min(x => x.OrderDate.Year)); i <= dataSource.Customers.Where(y => y.Orders.Length != 0).Max(z => z.Orders.Max(x => x.OrderDate.Year)); i++)
            {
                decimal count = 0;
                foreach (var item in dataSource.Customers.Where(x => x.Orders.Length != 0))
                {
                    count += item.Orders.Where(y => y.OrderDate.Year == i).Count();
                }
                Console.WriteLine($"year:{i} medium activity:{(count / dataSource.Customers.Count()).ToString("0.00")}");
            }
            Console.WriteLine("-------------------------");
            Console.WriteLine("By Year and Month");
            for (int j = 1; j < 13; j++)
            {
                for (int i = dataSource.Customers.Where(y => y.Orders.Length != 0).Min(z => z.Orders.Min(x => x.OrderDate.Year)); i <= dataSource.Customers.Where(y => y.Orders.Length != 0).Max(z => z.Orders.Max(x => x.OrderDate.Year)); i++)
                {
                    decimal count = 0;
                    foreach (var item in dataSource.Customers.Where(x => x.Orders.Length != 0))
                    {
                        count += item.Orders.Where(y => y.OrderDate.Year == i&&y.OrderDate.Month==j).Count();
                    }
                    Console.WriteLine($"year:{i} month:{j} medium activity:{(count / dataSource.Customers.Count()).ToString("0.00")}");
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
