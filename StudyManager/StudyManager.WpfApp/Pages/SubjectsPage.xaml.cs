using System.Windows;
using System.Windows.Controls;
using StudyManager.App;
using StudyManager.Services;

namespace StudyManager.WpfApp.Pages;

public partial class SubjectsPage : Page
{
    private readonly IStudyRepository _repo;
    private readonly Func<Guid, SubjectDetailsPage> _subjectDetailsFactory;

    public SubjectsPage(IStudyRepository repo, Func<Guid, SubjectDetailsPage> subjectDetailsFactory)
    {
        InitializeComponent();
        _repo = repo;
        _subjectDetailsFactory = subjectDetailsFactory;

        Loaded += SubjectsPage_Loaded;
    }

    private void SubjectsPage_Loaded(object sender, RoutedEventArgs e)
    {
        SubjectsList.ItemsSource = _repo.GetAllSubjects();
    }

    private void Open_Click(object sender, RoutedEventArgs e)
    {
        if (SubjectsList.SelectedItem is not SubjectView subject)
        {
            MessageBox.Show("Оберіть предмет зі списку.");
            return;
        }

        NavigationService?.Navigate(_subjectDetailsFactory(subject.Id));
    }
}