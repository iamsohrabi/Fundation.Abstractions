namespace Fundation.Abstractions.Domain;

public interface IHaveCreator
{
    DateTime Created { get; }
    int? CreatedBy { get; }
}
