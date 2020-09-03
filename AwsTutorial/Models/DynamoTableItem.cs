using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwsTutorial.Models
{
    public class DynamoTableItem
    {
        public IEnumerable<Item> Items { get; set; }
    }

    public class Item
    {
        public int Id { get; set; }
        public string ReplyDateTime { get; set; }

        public double Price { get; set; }
    }
}
