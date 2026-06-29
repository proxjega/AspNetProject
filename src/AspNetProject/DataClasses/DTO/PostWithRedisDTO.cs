using AspNetProject.Models;

namespace AspNetProject.DTOs;

public record PostWithRedisDTO
{
    
    public IEnumerable<PostDTO>? Posts {get; init;}
    public long ElapsedTime {get; init;}

    public PostWithRedisDTO(IEnumerable<PostDTO> posts, long elapsedTime)
    {
        Posts = posts;
        ElapsedTime = elapsedTime;
    }
}