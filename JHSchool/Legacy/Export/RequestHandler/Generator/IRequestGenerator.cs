using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using JHSchool.Legacy.Export.RequestHandler.Generator.Condition;
using JHSchool.Legacy.Export.RequestHandler.Generator.Orders;

namespace JHSchool.Legacy.Export.RequestHandler.Generator
{
    public interface IRequestGenerator
    {
        void AddCondition(ICondition condition);
        void AddOrder(Order order);
        void SetSelectedFields(FieldCollection selectedFields);
        DSRequest Generate();
    }
}
