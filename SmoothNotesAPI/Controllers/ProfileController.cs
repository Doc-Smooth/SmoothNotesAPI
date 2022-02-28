using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmoothNotesAPI.Models;
using SmoothNotesAPI.Models.Interfaces;

namespace SmoothNotesAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly DataContext _context;

    public ProfileController(DataContext context)
    {
        _context = context;
    }

    //Create
    // POST api/<ValuesController>
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Profile args)
    {
        try
        {
            Profile item = new Profile()
            {
                Id = Guid.NewGuid(),
                Name = args.Name,
                PW = args.PW,
                Salt = args.Salt,
                PrK = args.PrK,
                PuK = args.PuK,
                CrDate = DateTime.Now.Date,
                EdDate = DateTime.Now.Date
            };

            await _context.Profiles.AddAsync(item);
            await _context.SaveChangesAsync();
            return Ok("Created Successful");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //Read
    // GET: api/<ValuesController>
    [HttpGet]
    public async Task<ActionResult<List<IBase>>> Get()
    {
        try
        {
            //return Ok(await _context.Profiles.Include(s => s.folders).ToListAsync());

            return Ok(await _context.Profiles.Include(f => f.folders).ThenInclude(n => n.notes).ToListAsync());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    // GET: api/<ValuesController>/id
    [HttpGet("id")]
    public async Task<ActionResult<IBase>> GetById(Guid id)
    {
        try
        {
            //var item = await _context.Profiles.Include(s => s.folders).FirstAsync(u => u.Id == id);
            var item = await _context.Profiles.Where(p => p.Id == id).Include(f => f.folders).ThenInclude(n => n.notes).FirstOrDefaultAsync();
            if (item == null)
                return NotFound();

            return Ok(item);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //Update
    // PUT api/<ValuesController>/id
    [HttpPut("{id}")]
    public async Task<ActionResult> Put([FromBody] Profile item)
    {
        try
        {
            _context.Entry(item).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok("Updated Successful");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //Delete
    // DELETE api/<ValuesController>/id
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            //Finding Item with Id == id
            var item = await _context.Profiles.FindAsync(id);

            if (item == null)
                return NotFound();

            _context.Profiles.Remove(item);
            await _context.SaveChangesAsync();
            return Ok("Deletion Successful");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
