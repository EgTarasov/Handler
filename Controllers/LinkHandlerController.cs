using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[ApiController]
[Route("[controller]")]
public class LinkHandlerController : ControllerBase
{
    private const string UrlLinkPaternInHTMLFIle = @"(?:href=|img alt=\\)""(.+?)""";
    
    private readonly ILogger<LinkHandlerController> _logger;

    public LinkHandlerController(ILogger<LinkHandlerController> logger)
    {
        _logger = logger;
    }
    
    [NonAction]
    private List<string> FindLinks(string text)
    {
        var list = Regex
            .Matches(text, UrlLinkPaternInHTMLFIle)
            .Select(x => x.Groups[1].Value)
            .ToList();
        return list;
    }

    [NonAction]
    private async Task<string> GetHtmlText(string url)
    {
        var client = new HttpClient();
        var text = await client.GetStringAsync(url);
        return text;
    }

    [HttpGet("FindAllLinks")]
    public async Task<ActionResult> GetAllLinks(string link)
    {
        try
        {
            var text = await GetHtmlText(link);
            var links = new LinksCollection(FindLinks(text));
            var res = $"<!DOCTYPE html><html lang=\"en\"><body>";
            res += links;
            res += "</body></html>";
            return base.Content(res, contentType: "text/html");
        }
        catch (InvalidOperationException)
        {
            return base.BadRequest("Incorrect link format");
        }
        catch
        {
            return base.BadRequest("Denied");
        }
    }
}