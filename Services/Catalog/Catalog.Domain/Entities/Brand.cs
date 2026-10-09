using Catalog.Domain.Exceptions;

namespace Catalog.Domain.Entities;

public class Brand
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    private Brand()
    {
        // Required by EF Core.
    }

    public Brand(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Brand name is required.");

        Id = Guid.NewGuid();
        Name = name;
    }
}