using AspNetProject.DTOs;

namespace AspNetProject.DTOs;

public record PostWithCommentDTO
{
    public required PostDTO PostDTO {get; init;}
    public required CommentDTO CommentDTO { get; init; }
}