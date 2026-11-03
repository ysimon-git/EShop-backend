using Order.Application.Orders.Commands.CreateOrder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Application.Orders.Queries.GetOrderByCustomerId
{
    public interface IGetOrderByCustomerIdQueryHandler
    {
        Task<List<OrderSummaryDto>> HandleAsync(GetOrderByCustomerIdQuery query,CancellationToken cancellationToken = default);
    }
}
