using ApbdCw7.Api.Dtos.Requests;
using ApbdCw7.Api.Dtos.Responses;
using ApbdCw7.Api.Mapping;
using ApbdCw7.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApbdCw7.Api.Controllers;

[ApiController]
[Route("api/pcs")]
public class PcsController : ControllerBase
{
    private readonly IPcService _pcService;
    private readonly ILogger<PcsController> _logger;

    public PcsController(IPcService pcService, ILogger<PcsController> logger)
    {
        _pcService = pcService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PcListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<PcListItemResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var pcs = await _pcService.GetAllAsync(cancellationToken);
            return Ok(pcs.Select(p => p.ToResponse()).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve PCs");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("{id:int}/components")]
    [ProducesResponseType(typeof(PcWithComponentsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PcWithComponentsResponse>> GetWithComponents(int id, CancellationToken cancellationToken)
    {
        try
        {
            var pc = await _pcService.GetWithComponentsAsync(id, cancellationToken);
            if (pc is null)
            {
                return NotFound();
            }

            return Ok(pc.ToResponse());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve PC components for {PcId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(PcListItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PcListItemResponse>> Create([FromBody] CreatePcRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var created = await _pcService.CreateAsync(request.ToCreateDto(), cancellationToken);
            var response = created!.ToResponse();
            return CreatedAtAction(nameof(GetWithComponents), new { id = response.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create PC");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PcListItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PcListItemResponse>> Update(int id, [FromBody] UpdatePcRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updated = await _pcService.UpdateAsync(id, request.ToUpdateDto(), cancellationToken);
            if (updated is null)
            {
                return NotFound();
            }

            return Ok(updated.ToResponse());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update PC {PcId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _pcService.DeleteAsync(id, cancellationToken);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete PC {PcId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
