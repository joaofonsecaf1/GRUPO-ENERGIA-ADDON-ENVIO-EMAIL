using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Globalization;
using Application = SAPbouiCOM.Framework.Application;

namespace B2F.Addon.EnvioEmail
{
    public class CommonController
    {
        public static DateTimeFormatInfo DateTimeFormatInfo { get; private set; }
        public static NumberFormatInfo SumDecFormatInfo { get; private set; }
        public static NumberFormatInfo PriceDecFormatInfo { get; private set; }
        public static NumberFormatInfo RateFormatInfo { get; private set; }
        public static NumberFormatInfo QtyFormatInfo { get; private set; }
        public static NumberFormatInfo PercentDecFormatInfo { get; private set; }
        public static NumberFormatInfo MeasureDecFormatInfo { get; private set; }
        public static NumberFormatInfo QueryDecFormatInfo { get; private set; }
        public static bool LazyProcess { get; set; }
        public static bool InProcess { get; set; }
        public static SAPbobsCOM.Company Company { get; set; }
        public static CompanyService CompanyService { get; set; }
        public static int FormFatherCount { get; set; }
        public static string FormFatherType { get; set; }
        public static IForm FatherForm { get; set; }
        public static int FatherRowNumber { get; set; }
        public static object TransitionObject { get; set; }

        public static void GetCompany()
        {
            Company = (SAPbobsCOM.Company)Application.SBO_Application.Company.GetDICompany();
            CompanyService = Company.GetCompanyService();
            DateTimeFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;
            FormatInitializer();
        }

        private static void FormatInitializer()
        {
            string dateFormat;
            var adminInfo = Company.GetCompanyService().GetAdminInfo();

            var sumDecFormatInfo = new NumberFormatInfo
            {
                NumberDecimalSeparator = adminInfo.DecimalSeparator,
                NumberGroupSeparator = adminInfo.ThousandsSeparator,
                NumberDecimalDigits = adminInfo.TotalsAccuracy
            };

            SumDecFormatInfo = sumDecFormatInfo;

            var priceDecFormatInfo = new NumberFormatInfo
            {
                NumberDecimalSeparator = adminInfo.DecimalSeparator,
                NumberGroupSeparator = adminInfo.ThousandsSeparator,
                NumberDecimalDigits = adminInfo.PriceAccuracy
            };

            PriceDecFormatInfo = priceDecFormatInfo;

            var rateFormatInfo = new NumberFormatInfo
            {
                NumberDecimalSeparator = adminInfo.DecimalSeparator,
                NumberGroupSeparator = adminInfo.ThousandsSeparator,
                NumberDecimalDigits = adminInfo.RateAccuracy
            };

            RateFormatInfo = rateFormatInfo;

            var qtyFormatInfo = new NumberFormatInfo
            {
                NumberDecimalSeparator = adminInfo.DecimalSeparator,
                NumberGroupSeparator = adminInfo.ThousandsSeparator,
                NumberDecimalDigits = adminInfo.AccuracyofQuantities
            };

            QtyFormatInfo = qtyFormatInfo;

            var percentDecFormatInfo = new NumberFormatInfo
            {
                NumberDecimalSeparator = adminInfo.DecimalSeparator,
                NumberGroupSeparator = adminInfo.ThousandsSeparator,
                NumberDecimalDigits = adminInfo.PercentageAccuracy
            };

            PercentDecFormatInfo = percentDecFormatInfo;

            var measureDecFormatInfo = new NumberFormatInfo
            {
                NumberDecimalSeparator = adminInfo.DecimalSeparator,
                NumberGroupSeparator = adminInfo.ThousandsSeparator,
                NumberDecimalDigits = adminInfo.MeasuringAccuracy
            };

            MeasureDecFormatInfo = measureDecFormatInfo;

            var queryDecFormatInfo = new NumberFormatInfo
            {
                NumberDecimalSeparator = adminInfo.DecimalSeparator,
                NumberGroupSeparator = adminInfo.ThousandsSeparator,
                NumberDecimalDigits = adminInfo.QueryAccuracy
            };

            QueryDecFormatInfo = queryDecFormatInfo;

            switch (adminInfo.DateTemplate)
            {
                case BoDateTemplate.dt_DDMMYY:
                    {
                        dateFormat = "dd{0}MM{0}yy";
                        break;
                    }
                case BoDateTemplate.dt_DDMMCCYY:
                    {
                        dateFormat = "dd{0}MM{0}yyyy";
                        break;
                    }
                case BoDateTemplate.dt_MMDDYY:
                    {
                        dateFormat = "MM{0}dd{0}yy";
                        break;
                    }
                case BoDateTemplate.dt_MMDDCCYY:
                    {
                        dateFormat = "MM{0}dd{0}yyyy";
                        break;
                    }
                case BoDateTemplate.dt_CCYYMMDD:
                    {
                        dateFormat = "yyyy{0}MM{0}dd";
                        break;
                    }
                case BoDateTemplate.dt_DDMonthYYYY:
                    {
                        dateFormat = "dd{0}MMMM{0}yyyy";
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException("Formato de data identificado não é conhecido (AdminInfo.DateTemplate).");
                    }
            }

            var datePattern = string.Format(dateFormat, adminInfo.DateSeparator);
            var hourFormat = (adminInfo.TimeTemplate == BoTimeTemplate.tt_24H ? "HH:mm" : "hh:mm");
            var dateSeparator = (DateTimeFormatInfo)DateTimeFormatInfo.Clone();

            dateSeparator.DateSeparator = adminInfo.DateSeparator;
            dateSeparator.LongDatePattern = datePattern;
            dateSeparator.ShortDatePattern = datePattern;
            dateSeparator.LongTimePattern = hourFormat;
            dateSeparator.ShortTimePattern = hourFormat;

            DateTimeFormatInfo = dateSeparator;
        }

        public static string GetCountyByAbsId(int absId)
        {
            var recordset = (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            recordset.DoQuery($@"select ""Name"" from OCNT where ""AbsId"" = {absId}");
            return recordset.RecordCount > 0 ? recordset.Fields.Item("Name").Value.ToString() : string.Empty;
        }
    }
}
