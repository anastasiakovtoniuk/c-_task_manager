using System;
using System.Collections.Generic;
using System.Text;

using StudyManager.Storage;

namespace StudyManager.App;

public sealed class SubjectView
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public int EctsCredits { get; private set; }
    public SubjectDomain Domain { get; private set; }

    // колекція другого рівня (дозволено в App-класі)
    public IReadOnlyList<LessonView> Lessons { get; private set; } = Array.Empty<LessonView>();
    public bool LessonsLoaded { get; private set; }

    // обчислюване поле
    public TimeSpan TotalDuration => Lessons.Aggregate(TimeSpan.Zero, (sum, l) => sum + l.Duration);

    public SubjectView(Guid id, string name, int ectsCredits, SubjectDomain domain)
    {
        Id = id;
        Name = name;
        EctsCredits = ectsCredits;
        Domain = domain;
    }

    public void Edit(string name, int ectsCredits, SubjectDomain domain)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (ectsCredits <= 0) throw new ArgumentOutOfRangeException(nameof(ectsCredits));

        Name = name.Trim();
        EctsCredits = ectsCredits;
        Domain = domain;
    }

    public void SetLessons(IReadOnlyList<LessonView> lessons)
    {
        Lessons = lessons ?? Array.Empty<LessonView>();
        LessonsLoaded = true;
    }

    public override string ToString()
        => $"{Name} | {EctsCredits} ECTS | {Domain}";
}