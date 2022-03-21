using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmoothNotesAPI.Models;
using SmoothNotesAPI.Models.Interfaces;
using SmoothNotesAPI.Models.Login;
using SmoothNotesAPI.Models.Registration;
using SmoothNotesAPI.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SmoothNotesAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly DataContext _context;
    private readonly HashingService _hashingService;
    private readonly IConfiguration _configuration;

    public ProfileController(DataContext context, IConfiguration configuration)
    {
        _configuration = configuration;
        _context = context;
        _hashingService = new HashingService();
    }

    //Create
    // POST api/<ValuesController>
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] Register args)
    {
        try
        {
            if (await _context.Profiles.AnyAsync(p => p.Name == args.Name))
                return Ok("1");

            string hpw = _hashingService.HashPW(args.Pw);

            

            //Test recive version
            Profile item = new Profile()
            {
                Id = Guid.NewGuid().ToString(),
                Name = args.Name,
                PW = hpw,
                PrK = args.Prk,
                PuK = args.PuK,
                CrDate = DateTime.Now,
                EdDate = DateTime.Now
            };

            await _context.Profiles.AddAsync(item);
            await _context.SaveChangesAsync();
            return Ok(item.Id);
        }
        catch (Exception e)
        {
            return BadRequest("Error##: " + e.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(ProfileDto request)
    {
        var p = await _context.Profiles.FirstOrDefaultAsync(u => u.Name == request.username);
        if(p != null)
        {
            if (await VerifyPassword(request.username, request.password))
            {
                string token = CreateToken(await _context.Profiles.FirstOrDefaultAsync(p => p.Name == request.username));
                return Ok(token);
            }
        }
        return BadRequest("Login failed");
    }

    [HttpGet("refresh/username"), Authorize]
    public async Task<ActionResult<string>> Refresh(string username)
    {
        string token = CreateToken(await _context.Profiles.FirstOrDefaultAsync(p => p.Name == username));
        return Ok(token);
    }

    private async Task<bool> VerifyPassword(string username, string password)
    {
        Profile profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Name == username);
        return _hashingService.VerifyPW(password, profile.PW);
    }


    private string CreateToken(Profile profile)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, profile.Id.ToString().ToUpper()),
            new Claim(ClaimTypes.Name, profile.Name)
        };

        //Creating a symmetric key to be use for the creation of the token.
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddMinutes(5), signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    //Read

    [HttpGet("username"), Authorize]
    public async Task<ActionResult<LProfile>> GetLogin(string username)
    {
        try
        {
            var item = await _context.Profiles.Where(p => p.Name == username).Include(f => f.folders).ThenInclude(n => n.notes).FirstOrDefaultAsync();
            if (item == null)
                return NotFound();

            LProfile p = new LProfile();
            p.Id = item.Id.ToString();
            p.Name = item.Name;
            p.PrK = item.PrK;
            p.PuK = item.PuK;
            p.folders = item.folders;
            return p;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // GET: api/<ValuesController>
    [HttpGet, Authorize]
    public async Task<ActionResult<List<IBase>>> Get()
    {
        try
        {
            return Ok(await _context.Profiles.Include(f => f.folders).ThenInclude(n => n.notes).ToListAsync());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //Update
    // PUT api/<ValuesController>/id
    [HttpPut("{id}"), Authorize]
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
    [HttpDelete("id"), Authorize]
    public async Task<ActionResult> Delete(string id)
    {
        try
        {
            //Getting Profile from id
            var profile = await _context.Profiles.FindAsync(id);
            
            //In case no profile is found
            if (profile == null)
                return NotFound();

            //Getting all related folders, notes and removing them
            var folders = await _context.Folders.Where(f => f.ProfileId == id).ToListAsync();
            foreach (var folder in folders)
            {
                var notes = await _context.Notes.Where(n => n.FolderId == folder.Id).ToListAsync();
                foreach (var note in notes)
                    _context.Notes.Remove(note);
                _context.Folders.Remove(folder);
            }

            //Remove profile and save
            _context.Profiles.Remove(profile);
            await _context.SaveChangesAsync();
            return Ok("Deletion Successful");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
