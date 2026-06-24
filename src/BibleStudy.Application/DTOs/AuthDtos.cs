namespace BibleStudy.Application.DTOs;

public record RegisterDto(string Email, string Username, string Password);

public record LoginDto(string Username, string Password);

public record AuthResponseDto(string AccessToken, string RefreshToken);

public record RefreshRequestDto(string RefreshToken);