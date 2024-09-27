using DS_Project.Statistics.Entity;
using DS_Project.Statistics.Middleware;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace DS_Project.Statistics.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StatsController(LogsConsumer consumer, StatsDbContext context, JwtValidator jwtValidator) : Controller
    {
        readonly LogsConsumer _consumer = consumer;
        readonly StatsDbContext _appDbContext = context;
        readonly JwtValidator _jwtValidator = jwtValidator;

        [HttpGet("statistics/get")]
        public async Task<IActionResult> Get([FromHeader(Name = "scopeToken"), Required] string scopeToken)
        {
            if (string.IsNullOrWhiteSpace(scopeToken))
                return Unauthorized();

            var user = _jwtValidator.GetUserDataFromToken(scopeToken);
            if (user is null)
                return Unauthorized();

            if (user.Role != "admin")
                return BadRequest();

            var result = new List<string>();
            while (true)
            {
                var x = _consumer.Consume();
                if (x == "plz stop")
                {
                    break;
                }
                result.Add(x);
            }
            var moreResults = _appDbContext.Set<Stat>().ToList();
            var entities = result.Select(x => new Stat() { Text = x }).ToArray();
            await _appDbContext.AddRangeAsync(entities);
            if (moreResults != null && moreResults.Any())
            {
                result.AddRange(moreResults.Select(x => x.Text).ToList());
            }
            _appDbContext.SaveChanges();

            return Ok(result);
        }
    }
}
