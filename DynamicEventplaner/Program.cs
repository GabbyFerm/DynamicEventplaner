using Figgle;
using Spectre.Console;
using DynamicEventplaner.Classes;

namespace DynamicEventplaner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            EventManager eventManager = new EventManager();

            while (true)
            {
                Console.Clear();

                // Banner
                AnsiConsole.Write(
                    new FigletText("Event Planner")
                        .LeftJustified()
                        .Color(Color.LightSeaGreen));

                // Menu
                AnsiConsole.MarkupLine("[yellow]=========== MENU ===========[/]");
                AnsiConsole.MarkupLine("[springgreen3]1. Create Event[/]");
                AnsiConsole.MarkupLine("[springgreen3]2. Create Recurring Event[/]");
                AnsiConsole.MarkupLine("[springgreen3]3. Show All Coming Events[/]");
                AnsiConsole.MarkupLine("[springgreen3]4. Show Countdown to Event[/]");
                AnsiConsole.MarkupLine("[springgreen3]5. Exit[/]");
                AnsiConsole.MarkupLine("[yellow]==========================[/]");

                int menuOption = AnsiConsole.Prompt(
                    new TextPrompt<int>("[lightseagreen]Choose an option:[/]")
                        .Validate(choice => choice is >= 1 and <= 7
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Please enter a number between 1 and 5.[/]")));

                switch (menuOption)
                {
                    case 1:
                        eventManager.CreateEvent();
                        break;
                    case 2:
                        eventManager.CreateRecurringEvent();
                        break;
                    case 3:
                        eventManager.ShowAllComingEvents();
                        break;
                    case 4:
                        eventManager.ShowCountdownToExistingEvent();
                        break;
                    case 5:
                        AnsiConsole.MarkupLine("[green]Goodbye![/]");
                        return;
                    default:
                        AnsiConsole.MarkupLine("[red]Invalid option. Try again.[/]");
                        break;
                }

                AnsiConsole.MarkupLine("[yellow]Press any key to return to the menu.[/]");
                Console.ReadKey();
            }
        }
    }
}