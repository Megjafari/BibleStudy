using System.Security.Claims;
using BibleStudy.Application.Interfaces;

namespace BibleStudy.Api.Auth;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId
    {
        get
        {
            var sub = _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier)
                ?? _httpContextAccessor.HttpContext?.User
                .FindFirstValue("sub");

            if (sub is null || !int.TryParse(sub, out var id))
                throw new UnauthorizedAccessException("No authenticated user.");

            return id;
        }
    }
}