using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Dtos;
using Order.Api.DTOs;
using Order.Application.Catalog.Interfaces;
using Order.Application.Events;
using Order.Application.Interfaces;
using Order.Application.Orders.Commands.AddOrderItem;
using Order.Application.Orders.Commands.CreateOrder;
using Order.Application.Orders.Commands.CreateOrderWithItems;
using Order.Application.Orders.Queries.GetOrderByCustomerId;
using Order.Application.Orders.Queries.GetOrderById;
using System.Security.Claims;


namespace Order.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly ICreateOrderCommandHandler _createOrderHandler;
    private readonly IAddOrderItemCommandHandler _addOrderItemHandler;
    private readonly ICreateOrderWithItemsHandler _createOrderWithItemsHandler;
    private readonly IGetOrderByIdQueryHandler _getOrderByIdQueryHandler;
    private readonly IGetOrderByCustomerIdQueryHandler _getOrderByCustomerIdQueryHandler;
    private readonly IOrderRepository _orderRepository;
    private readonly ICatalogServiceClient _catalogServiceClient;
  


    public OrdersController(ICreateOrderCommandHandler createOrderHandler,
                        IAddOrderItemCommandHandler addOrderItemHandler,
                        ICreateOrderWithItemsHandler createOrderWithItemsHandler,
                        IGetOrderByIdQueryHandler getOrderByIdQueryHandler,
                        IGetOrderByCustomerIdQueryHandler getOrderByCustomerIdQueryHandler,
                        IOrderRepository orderRepository,
                        ICatalogServiceClient catalogServiceClient)
    {                    
        _createOrderHandler = createOrderHandler;
        _addOrderItemHandler = addOrderItemHandler;
        _createOrderWithItemsHandler= createOrderWithItemsHandler;
        _getOrderByIdQueryHandler = getOrderByIdQueryHandler;
        _getOrderByCustomerIdQueryHandler = getOrderByCustomerIdQueryHandler;

        _orderRepository = orderRepository;
        _catalogServiceClient = catalogServiceClient;


    }




    [Authorize(Roles = "User,Admin")]
    [HttpPost("createorder")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
     CreateOrderDto dto,
     CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userId, out var customerId))
            return Unauthorized();

        var command = new CreateOrderCommand(
            customerId,
            dto.Note);

        var result = await _createOrderHandler.HandleAsync(
            command,
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            new
            {
                Id = result.Id,
                OrderNumber = result.OrderNumber
            });
    }



    [Authorize(Roles = "User,Admin")]
    [HttpPost("createorderwithitems")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateOrderWithItems(
        CreateOrderWithItemsDto dto,
        CancellationToken cancellationToken)
    {
        // 1. Get authenticated customer from JWT
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var customerId))
            return Unauthorized();

        // 2. Validate items
        if (dto.items is null || dto.items.Length == 0)
            return BadRequest("The order must contain at least one item.");

        // 3. Map DTO to application command
        var command = new CreateOrderWithItemsCommand(
            customerId,
            dto.note,
            dto.items.Select(item =>
                new CreateOrderWithItemsItem(
                    item.ProductId,
                    item.Quantity
                )).ToList()
        );

        // 4. Execute the handler
        var result = await _createOrderWithItemsHandler.HandleAsync(
            command,
            cancellationToken
        );

        // 5. Return the created order
        return StatusCode(
            StatusCodes.Status201Created,
            new
            {
                Id = result.Id,
                OrderNumber = result.OrderNumber
            }
        );
    }


    [Authorize(Roles = "User,Admin")]
    [HttpPost("{orderId:guid}/items")]
    public async Task<IActionResult> AddItem(
    Guid orderId,
    [FromBody] AddOrderItemDto dto,
    CancellationToken cancellationToken)
    {
        var command = new AddOrderItemCommand(
            orderId,
            dto.ProductId,
            dto.Quantity);

        await _addOrderItemHandler.HandleAsync(command, cancellationToken);

        return NoContent();
    }




    [Authorize(Roles = "User,Admin")]
    [HttpGet("GetById/{id:guid}")]
    public async Task<IActionResult> GetById(
    Guid id,
    CancellationToken cancellationToken)
    {
        var query=new GetOrderByIdQuery(id);
        var order=await _getOrderByIdQueryHandler.HandleAsync(query, cancellationToken);
              

        if (order is null)
            return NotFound();

        // Admin can access any order
        if (User.IsInRole("Admin"))
            return Ok(order);

        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // User can access only his own orders
        if (order.CustomerId != userId)
            return Forbid();

        return Ok(order);
    }

    

    [Authorize(Roles = "User,Admin")]
    [HttpGet("GetByIdCustomer/{customerId:guid}")]
    public async Task<IActionResult> GetByIdCustomer(
    Guid customerId,
    CancellationToken cancellationToken)
    {
        // Admin can access any customer's orders
        if (!User.IsInRole("Admin"))
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            // User can access only his own orders
            if (customerId != userId)
                return Forbid();
        }


        var query = new GetOrderByCustomerIdQuery(customerId);
        var orderList = await _getOrderByCustomerIdQueryHandler.HandleAsync(query, cancellationToken);

        return Ok(orderList);
    }
    


    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
      Guid id,
      int targetStatusId,
      CancellationToken cancellationToken)
    {
        var success = await _orderRepository.ChangeStatusAsync(
            id,
            targetStatusId,
            cancellationToken);

        if (!success)
            return BadRequest();

        return NoContent();
    }





   
}