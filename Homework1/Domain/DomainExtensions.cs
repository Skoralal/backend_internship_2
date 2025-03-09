namespace Fuse8.BackendInternship.Domain;

public static class DomainExtensions
{
    // ToDo: реализовать экстеншены
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> values)
    {
        return values == null || !values.Any();
    }
    public static string JoinToString<T>(this IEnumerable<T> values)
    {
        return string.Join(", ", values);
    }
    public static int DaysCountBetween(this DateTimeOffset firstPoint, DateTimeOffset secondPoint)
    {
        return Math.Abs((firstPoint - secondPoint).Days);
    }
}