using System.ComponentModel.DataAnnotations;

public class UpdateOrderRequestDTO
{
    [Required(ErrorMessage = "Cần phải nhập Order Id ")]

    public Guid OrderId { get; set; }
    public Guid? CustomerId { get; set; }
    public List<OrderServiceUpdateItemDTO>? Services { get; set; } = new();
}

public class OrderServiceUpdateItemDTO
{
    public Guid ServiceId { get; set; }
    public DateTime ScheduledTime { get; set; }
}
