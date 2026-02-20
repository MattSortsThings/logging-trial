namespace LoggingTrial.Internals;

/// <summary>
///     Represents an error that occurs in the domain while handling an unsuccessful request.
/// </summary>
public sealed record DomainError
{
    /// <summary>
    ///     Gets or initializes the domain error's unique title.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    ///     Gets or initializes the domain error's type.
    /// </summary>
    public required DomainErrorType Type { get; init; }

    /// <summary>
    ///     Gets or initializes the domain error's description.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    ///     Gets or initializes the domain error's optional additional data.
    /// </summary>
    /// <remarks>This is the only property on the <see cref="DomainError" /> type that contains request-specific values.</remarks>
    public IReadOnlyDictionary<string, object>? AdditionalData { get; init; }

    /// <inheritdoc />
    public bool Equals(DomainError? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Title == other.Title
            && Type == other.Type
            && Description == other.Description
            && EqualAdditionalData(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Title, (int)Type, Description, AdditionalData);
    }

    private bool EqualAdditionalData(DomainError other)
    {
        if (AdditionalData is null && other.AdditionalData is null)
        {
            return true;
        }

        return AdditionalData is not null
            && other.AdditionalData is { } otherAdditionalData
            && AdditionalData.Count == otherAdditionalData.Count
            && AdditionalData.All(kvp =>
                otherAdditionalData.TryGetValue(kvp.Key, out object? otherValue)
                && otherValue.Equals(kvp.Value)
            );
    }
}
