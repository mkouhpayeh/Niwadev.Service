using EnergyService.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace EnergyService.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ServicesController(AppDBContext context) => _context = context;


        // --------------
        // Customer
        // --------------

        /// <summary>
        /// Retrieve All Customers (Active and Inactive)
        /// </summary>
        /// <returns>
        /// List of Customers
        /// </returns>
        [HttpGet("ReadAllCustomers")]
        public async Task<IActionResult> ReadAllCustomers()
        {
            try
            {
                var customers = await _context.Customers
                    .IgnoreQueryFilters()
                    .ToListAsync();

                return Ok(new ResponseModel() { Message = "Customers found successfully!", Status = ResponseStatusEnum.Success, Data = customers });
            }
            catch (Exception ex)
            {
                // Log
                return StatusCode(500, new ResponseModel() { Message = "Get customers failed! ", Status = ResponseStatusEnum.Exception, Data = ex.Message });
            }
        }


        /// <summary>
        /// Retrieve All Customers (Active)
        /// </summary>
        /// <returns>
        /// List of Customers
        /// </returns>
        [HttpGet("ReadCustomers")]
        public async Task<IActionResult> ReadCustomers()
        {
            try
            {
                var customers = await _context.Customers.ToListAsync();

                return Ok(new ResponseModel() { Message = "Customers found successfully!", Status = ResponseStatusEnum.Success, Data = customers });
            }
            catch (Exception ex)
            {
                // Log
                return StatusCode(500, new ResponseModel() { Message = "Get customers failed! ", Status = ResponseStatusEnum.Exception, Data = ex.Message });
            }
        }

        /// <summary>
        /// Retrieve one Customer (Active)
        /// </summary>
        /// <returns>
        /// Customer
        /// </returns>
        [HttpGet("ReadCustomer")]
        public async Task<IActionResult> ReadCustomer(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    // Log
                    return BadRequest(new ResponseModel { Message = "Model state is not valid", Status = ResponseStatusEnum.InvalidModelState });
                }

                var customer = await _context.Customers
                    .Where(c => c.Name == name)
                    .FirstOrDefaultAsync();

                return Ok(new ResponseModel() { Message = "Customer found successfully!", Status = ResponseStatusEnum.Success, Data = customer });
            }
            catch (Exception ex)
            {
                // Log
                return StatusCode(500, new ResponseModel() { Message = "Get customer failed! ", Status = ResponseStatusEnum.Exception, Data = ex.Message });
            }
        }

        /// <summary>
        /// Create a new active customer
        /// </summary>
        /// <returns>
        /// Customer
        /// </returns>
        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Log
                    return BadRequest(new ResponseModel { Message = "Model state is not valid", Status = ResponseStatusEnum.InvalidModelState });
                }

                var customer = await _context.Customers
                    .IgnoreQueryFilters()
                    .Where(c => c.Name == model.Name).FirstOrDefaultAsync();

                if (customer != null)
                {
                    // with returned value you can find the existings customer is Active or Not
                    return StatusCode(409, new ResponseModel() { Message = "Customer already exists!", Status = ResponseStatusEnum.DuplicateValue, Data = customer });
                }

                await _context.Customers.AddAsync(model);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel() { Message = "Customer created successfully!", Status = ResponseStatusEnum.Success, Data = model });
            }

            catch (Exception ex)
            {
                // Log
                return StatusCode(500, new ResponseModel() { Message = "Create customer failed! ", Status = ResponseStatusEnum.Exception, Data = ex.Message });
            }
        }


        /// <summary>
        /// Update Customer
        /// </summary>
        /// <returns>
        /// Customer
        /// </returns>
        [HttpPost("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer([FromBody] Customer model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Log
                    return BadRequest(new ResponseModel { Message = "Model state is not valid", Status = ResponseStatusEnum.InvalidModelState });
                }

                var customer = await _context.Customers
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(c => c.Id == model.Id);

                if (customer == null)
                {
                    // with returned value you can find the existings customer is Active or Not
                    return StatusCode(404, new ResponseModel() { Message = "Customer not found!", Status = ResponseStatusEnum.NotFound, Data = customer });
                }

                _context.Entry(customer).CurrentValues.SetValues(model);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel() { Message = "Customer created successfully!", Status = ResponseStatusEnum.Success, Data = customer });
            }

            catch (Exception ex)
            {
                // Log
                return StatusCode(500, new ResponseModel() { Message = "Create customer failed! ", Status = ResponseStatusEnum.Exception, Data = ex.Message });
            }
        }

        /// <summary>
        /// Inactive Customer
        /// </summary>
        /// <returns>
        /// Customer
        /// </returns>
        [HttpPost("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomer([FromBody] Customer model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Log
                    return BadRequest(new ResponseModel { Message = "Model state is not valid", Status = ResponseStatusEnum.InvalidModelState });
                }

                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Id == model.Id);

                if (customer == null)
                {
                    // with returned value you can find the existings customer is Active or Not
                    return StatusCode(404, new ResponseModel() { Message = "Customer not found!", Status = ResponseStatusEnum.NotFound, Data = customer });
                }

                customer.IsActive = false;
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel() { Message = "Customer inactivated successfully!", Status = ResponseStatusEnum.Success, Data = customer });
            }

            catch (Exception ex)
            {
                // Log
                return StatusCode(500, new ResponseModel() { Message = "Inactivate customer failed! ", Status = ResponseStatusEnum.Exception, Data = ex.Message });
            }
        }


        // --------------
        // Product
        // --------------

        // --------------
        // Order
        // --------------

        /// <summary>
        /// Create an Order. 
        /// If one of the items in the model list got error => all process will terminate ????
        /// </summary>
        /// <returns>
        /// Order
        /// </returns>
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestModel model, CancellationToken ct)
        {
            if (model is null || model.Items is null || model.Items.Count == 0)
                return BadRequest(new ResponseModel { Message = "Model state is not valid!", Status = ResponseStatusEnum.InvalidModelState });

            try
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == model.CustomerId, ct);
                if (customer is null)
                    return NotFound(new ResponseModel { Message = "Customer not found!", Status = ResponseStatusEnum.NotFound });

                var lineWork = new List<(OrderItemRequest item, Product prod, Tariff tariff)>();

                foreach (var item in model.Items)
                {
                    if (item.EstimatedYearlyQuantity < 0)
                        return BadRequest(new ResponseModel { Message = "EstimatedYearlyQuantity must be >= 0.", Status = ResponseStatusEnum.InvalidModelState });

                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, ct);
                    if (product is null)
                        return NotFound(new ResponseModel { Message = $"Product not found/inactive.", Status = ResponseStatusEnum.NotFound, Data = item.ProductId });

                    if (model.ActiveDate == null)
                        model.ActiveDate = DateOnly.FromDateTime(DateTime.UtcNow);

                    var tariff = await _context.Tariffs
                         .Include(t => t.Unit)
                         .FirstOrDefaultAsync(t =>
                                 t.Id == item.TariffId &&
                                 t.ProductId == product.Id &&
                                 t.EffectiveFrom <= model.ActiveDate &&
                                 model.ActiveDate < t.EffectiveTo, ct);

                    if (tariff is null)
                        return Conflict(new ResponseModel
                        {
                            Message = $"No tariff version for (Id={item.TariffId}) covering {model.ActiveDate:yyyy-MM-dd} for Product {item.ProductId}.",
                            Status = ResponseStatusEnum.DuplicateValue
                        });

                    if (tariff.Unit is null)
                        return Conflict(new ResponseModel { Message = $"Tariff {tariff.Id} has no valid Unit.", Status = ResponseStatusEnum.DuplicateValue });


                    lineWork.Add((item, product, tariff));
                }

                using var tx = await _context.Database.BeginTransactionAsync(ct);
                try
                {
                    var order = new Order
                    {
                        CustomerId = model.CustomerId,
                        OrderDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmssfff}",
                        Description = $"Created on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}Z",
                        IsActive = true
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync(ct);

                    var items = new List<OrderItem>();
                    foreach (var (item, product,tarif) in lineWork)
                    {
                        var monthlyQty = item.EstimatedYearlyQuantity / 12m; // yearly → monthly (same unit)
                        items.Add(new OrderItem
                        {
                            OrderId = order.Id,
                            ProductId = product.Id,
                            TariffId = tarif.Id,
                            EstimatedMonthlyQuantity = Math.Round(monthlyQty, 3, MidpointRounding.AwayFromZero)
                        });
                    }

                    _context.OrderItems.AddRange(items);
                    await _context.SaveChangesAsync(ct);

                    await tx.CommitAsync(ct);

                    var resp = new InvoiceModel()
                    {
                        OrderId = order.Id,
                        OrderNumber = order.OrderNumber,
                        InvoiceDate = order.OrderDate,
                        ActiveDate = order.ActiveDate,
                        CustomerId = model.CustomerId,
                        CustomerName = customer.Name,
                        CustomerPhone = customer.Phone ?? "-",
                        CustomerAddress = customer.Address ?? "-",
                        InvoiceItems = items.Select((d, idx) =>
                        {
                            var (i,p,t) = lineWork[idx];
                            var monthlyQty = i.EstimatedYearlyQuantity / 12m;
                            var monthlyPrice = CalcMonthly(t.BaseMonthly, t.PricePerUnit, monthlyQty);
                            return new InvoiceItemModel
                            {
                                OrderItemId = d.Id,
                                ProductId = d.ProductId,
                                ProductName = d.Product.Name,
                                UnitName = t.Unit.Name,
                                TariffId = t.Id,
                                TariffName = t.Name,
                                TariffMonthlyPrice = monthlyPrice,
                                EstimatedMonthlyQuantity = monthlyQty
                            };
                        }).ToList()
                    };

                    resp.InvoiceTotalPrice = resp.InvoiceItems.Sum(i=>i.TariffMonthlyPrice);

                    return Ok(new ResponseModel
                    {
                        Message = "Order created successfully.",
                        Status = ResponseStatusEnum.Success,
                        Data = resp
                    });
                }
                catch (Exception internalEx)
                {
                    await tx.RollbackAsync(ct);
                    return StatusCode(500, new ResponseModel { Message = "Create order failed.", Status = ResponseStatusEnum.Exception, Data = internalEx.Message });
                }
            }
            catch (Exception ex)
            {
                // Log
                return StatusCode(500, new ResponseModel { Message = "Create order failed.", Status = ResponseStatusEnum.Exception, Data = ex.Message });
            }
        }


        // --------------
        // Tarif
        // --------------

        // --------------
        // Invoice
        // --------------


        #region Helper

        private static decimal CalcMonthly(decimal baseMonthly, decimal pricePerUnit, decimal qty)
        {
            var total = baseMonthly + (pricePerUnit * qty);
            return Math.Round(total, 2, MidpointRounding.AwayFromZero);
        }

        #endregion
    }
}
