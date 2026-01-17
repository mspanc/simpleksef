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
    /// SimpleKsef internal identifier of the invoice accepted for submission to KSeF.
    /// </summary>
    /// <remarks>
    /// UUID version 7 (time-ordered).
    /// </remarks>
    /// <example>019bcd7d-c7e3-730c-b4f4-3ebbc2161a12</example>
    public required Guid Id { get; init; }
}

public sealed record GetInvoiceResponse
{
    /// <summary>
    /// SimpleKsef internal identifier of the invoice accepted for submission to KSeF.
    /// </summary>
    /// <remarks>
    /// UUID version 7 (time-ordered).
    /// </remarks>
    /// <example>019bcd7d-c7e3-730c-b4f4-3ebbc2161a12</example>
    public required Guid Id { get; init; }
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
    /// Create a new invoice
    /// </summary>
    /// <remarks>
    /// Accepts an invoice and queues its submission to KSeF.
    /// </remarks>
    /// <response code="202">Invoice was accepted and queued for submission</response>
    /// <response code="400">Invalid invoice data</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateInvoiceResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult CreateInvoice([FromBody] CreateInvoiceRequest request)
    {
        var id = Guid.NewGuid();
        return AcceptedAtAction(
            nameof(GetInvoice),
            new { id }, 
            new CreateInvoiceResponse
            {
                Id = id
            });
    }

    /// <summary>
    /// Read a single invoice
    /// </summary>
    /// <remarks>
    /// Retrieves the invoice by its identifier.
    /// </remarks>
    /// <response code="200">Invoice was successfully retrieved</response>
    /// <response code="404">Invoice with given identifier was not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetInvoiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetInvoice(Guid id)
    {
        throw new NotImplementedException();
    }
}