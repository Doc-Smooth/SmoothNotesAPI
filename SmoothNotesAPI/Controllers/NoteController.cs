using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmoothNotesAPI.Models;
using SmoothNotesAPI.Models.Interfaces;
using SmoothNotesAPI.Service;

namespace SmoothNotesAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class NoteController : ControllerBase
{
    private readonly DataContext _context;

    public NoteController(DataContext context)
    {
        _context = context;
    }

    //Create
    // POST api/<ValuesController>
    [HttpPost("add"), Authorize]
    public async Task<ActionResult> Post([FromBody] Note args)
    {
        try
        {

            Note item = new Note()
            {
                Id = Guid.NewGuid().ToString(),
                FolderId = args.FolderId,
                Name = args.Name,
                Text = args.Text,
                CrDate = DateTime.Now,
                EdDate = DateTime.Now
            };

            await _context.Notes.AddAsync(item);
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
    [HttpGet, Authorize]
    public async Task<ActionResult<List<IBase>>> Get()
    {
        try
        {
            return Ok(await _context.Notes.ToListAsync());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    // GET: api/<ValuesController>/id
    [HttpGet("folderid"), Authorize]
    public async Task<ActionResult<List<IBase>>> GetByFolderId(string folderid)
    {
        try
        {
            var items = await _context.Notes.Where(u => u.FolderId == folderid).ToListAsync();
            if (items == null)
                return NotFound();

            return Ok(items);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //Update
    // PUT api/<ValuesController>/id
    [HttpPut("edit"), Authorize]
    public async Task<ActionResult> Put([FromBody] Note item)
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
    [HttpDelete("{id}"), Authorize]
    public async Task<ActionResult> Delete(string id)
    {
        try
        {
            //Finding Item with Id == id
            var item = await _context.Notes.FindAsync(id);

            if (item == null)
                return NotFound();

            _context.Notes.Remove(item);
            await _context.SaveChangesAsync();
            return Ok("Deletion Successful");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
