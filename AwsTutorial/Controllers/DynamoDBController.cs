using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwsTutorial.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AwsTutorial.Controllers
{
    [Route("api/[Controller]")]
    public class DynamoDBController : ControllerBase
    {
        private readonly IDynamoDBService _dynamoDBService;

        public DynamoDBController(IDynamoDBService dynamoDBService)
        {
            _dynamoDBService = dynamoDBService;
        }

        [HttpPost]
        [Route("{tableName}")]
        public void CreateDynamoDBTable([FromRoute] string tableName)
        {
            _dynamoDBService.CreateDynamoDBTable(tableName);
        }

        [HttpPost]
        [Route("putitem")]
        public IActionResult PutItemIntoDynamoDBTable([FromQuery] int id, [FromQuery]string replyDateTime, [FromQuery]string tableName, [FromQuery] double price)
        {
            _dynamoDBService.PutItemIntoDynamoDBTable(id, replyDateTime, tableName, price);
            return Ok();
        }

        [HttpGet]
        [Route("getitem")]
        public async Task<IActionResult> GetItemFromDynamoDBTable([FromQuery] string tableName, [FromQuery]int? id)
        {
            var response = await _dynamoDBService.GetItemFromDynamoDBTableAsync(tableName, id);
            return Ok(response);
        }

        [HttpPut]
        [Route("updateitem")]
        public async Task<IActionResult> UpdateItemToDynamoDBTable([FromQuery] string tableName, [FromQuery]int id, [FromQuery]double price)
        {
            var response = await _dynamoDBService.UpdateItemToDynamoDBTableAsync(tableName, id, price);
            return Ok(response);
        }

        [HttpDelete]
        [Route("deleteitem")]
        public async Task<IActionResult> DeleteItemFromDynamoDBTable([FromQuery] string tableName, [FromQuery]int id)
        {
            await _dynamoDBService.DeleteItemFromDynamoDBTableAsync(tableName, id);
            return Ok();
        }
    }
}