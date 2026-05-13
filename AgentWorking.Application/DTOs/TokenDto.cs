namespace AgentWorking.Application.DTOs;

public record TokenDto(string Token, DateTime Expiry, UserDto User);
