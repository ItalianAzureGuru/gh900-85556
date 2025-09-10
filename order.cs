using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Models
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled,
        Returned
    }

    public enum PaymentStatus
    {
        NotPaid,
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public class OrderItem
    {
        [Required]
        public string SKU { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        public decimal LineTotal => Quantity * UnitPrice;
    }

    public class Order
    {
        public Guid OrderId { get; set; } = Guid.NewGuid();

        public string CustomerId { get; set; }

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        public decimal Subtotal => Items.Sum(i => i.LineTotal);

        public decimal ShippingAmount { get; set; } = 0m;

        public decimal TaxAmount { get; set; } = 0m;

        public decimal DiscountAmount { get; set; } = 0m;

        public decimal Total => Math.Max(0, Subtotal + ShippingAmount + TaxAmount - DiscountAmount);

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.NotPaid;

        public string ShippingAddress { get; set; }

        public string BillingAddress { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public void AddItem(OrderItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.Quantity <= 0) throw new ArgumentException("Quantity must be greater than zero", nameof(item));
            var existing = Items.FirstOrDefault(i => i.SKU == item.SKU);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
                existing.UnitPrice = item.UnitPrice; // choose strategy: overwrite price
            }
            else
            {
                Items.Add(item);
            }
            UpdatedAt = DateTime.UtcNow;
        }

        public bool RemoveItem(string sku, int quantity = int.MaxValue)
        {
            var existing = Items.FirstOrDefault(i => i.SKU == sku);
            if (existing == null) return false;
            if (quantity >= existing.Quantity)
            {
                Items.Remove(existing);
            }
            else
            {
                existing.Quantity -= quantity;
            }
            UpdatedAt = DateTime.UtcNow;
            return true;
        }

        public void ApplyDiscount(decimal amount)
        {
            if (amount < 0) throw new ArgumentException("Discount cannot be negative", nameof(amount));
            DiscountAmount = amount;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool Validate(out IEnumerable<ValidationResult> errors)
        {
            var ctx = new ValidationContext(this);
            var results = new List<ValidationResult>();
            // Validate order-level attributes
            Validator.TryValidateObject(this, ctx, results, validateAllProperties: true);
            // Validate items
            foreach (var item in Items)
            {
                var itemCtx = new ValidationContext(item);
                Validator.TryValidateObject(item, itemCtx, results, validateAllProperties: true);
            }
            errors = results;
            return !results.Any();
        }

        public override string ToString()
        {
            return $"Order {OrderId} - Customer: {CustomerId ?? "N/A"} - Items: {Items.Count} - Total: {Total:C}";
        }
    }
}
```// filepath: /Models/Order.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Models
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled,
        Returned
    }

    public enum PaymentStatus
    {
        NotPaid,
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public class OrderItem
    {
        [Required]
        public string SKU { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        public decimal LineTotal => Quantity * UnitPrice;
    }

    public class Order
    {
        public Guid OrderId { get; set; } = Guid.NewGuid();

        public string CustomerId { get; set; }

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        public decimal Subtotal => Items.Sum(i => i.LineTotal);

        public decimal ShippingAmount { get; set; } = 0m;

        public decimal TaxAmount { get; set; } = 0m;

        public decimal DiscountAmount { get; set; } = 0m;

        public decimal Total => Math.Max(0, Subtotal + ShippingAmount + TaxAmount - DiscountAmount);

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.NotPaid;

        public string ShippingAddress { get; set; }

        public string BillingAddress { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public void AddItem(OrderItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.Quantity <= 0) throw new ArgumentException("Quantity must be greater than zero", nameof(item));
            var existing = Items.FirstOrDefault(i => i.SKU == item.SKU);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
                existing.UnitPrice = item.UnitPrice; // choose strategy: overwrite price
            }
            else
            {
                Items.Add(item);
            }
            UpdatedAt = DateTime.UtcNow;
        }

        public bool RemoveItem(string sku, int quantity = int.MaxValue)
        {
            var existing = Items.FirstOrDefault(i => i.SKU == sku);
            if (existing == null) return false;
            if (quantity >= existing.Quantity)
            {
                Items.Remove(existing);
            }
            else
            {
                existing.Quantity -= quantity;
            }
            UpdatedAt = DateTime.UtcNow;
            return true;
        }

        public void ApplyDiscount(decimal amount)
        {
            if (amount < 0) throw new ArgumentException("Discount cannot be negative", nameof(amount));
            DiscountAmount = amount;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool Validate(out IEnumerable<ValidationResult> errors)
        {
            var ctx = new ValidationContext(this);
            var results = new List<ValidationResult>();
            // Validate order-level attributes
            Validator.TryValidateObject(this, ctx, results, validateAllProperties: true);
            // Validate items
            foreach (var item in Items)
            {
                var itemCtx = new ValidationContext(item);
                Validator.TryValidateObject(item, itemCtx, results, validateAllProperties: true);
            }
            errors = results;
            return !results.Any();
        }

        public override string ToString()
        {
            return $"Order {OrderId} - Customer: {CustomerId ?? "N/A"} - Items: {Items.Count} - Total: {Total:C}";
        }
    }