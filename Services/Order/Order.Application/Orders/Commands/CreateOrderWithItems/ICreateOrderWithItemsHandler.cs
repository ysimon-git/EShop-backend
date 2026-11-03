using Order.Application.Orders.Commands.CreateOrder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Application.Orders.Commands.CreateOrderWithItems
{
    public interface ICreateOrderWithItemsHandler
    {
        Task<CreateOrderWithItemsResult> HandleAsync(
            CreateOrderWithItemsCommand command,
            CancellationToken cancellationToken = default);
    }
}
