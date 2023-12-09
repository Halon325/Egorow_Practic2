using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Egorow_Practic
{
    public static class OrderUpdater
    {
        // Метод для обновления заказов
        public static void UpdateOrders()
        {
            // Использование контекста базы данных
            using (var context = new Egorow_Dem23Entities1())
            {
                // Получение заказов со статусом 1
                var orders = context.Order.Where(o => o.idStatus == 1);
                // Обновление каждого заказа
                foreach (var order in orders)
                {
                    // Вычисление времени закрытия заказа
                    DateTime closeTime = order.DateCreate.Add(order.TimeOrder).AddMinutes(order.TimeRental);
                    // Если время закрытия заказа меньше или равно текущему времени и дата закрытия заказа меньше или равна текущей дате
                    if (closeTime <= DateTime.Now && order.DateClose.Value.Date <= DateTime.Now.Date)
                    {
                        // Изменение статуса заказа на 2
                        order.idStatus = 2;
                    }
                }
                // Сохранение изменений в базе данных
                context.SaveChanges();
            }
        }
    }
}