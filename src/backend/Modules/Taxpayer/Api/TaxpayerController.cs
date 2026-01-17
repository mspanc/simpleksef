using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SimpleKsef.Infrastructure.Web;
using SimpleKsef.Schema.Types;

namespace SimpleKsef.Modules.Taxpayer.Api;

public enum TStatusInfoPodatnika
{
    /// <summary>
    /// Taxpayer in liquidation
    /// </summary>
    Liquidation,
    
    /// <summary>
    /// Taxpayer under restructuring proceedings.
    /// </summary>
    Restructuring,
    
    /// <summary>
    /// Bankruptcy proceedings against the taxpayer.
    /// </summary>
    Bankruptcy,

    /// <summary>
    /// Taxpayer in receivership.
    /// </summary>
    Inheritance
}

public sealed record TDaneKontaktoweDto
{
    /// <summary>
    /// Email address of the contact.
    /// </summary>
    /// <example>
    /// kontakt@przykladowy.pl
    /// </example>
    /// TODO etd:TAdresEmail
    public string? Email { get; init; }

    /// <summary>
    /// Phone number of the contact.
    /// </summary>
    /// <example>
    /// TODO
    /// </example>
    /// TODO etd:TNumerTelefonu
    public string? Phone { get; init; }
}


public sealed record TPodmiot1Dto
{
    /// <summary>
    /// NIP number of the taxpayer.
    /// TODO add validation, strip non-digit characters
    /// </summary>
    /// <example>123-456-32-18</example>
    public required string NipNumber { get; init; }

    /// <summary>
    /// Name of the taxpayer.
    /// </summary>
    /// <example>Firma XYZ Sp. z o.o.</example>
    [TZnakowy512]
    public required string Name { get; init; }
}


public sealed record TAdresDto
{
    /// TODO KodKraju  etd:TKodKraju ??
    
    /// <summary>
    /// Address line 1 of the taxpayer.
    /// </summary>
    /// <example>ul. Przykładowa 10</example>
    [TZnakowy512]
    public required string AddressLine1 { get; init; }

    /// <summary>
    /// Address line 2 of the taxpayer.
    /// </summary>
    /// <example>05-500 Warszawa</example>
    [TZnakowy512]
    public string? AddressLine2 { get; init; }

    /// <summary>
    /// Global Location Number (GLN) (if applicable).
    /// </summary>
    [TZnakowy512]
    public string? GlobalLocationNumber { get; init; }

    [MinLength(0)]
    [MaxLength(3)]
    public List<TDaneKontaktoweDto>? ContactInfos { get; init; }    
}

public sealed record CreateTaxpayerRequest
{
    /// <summary>
    /// EORI number of the taxpayer (if applicable).
    /// </summary>
    /// <remarks>
    /// EORI (Economic Operators Registration and Identification) number is used for customs
    /// identification in the European Union. It is assigned to businesses and individuals
    /// engaging in customs activities.
    /// </remarks>
    [TZnakowy]    
    public string? EoriNumber { get; init; }

    public required TPodmiot1Dto IdentificationData { get; init; }
    
    public required TAdresDto Address { get; init; }

    public TAdresDto? CorrespondenceAddress { get; init; }

    public TStatusInfoPodatnika? Status { get; init; }
}

public sealed record CreateTaxpayerResponse
{
    /// <summary>
    /// SimpleKsef internal identifier of the taxpayer.
    /// </summary>
    /// <remarks>
    /// UUID version 7 (time-ordered).
    /// </remarks>
    /// <example>019bcd7d-c7e3-730c-b4f4-3ebbc2161a12</example>
    public required Guid Id { get; init; }
}

public sealed record GetTaxpayerResponse
{
    /// <summary>
    /// SimpleKsef internal identifier of the taxpayer.
    /// </summary>
    /// <remarks>
    /// UUID version 7 (time-ordered).
    /// </remarks>
    /// <example>019bcd7d-c7e3-730c-b4f4-3ebbc2161a12</example>
    public required Guid Id { get; init; }
}


/// <summary>
/// Taxpayer management endpoints
/// </summary>
public sealed class TaxpayerController : BaseApiController
{
    /// <summary>
    /// Create a new taxpayer
    /// </summary>
    /// <remarks>
    /// Creates a taxpayer
    /// </remarks>
    /// <response code="201">Taxpayer was created</response>
    /// <response code="400">Invalid taxpayer data</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateTaxpayerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult CreateTaxpayer([FromBody] CreateTaxpayerRequest request)
    {
        var id = Guid.NewGuid();
        return CreatedAtAction(
            nameof(GetTaxpayer),
            new { id }, 
            new CreateTaxpayerResponse
            {
                Id = id
            });
    }

    /// <summary>
    /// Read a single taxpayer
    /// </summary>
    /// <remarks>
    /// Retrieves the taxpayer by its identifier.
    /// </remarks>
    /// <response code="200">Taxpayer was successfully retrieved</response>
    /// <response code="404">Taxpayer with given identifier was not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetTaxpayerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetTaxpayer(Guid id)
    {
        throw new NotImplementedException();
    }
}