using System.Windows;
using System.Windows.Controls;
using StudyManager.Services;

namespace StudyManager.WpfApp.Pages;

public partial class LessonDetailsPage : Page
{
    private readonly IStudyRepository _repo;
    private readonly Guid _lessonId;

    public LessonDetailsPage(IStudyRepository repo, Guid lessonId)
    {
        InitializeComponent();
        _repo = repo;
        _lessonId = lessonId;

        Loaded += LessonDetailsPage_Loaded;
    }

    private void LessonDetailsPage_Loaded(object sender, RoutedEventArgs e)
    {
        var lesson = _repo.GetLesson(_lessonId);
        if (lesson is null)
        {
            MessageBox.Show("Заняття не знайдено.");
            return;
        }

        TitleText.Text = lesson.Topic;
        InfoText.Text = $"Дата: {lesson.Date:yyyy-MM-dd} | {lesson.StartTime:HH\\:mm}-{lesson.EndTime:HH\\:mm} | Тип: {lesson.Type}";
        DurationText.Text = $"Тривалість: {(int)lesson.Duration.TotalHours} год {lesson.Duration.Minutes} хв";
    }
}