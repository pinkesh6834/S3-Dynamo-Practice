using Amazon.DynamoDBv2.Model;
using AwsTutorial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwsTutorial.Services
{
    public interface IDynamoDBService
    {
        void CreateDynamoDBTable(string tableName);
        void PutItemIntoDynamoDBTable(int id, string replyDateTime, string tableName, double price);
        Task<DynamoTableItem> GetItemFromDynamoDBTableAsync(string tableName, int? id);
        Task<Item> UpdateItemToDynamoDBTableAsync(string tableName, int id, double price);
        Task DeleteItemFromDynamoDBTableAsync(string tableName, int id);
    }
}
