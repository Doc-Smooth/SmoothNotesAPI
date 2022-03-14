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
    [HttpPost, Authorize]
    public async Task<ActionResult> Post([FromBody] Note args)
    {
        try
        {
            //Testing
            //Move to Applikation side when possible
            #region Testing
            Folder folder = await _context.Folders.FirstOrDefaultAsync(f => f.Id == args.FolderId);
            Profile profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == folder.ProfileId);
            string key = ConverterService.ReadEncodedKey(ConverterService.ByteArrayToHexString(Convert.FromBase64String(profile.PuK)));
            Note item = new Note()
            {
                Id = Guid.NewGuid().ToString(),
                FolderId = args.FolderId,
                Name = args.Name,
                Text = RSAService.RSAEncrypt(args.Text, key, false),
                CrDate = DateTime.Now,
                EdDate = DateTime.Now
            };
            #endregion

            //Note item = new Note()
            //{
            //    Id = Guid.NewGuid(),
            //    FolderId = args.FolderId,
            //    Name = args.Name,
            //    Text = args.Text,
            //    CrDate = DateTime.Now,
            //    EdDate = DateTime.Now
            //};

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
    [HttpGet("id"), Authorize]
    public async Task<ActionResult<IBase>> GetById(string id)
    {
        try
        {
            var item = await _context.Notes.FirstOrDefaultAsync(u => u.Id == id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //Testing ONLY
    // GET: api/<ValuesController>/id
    [HttpGet("id/profileId/show"), Authorize]
    public async Task<ActionResult<IBase>> GetById(string id, string profileId, int show = 0)
    {
        try
        {
            AESService aes = new AESService();
            Profile profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == profileId);
            var item = await _context.Notes.FirstOrDefaultAsync(u => u.Id == id);
            item.Text = RSAService.RSADecrypt(item.Text, ConverterService.ReadEncodedKey(aes.Decrypt(profile.PrK, "Password123")), false);
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
    [HttpPut("{id}"), Authorize]
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
    public async Task<ActionResult> Delete(Guid id)
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
