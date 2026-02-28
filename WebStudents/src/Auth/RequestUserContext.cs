using Microsoft.AspNetCore.Mvc;

namespace WebStudents.src.Auth;

public sealed class RequestUserContext
{
    public string Role { get; }
    public Guid? LinkedPersonId { get; }

    public RequestUserContext(string role, Guid? linkedPersonId)
    {
        Role = role;
        LinkedPersonId = linkedPersonId;
    }

    public bool IsInRole(params string[] roles)
    {
        return roles.Any(r => string.Equals(r, Role, StringComparison.OrdinalIgnoreCase));
    }

    public static RequestUserContext FromHeaders(IHeaderDictionary headers)
    {
        var role = headers["X-Role"].FirstOrDefault() ?? string.Empty;
        var linked = headers["X-LinkedPersonId"].FirstOrDefault();

        Guid? linkedPersonId = null;
        if (!string.IsNullOrWhiteSpace(linked) && Guid.TryParse(linked, out var parsed))
        {
            linkedPersonId = parsed;
        }

        return new RequestUserContext(role.Trim(), linkedPersonId);
    }
}

public static class ControllerAuthExtensions
{
    public static RequestUserContext GetUserContext(this ControllerBase controller)
    {
        return RequestUserContext.FromHeaders(controller.Request.Headers);
    }

    public static IActionResult? RequireRoles(this ControllerBase controller, params string[] roles)
    {
        var user = controller.GetUserContext();
        if (user.IsInRole(roles))
        {
            return null;
        }

        return controller.StatusCode(StatusCodes.Status403Forbidden, new { message = "Недостаточно прав" });
    }
}
