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
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public PostController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Post
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDTO>>> GetPosts()
        {
            return await _context.Posts
            .Select(x => PostToDTO(x))
            .ToListAsync();
        }

        // GET: api/Post/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDTO>> GetPost(long id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return PostToDTO(post);
        }

        // PUT: api/Post/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(long id, PostDTO postDTO)
         {
            if (id != postDTO.Id)
            {
                return BadRequest();
            }

            var currentPost = await _context.Posts.FindAsync(id);
            if(currentPost == null) {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(postDTO.UserId);
            if (user == null)
            {
                return NotFound("User with this Id is not found");
            }

            currentPost.Id = postDTO.Id;
            currentPost.Title = postDTO.Title;
            currentPost.Content = postDTO.Content;
            currentPost.UserId = postDTO.UserId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
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

        // POST: api/Post
        [HttpPost]
        public async Task<ActionResult<PostDTO>> PostPost([FromBody] PostDTO postDTO)
        {
            var user = await _context.Users.FindAsync(postDTO.UserId);
            if (user == null)
            {
                return NotFound("A user of this post does not exist!");
            }
            if (user.IsVerified != true)
            {
                return BadRequest("User that is not verified can not create posts!");
            }
            
            var post = DTOToPost(postDTO);
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { id = postDTO.Id }, postDTO);
        }

        // DELETE: api/Post/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(long id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(long id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        private static Post DTOToPost (PostDTO postDTO) => 
            new Post
            {
                Id = postDTO.Id,
                Title = postDTO.Title,
                Content = postDTO.Content,
                UserId = postDTO.UserId
            };

        private static PostDTO PostToDTO(Post post) =>
            new PostDTO
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                UserId = post.UserId
            };
    }
}
