using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleTweetApi.Models.App;
using SimpleTweetApi.Models.Auth;
using SimpleTweetApi.Resources.Requests;
using SimpleTweetApi.Resources.Responses;
using SimpleTweetApi.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SimpleTweetApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TweetController : ControllerBase
{
    private readonly TweetCoreService _tweetCoreService;
    private readonly UserManager<User> _userManager;

    public TweetController(TweetCoreService tweetCoreService, UserManager<User> userManager)
    {
        _tweetCoreService = tweetCoreService;
        _userManager = userManager;
    }

    // GET: api/<ValuesController>
    [HttpGet]
    public async Task<IActionResult>
        Get([FromQuery] string keyword = null, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new BaseResponse<Tweet>(
                Status: 401,
                Message: "User not authenticated",
                Data: null
                ));
        }

        var tweets = await _tweetCoreService.Tweets(keyword, page, limit);

        var response = new BaseResponse<BasePagination<Tweet>>(
            Status: 200,
            Message: "Tweets fetched successfully",
            Data: new BasePagination<Tweet>(Page: page, Limit: limit, Items: tweets)
            );

        return Ok(response);

    }

    // GET api/<ValuesController>/5
    [HttpGet("{uuid}")]
    public async Task<IActionResult> Get(Guid uuid)
    {
        var tweet = await _tweetCoreService.Tweet(uuid);

        if (tweet == null)
        {
            return NotFound(new BaseResponse<Tweet>(
                Status: 404,
                Message: "Tweet not found",
                Data: null
                ));
        }

        return Ok(new BaseResponse<Tweet>(
            Status: 200,
            Message: "Tweet fetched successfully",
            Data: tweet
            ));
    }

    // POST api/<ValuesController>
    [HttpPost]
    public async Task<IActionResult> Post(TweetPostForm requestForm)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new BaseResponse<Tweet>(
                Status: 401, 
                Message: "User not authenticated", 
                Data: null
                ));
        }

        var tweet = new Tweet
        {
            Content = requestForm.Content,
            UserId = user.Id
        };
        var createdTweet = await _tweetCoreService.Create(tweet);
        
        var response = new BaseResponse<Tweet>(
            Status: 201,
            Message: "Tweet created successfully",
            Data: createdTweet
            );

        return CreatedAtAction(nameof(Get), new { uuid = createdTweet.Uuid }, response);
    }

    // PUT api/<ValuesController>/5
    [HttpPut("{uuid}")]
    public async Task<IActionResult> Put(Guid uuid, TweetPostForm requestForm)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new BaseResponse<Tweet>(
                Status: 401,
                Message: "User not authenticated",
                Data: null
                ));
        }

        var originalTweet = await _tweetCoreService.Tweet(uuid);

        if (originalTweet == null)
        {
            return NotFound(new BaseResponse<Tweet>(
                Status: 404,
                Message: "Tweet not found",
                Data: null
                ));
        }

        if (originalTweet.UserId != user.Id)
        {
            return Unauthorized(new BaseResponse<Tweet>(
                Status: 401,
                Message: "User not authorized to update this tweet",
                Data: null
                ));
        }

        originalTweet.Content = requestForm.Content;
        var updatedTweet = await _tweetCoreService.Update(uuid, originalTweet);

        var response = new BaseResponse<Tweet>(
            Status: 200,
            Message: "Tweet updated successfully",
            Data: updatedTweet
            );

        return Ok(response);
    }

    // DELETE api/<ValuesController>/5
    [HttpDelete("{uuid}")]
    public async Task<IActionResult> Delete(Guid uuid)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new BaseResponse<Tweet>(
                Status: 401,
                Message: "User not authenticated",
                Data: null
                ));
        }

        var originalTweet = await _tweetCoreService.Tweet(uuid);

        if (originalTweet == null)
        {
            return NotFound(new BaseResponse<Tweet>(
                Status: 404,
                Message: "Tweet not found",
                Data: null
                ));
        }

        if (originalTweet.UserId != user.Id)
        {
            return Unauthorized(new BaseResponse<Tweet>(
                Status: 401,
                Message: "User not authorized to delete this tweet",
                Data: null
                ));
        }

        var deleted = await _tweetCoreService.Delete(uuid);

        if (!deleted)
        {
            return BadRequest(new BaseResponse<Tweet>(
                Status: 400,
                Message: "Tweet could not be deleted",
                Data: null
                ));
        }

        return Ok(new BaseResponse<Tweet>(
            Status: 200,
            Message: "Tweet deleted successfully",
            Data: null
            ));
    }

}
