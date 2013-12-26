using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Legacy.Export.RequestHandler.Generator.Orders
{
    public class Order
    {
        public Order(string orderName, OrderSort sort)
        {
            _orderName = orderName;
            _orderSort = sort;
        }

        public Order(string orderName)
        {
            _orderName = orderName;
            _orderSort = OrderSort.Asc;
        }

        private string _orderName;

        public string OrderName
        {
            get { return _orderName; }
            set { _orderName = value; }
        }
        private OrderSort _orderSort;

        public OrderSort OrderSort
        {
            get { return _orderSort; }
            set { _orderSort = value; }
        }
    }

    public enum OrderSort
    {
        Asc, Desc
    }
}
