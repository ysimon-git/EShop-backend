using Order.Application.Orders.Queries.GetOrderByCustomerId;
using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Application.Orders.Queries.GetOrderById
{
    public interface IGetOrderByIdQueryHandler
    {

        Task<OrderDetailsResult> HandleAsync(GetOrderByIdQuery query, CancellationToken cancellationToken = default);


    }
}
