using System.ComponentModel.DataAnnotations;

namespace SimpleKsef.Schema.Types;

/// <summary>
/// Validation attribute that matches TZnakowy from the KSeF Schema.
/// </summary>
/// <remarks>
/// Corresponds to the following XML Schema definition:
/// 
/// <![CDATA[
///     <xsd:simpleType name="TZnakowy">
///       <xsd:annotation>
///         <xsd:documentation>
///           Typ znakowy ograniczony do 256 znaków
///         </xsd:documentation>
///       </xsd:annotation>
///       <xsd:restriction base="xsd:token">
///         <xsd:minLength value="1"/>
///         <xsd:maxLength value="256"/>
///       </xsd:restriction>
///     </xsd:simpleType>
/// ]]>
/// 
/// and similar ones.
/// 
/// Please note that xsd:token is not the same as xsd:string.
/// 
/// It applies whitespace normalization:
/// 
/// <list type="bullet">
///   <item>Leading whitespace is removed.</item>
///   <item>Trailing whitespace is removed.</item>
///   <item>Internal sequences of whitespace are collapsed to a single space.</item>
/// </list>
/// </remarks>
public abstract class TZnakowyBaseAttribute(int minLength, int maxLength) : ValidationAttribute
{
    public int MinLength { get; } = minLength;
    public int MaxLength { get; } = maxLength;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            if (MinLength == 0)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"{validationContext.DisplayName} is required.");
        }

        if (value is not string s)
        {
            return new ValidationResult($"{validationContext.DisplayName} must be a string.");
        }

        var normalized = NormalizeToken(s);

        if (normalized.Length < MinLength)
        {
            return new ValidationResult(
                $"{validationContext.DisplayName} must be at least {MinLength} characters.");
        }

        if (normalized.Length > MaxLength)
        {
            return new ValidationResult(
                $"{validationContext.DisplayName} must be at most {MaxLength} characters.");
        }

        return ValidationResult.Success;
    }

    public static string NormalizeToken(string value)
    {
        // xsd:token semantics: trim + collapse whitespace
        return string.Join(
            ' ',
            value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }
}

public sealed class TZnakowyAttribute()
    : TZnakowyBaseAttribute(minLength: 1, maxLength: 256)
{
}

public sealed class TZnakowy2Attribute()
    : TZnakowyBaseAttribute(minLength: 0, maxLength: 256)
{
}

public sealed class TZnakowy20Attribute()
    : TZnakowyBaseAttribute(minLength: 1, maxLength: 20)
{
}

public sealed class TZnakowy50Attribute()
    : TZnakowyBaseAttribute(minLength: 1, maxLength: 50)
{
}

public sealed class TZnakowy512Attribute()
    : TZnakowyBaseAttribute(minLength: 1, maxLength: 512)
{
}