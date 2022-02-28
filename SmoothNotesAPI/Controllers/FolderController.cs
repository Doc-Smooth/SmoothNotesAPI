using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmoothNotesAPI.Models;
using SmoothNotesAPI.Models.Interfaces;

namespace SmoothNotesAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FolderController : ControllerBase
{
    private readonly DataContext _context;

    public FolderController(DataContext context)
    {
        _context = context;
    }

    //Create
    // POST api/<ValuesController>
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Folder args)
    {
        try
        {
            Folder item = new Folder()
            {
                Id = Guid.NewGuid(),
                ProfileId = args.ProfileId,
                Name = args.Name,
                Fav = false,
                CrDate = DateTime.Now.Date,
                EdDate = DateTime.Now.Date
            };

            await _context.Folders.AddAsync(item);
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
            return Ok(await _context.Folders.Include(n => n.notes).ToListAsync());
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
            var item = await _context.Folders.Include(n => n.notes).FirstOrDefaultAsync(u => u.Id == id);
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
    public async Task<ActionResult> Put([FromBody] Folder item)
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
            var item = await _context.Folders.FindAsync(id);

            if (item == null)
                return NotFound();

            _context.Folders.Remove(item);
            await _context.SaveChangesAsync();
            return Ok("Deletion Successful");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
