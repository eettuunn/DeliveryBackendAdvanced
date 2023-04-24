﻿using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace AuthApi.Common.Middlewares;

public class LoggerMiddleware
{
    private readonly RequestDelegate _next;

    public LoggerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        //todo: incorrect status code
        var curPath = Directory.GetCurrentDirectory();
        var logPath = Path.GetFullPath(Path.Combine(curPath, @$"..\AuthApi.Common\Logs\{DateTime.UtcNow.ToShortDateString()}.txt"));
        var log = CreateLogMessage(context);
        await using (StreamWriter sw = File.AppendText(logPath))
        {
            await sw.WriteLineAsync(log);
            if (context.Request.Method == "POST" || context.Request.Method == "PUT")
            {
                var body = await CreateBody(context.Request);
                await sw.WriteLineAsync(body);
            }

            await sw.WriteLineAsync();
        }

        await _next(context);
    }



    private string CreateLogMessage(HttpContext context)
    {
        var url = context.Request.GetDisplayUrl();
        var method = context.Request.Method;
        var protocol = context.Request.Protocol;
        var status = context.Response.StatusCode;

        var message = DateTime.UtcNow.TimeOfDay + "   URL: " + url + "   METHOD: " + method + "   PROTOCOL: " + protocol + "   STATUS_CODE: " + status;

        return message;
    }

    private async Task<string> CreateBody(HttpRequest request)
    {
        request.EnableBuffering();
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        await request.Body.ReadAsync(buffer, 0, buffer.Length);
        var body = Encoding.UTF8.GetString(buffer);
        request.Body.Position = 0;

        return body;
    }
}