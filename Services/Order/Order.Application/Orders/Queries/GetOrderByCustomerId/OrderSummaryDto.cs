using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Application.Orders.Queries.GetOrderByCustomerId
{
    public sealed record OrderSummaryDto(
         Guid Id,
         string OrderNumber,
         Guid CustomerId,
         DateTime CreatedAtUtc,
         string Status,
         decimal TotalAmount
    );
}
