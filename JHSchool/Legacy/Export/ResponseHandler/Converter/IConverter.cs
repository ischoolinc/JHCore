using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Legacy.Export.ResponseHandler.Converter
{
    public interface IConverter
    {
        string Convert(string value);
    }
}
