using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CR_ProposalNumber.Models
{
    public class ProposalDetails
    {
        public string ProposalNumber { get; set; }
        public string Status { get; set; }
        public string SubStatus { get; set; }
        public string CollectionType { get; set; }
        public string IMDCode { get; set; }
        public string PolicyHolder { get; set; }
        public string PremiumPayerApplicable { get; set; }
        public string PayerID { get; set; }
        public string Premium { get; set; }
        public string TotalTaxes { get; set; }
        public string TotalPremiumDue { get; set; }
        public string CollectionNumber { get; set; }
        public string CollectionDate { get; set; }
    }

}