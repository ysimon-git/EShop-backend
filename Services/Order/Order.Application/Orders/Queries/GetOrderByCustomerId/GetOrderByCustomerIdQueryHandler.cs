using Order.Application.Interfaces;
using Order.Application.Orders.Queries.GetOrderById;
using Order.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Application.Orders.Queries.GetOrderByCustomerId
{
    //Get all orders summary for one customer handler
    public sealed class GetOrderByCustomerIdQueryHandler:IGetOrderByCustomerIdQueryHandler
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByCustomerIdQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }


        public async Task<List<OrderSummaryDto?>> HandleAsync(
            GetOrderByCustomerIdQuery query,
            CancellationToken cancellationToken = default)
        {

            var orderList = await _orderRepository.GetByCustomerIdAsync(query.customerId,cancellationToken);

            if (orderList is null)
                return null;

            return orderList;

        }
    }
}
