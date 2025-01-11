using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleTweetApi.Enum;
using SimpleTweetApi.Models.App;
using SimpleTweetApi.Resources.Requests;
using SimpleTweetApi.Resources.Responses;
using SimpleTweetApi.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SimpleTweetApi.Controllers;

[Route("api/master/flag")]
[ApiController]
public class MasterFlagController : ControllerBase
{

    private readonly FlagService _flagService;

    public MasterFlagController(FlagService flagService)
    {
        _flagService = flagService;
    }

    
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string keyword = null, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        IEnumerable<Flag> Flags = await _flagService.Flags(keyword, page, limit);
        BaseResponse<BasePagination<Flag>> response = new BaseResponse<BasePagination<Flag>>(
            Status: 200,
            Message: "Flags fetched successfully",
            Data: new BasePagination<Flag>(page, limit, Flags)
            );

        return Ok(response);
    }

    
    [HttpGet("{code}")]
    public async Task<IActionResult> Get(string code)
    {
        Flag? flag = await _flagService.Flag(code);

        if (flag == null)
        {
            return NotFound(new BaseResponse<Flag>(
                Status: 404,
                Message: "Flag not found",
                Data: null
            ));
        }

        return Ok(new BaseResponse<Flag>(
            Status: 200,
            Message: "Flag fetched successfully",
            Data: flag
        ));
    }

    
    [HttpPost]
    public async Task<IActionResult> Post([FromForm] string type, [FromForm] string name, [FromForm] string description, [FromForm] string? icon)
    {
        FlagType flagType;

        if (type == FlagType.REPORT.ToString())
        {
            flagType = FlagType.REPORT;
        }
        else if (type == FlagType.PENDING_REPORT.ToString())
        {
            flagType = FlagType.PENDING_REPORT;
        }
        else if (type == FlagType.RESTRICT_DOM.ToString())
        {
            flagType = FlagType.RESTRICT_DOM;
        } 
        else if (type == FlagType.PROMOTE.ToString())
        {
            flagType = FlagType.PROMOTE;
        }
        else
        {
            return BadRequest(new BaseResponse<Flag>(
                Status: 400,
                Message: "Invalid flag type",
                Data: null
            ));
        }

        Flag flag = await _flagService.Create(flagType, name, description, icon);

        return CreatedAtAction(nameof(Get), new { code = flag.Code }, new BaseResponse<Flag>(
            Status: 201,
            Message: "Flag created successfully",
            Data: flag
        ));
    }

    [HttpPut("{code}")]
    public async Task<IActionResult> Put(string code, FlagPostForm requestForm)
    {
        Flag? flag = await _flagService.Flag(code);
        if (flag == null)
        {
            return NotFound(new BaseResponse<Flag>(
                Status: 404,
                Message: "Flag not found",
                Data: null
            ));
        }

        Flag tempFlag = new Flag
        {
            Code = code,
            Name = requestForm.Name ?? flag.Name,
            Description = requestForm.Description ?? flag.Description,
            Icon = requestForm.Icon ?? flag.Icon
        };

        Flag? updatedFlag = await _flagService.Update(code, tempFlag);
        return Ok(new BaseResponse<Flag?>(
            Status: 200,
            Message: "Flag updated successfully",
            Data: updatedFlag
        ));
    }

    
    [HttpDelete("{code}")]
    public async Task<IActionResult> Delete(string code)
    {
        bool result = await _flagService.Delete(code);

        if (result)
        {
            return Ok(new BaseResponse<Boolean>(
                Status: 200,
                Message: "Flag deleted successfully",
                Data: result
            ));
        }
        else
        {
            return NotFound(new BaseResponse<Boolean>(
                Status: 404,
                Message: "Flag not found",
                Data: result
            ));
        }
    }

    [HttpPost("generate-basic-flag")]
    public async Task<IActionResult> GenerateBasicFlag()
    {
        var result = await _flagService.GenerateBasicFlagCore();

        if (result)
        {
            return Ok(new BaseResponse<Boolean>(
                Status: 200,
                Message: "Flag generated successfully",
                Data: result
            ));
        } else
        {
            return BadRequest(new BaseResponse<Boolean>(
                Status: 400,
                Message: "Flag generation failed",
                Data: result
            ));
        }
        
    }
}
