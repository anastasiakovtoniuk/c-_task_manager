using System.Windows;
using System.Windows.Controls;
using StudyManager.App;
using StudyManager.Services;

namespace StudyManager.WpfApp.Pages;

public partial class SubjectDetailsPage : Page
{
    private readonly IStudyRepository _repo;
    private readonly Guid _subjectId;
    private readonly Func<Guid, LessonDetailsPage> _lessonDetailsFactory;

    private SubjectView? _subject;

    public SubjectDetailsPage(IStudyRepository repo, Guid subjectId, Func<Guid, LessonDetailsPage> lessonDetailsFactory)
    {
        InitializeComponent();
        _repo = repo;
        _subjectId = subjectId;
        _lessonDetailsFactory = lessonDetailsFactory;

        Loaded += SubjectDetailsPage_Loaded;
    }

    private void SubjectDetailsPage_Loaded(object sender, RoutedEventArgs e)
    {
        _subject = _repo.GetSubject(_subjectId);
        if (_subject is null)
        {
            MessageBox.Show("Предмет не знайдено.");
            return;
        }

        _repo.EnsureLessonsLoaded(_subject);

        TitleText.Text = _subject.Name;
        InfoText.Text = $"ECTS: {_subject.EctsCredits} | Сфера: {_subject.Domain}";
        DurationText.Text = $"Загальна тривалість: {(int)_subject.TotalDuration.TotalHours} год {_subject.TotalDuration.Minutes} хв";

        LessonsList.ItemsSource = _subject.Lessons;
        EmptyStateText.Visibility = _subject.Lessons.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

    }

    private void OpenLesson_Click(object sender, RoutedEventArgs e)
    {
        if (LessonsList.SelectedItem is not LessonView lesson)
        {
            MessageBox.Show("Оберіть заняття зі списку.");
            return;
        }

        NavigationService?.Navigate(_lessonDetailsFactory(lesson.Id));
    }
}