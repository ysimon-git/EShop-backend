using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Application.Catalog.Models
{
    public sealed record CatalogProductStockChange(
    Guid Id,
    int StockQuantity);
}
