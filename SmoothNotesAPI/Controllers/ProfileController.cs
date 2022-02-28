using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmoothNotesAPI.Models;

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
    // POST api/profile
    [HttpPost]
    public async Task<ActionResult<Profile>> PostProfile([FromBody] Profile args)
    {
        try
        {
            Profile profile = new Profile()
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

            await _context.Profiles.AddAsync(profile);
            await _context.SaveChangesAsync();
            return Ok("Profile was created");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //Read
    // GET: api/profile
    [HttpGet]
    public async Task<ActionResult<List<Profile>>> Get()
    {
        try
        {
            return Ok(await _context.Profiles.ToListAsync());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    // GET: api/profile/id
    [HttpGet("id")]
    public async Task<ActionResult<Profile>> GetById(Guid id)
    {
        try
        {
            var profile = await _context.Profiles.FirstAsync(u => u.Id == id);
            if (profile == null)
                return NotFound();

            return Ok(profile);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //Update
    // PUT api/<ValuesController>/5
    [HttpPut("{id}")]
    public async Task<ActionResult> Put([FromBody] Profile profile)
    {
        try
        {
            _context.Entry(profile).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok("Profile Updated");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //Delete
    // DELETE api/<ValuesController>/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            //Finding Profile with Id == id
            var profile = await _context.Profiles.FindAsync(id);

            if (profile == null)
                return NotFound();

            _context.Profiles.Remove(profile);
            await _context.SaveChangesAsync();
            return Ok("Profile Deleted");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
