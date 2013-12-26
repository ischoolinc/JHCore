using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using FISCA.Authentication;

namespace JHSchool.Feature.Legacy
{
    [AutoRetryOnWebException()]
    public class QueryDiscipline
    {
        //public static DSResponse GetDiscipline(DSRequest request)
        //{
        //    return DSAServices.CallService("SmartSchool.Student.Discipline.GetDiscipline", request);
        //}

        //已被取代
        //public static DSResponse GetDemeritStatistic(DSRequest request)
        //{
        //    return DSAServices.CallService("SmartSchool.Student.Discipline.GetDemeritStatistic", request);
        //}

        //public static DSResponse GetMeritStatistic(DSRequest request)
        //{
        //    return DSAServices.CallService("SmartSchool.Student.Discipline.GetMeritStatistic", request);
        //}

        //public static DSResponse GetMeritIgnoreDemerit(DSRequest request)
        //{
        //    return DSAServices.CallService("SmartSchool.Student.Discipline.GetMeritIgnoreDemerit", request);
        //}

        //public static DSResponse GetMeritIgnoreUnclearedDemerit(DSRequest request)
        //{
        //    return DSAServices.CallService("SmartSchool.Student.Discipline.GetMeritIgnoreUnclearedDemerit", request);
        //}
    }
}
