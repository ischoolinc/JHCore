using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using JHSchool.Legacy.Export.RequestHandler.Generator.Condition;
using JHSchool.Legacy.Export.RequestHandler.Generator.Orders;

namespace JHSchool.Legacy.Export.RequestHandler.Generator
{
    public abstract class AbstractRequestGenerator : IRequestGenerator
    {
        private FieldCollection _fieldCollection;
        private List<ICondition> _conditions;
        private List<Order> _orders;

        public virtual DSRequest Generate()
        {
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            foreach (Field field in _fieldCollection)
            {
                helper.AddElement("Field", field.FieldName);
            }

            helper.AddElement("Condition");
            foreach (ICondition condition in _conditions)
            {
                helper.AddElement("Condition", condition.GetConditionElement());
            }

            if (_orders.Count > 0)
            {
                helper.AddElement("Order");
                foreach (Order order in _orders)
                {
                    helper.AddElement("Order", order.OrderName, order.OrderSort.ToString());
                }
            }
            return new DSRequest(helper);
        }

        public virtual void Initialize()
        {
            _orders = new List<Order>();
            _conditions = new List<ICondition>();
            _fieldCollection = new FieldCollection();
        }

        public virtual void SetSelectedFields(FieldCollection selectedFields)
        {
            _fieldCollection = selectedFields;
        }

        public virtual void AddCondition(ICondition condition)
        {
            _conditions.Add(condition);
        }

        public virtual void AddOrder(Order order)
        {
            _orders.Add(order);
        }
    }
}
