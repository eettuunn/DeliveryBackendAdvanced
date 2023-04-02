using System.ComponentModel;
using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models.Dtos;
using Microsoft.AspNetCore.Http;

namespace delivery_backend_advanced.Services.ExceptionHandler;

public class ExceptionMiddlewareService
{
    private readonly RequestDelegate _next;

    public ExceptionMiddlewareService(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ConflictException exception)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(new { message = exception.Message });
        }
        catch (ForbiddenException exception)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { message = exception.Message });
        }
        catch (NotAuthorizedException exception)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = exception.Message });
        }
        catch (BadRequestException exception)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { message = exception.Message });
        }
        catch (NotFoundException exception)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new { message = exception.Message });
        }
        catch (CantFindByIdException exception)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { message = exception.Message });
        }
        catch (Exception exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(exception.Message);
        }
    }
}