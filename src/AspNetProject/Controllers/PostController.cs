using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetProject.Models;
using AspNetProject.DTOs;
using StackExchange.Redis;
using System.Text.Json;
using System.Diagnostics;

namespace AspNetProject.Controllers;

[Route("api/posts")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly ApplicationContext _context;
    private readonly HttpClient _client;
    private readonly IDatabase _redis;
    private readonly ILogger _logger;

    public PostController(ApplicationContext context, HttpClient client, IConnectionMultiplexer muxer, ILogger<PostController> logger)
    {
        _context = context;
        _client = client;
        _redis = muxer.GetDatabase();
        _logger = logger;
    }

    // GET: api/posts
    [HttpGet]
    public async Task<ActionResult<PostWithRedisDTO>> GetPosts()
    {
        var watch = Stopwatch.StartNew();

        var keyName = "posts";
        var json = await _redis.StringGetAsync(keyName);
        if (string.IsNullOrEmpty(json))
        {
            _logger.LogInformation("PostController: cache miss!");
            var posts = await _context.Posts
            .Select(x => PostToDTO(x))
            .ToListAsync();
            json = JsonSerializer.Serialize(posts);
            var setTask = _redis.StringSetAsync(keyName, json);
            var expireTask = _redis.KeyExpireAsync(keyName, TimeSpan.FromSeconds(3600));
            await Task.WhenAll(setTask, expireTask);
        }
        else
        {
            _logger.LogInformation("PostController: cache hit!");
        }

        watch.Stop();
        var postsResult = JsonSerializer.Deserialize<IEnumerable<PostDTO>>(json.ToString());
        var result = new PostWithRedisDTO(postsResult, watch.ElapsedMilliseconds);
        return Ok(result);
        
    }

    // GET: api/posts/5
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

    // PUT: api/posts/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPost(long id, PostDTO postDTO)
        {

        var currentPost = await _context.Posts.FindAsync(id);
        if(currentPost == null) {
            return NotFound();
        }

        var user = await _context.Users.AnyAsync(u => u.Id == postDTO.UserId);
        if (!user)
        {
            return NotFound("User with this Id is not found");
        }

        currentPost.Title = postDTO.Title;
        currentPost.Content = postDTO.Content;
        currentPost.UserId = postDTO.UserId;
        currentPost.IsDraft = postDTO.IsDraft;

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

    // POST: api/posts
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

        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, postDTO);
    }

    // DELETE: api/posts/5
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

    // GET: /api/users/5/posts
    [HttpGet("/api/users/{userId}/posts")]
    public async Task<ActionResult<IEnumerable<PostDTO>>> GetPostsFromUser([FromRoute] long userId)
    {   
        var user = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!user)
        {   
            return NotFound("User with this id not found");
        }

        var posts = await _context.Posts
            .Where(p => p.UserId == userId)
            .Select(p => PostToDTO(p))
            .ToListAsync();

        return Ok(posts);
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
            UserId = postDTO.UserId,
            IsDraft = postDTO.IsDraft,
            CreatedAt = postDTO.CreatedAt
        };

    private static PostDTO PostToDTO(Post post) =>
        new PostDTO
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            UserId = post.UserId,
            IsDraft = post.IsDraft,
            CreatedAt = post.CreatedAt
        };
}
