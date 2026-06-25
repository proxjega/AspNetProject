using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetProject.Models;

namespace AspNetProject.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public UserController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            return await _context.Users
            .Select(x => UserToDTO(x))
            .ToListAsync();
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser([FromRoute] long id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return UserToDTO(user);
        }

        // PUT: api/users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser([FromRoute] long id, [FromBody] UserDTO userDTO)
        {
            if (id != userDTO.Id)
            {
                return BadRequest();
            }

            var currentUser = await _context.Users.FindAsync(id);
            if(currentUser == null) {
                return NotFound();
            }

            currentUser.Name = userDTO.Name;
            currentUser.Surname = userDTO.Surname;
            currentUser.DateOfBirth = userDTO.DateOfBirth;
            currentUser.Gender = userDTO.Gender;
            currentUser.Email = userDTO.Email;
            currentUser.Address = userDTO.Address;
            currentUser.UserNotes = userDTO.UserNotes;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser([FromBody] UserDTO userDTO)
        {   
            var user = DTOToUser(userDTO);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = userDTO.Id }, userDTO);
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] long id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/users/{id}/verify-email
        [HttpPost("{id}/verify-email")]
        public async Task<IActionResult> VerifyEmail([FromRoute] long id, [FromBody] string token)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (token != "verify-me")
            {
                return BadRequest(new { message = "Invalid token" });
            }

            user.IsVerified = true;
            await _context.SaveChangesAsync();
            
            return Ok(new { message = "Email verified successfully" });
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private static UserDTO UserToDTO(User user) =>
            new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                UserNotes = user.UserNotes
            };

        private static User DTOToUser(UserDTO userDTO) =>
            new User
            {
                Id = userDTO.Id,
                Name = userDTO.Name,
                Surname = userDTO.Surname,
                Email = userDTO.Email,
                Gender = userDTO.Gender,
                DateOfBirth = userDTO.DateOfBirth,
                Address = userDTO.Address,
                UserNotes = userDTO.UserNotes
            };
    }
}
