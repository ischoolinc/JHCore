using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Legacy.Export.ResponseHandler.Converter
{
    public class ConverterFactory
    {
        private static Dictionary<string, IConverter> _converters;

        public static IConverter GetInstance(string name)
        {
            if (_converters == null)
            {
                _converters = new Dictionary<string, IConverter>();
                _converters.Add("", new BaseConverter());
            }

            switch (name)
            {
                default:
                    return _converters[""];
            }
        }
    }
}
