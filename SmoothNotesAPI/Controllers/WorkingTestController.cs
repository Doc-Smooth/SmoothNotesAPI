using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmoothNotesAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmoothNotesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkingTestController : ControllerBase
    {
        private readonly DataContext _context;

        public WorkingTestController(DataContext context)
        {
            _context = context;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<ActionResult<List<WorkingTest>>> Get()
        {
            return Ok(await _context.WorkingTests.ToListAsync());
        }

        //// GET api/<ValuesController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<ValuesController>
        [HttpPost]
        public async Task<ActionResult<WorkingTest>> Post(string text)
        {
            WorkingTest test = new WorkingTest();
            test.testc = text;

            _context.WorkingTests.Add(test);
            _context.SaveChanges();

            return Ok(test);
        }

        //// PUT api/<ValuesController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<ValuesController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
