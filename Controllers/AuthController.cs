using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SimpleTweetApi.Models.Auth;
using SimpleTweetApi.Resources.Responses;
using SimpleTweetApi.Services;
using SimpleTweetApi.Resources.Requests;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SimpleTweetApi.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{

    private readonly UserManager<User> _userManager;
    private readonly UserService _userService;

    public AuthController(UserManager<User> userManager, UserService userService)
    {
        _userManager = userManager;
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return BadRequest(new BaseResponse<object>(
                Status: 400,
                Message: "User not found",
                Data: null
                ));
        }

        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!result)
        {
            return BadRequest(new BaseResponse<object>(
                Status: 400,
                Message: "Invalid password",
                Data: null
                ));
        }

        var token = _userService.IssueToken(user);

        return Ok(new BaseResponse<object>(
            Status: 200,
            Message: "User authenticated",
            Data: new AuthResponse(token)
            ));

    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterUserForm request)
    {
        var user = new User
        {
            UserName = request.Username,
            Email = request.Email,
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            // handle the error:
            var errors = result.Errors.Select(e => e.Description).ToList();


            return BadRequest(new BaseResponse<object>(
                Status: 400,
                Message: "Registration failed",
                Data: errors
                ));
        }

        return Ok(new BaseResponse<object>(
            Status: 200,
            Message: "Register success",
            Data: user
            ));
    }

    [HttpGet("confirm-email")]
    public async Task<ActionResult> ConfirmEmail(string token, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return BadRequest(new BaseResponse<object>(
                Status: 400,
                Message: "Invalid email.",
                Data: null
            ));
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            return BadRequest(new BaseResponse<object>(
                Status: 400,
                Message: "Email confirmation failed.",
                Data: result.Errors.Select(e => e.Description).ToList()
            ));
        }

        return Ok(new BaseResponse<object>(
            Status: 200,
            Message: "Email confirmed successfully.",
            Data: null
        ));
    }


}
