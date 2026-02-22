using System;
using System.Collections.Generic;
using System.Text;

namespace StudyManager.Storage;

public sealed class LessonRecord
{
    public Guid Id { get; }
    public Guid SubjectId { get; } // лише Id предмета (без посилання на об'єкт Subject)
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Topic { get; set; }
    public LessonType Type { get; set; }

    public LessonRecord(
        Guid id,
        Guid subjectId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        string topic,
        LessonType type)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty.", nameof(id));
        if (subjectId == Guid.Empty) throw new ArgumentException("SubjectId cannot be empty.", nameof(subjectId));
        if (string.IsNullOrWhiteSpace(topic)) throw new ArgumentException("Topic is required.", nameof(topic));
        if (endTime <= startTime) throw new ArgumentException("EndTime must be after StartTime.");

        Id = id;
        SubjectId = subjectId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        Topic = topic.Trim();
        Type = type;
    }

    public LessonRecord(
        Guid subjectId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        string topic,
        LessonType type)
        : this(Guid.NewGuid(), subjectId, date, startTime, endTime, topic, type)
    {
    }
}