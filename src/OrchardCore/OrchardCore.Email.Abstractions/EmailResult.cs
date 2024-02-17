using System.Collections.Generic;
using Microsoft.Extensions.Localization;

namespace OrchardCore.Email;

/// <summary>
/// Represents the result of sending an email.
/// </summary>
public class EmailResult
{
    /// <summary>
    /// Returns an <see cref="EmailResult"/> indicating a successful Smtp operation.
    /// </summary>
    public static readonly EmailResult SuccessResult = new() { Succeeded = true };

    /// <summary>
    /// An <see cref="IEnumerable{LocalizedString}"/> containing errors that may occurred during the email sending operation.
    /// </summary>
    public IEnumerable<LocalizedString> Errors { get; protected set; }

    /// <summary>
    /// Get or sets the response text from the email sending service.
    /// </summary>
    public string Response { get; set; }

    /// <summary>
    /// Whether the operation succeeded or not.
    /// </summary>
    public bool Succeeded { get; protected set; }

    /// <summary>
    /// Creates an <see cref="EmailResult"/> indicating a failed email sending operation, with a list of errors if applicable.
    /// </summary>
    /// <param name="errors">An optional array of <see cref="LocalizedString"/> which caused the operation to fail.</param>
    public static EmailResult FailedResult(params LocalizedString[] errors)
        => new()
        {
            Succeeded = false,
            Errors = errors
        };

    public static EmailResult GetSuccessResult(string response)
        => new()
        {
            Succeeded = true,
            Response = response,
        };
}
