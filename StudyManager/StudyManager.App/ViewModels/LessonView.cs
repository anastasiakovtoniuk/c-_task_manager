using System;
using System.Collections.Generic;
using System.Text;

using StudyManager.Storage;

namespace StudyManager.App;

public sealed class LessonView
{
    public Guid Id { get; }
    public Guid SubjectId { get; }
    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public string Topic { get; private set; }
    public LessonType Type { get; private set; }

    // обчислюване поле (дозволено в App-класі)
    public TimeSpan Duration => EndTime.ToTimeSpan() - StartTime.ToTimeSpan();

    public LessonView(Guid id, Guid subjectId, DateOnly date, TimeOnly start, TimeOnly end, string topic, LessonType type)
    {
        Id = id;
        SubjectId = subjectId;
        Date = date;
        StartTime = start;
        EndTime = end;
        Topic = topic;
        Type = type;
    }

    // базове “редагування”
    public void Edit(DateOnly date, TimeOnly start, TimeOnly end, string topic, LessonType type)
    {
        if (string.IsNullOrWhiteSpace(topic)) throw new ArgumentException("Topic is required.", nameof(topic));
        if (end <= start) throw new ArgumentException("EndTime must be after StartTime.");

        Date = date;
        StartTime = start;
        EndTime = end;
        Topic = topic.Trim();
        Type = type;
    }

    public override string ToString()
        => $"{Date:yyyy-MM-dd} {StartTime:HH\\:mm}-{EndTime:HH\\:mm} | {Type} | {Topic}";
}