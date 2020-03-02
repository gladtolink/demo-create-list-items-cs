using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace CreateListItems
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateListItems();
        }

        static void CreateListItems()
        {
            using (var httpClientHandler = new HttpClientHandler())
            {
                // Do not use in production code because of security issues. Just for testing.
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };
                ///////

                var client = new HttpClient(httpClientHandler);
                client.BaseAddress = new Uri("https://www.gladtolink.com:8080");

                // We get and format the data we want to send
                var dataToSend = FormatData();
                // Convert it to a JSON stringified in order to send it
                var dataStr = JsonConvert.SerializeObject(dataToSend);

                // Our package travels in the POST request as "data"
                var content = new FormUrlEncodedContent(new[]{
                    new KeyValuePair<string, string>("data", dataStr)
                });

                // Replace with your g2l integration token here
                var integrationToken = "[YOUR_G2L_INTEGRATION_TOKEN_HERE]";
                try
                {
                    var resp = client.PostAsync("/api/G2LIntegration/" + integrationToken, content).Result;
                    if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine("Everything went okay!");
                    }
                    else
                    {
                        Console.WriteLine("Something bad happened");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("There was an error in the call");
                }
            }
        }

        static List<ListItem> FormatData()
        {
            var listItems = new List<ListItem>();

            // We create some dummy data in order to fill our list items for this example.
            // You should replace this logic with your own in order to send the data you need to Gladtolink.

            /* CONSIDERATIONS:
             * - Attribute "name" in a List Item is the primary key, so it has to be unique
             * - "name" string length is restricted to a maximum of 30 characters
             * */
            for (var i = 1; i <= 100; i++)
            {
                // Defining each list element
                var listItem = new ListItem
                {
                    name = "List item " + i,
                    description = "Description for List Item " + i,
                    position = i,
                    datas = new List<ListItemData>()
                };

                // For the extended form fields, we need to feed each item's "datas" attribute
                // "Name" is the name of the field of the extended form, while "value" is the value of the data we want to set.
                /* Keep in mind that the value format depends on the type of field we are feeding:
                 * - YES/NO: use "true" or "false"
                 * - Numeric: use point as decimal separator. Examples: 144.25, 123123123.123, 123
                 * - Date: use format YYYY-MM-DD (2020-01-20)
                 * - Hour: HH:mm (23:59)
                 * */

                var invoiceCode = new ListItemData
                {
                    name = "Invoice code",
                    value = "COD" + i,
                };
                listItem.datas.Add(invoiceCode);

                var orderDate = new ListItemData
                {
                    name = "Order date",
                    value = "2020-01-01",
                };
                listItem.datas.Add(orderDate);

                var orderTime = new ListItemData
                {
                    name = "Order time",
                    value = "22:00",
                };
                listItem.datas.Add(orderTime);

                var totalElements = new ListItemData
                {
                    name = "Total elements",
                    value = i + "",
                };
                listItem.datas.Add(totalElements);


                listItems.Add(listItem);
            }

            return listItems;
        }
    }

    class ListItem
    {
        public string name;
        public string description;
        public int position;
        public List<ListItemData> datas;
    }

    class ListItemData
    {
        public string name;
        public string value;
    }
}
