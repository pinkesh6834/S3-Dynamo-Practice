using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AwsTutorial.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AwsTutorial.Services
{
    public class DynamoDBService : IDynamoDBService
    {
        private readonly IAmazonDynamoDB _amazonDynamoDBClient;
        public DynamoDBService(IAmazonDynamoDB amazonDynamoDBClient)
        {
            _amazonDynamoDBClient = amazonDynamoDBClient;
        }

        public void CreateDynamoDBTable(string tableName)
        {
            try
            {
                this.CreateTable(tableName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<DynamoTableItem> GetItemFromDynamoDBTableAsync(string tableName, int? id)
        {
            ScanRequest request;

            if (!id.HasValue)
            {
                request = new ScanRequest()
                {
                    TableName = tableName
                };
            }
            else
            {
                request = new ScanRequest()
                {
                    TableName = tableName,
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                {
                    {
                    ":Id", new AttributeValue() { N = id.ToString()}
                    }
                },
                    FilterExpression = "Id = :Id",
                    ProjectionExpression = "Id, ReplyDateTime, Price"
                };
            }

            var result = await _amazonDynamoDBClient.ScanAsync(request);

            return new DynamoTableItem
            {
                Items = result.Items.Select(Map).ToList()
            };
        }

        private Item Map(Dictionary<string, AttributeValue> result)
        {
            return new Item
            {
                Id = Convert.ToInt32(result["Id"].N),
                ReplyDateTime = result["ReplyDateTime"].N,
                Price = Convert.ToDouble(result["Price"].N)
            };
        }

        public void PutItemIntoDynamoDBTable(int id, string replyDateTime, string tableName, double price)
        {
            var item = new Dictionary<string, AttributeValue>()
            {
                { "Id", new AttributeValue() { N = id.ToString()} },
                { "ReplyDateTime", new AttributeValue() { N = replyDateTime} },
                { "Price", new AttributeValue(){ N = price.ToString(CultureInfo.InvariantCulture)} }
            };

            var putRequest = new PutItemRequest()
            {
                Item = item,
                TableName = tableName
            };

            _amazonDynamoDBClient.PutItemAsync(putRequest);
        }

        private void CreateTable(string tableName)
        {
            Console.WriteLine("Creating Table");

            var request = new CreateTableRequest
            {
                AttributeDefinitions = new List<AttributeDefinition>
        {
            new AttributeDefinition
            {
                AttributeName = "Id",
                AttributeType = "N"
            },
            new AttributeDefinition
            {
                AttributeName = "ReplyDateTime",
                AttributeType = "N"
            }
        },
                KeySchema = new List<KeySchemaElement>
        {
            new KeySchemaElement
            {
                AttributeName = "Id",
                KeyType = "HASH" // Partition Key
            },
            new KeySchemaElement
            {
                AttributeName = "ReplyDateTime",
                KeyType = "Range" // Sort Key
            }
        },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                },
                TableName = tableName
            };

            var response = _amazonDynamoDBClient.CreateTableAsync(request);

            TryAgainToCreateTable(tableName);
        }

        private void TryAgainToCreateTable(string tableName)
        {
            string status = string.Empty;

            do
            {
                Thread.Sleep(5000);

                try
                {
                    var res = _amazonDynamoDBClient.DescribeTableAsync(new DescribeTableRequest()
                    {
                        TableName = tableName
                    });

                    status = res.Result.Table.TableStatus;
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            while (status != "ACTIVE");
            {
                Console.WriteLine("Table Created Successfully");
            }
        }

        public async Task<Item> UpdateItemToDynamoDBTableAsync(string tableName, int id, double price)
        {
            var responseFromGet = this.GetItemFromDynamoDBTableAsync(tableName, id);
            var currentPrice = responseFromGet.Result.Items.Select(m => m.Price).FirstOrDefault();
            var currentReplyDateTime = responseFromGet.Result.Items.Select(m => m.ReplyDateTime).FirstOrDefault();

            var request = new UpdateItemRequest
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>() {
                    { "Id", new AttributeValue{ N = id.ToString() } },
                    { "ReplyDateTime", new AttributeValue{ N = currentReplyDateTime} }
                },
                ExpressionAttributeNames = new Dictionary<string, string> {
                    { "#P", "Price"}
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                {
                    {":newprice", new AttributeValue{ N=price.ToString()} },
                    {":currentprice", new AttributeValue{ N= currentPrice.ToString()} },
                },
                UpdateExpression = "SET #P = :newprice",
                ConditionExpression = "#P = :currentprice",
                ReturnValues = "ALL_NEW" // Return all the attributes of the updated item.
            };

            var response = await _amazonDynamoDBClient.UpdateItemAsync(request);

            return new Item
            {
                Id = Convert.ToInt32(response.Attributes["Id"].N),
                Price = Convert.ToDouble(response.Attributes["Price"].N),
                ReplyDateTime = response.Attributes["ReplyDateTime"].N
            };
        }

        public async Task DeleteItemFromDynamoDBTableAsync(string tableName, int id)
        {
            var responseFromGet = this.GetItemFromDynamoDBTableAsync(tableName, id);
            var currentReplyDateTime = responseFromGet.Result.Items.Select(m => m.ReplyDateTime).FirstOrDefault();

            var request = new DeleteItemRequest
            {
                Key = new Dictionary<string, AttributeValue>() {
                    { "Id", new AttributeValue{ N = id.ToString() } },
                    { "ReplyDateTime", new AttributeValue{ N = currentReplyDateTime} }
                },
                TableName = tableName
            };

            var response = await _amazonDynamoDBClient.DeleteItemAsync(request);
        }
    }
}
