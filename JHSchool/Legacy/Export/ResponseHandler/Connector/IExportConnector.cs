using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using JHSchool.Legacy.Export.RequestHandler;

namespace JHSchool.Legacy.Export.ResponseHandler.Connector
{
    public interface IExportConnector
    {      
        void SetSelectedFields(FieldCollection exportFields);
        void AddCondition(string argument);        
        ExportTable Export();
    }
}
