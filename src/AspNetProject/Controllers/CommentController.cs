using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetProject.Models;
using AspNetProject.DTOs;

namespace AspNetProject.Controllers;

[Route("api/comments")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ApplicationContext _context;

    public CommentController(ApplicationContext context)
    {
        _context = context;
    }

    // GET: api/comments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDTO>>> GetComments()
    {
        return await _context.Comments
        .Select(x => CommentToDTO(x))
        .ToListAsync();
    }

    // GET: api/comments/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CommentDTO>> GetComment(long id)
    {
        var comment = await _context.Comments.FindAsync(id);

        if (comment == null)
        {
            return NotFound();
        }

        return CommentToDTO(comment);
    }

    // PUT: api/comments/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutComment(long id, CommentDTO commentDTO)
        {
        if (id != commentDTO.Id)
        {
            return BadRequest();
        }

        var currentComment = await _context.Comments.FindAsync(id);
        if(currentComment == null) {
            return NotFound("Comment not found");
        }

        var user = await _context.Users.FindAsync(commentDTO.UserId);
        if (user == null)
        {
            return NotFound("User with this Id not found");
        }
        
        var post = await _context.Posts.FindAsync(commentDTO.PostId);
        if (post == null)
        {
            return NotFound("Post with this Id not found");
        }

        currentComment.Id = commentDTO.Id;
        currentComment.Content = commentDTO.Content;
        currentComment.UserId = commentDTO.UserId;
        currentComment.PostId = commentDTO.PostId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CommentExists(id))
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

    // POST: api/comments
    [HttpPost]
    public async Task<ActionResult<CommentDTO>> PostComment([FromBody] CommentDTO commentDTO)
    {
        var user = await _context.Users.FindAsync(commentDTO.UserId);
        if (user == null)
        {
            return NotFound("A user of this comment does not exist!");
        }
        if (user.IsVerified != true)
        {
            return BadRequest("User that is not verified can not create comments!");
        }

        var post = await _context.Posts.FindAsync(commentDTO.PostId);
        if (user == null)
        {
            return NotFound("Post not found");
        }
        
        var comment = DTOToComment(commentDTO);
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetComment), new { id = commentDTO.Id }, commentDTO);
    }

    // DELETE: api/comments/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(long id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
        {
            return NotFound();
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CommentExists(long id)
    {
        return _context.Comments.Any(e => e.Id == id);
    }

    private static Comment DTOToComment (CommentDTO commentDTO) => 
        new Comment
        {
            Id = commentDTO.Id,
            Content = commentDTO.Content,
            UserId = commentDTO.UserId,
            PostId = commentDTO.PostId
        };

    private static CommentDTO CommentToDTO(Comment comment) =>
        new CommentDTO
        {
            Id = comment.Id,
            Content = comment.Content,
            UserId = comment.UserId,
            PostId = comment.PostId
        };
}