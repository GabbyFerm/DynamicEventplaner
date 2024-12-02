using DynamicEventplaner.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using static DynamicEventplaner.Classes.Event;

namespace DynamicEventplaner.Classes
{
    public class EventManager
    {
        private List<Event> events = new List<Event>();

        public void CreateEvent()
        {
            AnsiConsole.MarkupLine("[yellow]Create a new event[/]");

            string eventName = AnsiConsole.Ask<string>("[springgreen3]Enter event name:[/]");
            if (string.IsNullOrWhiteSpace(eventName))
            {
                AnsiConsole.MarkupLine("[red]Event name cannot be empty.[/]");
                return;
            }

            DateTime eventDateTime;
            while (true)
            {
                string dateInput = AnsiConsole.Ask<string>("[springgreen3]Enter event date and time (yyyy-MM-dd HH:mm):[/]");
                if (DateTime.TryParse(dateInput, out eventDateTime))
                    break;

                AnsiConsole.MarkupLine("[red]Invalid date and time. Please use the format yyyy-MM-dd HH:mm.[/]");
            }

            TimeSpan duration;
            while (true)
            {
                string durationInput = AnsiConsole.Ask<string>("[springgreen3]Enter duration (e.g., 1:30):[/]");
                string[] parts = durationInput.Split(':');
                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out int hours) &&
                    int.TryParse(parts[1], out int minutes))
                {
                    duration = new TimeSpan(hours, minutes, 0);
                    break;
                }

                AnsiConsole.MarkupLine("[red]Invalid duration format. Please use the format HH:MM.[/]");
            }

            string timeZone = TimeZoneHelper.SelectTimeZone();

            var newEvent = new Event
            {
                Name = eventName,
                StartTime = eventDateTime,
                Duration = duration,
                TimeZone = timeZone
            };

            // Kontrollera om det nya eventet överlappar med befintliga events
            if (CheckEventConflict(newEvent))
            {
                AnsiConsole.MarkupLine("[red]Conflict detected: This event overlaps with an existing event.[/]");
                return;
            }

            events.Add(newEvent);
            AnsiConsole.MarkupLine($"[lightseagreen]Event '{newEvent.Name}' successfully created![/]");

            // Visa eventets tid i flera tidszoner
            ShowEventTimeInMultipleTimeZones(newEvent);

