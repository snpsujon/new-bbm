using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeWeaverAccount.Areas.Merchandising.ViewModels
{
    public class BudgetBillMatVM
    {
        public int MaterialCatagory { get; set; }
        public int style { get; set; }
        public string Units { get; set; }
        public int UnitsId { get; set; }
        public string stylename { get; set; }
        public string Materialname { get; set; }
        public int MaterialId { get; set; }
        public string parameter { get; set; }
        public string consumtion { get; set; }
        public string wastage { get; set; }
        public string perunitprice { get; set; }
        public string totalprice { get; set; }
        public string totalquantity { get; set; }
        public string workorder { get; set; }
        public List<StyleSetCompositionVM> setcom { get; set; }
        public List<inventoryCatagoryFeildDataVM> catagoryFeildDataVMs { get; set; }

    }

    public class StyleSetCompositionVM
    {

        public int SetCompositionId { get; set; }
        public string inseam { get; set; }

        public string gmtSize { get; set; }

        public string color { get; set; }

        public string purchaseOrder { get; set; }

        public string country { get; set; }

        public int quantity { get; set; }
        
    }

    public class inventoryCatagoryFeildDataVM
    {
        public int inventoryCatagoryId { get; set; }
        public int inventoryCatagoryFeildId { get; set; }
        public string fieldData { get; set; }

    }

    public class BudgetOtherExpenceVM
    {
        public string style { get; set; }
        public string productquantity { get; set; }
        public string expenceCatagory { get; set; }
        public string totalpriceO { get; set; }
        public string unitPrice { get; set; }
        public string workorder { get; set; }
        

    }



    }
