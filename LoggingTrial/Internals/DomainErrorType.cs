namespace LoggingTrial.Internals;

/// <summary>
///     Specifies a domain error's type.
/// </summary>
public enum DomainErrorType
{
    /// <summary>
    ///     The "Unexpected" domain error type; this should only ever occur due to a bug.
    /// </summary>
    Unexpected,

    /// <summary>
    ///     The "Not Found" domain error type; this occurs when the requested domain aggregate does not exist.
    /// </summary>
    NotFound,

    /// <summary>
    ///     The "Extrinsic" domain error type; this occurs when the request violates one or more domain invariants given the
    ///     current state of the domain aggregates.
    /// </summary>
    Extrinsic,

    /// <summary>
    ///     The "Intrinsic" domain error type; this occurs when the request in itself violates one or more domain invariants,
    ///     irrespective of the current state of the domain aggregates.
    /// </summary>
    Intrinsic,
}
