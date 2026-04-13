using StudyManager.Storage;

namespace StudyManager.Repositories;

internal sealed class FakeStudyStore : IStudyStore
{
    public IReadOnlyList<SubjectRecord> Subjects { get; }
    public IReadOnlyList<LessonRecord> Lessons { get; }

    public FakeStudyStore()
    {
        var s1 = new SubjectRecord(Guid.NewGuid(), "Алгоритми і структури даних", 5, SubjectDomain.Programming);
        var s2 = new SubjectRecord(Guid.NewGuid(), "Дискретна математика", 4, SubjectDomain.Mathematics);
        var s3 = new SubjectRecord(Guid.NewGuid(), "Інженерна етика", 3, SubjectDomain.Engineering);

        Subjects = new List<SubjectRecord> { s1, s2, s3 };

        Lessons = new List<LessonRecord>
        {
            // 10 занять для s1
            new(s1.Id, new DateOnly(2026, 2, 24), new TimeOnly(10, 0), new TimeOnly(11, 30), "Вступ. Оцінка складності. Big-O", LessonType.Lecture),
            new(s1.Id, new DateOnly(2026, 2, 26), new TimeOnly(12, 0), new TimeOnly(13, 30), "Масиви та списки: операції і компроміси", LessonType.Lecture),
            new(s1.Id, new DateOnly(2026, 3, 3),  new TimeOnly(10, 0), new TimeOnly(11, 30), "Стек і черга. Реалізації", LessonType.Lecture),
            new(s1.Id, new DateOnly(2026, 3, 5),  new TimeOnly(12, 0), new TimeOnly(13, 30), "Практика: стек/черга в задачах", LessonType.Practice),
            new(s1.Id, new DateOnly(2026, 3, 10), new TimeOnly(10, 0), new TimeOnly(11, 30), "Рекурсія та рекурентні співвідношення", LessonType.Lecture),
            new(s1.Id, new DateOnly(2026, 3, 12), new TimeOnly(12, 0), new TimeOnly(13, 30), "Сортування: merge/quick, стабільність", LessonType.Lecture),
            new(s1.Id, new DateOnly(2026, 3, 17), new TimeOnly(10, 0), new TimeOnly(11, 30), "Бінарний пошук і інваріанти", LessonType.Seminar),
            new(s1.Id, new DateOnly(2026, 3, 19), new TimeOnly(12, 0), new TimeOnly(13, 30), "Лаба: реалізація сортувань та тести", LessonType.Lab),
            new(s1.Id, new DateOnly(2026, 3, 24), new TimeOnly(10, 0), new TimeOnly(11, 30), "Хеш-таблиці: колізії, load factor", LessonType.Lecture),
            new(s1.Id, new DateOnly(2026, 3, 26), new TimeOnly(12, 0), new TimeOnly(13, 30), "Лаба: хеш-мапа (open addressing)", LessonType.Lab),

            // 2 заняття для s2
            new(s2.Id, new DateOnly(2026, 2, 25), new TimeOnly(14, 0), new TimeOnly(15, 30), "Множини, відношення, функції", LessonType.Lecture),
            new(s2.Id, new DateOnly(2026, 3, 4),  new TimeOnly(14, 0), new TimeOnly(15, 30), "Графи: базові означення та приклади", LessonType.Lecture),

            // 2 заняття для s3
            new(s3.Id, new DateOnly(2026, 3, 6), new TimeOnly(9, 0), new TimeOnly(10, 30), "Професійна відповідальність інженера", LessonType.Lecture),
new(s3.Id, new DateOnly(2026, 3, 13), new TimeOnly(9, 0), new TimeOnly(10, 30), "Етика рішень і ризиків у проєктах", LessonType.Seminar),
        };
    }
}