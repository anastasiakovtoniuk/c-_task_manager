using System;
using System.Collections.Generic;
using System.Text;

namespace StudyManager.Storage;

public sealed class SubjectRecord
{
    public Guid Id { get; }
    public string Name { get; set; }
    public int EctsCredits { get; set; }
    public SubjectDomain Domain { get; set; }

    public SubjectRecord(Guid id, string name, int ectsCredits, SubjectDomain domain)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (ectsCredits <= 0) throw new ArgumentOutOfRangeException(nameof(ectsCredits), "ECTS must be positive.");

        Id = id;
        Name = name.Trim();
        EctsCredits = ectsCredits;
        Domain = domain;
    }

    public SubjectRecord(string name, int ectsCredits, SubjectDomain domain)
        : this(Guid.NewGuid(), name, ectsCredits, domain)
    {
    }
}