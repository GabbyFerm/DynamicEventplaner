using System;
using System.Linq;
using Spectre.Console;

public static class TimeZoneHelper
{
    private static readonly List<string> CommonTimeZones = new()
    {
        "UTC",
        "Europe/Stockholm",
        "Europe/London",
        "America/New_York",
        "Asia/Tokyo",
        "Australia/Sydney"
    };
    public static void ShowAvailableTimeZones()
    {
        AnsiConsole.MarkupLine("[yellow]Available Time Zones:[/]");
        foreach (var timeZone in CommonTimeZones)
        {
            AnsiConsole.MarkupLine($"- [lightseagreen]{timeZone}[/]");
        }
    }
    public static string SelectTimeZone()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a [yellow]time zone[/]:")
                .PageSize(6)
                .AddChoices(CommonTimeZones)
                .HighlightStyle(new Style(foreground: Color.LightSeaGreen))); // Set highlight color
    }
    public static bool IsValidTimeZone(string timeZoneId)
    {
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return true;
        }
        catch (TimeZoneNotFoundException)
        {
            return false;
        }
    }
    public static DateTime ConvertToTimeZone(DateTime dateTime, string sourceTimeZone, string targetTimeZone)
    {
        try
        {
            var sourceZone = TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZone);
            var targetZone = TimeZoneInfo.FindSystemTimeZoneById(targetTimeZone);

            DateTime sourceTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(sourceTime, sourceZone);
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, targetZone);
        }
        catch (TimeZoneNotFoundException ex)
        {
            Console.WriteLine($"Error: Time zone not found ({ex.Message}).");
            throw;
        }
        catch (InvalidTimeZoneException ex)
        {
            Console.WriteLine($"Error: Invalid time zone ({ex.Message}).");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            throw;
        }
    }
}