using StudyManager.Repositories.Models;
using StudyManager.Storage;

namespace StudyManager.Repositories.Storage;

internal static class SeedData
{
    public static StudyStoreData Create()
    {
        var s1 = new SubjectEntity
        {
            Id = Guid.NewGuid(),
            Name = "Алгоритми і структури даних",
            EctsCredits = 5,
            Domain = SubjectDomain.Programming
        };

        var s2 = new SubjectEntity
        {
            Id = Guid.NewGuid(),
            Name = "Дискретна математика",
            EctsCredits = 4,
            Domain = SubjectDomain.Mathematics
        };

        var s3 = new SubjectEntity
        {
            Id = Guid.NewGuid(),
            Name = "Інженерна етика",
            EctsCredits = 3,
            Domain = SubjectDomain.Engineering
        };

        var s4 = new SubjectEntity
        {
            Id = Guid.NewGuid(),
            Name = "Бази даних",
            EctsCredits = 5,
            Domain = SubjectDomain.Programming
        };

        var s5 = new SubjectEntity
        {
            Id = Guid.NewGuid(),
            Name = "WEB технології",
            EctsCredits = 4,
            Domain = SubjectDomain.Engineering
        };

        var s6 = new SubjectEntity
        {
            Id = Guid.NewGuid(),
            Name = "Теорія ймовірностей",
            EctsCredits = 3,
            Domain = SubjectDomain.Mathematics
        };
        var lessons = new List<LessonEntity>
        {
            // 10 занять для s1
            L(s1.Id, 2026,2,24, 10,00, 11,30, "Вступ. Оцінка складності. Big-O", LessonType.Lecture),
            L(s1.Id, 2026,2,26, 12,00, 13,30, "Масиви та списки: операції і компроміси", LessonType.Lecture),
            L(s1.Id, 2026,3,03, 10,00, 11,30, "Стек і черга. Реалізації", LessonType.Lecture),
            L(s1.Id, 2026,3,05, 12,00, 13,30, "Практика: стек/черга в задачах", LessonType.Practice),
            L(s1.Id, 2026,3,10, 10,00, 11,30, "Рекурсія та рекурентні співвідношення", LessonType.Lecture),
            L(s1.Id, 2026,3,12, 12,00, 13,30, "Сортування: merge/quick, стабільність", LessonType.Lecture),
            L(s1.Id, 2026,3,17, 10,00, 11,30, "Бінарний пошук і інваріанти", LessonType.Seminar),
            L(s1.Id, 2026,3,19, 12,00, 13,30, "Лаба: реалізація сортувань та тести", LessonType.Lab),
            L(s1.Id, 2026,3,24, 10,00, 11,30, "Хеш-таблиці: колізії, load factor", LessonType.Lecture),
            L(s1.Id, 2026,3,26, 12,00, 13,30, "Лаба: хеш-мапа (open addressing)", LessonType.Lab),

            // 2 заняття для s2
            L(s2.Id, 2026,2,25, 14,00, 15,30, "Множини, відношення, функції", LessonType.Lecture),
            L(s2.Id, 2026,3,04, 14,00, 15,30, "Графи: базові означення та приклади", LessonType.Lecture),
        
            
            L(s4.Id, 2026, 4, 1, 11, 00, 12, 30, "ER-модель та сутності", LessonType.Lecture),
            L(s4.Id, 2026, 4, 3, 11, 00, 12, 30, "SQL SELECT: базові запити", LessonType.Lab),
            L(s4.Id, 2026, 4, 8, 11, 00, 12, 30, "JOIN-и та зв’язки", LessonType.Lecture),

            L(s5.Id, 2026, 4, 2, 13, 00, 14, 30, "HTML структура сторінки", LessonType.Lecture),
            L(s5.Id, 2026, 4, 9, 13, 00, 14, 30, "CSS: блокова модель", LessonType.Seminar),
            L(s5.Id, 2026, 4, 16, 13, 00, 14, 30, "JS: події та обробники", LessonType.Lab),

            L(s6.Id, 2026, 4, 4, 9, 00, 10, 30, "Ймовірність. Основні поняття", LessonType.Lecture),
            L(s6.Id, 2026, 4, 11, 9, 00, 10, 30, "Дискретні розподіли", LessonType.Seminar),
         };

        return new StudyStoreData
        {
            Subjects = new List<SubjectEntity> { s1, s2, s3, s4, s5, s6 },
            Lessons = lessons
        };
    }

    private static LessonEntity L(Guid subjectId, int y, int m, int d, int sh, int sm, int eh, int em, string topic, LessonType type)
        => new LessonEntity
        {
            Id = Guid.NewGuid(),
            SubjectId = subjectId,
            Date = new DateOnly(y, m, d),
            StartTime = new TimeOnly(sh, sm),
            EndTime = new TimeOnly(eh, em),
            Topic = topic,
            Type = type
        };
}