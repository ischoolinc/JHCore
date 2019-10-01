using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Feature.Legacy
{
    public class QueryRequestAttribute : FISCA.Authentication.AutoRetryOnWebExceptionAttribute
    {
        private int _ReTryTimes = 3;
        public int ReTryTimes
        {
            get { return _ReTryTimes; }
        }
        public QueryRequestAttribute() { }
        public QueryRequestAttribute(int retryTimes) 
        {
            _ReTryTimes = retryTimes;
        }
    }
}
