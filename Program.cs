using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapPost("/api/register", async ([FromBody] monivestuserapi.Models.UserRegistrationRequest request) =>
{
    var validationResults = new List<ValidationResult>();
    var context = new ValidationContext(request);

    Validator.TryValidateObject(request, context, validationResults, true);

    ValidateEmail(request.Email, validationResults);
    ValidatePassword(request.Password, validationResults);
    ValidatePhone(request.PhoneNumber, validationResults);

    if (validationResults.Any())
    {
        var errors = validationResults
            .GroupBy(r => r.MemberNames.FirstOrDefault() ?? string.Empty)
            .ToDictionary(
                g => g.Key,
                g => g.Select(r => r.ErrorMessage ?? "Invalid value").ToArray()
            );

        return Results.ValidationProblem(errors);
    }

    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

    var normalizedPhone = NormalizePhone(request.PhoneNumber);

    return Results.Ok(new
    {
        Message = "Account created successfully!",
        User = new
        {
            request.FullName,
            request.Email,
            PhoneNumber = normalizedPhone
        }
    });

})
.WithName("RegisterUser")
.WithDescription("Creates a new user account");

app.MapGet("/", () => Results.Redirect("/swagger"))
   .WithName("Root")
   .WithDescription("Redirects to Swagger UI");

app.Run();



static void ValidateEmail(string email, List<ValidationResult> results)
{
    if (string.IsNullOrWhiteSpace(email))
        return;

    if (!new EmailAddressAttribute().IsValid(email))
    {
        results.Add(new ValidationResult(
            "Invalid email address format.",
            new[] { nameof(email) }
        ));
    }
}

static void ValidatePassword(string password, List<ValidationResult> results)
{
    if (string.IsNullOrWhiteSpace(password))
        return;

    if (password.Length < 8 ||
        !password.Any(char.IsUpper) ||
        !password.Any(char.IsLower) ||
        !password.Any(char.IsDigit) ||
        !password.Any(ch => !char.IsLetterOrDigit(ch)))
    {
        results.Add(new ValidationResult(
            "Password must be at least 8 characters and include uppercase, lowercase, number, and special character.",
            new[] { nameof(password) }
        ));
    }
}

static void ValidatePhone(string phone, List<ValidationResult> results)
{
    if (string.IsNullOrWhiteSpace(phone))
        return;

    if (!IsValidPhone(phone))
    {
        results.Add(new ValidationResult(
            "Phone number must be a valid Nigerian number (11 digits)",
            new[] { nameof(phone) }
        ));
    }
}


static bool IsValidPhone(string phone)
{
    phone = phone.Replace(" ", "");

    if (phone.StartsWith("+234"))
        phone = "0" + phone.Substring(4);

    return phone.Length == 11 && phone.All(char.IsDigit);
}

static string NormalizePhone(string phone)
{
    phone = phone.Replace(" ", "");

    if (phone.StartsWith("+234"))
        return "0" + phone.Substring(4);

    return phone;
}