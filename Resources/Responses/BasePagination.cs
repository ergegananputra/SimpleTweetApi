using System.Collections.Generic;

namespace SimpleTweetApi.Resources.Responses;

public record class BasePagination<ItemDataType>
(
    int Page,
    int Limit,
    IEnumerable<ItemDataType> Items
);

