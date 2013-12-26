using System;
using System.Collections.Generic;
using System.Text;
using DocValidate;

namespace JHSchool.Legacy.ImportSupport
{
    public interface IValidatorFactory : IFieldValidatorFactory, IRowValidatorFactory
    {
    }
}
