using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmoothNotesAPI.Models;
using SmoothNotesAPI.Models.Interfaces;
using SmoothNotesAPI.Service;
using System.Security.Cryptography;
using System.Text;

namespace SmoothNotesAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly DataContext _context;
    private readonly HashingService _hashingService;

    //TODO: Remove when application side is implemented
    //Testing Only!!!
    private AESService aesService = new AESService();
    //private readonly string constIV = "9AF6666D-A8FA-4BB9-A58C-3178DCFEBA30";
    private readonly byte[] constIV = Convert.FromBase64String("IOMLgEgejhBsXS/r+gZWQA==");

    public ProfileController(DataContext context)
    {
        _context = context;
        _hashingService = new HashingService();
    }

    //Create
    // POST api/<ValuesController>
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Profile args)
    {
        try
        {
            string hpw = _hashingService.HashPW(args.PW);
            //if (args.PW.Length < 10)
            //    return BadRequest("Password needs to be more than 10 characters");

            //TODO: Remove when application side is implemented
            //Testing only move to application side, when possible
            #region Testing Only
            //Aes encryption
            string ePrK = "";
            try
            {
                ePrK = aesService.Encrypt(args.PrK, args.PW);
                //byte[] encrypted = aesService.Encrypt(args.PrK, hpw, constIV);
                //ePrK = Convert.ToBase64String(encrypted);
                //foreach (var b in encrypted)
                //{
                //    ePrK += "" + b;
                //}
                //Console.WriteLine(ePrK);
            }
            catch (Exception e)
            {
                return BadRequest($"Error: {e.Message}");
            }
            #endregion


            Profile item = new Profile()
            {
                Id = Guid.NewGuid(),
                Name = args.Name,
                PW = hpw,
                PrK = ePrK,
                //PrK = args.PrK,
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
    // GET: api/<ValuesController>/id/show
    [HttpGet("id/show")]
    public async Task<ActionResult<IBase>> GetById(Guid id, int show = 0)
    {
        try
        {
            //var item = await _context.Profiles.Include(s => s.folders).FirstAsync(u => u.Id == id);
            var item = await _context.Profiles.Where(p => p.Id == id).Include(f => f.folders).ThenInclude(n => n.notes).FirstOrDefaultAsync();
            if (item == null)
                return NotFound();

            //TODO: Remove when application side is implemented
            //Testing ONLY
            if (show == 1)
            {
                try
                {
                    string dePrK = aesService.Decrypt(item.PrK, "password123");
                    item.PrK = dePrK;
                }
                catch (Exception e)
                {
                    Console.WriteLine(Environment.NewLine + $"Error: {e.Message}");
                }
            }

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
