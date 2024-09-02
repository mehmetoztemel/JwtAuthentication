namespace JwtAuthentication.Dtos
{
    public record CreateAppUserDto(string FirstName, string LastName, string Email, string Password)
    {
    }
}
