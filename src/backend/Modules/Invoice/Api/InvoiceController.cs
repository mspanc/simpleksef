using Microsoft.AspNetCore.Mvc;
using SimpleKsef.Infrastructure.Web;

namespace SimpleKsef.Modules.Invoice.Api;

/// <summary>
/// Invoice creation request.
/// </summary>
public sealed record CreateInvoiceRequest
{
    /// <summary>
    /// Invoice number assigned by the issuer.
    /// </summary>
    /// <example>FV/01/2026</example>
    public required string Number { get; init; }
}

public sealed record CreateInvoiceResponse
{
    /// <summary>
    /// SimpleKsef identifier of the created invoice
    /// </summary>
    public required string Id { get; init; }
}


/// <summary>
/// Invoice management endpoints
/// </summary>
/// <remarks>
/// Handles creation and submission of invoices to KSeF.
/// </remarks>
public sealed class InvoiceController : BaseApiController
{
    /// <summary>
    /// Creates a new invoice
    /// </summary>
    /// <remarks>
    /// Generates an invoice and sends it to KSeF.
    /// </remarks>
    /// <response code="200">Invoice was successfully created</response>
    /// <response code="400">Invalid invoice data</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateInvoiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult CreateInvoice([FromBody] CreateInvoiceRequest request)
    {
        return Ok(new CreateInvoiceResponse
        {
            Id = Guid.NewGuid().ToString()
        });
    }
}