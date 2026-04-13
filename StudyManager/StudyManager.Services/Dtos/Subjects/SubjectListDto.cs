namespace StudyManager.Services.Dtos.Subjects;

public sealed record SubjectListDto(
    Guid Id,
    string Name,
    int EctsCredits,
    string Domain
);