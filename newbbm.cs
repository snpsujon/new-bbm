using AspNetCore.Reporting;
using CodeWeaverAccount.Areas.Merchandising.ViewModels;
using CodeWeaverAccount.Data;
using ERPProject.Models;
using ERPProject.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeWeaverAccount.Areas.Merchandising.Controllers
{
    [Area("Mercha" +
        "" +
        "" +
        "" +
        "ndising")]
    public class BudgetbillnewController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public BudgetbillnewController(DataContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
        }


        public IActionResult Index(string data)
        {
            ViewBag.WorkOrderNo = data;
            var model = _context.BudgedofWorkOrders.Where(u => u.WordOrderNo == data).ToList();

            return View(model);
            
        }



        public IActionResult Create(string data)
        {

            var BudgedId = _context.BudgedofWorkOrders.Where(u => u.WordOrderNo == data).FirstOrDefault().Id;
            ViewBag.TotalAmmount = _context.BudgedofWorkOrders.Find(BudgedId).TotalAmount;
            ViewBag.Workorder = data;
            ViewBag.BudgetNo = "Budget" + data.Substring(5);
            var styles = _context.Styles.Where(x => x.WorkOrderNo == data).ToList();
            IEnumerable<SelectListItem> selectList = from s in styles
                                                     select new SelectListItem
                                                     {
                                                         Value = s.Id.ToString(),
                                                         Text = s.StyleName + " - " + s.StyleCode
                                                     };
            ViewBag.styles = selectList;
            


            var units = _context.Units.ToList();
            ViewBag.units = units;
            var catagory = _context.InventoryCatagories.ToList();
            var inventCatagoryCharacteristics = _context.InventCatagoryCharacteristics.ToList();
            ViewBag.catagory = catagory;
            ViewBag.inventCatagoryCharacteristics = inventCatagoryCharacteristics;
            var expence = _context.MarchentExpensesTypes.ToList();
            ViewBag.expence = expence;

            var budget = (from style in _context.Styles.Where(x => x.WorkOrderNo == data)
                          join bmd in _context.BillofMaterialDetails
                          on style.Id equals bmd.StyleId
                          join mat in _context.InventoryCatagories
                          on bmd.CatagoryId equals mat.Id
                          join unit in _context.Units
                          on bmd.UnitId equals unit.Id
                          join bbm in _context.BudgedofBillofMaterials
                          on bmd.Id equals bbm.BillofMaterialId
                          


                          select new BudgetBillMatVM
                          {
                              stylename = style.StyleName + "-" + style.StyleCode,
                              style = Convert.ToInt32(style.Id),
                              Materialname = mat.Catagory,
                              MaterialId = mat.Id,
                              Units = unit.UnitName,
                              consumtion = bbm.Consumption.ToString(),
                              wastage = bbm.Wastage.ToString(),
                              perunitprice = bbm.PerUnitPrice.ToString(),
                              totalquantity = bbm.TotalQuintity.ToString(),
                              totalprice = bbm.TotalPrice.ToString(),
                          }).OrderBy(x=>x.stylename);
                         



            return View(budget);
        }


        public JsonResult getdata(int StyleID,int MatID)
        {

            var budedit = (from bmd in _context.BillofMaterialDetails.Where(x => x.StyleId == StyleID && x.CatagoryId == MatID)
                           join mat in _context.InventoryCatagories
                           on bmd.CatagoryId equals mat.Id
                           join unit in _context.Units
                           on bmd.UnitId equals unit.Id
                           join bbm in _context.BudgedofBillofMaterials
                           on bmd.Id equals bbm.BillofMaterialId

                           select new BudgetBillMatVM
                          {
                              parameter = bmd.Width,
                              UnitsId = unit.Id,
                              wastage = bbm.Wastage.ToString(),
                              consumtion = bbm.Consumption.ToString(),
                              perunitprice = bbm.PerUnitPrice.ToString(),
                              totalquantity = bbm.TotalQuintity.ToString(),
                              totalprice = bbm.TotalPrice.ToString(),


                          }).FirstOrDefault();


            return Json(budedit);
        }
        public JsonResult setcom(int StyleID , int MatID)
        {
            var viewmodel = new List<ERPProject.Models.BOMSetComposition>();
            var model = new ERPProject.Models.BOMSetComposition();

            var bbmdid = _context.BillofMaterialDetails.Where(x => x.StyleId == StyleID && x.CatagoryId == MatID).FirstOrDefault().Id;

            var SelStelCom = _context.BOMSetCompositions.Where(x => x.BillofMaterialId == bbmdid).ToList();

            for (int i = 0; i < SelStelCom.Count(); i++)
            {
                viewmodel.Add(SelStelCom[i]);

            }

            return Json(viewmodel);
        }

        [HttpPost]
        public JsonResult Create(BudgetBillMatVM data)
        {
            try
            {
                var BudgedId = _context.BudgedofWorkOrders.Where(u => u.WordOrderNo == data.workorder).FirstOrDefault().Id;
                var bmd = new BillofMaterialDetails()
                {
                    StyleId = Convert.ToInt32(data.style),
                    UnitId = Convert.ToInt32(data.Units),
                    CatagoryId = Convert.ToInt32(data.MaterialCatagory),
                    Width = data.parameter
                };
                _context.BillofMaterialDetails.Add(bmd);
                _context.SaveChanges();

                foreach (var item in data.setcom)
                {
                    var setcoms = new BOMSetComposition()
                    {
                        StyleId = data.style,
                        BudgedId = BudgedId,
                        SetCompositionId = item.SetCompositionId,
                        BillofMaterialId = bmd.Id

                    };
                    _context.BOMSetCompositions.Add(setcoms);
                    _context.SaveChanges();
                }
                if(data.catagoryFeildDataVMs != null)
                {
                    foreach (var item in data.catagoryFeildDataVMs)
                    {
                        var feildData = new BOMInventoryCatagoryFeildData()
                        {
                            StyleId = Convert.ToInt32(data.style),
                            BudgedId = Convert.ToInt32(BudgedId),

                            BillofMaterialId = bmd.Id,
                            InventoryCatagoryFeildId = item.inventoryCatagoryFeildId,
                            InventoryCatagoryId = data.MaterialCatagory,
                            FeildData = item.fieldData


                        };
                        _context.BOMInventoryCatagoryFeildDatas.Add(feildData);
                        _context.SaveChanges();
                    }

                }
                

                var bbm = new BudgedofBillofMaterial()
                {
                    StyleId = Convert.ToInt32(data.style),
                    BudgedId = Convert.ToInt32(BudgedId),
                    BillofMaterialId = bmd.Id,
                    Consumption = Convert.ToDouble(data.consumtion),
                    Wastage = Convert.ToDouble(data.wastage),
                    PerUnitPrice = Convert.ToDouble(data.perunitprice),
                    TotalQuintity = Convert.ToDouble(data.totalquantity),
                    TotalPrice = Convert.ToDouble(data.totalprice)
                };
                _context.BudgedofBillofMaterials.Add(bbm);
                _context.SaveChanges();

                BudgedofWorkOrder bwo = _context.BudgedofWorkOrders.Find(BudgedId);

                var totalbudammunt = _context.BudgedofBillofMaterials.Where(x => x.BudgedId == Convert.ToInt32(BudgedId)).Sum(x => x.TotalPrice);
                var totalotrammunt = _context.BudgetOtherExpenses.Where(x => x.BudgedId == Convert.ToInt32(BudgedId)).Sum(x => x.TotalPrice);
                var totalammunt = totalbudammunt + totalotrammunt;
                bwo.TotalAmount = totalammunt;
                _context.BudgedofWorkOrders.Update(bwo);
                _context.SaveChanges();
                return Json(1);

            }
            catch
            {
                return Json(0);
            }
            




            
        }

        [HttpPost]
        public JsonResult OtherExpenceCreate(BudgetOtherExpenceVM data)
        {
            try
            {
                var BudgedId = _context.BudgedofWorkOrders.Where(u => u.WordOrderNo == data.workorder).FirstOrDefault().Id;
                var otherExpence = new BudgetOtherExpense
                {
                    StyleId = Convert.ToInt32(data.style),
                    BudgedId = Convert.ToInt32(BudgedId),
                    ExpensesTypeId = Convert.ToInt32(data.expenceCatagory),
                    ProductQuintity = Convert.ToInt32(data.productquantity),
                    UnitPrice = Convert.ToDouble(data.unitPrice),
                    TotalPrice = Convert.ToDouble(data.totalpriceO)
                };
                _context.BudgetOtherExpenses.Add(otherExpence);
                _context.SaveChanges();

                BudgedofWorkOrder bwo = _context.BudgedofWorkOrders.Find(BudgedId);

                var totalbudammunt = _context.BudgedofBillofMaterials.Where(x => x.BudgedId == Convert.ToInt32(BudgedId)).Sum(x => x.TotalPrice);
                var totalotrammunt = _context.BudgetOtherExpenses.Where(x => x.BudgedId == Convert.ToInt32(BudgedId)).Sum(x => x.TotalPrice);
                var totalammunt = totalbudammunt + totalotrammunt;
                bwo.TotalAmount = totalammunt;
                _context.BudgedofWorkOrders.Update(bwo);
                _context.SaveChanges();

                return Json(1);
            }
            catch
            {
                return Json(0);
            }
        }


        public JsonResult GetInseam(int StyleID)
        {

            //collecting inseam data
            var viewmodel = new List<ERPProject.Models.StyleSetComposition>();
            var model = new ERPProject.Models.StyleSetComposition();

            var SelStelCom = _context.StyleSetCompositions.Where(x => x.StyleId == StyleID).ToList();
            
            for (int i = 0; i < SelStelCom.Count(); i++)
            {
                viewmodel.Add(SelStelCom[i]);
               
            }

           

            //foreach (var items in SelStelCom)
            //{
            //    model.StyleId = items.StyleId;
            //    model.Inseam = items.Inseam;
            //    model.GmtSize = items.GmtSize;
            //    model.Color = items.Color;
            //    model.PurchaseOrder = items.PurchaseOrder;
            //    model.Country = items.Country;
            //    model.Quantity = items.Quantity;
            //    viewmodel.Add(model);
            //}
            return Json(viewmodel);
        }




            public JsonResult GetMaterialChar(int MaterialID)
        {

            //collecting inseam data
            var viewmodel = new List<ERPProject.Models.InventCatagoryCharacteristic>();

            
            var model = new ERPProject.Models.InventCatagoryCharacteristic();

            var SelStelCom = _context.InventCatagoryCharacteristics.Where(x => x.InventoryCatagoryId == MaterialID).ToList();

            for(int i = 0; i<SelStelCom.Count(); i++)
            {
                viewmodel.Add(SelStelCom[i]);
            }
            //foreach (var items in SelStelCom)
            //{
                
            //    model.InventoryCatagoryId = items.InventoryCatagoryId;
            //    model.Characterestic = items.Characterestic;
            //    viewmodel.Add(model);
            //}
            return Json(viewmodel);
        }


        




    }
}
