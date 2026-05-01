using Artroplus.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Artroplus.Api.Controllers;

/// <summary>
/// Tüm API controller'larının türetileceği temel controller.
/// CLAUDE.md Kural 1: [Authorize] tüm controller'larda zorunludur.
/// </summary>
[ApiController]
[Authorize] // CLAUDE.md: Tüm controller'lar korunmalıdır
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected ActionResult<ApiResponse<T>> Success<T>(T data, string? message = null)
        => Ok(ApiResponse<T>.Ok(data, message));

    protected ActionResult<ApiResponse> Success(string? message = null)
        => Ok(ApiResponse.Ok(message));

    protected ActionResult<ApiResponse<T>> Fail<T>(string message, int statusCode = 400)
        => StatusCode(statusCode, ApiResponse<T>.Fail(message));

    protected ActionResult<ApiResponse> Fail(string message, int statusCode = 400)
        => StatusCode(statusCode, ApiResponse.Fail(message));
}
