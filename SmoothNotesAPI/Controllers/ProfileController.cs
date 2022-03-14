using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmoothNotesAPI.Models;
using SmoothNotesAPI.Models.Interfaces;
using SmoothNotesAPI.Models.Login;
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

    //TODO: Remove when application side is implemented
    //Testing Only!!!
    private AESService aes = new AESService();
    private RSAService rsa = new RSAService();


    //Create
    // POST api/<ValuesController>
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] ProfileDto args)
    {
        try
        {
            if (await _context.Profiles.AnyAsync(p => p.Name == args.username))
                return Ok("1");

            string hpw = _hashingService.HashPW(args.password);

            //TODO: Remove when application side is implemented
            //Testing only move to application side, when possible
            #region Testing Only
            //Rsa Key Pair
            List<string> pair = new List<string>();
            try
            {
                pair = rsa.GenKeyPair();
            }
            catch (Exception e)
            {
                return BadRequest($"Error##: {e.Message}");
            }

            //Aes encryption
            string ePrK = "";
            try
            {
                ePrK = aes.Encrypt(pair[0], args.password);
            }
            catch (Exception e)
            {
                return BadRequest($"Error##: {e.Message}");
            }

            //Test recive version
            Profile item = new Profile()
            {
                Id = Guid.NewGuid().ToString(),
                Name = args.username,
                PW = hpw,
                PrK = ePrK,
                PuK = Convert.ToBase64String(ConverterService.HexStringToByteArray(pair[1])),
                CrDate = DateTime.Now,
                EdDate = DateTime.Now
            };
            #endregion

            //Profile item = new Profile()
            //{
            //    Id = Guid.NewGuid(),
            //    Name = args.Name,
            //    PW = hpw,
            //    PrK = args.PrK,
            //    PuK = args.PuK,
            //    CrDate = DateTime.Now,
            //    EdDate = DateTime.Now
            //};

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
        if (await _context.Profiles.AnyAsync(p => p.Name == request.username))
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
            //return Ok(await _context.Profiles.Include(s => s.folders).ToListAsync());

            return Ok(await _context.Profiles.Include(f => f.folders).ThenInclude(n => n.notes).ToListAsync());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // GET: api/<ValuesController>/id/show
    [HttpGet("id/show"), Authorize]
    public async Task<ActionResult<IBase>> GetById(string id, int show = 0)
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
                    string dePrK = ConverterService.ReadEncodedKey(aes.Decrypt(item.PrK, "Password123"));
                    item.PrK = dePrK;
                    item.PuK = ConverterService.ReadEncodedKey(ConverterService.ByteArrayToHexString(Convert.FromBase64String(item.PuK)));
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
    [HttpDelete("{id}"), Authorize]
    public async Task<ActionResult> Delete(string id)
    {
        try
        {
            //Finding Item with Id == id
            var profile = await _context.Profiles.FindAsync(id);
            var folders = await _context.Folders.Where(f => f.ProfileId == id).ToListAsync();
            foreach (var folder in folders)
            {
                var notes = await _context.Notes.Where(n => n.FolderId == folder.Id).ToListAsync();
                foreach (var note in notes)
                    _context.Notes.Remove(note);
                _context.Folders.Remove(folder);
            }

            if (profile == null)
                return NotFound();

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
