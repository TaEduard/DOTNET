using System;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;

namespace Lab1A
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = "https://www.emag.ro/docking-stations/c?ref=search_menu_category";
            HttpClient hClient = new HttpClient();
            var html = hClient.GetStringAsync(url).Result;
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            var elemntsnameonpage = doc.DocumentNode.SelectNodes("//*[@class='card-item js-product-data']//a[@title]");
            var elemntspriceonpage = doc.DocumentNode.SelectNodes("//*[@class='product-new-price']");
            var elems= elemntsnameonpage.Zip(elemntspriceonpage, (name, price) => new { Name=name, Price=price });
            try
            {

                foreach (var item in elems)
                {
                    string productname = item.Name.InnerHtml.ToString();
                    string productprice = item.Price.InnerHtml.ToString();
                    productname = productname.Replace("&quot;", "\"").Replace("&#039;","'");
                    productprice = productprice.Replace("<sup>", ",").Replace("</sup>", "").Replace(" <span>Lei</span>","");
                   Console.WriteLine(productname + " - " + productprice + " Lei");
                }
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

           }
    }
}