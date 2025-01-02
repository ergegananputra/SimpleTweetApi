namespace SimpleTweetApi.Resources.Responses;

public record class BaseResponse<DataType>
(
    int? Status,
    string? Message,
    DataType? Data
);
