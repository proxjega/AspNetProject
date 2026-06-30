using AspNetProject.Controllers;
using AspNetProject.DTOs;
using AspNetProject.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetProject.Services;

public class PostService : IAsyncDisposable
{
    private readonly ApplicationContext _context;
    
    public PostService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<PostWithCommentDTO> CreatePostWithComment(PostWithCommentDTO dto)
    {
        //validation
        bool UserExists = await _context.Users.AnyAsync(u => u.Id == dto.PostDTO.UserId);
        if (!UserExists) throw new DbUpdateException("User of post does not exist");

        UserExists = await _context.Users.AnyAsync(u => u.Id == dto.CommentDTO.UserId);
        if (!UserExists) throw new DbUpdateException("User of comment does not exist");

        if (dto.PostDTO.Id != dto.CommentDTO.PostId) throw new DbUpdateException("Post and Comment Ids differ");
        
        await using var transaction = await _context.Database.BeginTransactionAsync();
        var post = PostController.DTOToPost(dto.PostDTO);
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        
        var comment = CommentController.DTOToComment(dto.CommentDTO);
        comment.PostId = post.Id;
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        await _context.Database.CommitTransactionAsync();

        return new PostWithCommentDTO{PostDTO = PostController.PostToDTO(post), CommentDTO = CommentController.CommentToDTO(comment)};

    }

    public ValueTask DisposeAsync()
    {
        return _context.DisposeAsync();
    }
}