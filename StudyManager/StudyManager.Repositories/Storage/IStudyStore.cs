using StudyManager.Repositories.Models;

namespace StudyManager.Repositories.Storage;

public interface IStudyStore
{
    Task<StudyStoreData> LoadAsync(CancellationToken ct = default);
    Task SaveAsync(StudyStoreData data, CancellationToken ct = default);
}