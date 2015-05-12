using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Korea.Models.Domain;
using Korea.Models;
using System.Net;

using Korea.Models.Xls;
using LinqToExcel;
using System.Data.Entity;
using UnidecodeSharpFork;
using System.Xml.Linq;
using Korea.Models.Xml;
using System.Text.RegularExpressions;
using BytesRoad.Net.Ftp;

namespace Korea.Controllers
{
    //[Authorize]
    public class DownloadController : Controller
    {
        //
        // GET: /Download/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Loding(HttpPostedFileBase Price, HttpPostedFileBase Analog, HttpPostedFileBase Categorys)
        {
            List<string> ErrorList = new List<string>();

            string pathPrice = Server.MapPath("../Models/Xls/новый прайс.xls");
            if (Price != null)
            {
                if (System.IO.File.Exists(pathPrice))
                {
                    System.IO.File.Delete(pathPrice);
                }
                Stream rs = Price.InputStream;

                using (var fileCreate = System.IO.File.Create(pathPrice))
                {
                    rs.CopyTo(fileCreate);
                    fileCreate.Close();
                }
                rs.Close();
            }

            string pathAnalog = Server.MapPath("../Models/Xls/Аналоги.xls");
            if (Analog != null)
            {
                if (System.IO.File.Exists(pathAnalog))
                {
                    System.IO.File.Delete(pathAnalog);
                }
                Stream rs = Analog.InputStream;

                using (var fileCreate = System.IO.File.Create(pathAnalog))
                {
                    rs.CopyTo(fileCreate);
                    fileCreate.Close();
                }
                rs.Close();
            }

            string pathOstatok = Server.MapPath("../Models/Xls/Категории.xls");
            if (Categorys != null)
            {
                if (System.IO.File.Exists(pathOstatok))
                {
                    System.IO.File.Delete(pathOstatok);
                }
                Stream rs = Categorys.InputStream;

                using (var fileCreate = System.IO.File.Create(pathOstatok))
                {
                    rs.CopyTo(fileCreate);
                    fileCreate.Close();
                }
                rs.Close();
            }

            List<ProductForImport> ImportProducts = new List<ProductForImport>();
            Dictionary<string, string> PositionsDic = new Dictionary<string, string>();
            using (KoreaContext db = new KoreaContext())
            {


                var excel = new ExcelQueryFactory(Server.MapPath("../Models/Xls/новый прайс.xls"));
                //var excel2 = new ExcelQueryFactory(Server.MapPath("../Models/Xls/остатки.xls"));
                var excel3 = new ExcelQueryFactory(Server.MapPath("../Models/Xls/Аналоги.xls"));
                var excel4 = new ExcelQueryFactory(Server.MapPath("../Models/Xls/марки_модели.xls"));
                var excel5 = new ExcelQueryFactory(Server.MapPath("../Models/Xls/Категории.xls"));
                List<Price> xlsPraic = excel.Worksheet<Price>("Запрос3").ToList();
                //List<Osta> xlsPraic2 = excel2.Worksheet<Osta>("TDSheet").ToList();
                List<Analogs> xlsPraic3 = excel3.Worksheet<Analogs>("Аналоги").ToList();
                List<ModelXls> xlsPraic4 = excel4.Worksheet<ModelXls>("Лист1").ToList();
                List<CategoryXLS> xlsPraic5 = excel5.Worksheet<CategoryXLS>("Лист134").ToList();

                //List<ResidueModel> residues = new List<ResidueModel>();
                //ResidueModel residue = new ResidueModel();
                //for (int i = 0; i < Art.Count(); i++)
                //{
                //    residue.Article = Art[i];
                //    if (Num[i] != "") residue.Number = Convert.ToInt32(Num[i].Replace(" ", ""));
                //    if (Cost[i] != "") residue.Cost = Convert.ToInt32(Cost[i].Replace(" ", ""));
                //    residues.Add(residue);
                //}




                
                List<SupplierForImport> ImportSuppliers = new List<SupplierForImport>();
                List<CategoryForImport> ImportCategorys = new List<CategoryForImport>();
                List<AnalogsForImport> ImportAnalogs = new List<AnalogsForImport>();
                List<string> Suppliers = db.SupplierForImports.Select(s => s.Title).ToList();

                List<AnalogsForImport> Analogss = new List<AnalogsForImport>();
                List<string> ProductsDIC = new List<string>();
                Dictionary<Guid, string> ProductSupplierDic = new Dictionary<Guid, string>();


                Analogss = db.AnalogsForImports.ToList();
                ProductsDIC = db.ProductForImports.Select(p => p.Сode).ToList();
                ProductSupplierDic = db.ProductForImports.Where(p => p.SupplierId != null)
                                       .Include("Supplier")
                                       .ToDictionary(p => p.Id, p => p.Supplier.Title);
                foreach (Analogs An in xlsPraic3)
                {
                    AnalogsForImport ImportAnalog = new AnalogsForImport()
                    {
                         Id = Guid.NewGuid(),
                         Сode1 = An.Id1,
                         Сode2 = An.Id2
                    };
                    ImportAnalogs.Add(ImportAnalog);
                }
                List<ProductForImport> Errors = new List<ProductForImport>();


                List<Guid> AnalogsDelete = SearchGuids(false, Analogss, true, ImportAnalogs);
                AnalogsDelete.Concat(SearchGuids(true, Analogss, false, ImportAnalogs));

                foreach (Guid LineDelete in AnalogsDelete)
                {
                    db.AnalogsForImports.Remove(db.AnalogsForImports.FirstOrDefault(c => c.Id == LineDelete));
                };

                List<AnalogsForImport> CreateList = new List<AnalogsForImport>();


                CreateList = ImportAnalogs.Where(d => !Analogss.Select(i => i.Сode1).Contains(d.Сode1))
                                          .Where(d => !Analogss.Select(i => i.Сode2).Contains(d.Сode2))
                                          .ToList();
                CreateList.Concat(ImportAnalogs.Where(d => !Analogss.Select(i => i.Сode2).Contains(d.Сode1))
                                               .Where(d => !Analogss.Select(i => i.Сode1).Contains(d.Сode2))
                                               .ToList());
                ///Аналоги
                foreach (AnalogsForImport Line in CreateList)
                {
                    db.AnalogsForImports.Add(Line);
                }
                db.SaveChanges();


                //Data Reader methods
                foreach (Price line in xlsPraic)
                {
                    SupplierForImport ImportSupplier = new SupplierForImport();

                    if (ImportSuppliers.Where(i => i.Title == line.Suppliers).Count() == 0  
                        && line.Suppliers != null
                        && line.Suppliers != "")
                    {
                        ImportSupplier.Title = line.Suppliers;
                        ImportSupplier.Id = Guid.NewGuid();
                        ImportSupplier.Weight = 0;
                        ImportSuppliers.Add(ImportSupplier);
                    }
                }
                List<string> suppliersDelete = Suppliers.Where(c => !ImportSuppliers.Select(i => i.Title).Contains(c)).ToList();
                foreach (string supplierDelete in suppliersDelete)
                {

                    List<Guid> ProductGuidDelete = ProductSupplierDic.Where(p => p.Value == supplierDelete).Select(p => p.Key).ToList();
                    foreach (Guid ProductGuid in ProductGuidDelete)
                    {
                        db.ProductForImports.Remove(db.ProductForImports.Single(p => p.Id == ProductGuid));
                    }
                    db.SupplierForImports.Remove(db.SupplierForImports.FirstOrDefault(c => c.Title == supplierDelete));
                }
                db.SaveChanges();

                IEnumerable<SupplierForImport> suppliersEdit = ImportSuppliers.Where(i => Suppliers.Contains(i.Title)).ToList();
                foreach (SupplierForImport supplierEdit in suppliersEdit)
                {
                    if (null == db.SupplierForImports.Where(s => s == supplierEdit))
                    {
                        db.SupplierForImports.Attach(supplierEdit);
                        db.Entry(supplierEdit).State = EntityState.Modified;
                    }
                }
                db.SaveChanges();

                IEnumerable<SupplierForImport> suppliersCreate = ImportSuppliers.Where(i => !Suppliers.Contains(i.Title)).ToList();
                foreach (SupplierForImport supplierCreate in suppliersCreate)
                {
                    db.SupplierForImports.Add(supplierCreate);
                }
                db.SaveChanges();


                //Загрузка Брендов
                List<BrandForImport> ImportBrands = new List<BrandForImport>();
                List<ModelForImport> ImportModels = new List<ModelForImport>();
                List<string> Brands = db.BrandForImports.Select(s => s.Abbreviation).ToList();
                List<string> Models = db.ModelForImports.Select(s => s.Title).ToList();
                Guid BrandEmptGuid = Guid.NewGuid();

                foreach (ModelXls line in xlsPraic4)
                {
                    BrandForImport ImportBrand = new BrandForImport();
                    ImportBrand.Abbreviation = line.Abriviatura;
                    if (ImportBrands.Where(i => i.Abbreviation == ImportBrand.Abbreviation).Count() == 0)
                    {
                        ImportBrand.Id = Guid.NewGuid();
                        ImportBrand.Title = line.Brand;
                        ImportBrand.Weight = 0;
                        ImportBrand.ExtId = -1;
                        ImportBrand.Abbreviation = line.Abriviatura;
                        ImportBrands.Add(ImportBrand);
                    }

                }
                //BrandForImport BrandEmpt = new BrandForImport();
                //BrandEmpt.Id = BrandEmptGuid;
                //BrandEmpt.Weight = 0;
                //BrandEmpt.Title = "Без брэнда";
                //BrandEmpt.Abbreviation = "NONONONONO";
                //ImportBrands.Add(BrandEmpt);

                ImportBrands = ImportBrands.Distinct().ToList();

                List<string> itemsDelete = Brands.Where(c => !ImportBrands.Select(i => i.Abbreviation).Contains(c)).ToList();
                foreach (string itemDelete in itemsDelete)
                {
                    Guid id = db.BrandForImports.FirstOrDefault(c => c.Abbreviation == itemDelete).Id;
                }

                IEnumerable<BrandForImport> itemsEdit = ImportBrands.Where(i => Brands.Contains(i.Abbreviation));
                foreach (BrandForImport itemEdit in itemsEdit)
                {
                    if (null == db.BrandForImports.Where(c => c == itemEdit))
                    {
                        db.BrandForImports.Attach(itemEdit);
                        db.Entry(itemEdit).State = EntityState.Modified;
                    }
                }
                IEnumerable<BrandForImport> itemsCreate = ImportBrands.Where(i => !Brands.Contains(i.Abbreviation));
                foreach (BrandForImport itemCreate in itemsCreate)
                {
                    db.BrandForImports.Add(itemCreate);
                }
                db.SaveChanges();

                //Заполнение моделей
                Dictionary<string, Guid> BrandsDic = db.BrandForImports.ToDictionary(s => s.Abbreviation, r => r.Id);
                Brands = db.BrandForImports.Select(s => s.Title).ToList();
                foreach (ModelXls line in xlsPraic4)
                {
                    ModelForImport ImportModel = new ModelForImport();
                    ImportModel.Id = Guid.NewGuid();
                    ImportModel.Weight = 0;
                    ImportModel.ExtId = -1;
                    ImportModel.Title = line.Model;
                    ImportModel.BrandId = BrandsDic.FirstOrDefault(b => b.Key == line.Abriviatura).Value;
                    ImportModels.Add(ImportModel);
                }
                ////Зоздание модели: "Без модели"
                //foreach (string Brand in Brands)
                //{
                //    if (Brand != "Без брэнда")
                //    {
                //        ModelForImport ModelEmpt = new ModelForImport();
                //        ModelEmpt.Id = BrandEmptGuid;
                //        ModelEmpt.Weight = 0;
                //        ModelEmpt.Title = "Без модели";
                //        ModelEmpt.BrandId = BrandsDic.FirstOrDefault(b => b.Key == Brand).Value;
                //        ImportModels.Add(ModelEmpt);
                //    }

                //}

                List<string> ModelsDelete = Models.Where(c => !ImportModels.Select(i => i.Title).Contains(c)).ToList();
                foreach (string ModelDelete in ModelsDelete)
                {
                    Guid id = db.ModelForImports.FirstOrDefault(c => c.Title == ModelDelete).Id;
                }

                IEnumerable<ModelForImport> ModelsEdit = ImportModels.Where(i => Models.Contains(i.Title));
                foreach (ModelForImport ModelEdit in ModelsEdit)
                {
                    if (null == db.ModelForImports.Where(c => c == ModelEdit))
                    {
                        db.ModelForImports.Attach(ModelEdit);
                        db.Entry(ModelEdit).State = EntityState.Modified;
                    }
                }
                IEnumerable<ModelForImport> ModelsCreate = ImportModels.Where(i => !Models.Contains(i.Title));
                foreach (ModelForImport ModelCreate in ModelsCreate)
                {
                    db.ModelForImports.Add(ModelCreate);
                }
                db.SaveChanges();



                //Загрузка каталога
                //Загрузка 1ого уровня
                Guid CategoryEmptCategory = Guid.NewGuid();
                string CategoryLv1;
                string CategoryLv2;
                foreach (CategoryXLS line in xlsPraic5)
                {
                    
                    if (line.Lv1 != "" 
                        && line.Lv1 != null)
                    {
                        CategoryLv1 = line.Lv1.Trim(' ');
                        if (ImportCategorys.FirstOrDefault(c => c.Title == CategoryLv1) == null)
                        {
                            CategoryForImport ImportCategoryLv1 = new CategoryForImport();
                            ImportCategoryLv1.Id = Guid.NewGuid();
                            ImportCategoryLv1.Title = CategoryLv1;
                            ImportCategoryLv1.ExtId = -1;
                            ImportCategoryLv1.Weight = 0;
                            ImportCategoryLv1.CategoryId = null;
                            ImportCategorys.Add(ImportCategoryLv1);
                        }
                    }
                }
                ImportCategorys = ImportCategorys.Distinct().ToList();
                List<CategoryForImport> ImportCategorysLv1 = ImportCategorys;
                CategoryForImport ImportCategory = new CategoryForImport();
                ImportCategory.Id = CategoryEmptCategory;
                ImportCategory.Title = "Без категории";
                ImportCategory.ExtId = -1;
                ImportCategory.Weight = 0;
                ImportCategorys.Add(ImportCategory);


                List<CategoryForImport> CategorysProgram = db.CategoryForImports.Where(c => c.CategoryId == null)
                                                                                .ToList();
                ImportCategory.DeleteCategorys(ImportCategorys, CategorysProgram);
                ImportCategory.CreateCategorys(ImportCategorys, CategorysProgram);
                CategorysProgram = db.CategoryForImports.ToList();
                ImportCategorys = ImportCategory.EditCategorys(ImportCategorys, CategorysProgram, db);


                foreach (CategoryXLS line in xlsPraic5)
                {
                    if (line.Lv1 != "" && line.Lv3 != "" && line.Lv1 != null && line.Lv3 != null)
                    {
                        CategoryLv1 = line.Lv1.Trim(' ');
                        CategoryLv2 = line.Lv3.Trim(' ');
                        if (ImportCategorys.FirstOrDefault(c => c.Title == CategoryLv2) == null)
                        {
                            CategoryForImport ImportCategoryLv3 = new CategoryForImport();
                            ImportCategoryLv3.Title = CategoryLv2;
                            ImportCategoryLv3.Id = Guid.NewGuid();
                            ImportCategoryLv3.ExtId = -1;
                            ImportCategoryLv3.Weight = 0;
                            ImportCategoryLv3.CategoryId = ImportCategorys.FirstOrDefault(c => c.Title == CategoryLv1).Id;
                            ImportCategorys.Add(ImportCategoryLv3);
                        }
                    }
                }


                CategorysProgram = db.CategoryForImports.Where(c => c.CategoryId != null)
                                                        .ToList();
                ImportCategorys = ImportCategorys.Where(i => i.CategoryId != null)
                                                 .ToList();
                ImportCategory.DeleteCategorys(ImportCategorys, CategorysProgram);
                ImportCategory.CreateCategorys(ImportCategorys, CategorysProgram);
                CategorysProgram = db.CategoryForImports.ToList();
                ImportCategorys = ImportCategory.EditCategorys(ImportCategorys, CategorysProgram, db);
                


                //Загрузка продуктов
                Dictionary<string, Guid> SuppliersDic = db.SupplierForImports.ToDictionary(s => s.Title, r => r.Id);
                Dictionary<string, Guid> CategorysDic = db.CategoryForImports.ToDictionary(s => s.Title, r => r.Id);
                //CategorysDic = CategorysDic.ToDictionary()
                BrandsDic = db.BrandForImports.ToDictionary(s => s.Abbreviation, r => r.Id);
                List<GenerationForImport> GenerationProgram = db.GenerationForImports.ToList();
                List<ModelForImport> ModelsList = db.ModelForImports.ToList();
                ModelForImport ConnectModels = new ModelForImport();
                List<ModelForImport> FiltModelsList = new List<ModelForImport>();

                //Подгодтовка поколения для моделей
                List<GenerationForImport> Generations = new List<GenerationForImport>();

                //Позиции для продуктов
                List<PositionForImport> PositionsNew = new List<PositionForImport>();

                //Потрошение на продукты
                foreach (Price line in xlsPraic)
                {
                    ProductForImport ImportProduct = new ProductForImport();

                    if (line.Article != null && line.Codes != null && line.Names != null && line.Suppliers != null)
                    {
                        string names = line.Names;
                        string categoryName = "";
                        
                        if (ImportProducts.Where(i => i.Сode == ImportProduct.Сode).Count() == 0)
                        {
                            names = names
                                .Replace("\"", "")
                                .Replace("\'", "")
                                .Replace(")", " ")
                                .Replace("(", " ")
                                .Replace(",", "");
                            int numChar = 0;
                            if (names.IndexOf(" ") != -1)
                            {
                                if (RusTxt(names, numChar))
                                {
                                    while (RusTxt(names, ++numChar) && names.Length > numChar)
                                    {
                                        numChar = names.IndexOf(" ", numChar++);
                                        if (numChar == -1)
                                        {
                                            numChar = names.Length;
                                            names += " ";
                                            break;
                                        }
                                    };

                                }
                                else
                                {
                                    numChar = names.IndexOf(" ") + 1;
                                }
                            }
                            else
                            {
                                numChar = names.Length + 1;
                            }
                            numChar = numChar - 1;
                            categoryName = names.Substring(0, numChar).Trim(' ','.',',');
                        }


                        ImportProduct.Сode = line.Codes.Trim(' ', '.', ',');
                        ImportProduct.SupplierId = SuppliersDic.FirstOrDefault(s => s.Key == line.Suppliers).Value;
                        ImportProduct.Name = names;
                        ImportProduct.Tale = 0;
                        ImportProduct.ExtId = -1;

                        //Подключение брендов, моделей, поколеней отключено из-за отсутствия нормального списка поколеней.
                        if (0 != CategorysDic.Where(c => c.Key == categoryName).Count())
                        {
                            ImportProduct.CategoryId = CategorysDic.FirstOrDefault(c => c.Key == categoryName).Value;
                        }
                        else
                        {
                            // на случай если категория содерит агнлийский буквы
                            Dictionary<string, Guid> CategoryContains = CategorysDic.Where(c => c.Key.Contains(categoryName))
                                                                                    .ToDictionary(c => c.Key, c => c.Value);
                            ImportProduct.CategoryId = CategoryContains.FirstOrDefault(c => ImportProduct.Name
                                                                                                         .Contains(c.Key))
                                                                       .Value;
                            if (ImportProduct.CategoryId == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                            {
                                ImportProduct.CategoryId = CategorysDic.FirstOrDefault(i => i.Key == "Без категории").Value;
                            }
                        }

                        ImportProduct.Id = Guid.NewGuid();
                        ImportProducts.Add(ImportProduct);

                        //if (PositionsDic.Where(p => p.Key == ImportProduct.Сode
                        //                       && p.Value == line.Article).Count() == 0)
                        //{
                        //PositionForImport tempPosition = new PositionForImport()
                        //{
                        //    Id = Guid.NewGuid(),
                        //    Title = ImportProduct.Сode,
                        //    Products = line.Article
                        //};
                        //PositionsNew.Add();
                        //PositionsDic.Add(ImportProduct.Сode, line.Article);
                            
                        //}
                        
                    }
                };

                
            }
            using (KoreaContext db = new KoreaContext())
            {
                //Запсь, удаление и изменение в продуктах
                List<ProductForImport> Products = db.ProductForImports.Include("Generations")
                                                                      .Include("Positions")
                                                                      .ToList();

                Products = Products.Join(ImportProducts,
                                                                          p => p.Сode,
                                                                          i => i.Сode,
                                                                          (p, i) => p.SetAll(i)
                                                                          ).ToList();
                db.SaveChanges();

                List<ProductForImport> productsDelete = Products.Where(c => !ImportProducts
                                                                            .Select(i => i.Сode)
                                                                            .Contains(c.Сode))
                                                                            .ToList();
                foreach (ProductForImport productDelete in productsDelete)
                {
                    if (productDelete.Positions != null)
                    {
                        foreach (PositionForImport Position in productDelete.Positions) 
                        {
                            db.PositionForImports.Remove(Position);
                        }
                    }
                    db.ProductForImports.Remove(productDelete);
                }


                List<ProductForImport> productsCreate = ImportProducts.Where(i => !Products.Select(p => p.Сode)
                                                                                                  .Contains(i.Сode))
                                                                             .ToList();
                foreach (ProductForImport productCreate in productsCreate)
                {
                    db.ProductForImports.Add(productCreate);
                }
                db.SaveChanges();


                //Подключение позиций
                var excel = new ExcelQueryFactory(Server.MapPath("../Models/Xls/новый прайс.xls"));
                List<Price> xlsPraic = excel.Worksheet<Price>("Запрос3").ToList();
                List<ProductForImport> productsProgram = db.ProductForImports.ToList();
                List<PositionForImport> PositionsProgram = db.PositionForImports.ToList();
                //PositionsNew = xlsPraic.Select(x => new PositionForImport() 
                //                                    { 
                //                                        Id = Guid.NewGuid(),
                //                                        Title = x.Article,
                //                                        Weight = 0,
                //                                        Products = new List<ProductForImport>()
                //                                    })
                //                       .ToList();
                //PositionsNew = PositionsNew.Where(p => p.Title != null).ToList();
                List<PositionForImport> PositionsNewList = new List<PositionForImport>();
                foreach (Price line in xlsPraic)
                {
                    if (line.Article != null && line.Codes != null && line.Names != null && line.Suppliers != null)
                    {
                        string Article = line.Codes.Trim(' ', '.', ',');
                        string Title = line.Article.Trim(' ', '.', ',');
                        ProductForImport Product = productsProgram.FirstOrDefault(p => p.Сode == Article);
                        PositionForImport Position = PositionsNewList.FirstOrDefault(p => p.Title == Title);
                        if (Position == null)
                        {
                            Position = new PositionForImport()
                            {
                                Id = Guid.NewGuid(),
                                Title = Title,
                                Weight = 0,
                                Products = new List<ProductForImport>()
                            };
                            Position.Products.Add(Product);
                            PositionsNewList.Add(Position);
                        }
                        else
                        {
                            ErrorList.Add(Title + " повторяющаяся позиция");
                        }
                    }
                }
                //Удаление позиций
                List<PositionForImport> PositionsDelete = PositionsProgram.Where(p => !PositionsNewList.Select(p2 => p2.Title)
                                                                                                   .Contains(p.Title))
                                                                          .ToList();
                foreach (PositionForImport item in PositionsDelete)
                {
                    db.PositionForImports.Remove(item);
                }
                db.SaveChanges();
                //Редактирование позиций
                List<PositionForImport> PositionsEdit = PositionsProgram.Join(PositionsNewList,
                                                                              p1 => p1.Title,
                                                                              p2 => p2.Title,
                                                                              (p1, p2) => p1.Edit(p2.Products.ToList()))
                                                                          .ToList();
                db.SaveChanges();
                //Создание позиций
                List<PositionForImport> PositionsCreate = PositionsNewList.Where(p => !PositionsProgram.Select(p2 => p2.Title)
                                                                                                   .Contains(p.Title))
                                                                      .ToList();
                foreach (PositionForImport item in PositionsCreate)
                {
                    db.PositionForImports.Add(item);
                }
                db.SaveChanges();

                ////Загрузка остатков

                //ResidueController Residues = new ResidueController();
                //Residues.Index();
            }

            TempData["list"] = ErrorList.ToList();
            return RedirectToAction("ErrorList", "Product");
        }

        private bool RusTxt(string name, int ind)
        {
            if ((int)name[ind] == 1105 || (int)name[ind] == 1025 || (int)name[ind] >= 1040 && (int)name[ind] <= 1103)
            {
                return (true);
            }
            else
            {

                return (false);
            }
        }

        private List<Guid> SearchGuids(bool FirstBool, List<AnalogsForImport> dbList, bool SecondBool, List<AnalogsForImport> ImportAnalogs)
        {
            List<Guid> List = new List<Guid>();
            List = dbList.Where(d => FirstBool ^ ImportAnalogs.Select(i => i.Сode1).Contains(d.Сode1))
                         .Where(d => SecondBool ^ ImportAnalogs.Select(i => i.Сode2).Contains(d.Сode2))
                         .Select(r => r.Id)
                         .ToList();
            List.Concat(dbList.Where(d => FirstBool ^ ImportAnalogs.Select(i => i.Сode2).Contains(d.Сode1))
                         .Where(d => SecondBool ^ ImportAnalogs.Select(i => i.Сode1).Contains(d.Сode2))
                         .Select(r => r.Id)
                         .ToList());
            return (List);
        }


        

        private void SerchCategory(string name, int ind)
        {
            while (RusTxt(name, ind))
            {
                ind = name.IndexOf(" ", ind++) + 1;
            }
        }
    }
}