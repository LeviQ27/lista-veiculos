var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDTO loginDto) => 
{
    if (loginDto.Email == "user@example.com" && loginDto.Password == "password")
    {
        return Results.Ok("Login successful");
    }
    else
    {
        return Results.Unauthorized();
    }
});

app.Run();

public class LoginDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
}