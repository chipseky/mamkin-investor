namespace Mamkin.In.Infrastructure;

public static class BybitOrderStatusMapper
{
    public static Domain.OrderStatus MapOrderStatus(Bybit.Net.Enums.V5.OrderStatus bybitOrderStatus)
    {
        return bybitOrderStatus switch
        {
            Bybit.Net.Enums.V5.OrderStatus.Cancelled => Domain.OrderStatus.Cancelled,
            Bybit.Net.Enums.V5.OrderStatus.Created => Domain.OrderStatus.Cancelled,
            Bybit.Net.Enums.V5.OrderStatus.Rejected => Domain.OrderStatus.Cancelled,
            Bybit.Net.Enums.V5.OrderStatus.New => Domain.OrderStatus.Cancelled,
            Bybit.Net.Enums.V5.OrderStatus.PartiallyFilled => Domain.OrderStatus.Cancelled,
            Bybit.Net.Enums.V5.OrderStatus.Filled => Domain.OrderStatus.Cancelled,
            Bybit.Net.Enums.V5.OrderStatus.PartiallyFilledCanceled => Domain.OrderStatus.Cancelled,
            Bybit.Net.Enums.V5.OrderStatus.Untriggered => Domain.OrderStatus.Untriggered,
            Bybit.Net.Enums.V5.OrderStatus.Triggered => Domain.OrderStatus.Triggered,
            Bybit.Net.Enums.V5.OrderStatus.Deactivated => Domain.OrderStatus.Deactivated,
            Bybit.Net.Enums.V5.OrderStatus.Active => Domain.OrderStatus.Active,
            _ => throw new ArgumentOutOfRangeException(nameof(bybitOrderStatus), bybitOrderStatus, null)
        };
    }
    
    public static Domain.OrderSide MapOrderSide(Bybit.Net.Enums.OrderSide orderSide)
    {
        return orderSide switch
        {
            Bybit.Net.Enums.OrderSide.Buy => Domain.OrderSide.Buy,
            Bybit.Net.Enums.OrderSide.Sell => Domain.OrderSide.Sell,
            _ => throw new ArgumentOutOfRangeException(nameof(orderSide), orderSide, null)
        };
    }
}