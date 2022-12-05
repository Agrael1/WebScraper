using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebScraper
{
    internal class Program
    {
        static readonly Uri link = new Uri("https://www.citilink.ru");
        static async Task Init()
        {
            using var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        }
        static async Task Main()
        {
            await Init();
            using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
            using var page = await browser.NewPageAsync();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 8; i++)
            {
                string req = $"https://www.citilink.ru/catalog/smartfony/?p={i + 1}";
                await page.GoToAsync(req, timeout: 0);

                IElementHandle[] selection = await page.XPathAsync("//section[@class='ProductGroupList js--ProductGroupList']/div/div[@class='ProductCardHorizontal__header-block']/a");
                Parallel.ForEach(selection, async item =>
                {
                    string a = await item.EvaluateFunctionAsync<string>("(el)=>el.getAttribute(\"href\")");
                    Console.WriteLine(new Uri(link, a));
                });
            }
            await Task.WhenAll(tasks);
        }
    }
}
