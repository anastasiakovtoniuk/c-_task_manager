using StudyManager.App;
using StudyManager.Services;

namespace StudyManager.ConsoleApp;

internal static class Program
{
    private static readonly IStudyRepository Repo = new StudyRepository();

    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var subjects = Repo.GetAllSubjects().ToList();

        while (true)
        {
            Console.Clear();
            PrintSubjects(subjects);

            Console.WriteLine();
            Console.WriteLine("Команди: [номер] вибрати предмет | r оновити список | q вихід");
            Console.Write("Ввід: ");
            var input = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

            if (input == "q") return;
            if (input == "r")
            {
                subjects = Repo.GetAllSubjects().ToList();
                continue;
            }

            if (!int.TryParse(input, out var idx) || idx < 1 || idx > subjects.Count)
            {
                Pause("Невірний ввід.");
                continue;
            }

            ShowSubject(subjects[idx - 1]);
        }
    }

    private static void PrintSubjects(IReadOnlyList<SubjectView> subjects)
    {
        Console.WriteLine("=== Менеджер занять: Предмети ===");
        for (int i = 0; i < subjects.Count; i++)
        {
            var s = subjects[i];
            Console.WriteLine($"{i + 1}. {s}");
        }
    }

    private static void ShowSubject(SubjectView subject)
    {
        Repo.EnsureLessonsLoaded(subject);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Деталі предмету ===");
            Console.WriteLine($"Назва: {subject.Name}");
            Console.WriteLine($"ECTS: {subject.EctsCredits}");
            Console.WriteLine($"Сфера: {subject.Domain}");
            Console.WriteLine($"Загальна тривалість занять: {FormatDuration(subject.TotalDuration)}");
            Console.WriteLine();

            Console.WriteLine("=== Заняття ===");
            if (subject.Lessons.Count == 0)
            {
                Console.WriteLine("(Немає занять)");
            }
            else
            {
                for (int i = 0; i < subject.Lessons.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {subject.Lessons[i]}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Команди: [номер] деталі заняття | b назад | r перезавантажити заняття | q вихід");
            Console.Write("Ввід: ");
            var input = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

            if (input == "q") Environment.Exit(0);
            if (input == "b") return;

            if (input == "r")
            {
                var lessons = Repo.GetLessonsForSubject(subject.Id);
                subject.SetLessons(lessons);
                continue;
            }

            if (!int.TryParse(input, out var idx) || idx < 1 || idx > subject.Lessons.Count)
            {
                Pause("Невірний ввід.");
                continue;
            }

            ShowLesson(subject.Lessons[idx - 1]);
        }
    }

    private static void ShowLesson(LessonView lesson)
    {
        Console.Clear();
        Console.WriteLine("=== Деталі заняття ===");
        Console.WriteLine($"Дата: {lesson.Date:yyyy-MM-dd}");
        Console.WriteLine($"Час: {lesson.StartTime:HH\\:mm} - {lesson.EndTime:HH\\:mm}");
        Console.WriteLine($"Тип: {lesson.Type}");
        Console.WriteLine($"Тема: {lesson.Topic}");
        Console.WriteLine($"Тривалість: {FormatDuration(lesson.Duration)}");
        Console.WriteLine();
        Pause("Натисни Enter щоб повернутись...");
    }

    private static string FormatDuration(TimeSpan ts)
        => $"{(int)ts.TotalHours} год {ts.Minutes} хв";

    private static void Pause(string message)
    {
        Console.WriteLine(message);
        Console.ReadLine();
    }
}