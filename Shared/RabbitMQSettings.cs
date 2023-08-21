using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared;

public static class RabbitMQSettings
{
    public static string Stock_OrderCreatedEventQueue = "stock-order-created-event-queue";
    public static string Payment_StockReservedEventQueue = "payment-stock-reserved-event-queue";
    public static string Order_PaymentCompletedEventQueue = "order-payment-completed-event-queue";
}