            // Visa nedräkningen till eventet
            ShowCountdownToEvent(newEvent);
        }

        private bool CheckEventConflict(Event newEvent)
        {
            foreach (var existingEvent in events)
            {
                // Om eventen är i olika tidszoner, konvertera dem till samma tidszon
                DateTime newEventStartTimeInUTC = ConvertToTimeZone(newEvent.StartTime, "UTC");
                DateTime newEventEndTimeInUTC = newEventStartTimeInUTC.Add(newEvent.Duration);

                DateTime existingEventStartTimeInUTC = ConvertToTimeZone(existingEvent.StartTime, "UTC");
                DateTime existingEventEndTimeInUTC = existingEventStartTimeInUTC.Add(existingEvent.Duration);

                // Kontrollera om eventen överlappar varandra
                if ((newEventStartTimeInUTC < existingEventEndTimeInUTC) &&
                    (newEventEndTimeInUTC > existingEventStartTimeInUTC))
                {
                    return true; // Det finns en konflikt
                }
            }
            return false; // Ingen konflikt
        }

        private void ShowEventTimeInMultipleTimeZones(Event eventToDisplay)
        {
            // Lista av tidszoner som användaren kan välja mellan
            var timeZones = new List<string>
            {
                "UTC", "Eastern Standard Time", "Pacific Standard Time", "Central European Standard Time"
            };

            // Visa eventets starttid i olika tidszoner
            foreach (var timeZone in timeZones)
            {
                DateTime eventTimeInTimeZone = ConvertToTimeZone(eventToDisplay.StartTime, timeZone);
                AnsiConsole.MarkupLine($"Event '{eventToDisplay.Name}' in [yellow]{timeZone}[/] is at: [lightseagreen]{eventTimeInTimeZone:yyyy-MM-dd HH:mm}[/]");
            }
        }

        private DateTime ConvertToTimeZone(DateTime eventTime, string timeZoneId)
        {
            try
            {
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return TimeZoneInfo.ConvertTime(eventTime, timeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                AnsiConsole.MarkupLine($"[red]Time zone '{timeZoneId}' not found.[/]");
                return eventTime;
            }
            catch (InvalidTimeZoneException)
            {
                AnsiConsole.MarkupLine($"[red]Invalid time zone '{timeZoneId}' provided.[/]");
                return eventTime;
            }
        }

        public void CreateRecurringEvent()
        {
            AnsiConsole.MarkupLine("[yellow]Create a recurring event[/]");

            string eventName = AnsiConsole.Ask<string>("[springgreen3]Enter event name:[/]");
            if (string.IsNullOrWhiteSpace(eventName))
            {
                AnsiConsole.MarkupLine("[red]Event name cannot be empty.[/]");
                return;
            }

            DateTime eventDateTime;
            while (true)
            {
                string dateInput = AnsiConsole.Ask<string>("[springgreen3]Enter event date and time (yyyy-MM-dd HH:mm):[/]");
                if (DateTime.TryParse(dateInput, out eventDateTime))
                    break;

                AnsiConsole.MarkupLine("[red]Invalid date and time. Please use the format yyyy-MM-dd HH:mm.[/]");
            }

            TimeSpan duration;
            while (true)
            {
                string durationInput = AnsiConsole.Ask<string>("[springgreen3]Enter duration (e.g., 1:30):[/]");
                string[] parts = durationInput.Split(':');
                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out int hours) &&
                    int.TryParse(parts[1], out int minutes))
                {
                    duration = new TimeSpan(hours, minutes, 0);
                    break;
                }

                AnsiConsole.MarkupLine("[red]Invalid duration format. Please use the format HH:MM.[/]");
            }

            string timeZone = TimeZoneHelper.SelectTimeZone();

            Event.RecurrencePattern recurrencePattern; // Correctly reference the enum from the Event class
            while (true)
            {
                int choice = AnsiConsole.Ask<int>("[springgreen3]Choose recurrence pattern: 1. Daily, 2. Weekly, 3. Monthly[/]");
                if (choice == 1) recurrencePattern = Event.RecurrencePattern.Daily;
                else if (choice == 2) recurrencePattern = Event.RecurrencePattern.Weekly;
                else if (choice == 3) recurrencePattern = Event.RecurrencePattern.Monthly;
                else
                {
                    AnsiConsole.MarkupLine("[red]Invalid recurrence pattern. Try again.[/]");
                    continue;
                }
                break;
            }

            var newEvent = new Event
            {
                Name = eventName,
                StartTime = eventDateTime,
                Duration = duration,
                TimeZone = timeZone,
                IsRecurring = true,
                Pattern = recurrencePattern // Proper assignment
            };

            events.Add(newEvent);
            AnsiConsole.MarkupLine($"[lightseagreen]Recurring event '{newEvent.Name}' successfully created![/]");

            var eventDates = GetNextOccurrences(newEvent, 5);
            AnsiConsole.MarkupLine("[yellow]Next occurrences:[/]");
            foreach (var date in eventDates)
            {
                AnsiConsole.MarkupLine($"- [lightseagreen]{date:yyyy-MM-dd HH:mm}[/]");
            }
        }

        private List<DateTime> GetNextOccurrences(Event recurringEvent, int count)
        {
            var occurrences = new List<DateTime>();
            DateTime currentOccurrence = recurringEvent.StartTime;

            for (int i = 0; i < count; i++)
            {
                currentOccurrence = GetNextOccurrence(currentOccurrence, recurringEvent.Pattern);
                occurrences.Add(currentOccurrence);
            }

            return occurrences;
        }

        private DateTime GetNextOccurrence(DateTime currentOccurrence, Event.RecurrencePattern pattern)
        {
            switch (pattern)
            {
                case Event.RecurrencePattern.Daily:
                    return currentOccurrence.AddDays(1);
                case Event.RecurrencePattern.Weekly:
                    return currentOccurrence.AddDays(7);
                case Event.RecurrencePattern.Monthly:
                    return currentOccurrence.AddMonths(1);
                default:
                    return currentOccurrence;
            }
        }
        public void ShowCountdownToExistingEvent()
        {
            // Om inga event finns, informera användaren
            if (events.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No events available to show countdown for.[/]");
                return;
            }

            // Visa alla kommande event och låt användaren välja ett
            var upcomingEvents = events.Where(e => e.StartTime > DateTime.Now).ToList();
            if (upcomingEvents.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]There are no upcoming events to show countdown for.[/]");
                return;
            }

            // Skapa en lista av eventnamn utan null-värden
            var eventChoices = upcomingEvents.Select(e => e.Name).Where(name => name != null).ToList();

            // Om det finns eventnamn i listan, visa dem som val
            if (eventChoices.Any())
            {
                var selectedEventName = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select an event to show countdown")
                        .AddChoices(eventChoices)
                        .HighlightStyle(new Style(foreground: Color.LightSeaGreen)));

                // Hitta det valda eventet
                var selectedEvent = upcomingEvents.FirstOrDefault(e => e.Name == selectedEventName);

                // Visa nedräkningen för det valda eventet
                if (selectedEvent != null)
                {
                    ShowCountdownToEvent(selectedEvent);
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[red]No valid event names to choose from.[/]");
            }
        }
        private void ShowCountdownToEvent(Event eventToDisplay)
        {
            // Beräkna skillnaden mellan nuvarande tid och eventets starttid
            TimeSpan timeDifference = eventToDisplay.StartTime - DateTime.Now;

            if (timeDifference.TotalSeconds > 0)
            {
                // Beräkna dagar, timmar, minuter och sekunder kvar
                int daysLeft = timeDifference.Days;
                int hoursLeft = timeDifference.Hours;
                int minutesLeft = timeDifference.Minutes;
                int secondsLeft = timeDifference.Seconds;

                // Visa nedräkningen
                AnsiConsole.MarkupLine($"[yellow]Time left until event '{eventToDisplay.Name}':[/]");
                AnsiConsole.MarkupLine($"[lightseagreen]{daysLeft} days, {hoursLeft} hours, {minutesLeft} minutes, {secondsLeft} seconds[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]The event has already passed![/]");
            }
        }
        public void ShowAllComingEvents()
        {
            var upcomingEvents = new List<Event>();

            // Add all one-off events that are upcoming
            upcomingEvents.AddRange(events.Where(e => e.StartTime > DateTime.Now).OrderBy(e => e.StartTime));

            // Handle recurring events
            var recurringEvents = events.Where(e => e.IsRecurring && e.StartTime > DateTime.Now).ToList();

            foreach (var recurringEvent in recurringEvents)
            {
                // Generate the next 5 occurrences of the recurring event
                var nextOccurrences = GetNextOccurrences(recurringEvent, 5);

                // Add upcoming events (not DateTime values, but Event objects) to the list
                foreach (var occurrence in nextOccurrences)
                {
                    upcomingEvents.Add(new Event
                    {
                        Name = recurringEvent.Name,
                        StartTime = occurrence,
                        Duration = recurringEvent.Duration,
                        TimeZone = recurringEvent.TimeZone,
                        IsRecurring = true,
                        Pattern = recurringEvent.Pattern  // Correctly set RecurrencePattern
                    });
                }
            }

            // Order by start time and display events
            var allUpcomingEvents = upcomingEvents.OrderBy(e => e.StartTime).ToList();

            if (!allUpcomingEvents.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No upcoming events found.[/]");
                return;
            }

            AnsiConsole.MarkupLine("[yellow]Upcoming Events:[/]");
            foreach (var upcomingEvent in allUpcomingEvents)
            {
                AnsiConsole.MarkupLine($"- [lightseagreen]{upcomingEvent.Name}[/] - [springgreen3]{upcomingEvent.StartTime:yyyy-MM-dd HH:mm}[/]");
            }
        }
    }
}