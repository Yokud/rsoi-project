using DS_Project.Statistics.Entity;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DS_Project.Statistics.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StatsController(LogsConsumer consumer, StatsDbContext context) : Controller
    {
        readonly LogsConsumer _consumer = consumer;
        readonly StatsDbContext _appDbContext = context;

        [HttpGet("statistics/get")]
        public async Task<List<string>> Get()
        {
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
            return result;
        }
    }
}
