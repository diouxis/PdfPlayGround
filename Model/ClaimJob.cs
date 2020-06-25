using System;
using System.Collections.Generic;
using System.Text;

namespace PdfPlayGround.Model
{
    public class ClaimJob
    {
        public Company Insurer { get; set; }
        public ContactDetail Insured { get; set; }
        public Portfolio Building { get; set; }

        public ReportForm ReportForm { get; set; }
        public ReportData ReportData { get; set; }

        public void FillData()
        {
            var dataSet = ReportData.Data;
            foreach (var card in ReportForm.Cards)
            {
                foreach (var field in card.Fields)
                {
                    field.FillData(dataSet);
                }
            }
        }
    }

    public class Portfolio
    {
        public Company ScopingSupplier { get; set; }
        public Company AuthorisedSupplier { get; set; }
    }

    public class Company
    {
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone1 { get; set; }
        public string CompanyPhone2 { get; set; }
    }

    public class ContactDetail
    {
        public string Name { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Phone3 { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
    }
}
