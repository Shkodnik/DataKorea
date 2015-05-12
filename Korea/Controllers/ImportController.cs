using Hangfire;
using Korea.Models;
using Korea.Models.Domain;
using Korea.Models.HighlighterDb;
using Korea.Models.Xls;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Mvc;
using UnidecodeSharpFork;

namespace Korea.Controllers
{
    [Authorize]
    public class ImportController : Controller
    {

        [HttpPost]
        public ActionResult Load()
        {
            using (KoreaContext _db = new KoreaContext())
            {
                List<CodeSnippet> Snippets = _db.CodeSnippets
                                            .Where(c => c.Info == "В работе")
                                            .ToList();
                if (Snippets.Count() == 0)
                {
                    CodeSnippet snippet = new CodeSnippet()
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        Info = "В работе"
                    };
                    _db.CodeSnippets.Add(snippet);
                    _db.SaveChanges();

                    string temp2 = Server.MapPath("../Content/Data");
                    BackgroundJob.Enqueue(() => new ImportController().RecordExport(temp2, snippet));
                    
                    return View(snippet);
                }
                else
                {
                    object massege = (Object)"Операция уже выполняется сервером";

                    DateTime timeNow = DateTime.UtcNow;
                    foreach (CodeSnippet item in Snippets) 
                    {
                        TimeSpan Difference = timeNow - item.CreatedAt;
                        if (Difference.Hours > 1.5)
                        {
                            item.Info = "Ошибка";
                            _db.SaveChanges();
                            massege = (Object)"Операция выполнялась больше полутора часов и была остановленна, выполните операцию снова";
                            return View("Message", massege);
                        }
                    }
                    return View("Message", massege);
                }

                
            }
        }

        [HttpGet]
        public ActionResult Message(string info)
        {
            object temp = (Object)info;
            return View(temp);
        }

        [HttpGet]
        public ActionResult CheckImports(string puth)
        {
            Guid Id = Guid.Parse(puth);
            using (KoreaContext _db = new KoreaContext())
            {
                CodeSnippet Snippet = _db.CodeSnippets.Single(c => c.Id == Id);
                switch (Snippet.Info)
                {
                    case "В работе":
                        return new HttpStatusCodeResult(102);
                    case "Готово":
                        return new HttpStatusCodeResult(200);
                    default:
                        return new HttpStatusCodeResult(409);
                        
                }
            }
            

        }

        
        /// <summary>
        /// Экспорт это а не импорт
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public void RecordExport(string puth, CodeSnippet snippet)
        {
            try
            {
                string puth2 = puth;
                int internalID = 1;
                List<CategoresTree> categorysLvUpTemp = new List<CategoresTree>();
                List<CategoresTree> categorysLvUp = new List<CategoresTree>();
                List<CategoresTree> CategoreTree = new List<CategoresTree>();
                List<SaveCategoryTree> saveTree = new List<SaveCategoryTree>();
                List<SaveCategoryTree> newTree = new List<SaveCategoryTree>();


                List<ModelForImport> ModelsProgram = new List<ModelForImport>();
                List<BrandForImport> BrandsProgram = new List<BrandForImport>();
                List<CategoryForImport> CategorysProgram = new List<CategoryForImport>();
                List<ProductForImport> ProductsProgram = new List<ProductForImport>();
                List<AnalogsForImport> Analogss = new List<AnalogsForImport>();
                List<FiltCategory> FiltCategorys = new List<FiltCategory>();
                List<Category> FullSiteCategory = new List<Category>();
                string pathDataProduct = puth + "\\Product.dat";

                using (koreaEntities1 db = new koreaEntities1())
                {
                    using (KoreaContext db2 = new KoreaContext())
                    {
                        ModelsProgram = db2.ModelForImports.ToList();
                        BrandsProgram = db2.BrandForImports.ToList();
                        CategorysProgram = db2.CategoryForImports.ToList();
                        ProductsProgram = db2.ProductForImports.Include("Generations").ToList();
                        Analogss = db2.AnalogsForImports.ToList();
                        CategorysProgram.Remove(CategorysProgram.FirstOrDefault(c => c.Title == "Без категории"));
                        FullSiteCategory = db.Categories.ToList();



                        //List<Category> cats = db.Categories.ToList();
                        //cats.Remove(cats.Single(c => c.CategoryID == 0));
                        //foreach (Category cat in cats)
                        //{
                        //    db.Database.ExecuteSqlCommand("DELETE FROM [korea].[Catalog].[Category] WHERE CategoryID = " + cat.CategoryID + ";");
                        //}
                        //db.Database.ExecuteSqlCommand("DELETE FROM [korea].[Catalog].[Category] WHERE CategoryID <> 0;");
                        //db.SaveChanges();
                    }
                }

                // создаем объект BinaryFormatter
                BinaryFormatter formatter = new BinaryFormatter();
                // десериализация из файла persons.dat
                string pathData = puth + "\\Items.dat";
                if (System.IO.File.Exists(pathData))
                {
                    using (FileStream fs = new FileStream(pathData, FileMode.OpenOrCreate))
                    {
                        saveTree = (List<SaveCategoryTree>)formatter.Deserialize(fs);
                    }
                }
                saveTree = saveTree.Join(FullSiteCategory,
                                         s => s.IdSiteCat,
                                         f => f.CategoryID,
                                         (s, f) => s)
                                   .ToList();


                //Создаём список задействованных категорий
                foreach (ProductForImport product in ProductsProgram)
                {
                    if (product.CategoryId != null && product.Generations.Count() != 0)
                    {
                        if (product.Category.CategoryId != null)
                        {
                            List<Guid> models = new List<Guid>();
                            foreach (GenerationForImport Generation in product.Generations)
                            {
                                models.Add(Generation.ModelId);
                            }
                            models.Distinct();

                            foreach (Guid item in models)
                            {
                                FiltCategory Filt = new FiltCategory();
                                Filt.Id = Guid.NewGuid();
                                Filt.Product = product.Id;
                                //Filt.Generation = Generation.Id;
                                Filt.Model = item;
                                Filt.Brand = ModelsProgram.Single(m => m.Id == item)
                                                      .BrandId;
                                Filt.Category = product.CategoryId;
                                Filt.CategoryParent = CategorysProgram.Single(c => c.Id == product.CategoryId)
                                                                 .CategoryId
                                                                 .GetValueOrDefault();
                                FiltCategorys.Add(Filt);
                                if (ModelsProgram.Single(m => m.Id == item)
                                                      .Title.Contains("Test1"))
                                {
                                    int i = 0; ///!@!
                                }
                            }
                        }
                    }
                }

                //Формируем каталог
                //1ый уровень
                foreach (BrandForImport brand in BrandsProgram)
                {
                    CategoresTree categoryTemp = new CategoresTree();
                    internalID++;
                    categoryTemp = new ImportController().RekordCategory(brand.Id, 
                                                                         brand.Title,
                                                                         brand.Weight,
                                                                         0,
                                                                         CategoreTree,
                                                                         internalID);
                    categorysLvUpTemp.Add(categoryTemp);

                    SaveCategoryTree newTreeTemp = new SaveCategoryTree()
                    {
                        IdCach = categoryTemp.Id,
                        IdProgCat = brand.Id,
                        IdParent = null,
                        IdProgParent = null,
                        IdSiteCat = -1,
                    };
                    SaveCategoryTree saveTreeTemp = saveTree.FirstOrDefault(s => s.IdProgCat == brand.Id);
                    newTreeTemp.Id = (saveTreeTemp != null) ? saveTreeTemp.Id : Guid.NewGuid();
                    newTree.Add(newTreeTemp);

                    categoryTemp.IdTree = newTreeTemp.Id;
                    CategoreTree.Add(categoryTemp);
                }
                categorysLvUp = categorysLvUpTemp;
                categorysLvUpTemp = new List<CategoresTree>();

                //2ый уровень
                foreach (ModelForImport model in ModelsProgram)
                {
                    foreach (CategoresTree exportcategory in categorysLvUp)
                    {
                        if (model.BrandId == exportcategory.IdProdram)
                        {
                            if (FiltCategorys.Where(f => f.Model == model.Id)
                                             .Count() != 0)
                            {
                                CategoresTree categoryTemp = new CategoresTree();
                                internalID++;
                                categoryTemp = new ImportController().RekordCategory(model.Id, 
                                                                                     model.Title,
                                                                                     model.Weight,
                                                                                     exportcategory.category.CategoryID, 
                                                                                     CategoreTree,
                                                                                     internalID);
                                categorysLvUpTemp.Add(categoryTemp);

                                SaveCategoryTree newTreeTemp = new SaveCategoryTree()
                                {
                                    IdCach = categoryTemp.Id,
                                    IdParent = exportcategory.IdTree,
                                    IdProgCat = model.Id,
                                    IdProgParent = exportcategory.IdProdram,
                                    IdSiteCat = -1,
                                };
                                SaveCategoryTree saveTreeTemp = saveTree.FirstOrDefault(s => s.IdProgCat == model.Id);
                                newTreeTemp.Id = (saveTreeTemp != null) ? saveTreeTemp.Id : Guid.NewGuid();
                                newTree.Add(newTreeTemp);

                                categoryTemp.IdTree = newTreeTemp.Id;
                                CategoreTree.Add(categoryTemp);
                            }
                        }
                    }
                }
                categorysLvUp = categorysLvUpTemp;
                categorysLvUpTemp = new List<CategoresTree>();

                //3ый уровень
                foreach (CategoryForImport category in CategorysProgram)
                {
                    foreach (CategoresTree exportcategory in categorysLvUp)
                    {
                        if (category.CategoryId == null)
                        {
                            if (FiltCategorys.Where(f => f.CategoryParent == category.Id)
                                             .Where(f => f.Model == exportcategory.IdProdram)
                                             .Count() != 0)
                            {
                                CategoresTree categoryTemp = new CategoresTree();
                                internalID++;
                                categoryTemp = new ImportController().RekordCategory(category.Id,
                                                                                     category.Title,
                                                                                     category.Weight,
                                                                                     exportcategory.category.CategoryID,
                                                                                     CategoreTree,
                                                                                     internalID);
                                categorysLvUpTemp.Add(categoryTemp);

                                SaveCategoryTree newTreeTemp = new SaveCategoryTree()
                                {
                                    IdCach = categoryTemp.Id,
                                    IdProgCat = category.Id,
                                    IdParent = exportcategory.IdTree,
                                    IdProgParent = exportcategory.IdProdram,
                                    IdSiteCat = -1,
                                };
                                SaveCategoryTree saveTreeTemp = saveTree.Where(s => s.IdProgCat == category.Id)
                                                                        .FirstOrDefault(s => s.IdProgParent == exportcategory.IdProdram);
                                newTreeTemp.Id = (saveTreeTemp != null) ? saveTreeTemp.Id : Guid.NewGuid();
                                newTree.Add(newTreeTemp);

                                categoryTemp.IdTree = newTreeTemp.Id;
                                CategoreTree.Add(categoryTemp);
                            }
                        }
                    }
                }
                categorysLvUp = categorysLvUpTemp;
                categorysLvUpTemp = new List<CategoresTree>();

                //4ый уровень
                foreach (CategoryForImport category in CategorysProgram)
                {
                    foreach (CategoresTree exportcategory in categorysLvUp)
                    {
                        if (category.CategoryId != null)
                        {
                            Guid ModelIdTree = newTree.FirstOrDefault(n => n.Id == exportcategory.IdTree)
                                                      .IdParent
                                                      .GetValueOrDefault();
                            Guid ModelId = newTree.FirstOrDefault(n => n.Id == ModelIdTree).IdProgCat;
                            if (FiltCategorys.Where(f => f.Category == category.Id)
                                             .Where(f => f.CategoryParent == exportcategory.IdProdram)
                                             .Where(f => f.Model == ModelId)
                                             .Count() != 0)
                            {
                                CategoresTree categoryTemp = new CategoresTree();
                                internalID++;
                                categoryTemp = new ImportController().RekordCategory(category.Id,
                                                                                     category.Title, 
                                                                                     category.Weight,
                                                                                     exportcategory.category.CategoryID, 
                                                                                     CategoreTree,
                                                                                     internalID);
                                categorysLvUpTemp.Add(categoryTemp);

                                SaveCategoryTree newTreeTemp = new SaveCategoryTree()
                                {
                                    IdCach = categoryTemp.Id,
                                    IdProgCat = category.Id,
                                    IdParent = exportcategory.IdTree,
                                    IdProgParent = exportcategory.IdProdram,
                                    IdSiteCat = -1,
                                };
                                SaveCategoryTree saveTreeTemp = saveTree.Where(s => s.IdProgCat == category.Id)
                                                                        .FirstOrDefault(s => s.IdParent == exportcategory.IdTree);
                                newTreeTemp.Id = (saveTreeTemp != null) ? saveTreeTemp.Id : Guid.NewGuid();
                                newTree.Add(newTreeTemp);

                                categoryTemp.IdTree = newTreeTemp.Id;
                                CategoreTree.Add(categoryTemp);
                            }
                        }
                    }
                }


                // Создание, изменение, удаление категорий на сайте
                using (koreaEntities1 db = new koreaEntities1())
                {
                    List<Category> CategorysDelete = new List<Category>();
                    List<Category> SiteCategory = db.Categories.ToList();
                    List<CategoresTree> CategorysTreeCreate = new List<CategoresTree>();

                    List<SaveCategoryTree> CategorysOnEdit = saveTree.Join(
                        newTree,
                        c1 => c1.Id,
                        c2 => c2.Id,
                        (c1, c2) => new SaveCategoryTree
                        {
                            Id = c1.Id,
                            IdCach = c2.IdCach,
                            IdParent = c1.IdParent,
                            IdProgCat = c1.IdProgCat,
                            IdSiteCat = c1.IdSiteCat
                        }
                        ).ToList();

                    SiteCategory = SiteCategory.Join(CategoreTree,
                                                     S => S.UrlPath,
                                                     C => C.category.UrlPath,
                                                     (S, C) => S.Edit(C.category)).ToList();
                    db.SaveChanges();
                    SiteCategory = db.Categories.ToList();



                    //saveTree = saveTree.Join(newTree,
                    //                         s => s.Id,
                    //                         n => n.Id,
                    //                         (s, n) => s)
                    //                   .ToList();

                    CategorysDelete = SiteCategory.Where(c => !CategoreTree.Select(n => n.category.UrlPath)
                                                                           .Contains(c.UrlPath))
                                                  .ToList();
                    CategorysDelete.AddRange(SiteCategory.Where(s => s.ParentCategory == null)
                                                            .ToList());
                    CategorysDelete.AddRange(
                        SiteCategory.Where(c => SiteCategory.Where(s => s.CategoryID == c.ParentCategory)
                                                            .Count() == 0 )
                                    .ToList());
                    CategorysDelete.AddRange(
                        SiteCategory.Where(c => SiteCategory.Where(s => s.UrlPath == c.UrlPath)
                                                            .Count() > 1)
                                    .Where(c => c.Total_Products_Count == 0)
                                    .ToList());
                    CategorysDelete = CategorysDelete.Distinct().ToList();
                    CategorysDelete.Remove(CategorysDelete.Single(c => c.CategoryID == 0));
                    foreach (Category categoryDelete in CategorysDelete)
                    {
                        db.Categories.Remove(categoryDelete);

                    }
                    db.SaveChanges();

                    SiteCategory = db.Categories.ToList();
                    saveTree = saveTree.Join(SiteCategory,
                                         s => s.IdSiteCat,
                                         f => f.CategoryID,
                                         (s, f) => s)
                                   .ToList();
                    List<SaveCategoryTree> saveTreeTemp = new List<SaveCategoryTree>();
                    List<Guid> CategorysCreateID = newTree.Where(c => !saveTree.Select(n => n.Id)
                                                                               .Contains(c.Id))
                                                          .Select(c => c.IdCach)
                                                          .ToList();
                    CategorysTreeCreate = CategoreTree.Where(n => CategorysCreateID.Contains(n.Id))
                                                      .ToList();
                    Dictionary<Guid, int> DicCategorysCreate = CategoreTree.ToDictionary(c => c.Id, c => c.category.CategoryID);
                    foreach (CategoresTree CategoryCreate in CategorysTreeCreate)
                    {
                        db.Categories.Add(CategoryCreate.category);
                        saveTreeTemp.Add(newTree.FirstOrDefault(n => n.IdCach == CategoryCreate.Id));
                    }
                    db.SaveChanges();
                    foreach (SaveCategoryTree TreeTemp in saveTreeTemp)
                    {
                        TreeTemp.IdSiteCat = CategorysTreeCreate.Single(c => c.Id == TreeTemp.IdCach).category.CategoryID;
                        saveTree.Add(TreeTemp);
                    }
                    CategorysTreeCreate = CategorysTreeCreate.Where(c => c.category.ParentCategory != 0).ToList();
                    foreach (CategoresTree CategoryCreate in CategorysTreeCreate)
                    {
                        Guid TempCategoryId = saveTree.Single(s => s.IdCach == CategoryCreate.Id)
                                                      .IdParent.GetValueOrDefault();
                        CategoryCreate.category.ParentCategory = saveTree.Single(s => s.Id == TempCategoryId)
                                                                         .IdSiteCat;
                    }

                    foreach (CategoresTree CategoryCreate in CategorysTreeCreate)
                    {
                        if (CategoryCreate.category.ParentCategory != 0)
                        {
                            db.Categories.Attach(CategoryCreate.category);
                            db.Entry(CategoryCreate.category).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();


                    SiteCategory = db.Categories.ToList();
                    saveTree = saveTree.Where(s => SiteCategory.Select(c => c.CategoryID)
                                                               .Contains(s.IdSiteCat))
                                       .ToList();


                }

                try
                {
                    // получаем поток, куда будем записывать сериализованный объект
                    using (FileStream fs = new FileStream(pathData, FileMode.OpenOrCreate))
                    {
                        formatter.Serialize(fs, saveTree);
                    }
                }
                catch
                {
                    System.IO.File.Delete(pathData);
                }


                // Заливаем продукты
                List<SaveProduct> saveProd = new List<SaveProduct>();
                List<SaveProduct> newProd = new List<SaveProduct>();
                List<int> ProductsSiteId = new List<int>();
                List<Product> ProductsSite = new List<Product>();
                List<ProductTree> ProductsForSite = new List<ProductTree>();
                // десериализация из файла Product.dat
                if (System.IO.File.Exists(pathDataProduct))
                {
                    using (FileStream fs = new FileStream(pathDataProduct, FileMode.OpenOrCreate))
                    {
                        saveProd = (List<SaveProduct>)formatter.Deserialize(fs);
                    }
                }

                using (koreaEntities1 db = new koreaEntities1())
                {
                    ProductsSiteId = db.Products.Select(p => p.ProductId)
                                                     .ToList();
                    ProductsSite = db.Products.ToList();

                    using (KoreaContext db2 = new KoreaContext())
                    {

                        List<Guid> ProductsProgramId = db2.PositionForImports.Select(p => p.Id)
                                                                             .ToList();
                        saveProd = saveProd.Join(ProductsSite,
                                             s => s.IdSiteProd,
                                             f => f.ProductId,
                                             (s, f) => s)
                                       .ToList();
                        saveProd = saveProd.Join(ProductsProgramId,
                                             s => s.IdProgProd,
                                             p => p,
                                             (s, p) => s)
                                       .ToList();
                    }

                    ProductsForSite = new ImportController().RekordProduct(ProductsProgram, ProductsForSite, Analogss);
                    newProd = new ImportController().RekordNewProd(ProductsForSite);

                    //Изменение товаров
                    ProductsSite = ProductsSite.Select(f => f.ArtNoTrim(' ')).ToList();
                    List<Product> ProductsEdit = ProductsSite.Join(ProductsForSite,
                                                                        f => f.ArtNo,
                                                                        n => n.product.ArtNo,
                                                                        (f, n) => f.Edit(n.product)).ToList();
                    db.SaveChanges();
                    saveProd = ProductsSite.Join(ProductsForSite,
                                                     f => f.ArtNo,
                                                     p => p.product.ArtNo,
                                                     (f, p) => new SaveProduct().Edit(f, p))
                                               .ToList();

                    //Удаление товаров
                    //List<int> ProductsDeleteId = saveProd.Where(s => !newProd.Select(n => n.IdProgProd)
                    //                                                      .Contains(s.IdProgProd)
                    //                                          )
                    //                                     .Select(s => s.IdSiteProd)
                    //                                     .ToList();
                    //ProductsDeleteId = ProductsDeleteId.Join(ProductsSiteId,
                    //                                     p1 => p1,
                    //                                     p2 => p2,
                    //                                     (p1, p2) => p1)
                    //                               .ToList();
                    //List<Product> ProductsDelete = ProductsSite.Where(f => ProductsDeleteId.Contains(f.ProductId)).ToList();
                    List<Product> ProductsDelete = ProductsSite.Where(p => !saveProd.Select(n => n.IdSiteProd)
                                                                                    .Contains(p.ProductId))
                                                               .ToList();
                    foreach (Product ProductDelete in ProductsDelete)
                    {
                        db.Products.Remove(ProductDelete);
                    }
                    db.SaveChanges();

                    //Создание товара
                    List<Guid> ProductsCreateId = newProd.Where(s => !saveProd.Select(n => n.IdProgProd)
                                                                              .Contains(s.IdProgProd))
                                                         .Select(s => s.IdCach)
                                                         .ToList();
                    List<SaveProduct> saveProdTemp = new List<SaveProduct>();
                    List<ProductTree> ProductsCreate = ProductsForSite.Join(ProductsCreateId,
                                                                            p1 => p1.Id,
                                                                            p2 => p2,
                                                                            (p1, p2) => p1)
                                                                      .Where(p => !ProductsSite.Select(f => f.ArtNo)
                                                                                                   .Contains(p.product.ArtNo))
                                                                      .ToList();
                    Dictionary<Guid, int> DicProductsCreate = ProductsCreate.ToDictionary(c => c.Id, c => c.product.ProductId);
                    saveProdTemp = newProd.Where(n => ProductsCreate.Select(p => p.Id)
                                                                    .Contains(n.IdCach))
                                          .ToList();
                    foreach (ProductTree ProductCreate in ProductsCreate)
                    {
                        db.Products.Add(ProductCreate.product);
                        db.SaveChanges();
                        //saveProdTemp.Add(newProd.FirstOrDefault(n => n.IdCach == ProductCreate.Id));
                    }
                    db.SaveChanges();

                    foreach (SaveProduct ProdTemp in saveProdTemp)
                    {
                        ProdTemp.IdSiteProd = ProductsCreate.Single(c => c.Id == ProdTemp.IdCach).product.ProductId;
                    }
                    saveProd = saveProd.Concat(saveProdTemp)
                                       .Distinct()
                                       .ToList();
                }

                try
                {
                    // получаем поток, куда будем записывать сериализованный объект
                    using (FileStream fs = new FileStream(pathDataProduct, FileMode.OpenOrCreate))
                    {
                        formatter.Serialize(fs, saveProd);
                    }
                }
                catch
                {
                    System.IO.File.Delete(pathDataProduct);
                }



                //Подключение категорий
                List<SaveProductCategory> SaveProductCategorys = new List<SaveProductCategory>();
                List<ProductCategory> newProdCateg = new List<ProductCategory>();
                List<ProductCategory> FullSiteProdCateg = new List<ProductCategory>();
                List<Guid> ProductWithModel = new List<Guid>();

                string pathDataCat = puth2 + "\\ProdCateg.dat";
                if (System.IO.File.Exists(pathDataCat))
                {
                    try
                    {
                        using (FileStream fs = new FileStream(pathDataCat, FileMode.OpenOrCreate))
                        {
                            SaveProductCategorys = (List<SaveProductCategory>)formatter.Deserialize(fs);
                        }
                    }
                    catch
                    {
                        System.IO.File.Delete(pathDataCat);
                        SaveProductCategorys = new List<SaveProductCategory>();
                    }
                }
                //List<SaveCategoryTree> saveTreeEdits = saveTree.Where(s => !s.IdParent.Equals(null)).ToList();
                using (KoreaContext db2 = new KoreaContext())
                {
                    using (koreaEntities1 db = new koreaEntities1())
                    {
                        ProductWithModel = db2.ProductForImports.Include("Generations")
                                                               .Where(p => p.Generations.Count() != 0)
                                                               .Select(p => p.Id)
                                                               .ToList();
                        FullSiteProdCateg = db.ProductCategories.ToList();
                        SaveProductCategorys = SaveProductCategorys.Join(FullSiteProdCateg,
                                                                         s => s.CategoryId.ToString() + s.ProductId.ToString(),
                                                                         f => f.CategoryID.ToString() + f.ProductID.ToString(),
                                                                         (s, f) => s)
                                       .ToList();
                    }
                }
                saveProd = saveProd.Join(ProductWithModel,
                                                   f => f.IdProgProd,
                                                   p => p,
                                                   (f, p) => f)
                                             .ToList();

                foreach (FiltCategory FiltCategore in FiltCategorys)
                {
                    Guid thread = saveTree.Single(s => s.IdProgCat == FiltCategore.Brand)
                                          .Id;
                    thread = saveTree.Single(s => s.IdProgCat == FiltCategore.Model
                                             && s.IdParent == thread)
                                     .Id;
                    thread = saveTree.Single(s => s.IdProgCat == FiltCategore.CategoryParent
                                             && s.IdParent == thread)
                                     .Id;
                    int categoryTemp = saveTree.Single(s => s.IdProgCat == FiltCategore.Category
                                                       && s.IdParent == thread)
                                               .IdSiteCat;
                    int productTemp = saveProd.FirstOrDefault(s => s.IdProgProd == FiltCategore.Product).IdSiteProd;
                    ProductCategory newProdCategTemp = new ProductCategory()
                    {
                        CategoryID = categoryTemp,
                        ProductID = productTemp,
                        SortOrder = 0,
                        Main = false,
                    };
                    if (newProdCateg.Where(n => n.CategoryID == newProdCategTemp.CategoryID
                                           && n.ProductID == newProdCategTemp.ProductID)
                                    .Count() == 0)
                    {
                        newProdCateg.Add(newProdCategTemp);
                    }
                };

                using (koreaEntities1 db = new koreaEntities1())
                {
                    //db.Database.ExecuteSqlCommand("DELETE FROM [korea].[Catalog].[ProductCategories] WHERE CategoryID <> 0");
                    //db.SaveChanges();  // Знакомтесь: кастыль
                    FullSiteProdCateg = db.ProductCategories.ToList();

                    //Удаление связи категория-товар
                    List<ProductCategory> DeleteProdCategs = FullSiteProdCateg.Where(f => newProdCateg.Where(n => n.ProductID == f.ProductID
                                                                                                             && n.CategoryID == f.CategoryID)
                                                                                                      .Count() == 0)
                                                                              .ToList();

                    foreach (ProductCategory DeleteProdCateg in DeleteProdCategs)
                    {
                        db.ProductCategories.Remove(DeleteProdCateg);
                    }
                    db.SaveChanges();

                    //Создание связи категория-товар
                    List<ProductCategory> CreateProdCategs = newProdCateg.Where(f => FullSiteProdCateg.Where(n => n.ProductID == f.ProductID
                                                                                                             && n.CategoryID == f.CategoryID)
                                                                                                      .Count() == 0)
                                                                         .ToList();

                    foreach (ProductCategory CreateProdCateg in CreateProdCategs)
                    {
                        db.ProductCategories.Add(CreateProdCateg);
                    }
                    db.SaveChanges();
                }



                //Загрузка бренда
                using (KoreaContext db2 = new KoreaContext())
                {
                    using (koreaEntities1 db = new koreaEntities1())
                    {
                        List<SupplierForImport> Suppliers = new List<SupplierForImport>();
                        Suppliers = db2.SupplierForImports.ToList();

                        SaveBrand save = new SaveBrand();
                        List<SaveBrand> saveBrands = save.ReadSave(puth2);
                        List<SaveBrand> newSaveBrands = new List<SaveBrand>();

                        List<Brand> newBrands = new List<Brand>();
                        int i = 1;
                        foreach (SupplierForImport item in Suppliers)
                        {
                            Brand Brand = new Brand()
                            {
                                BrandID = i,
                                BrandName = item.Title,
                                BrandSiteUrl = item.Title.Unidecode(),
                                SortOrder = 0,
                                Enabled = true
                            };
                            newBrands.Add(Brand);
                            i += 1;

                            SaveBrand SaveBrand = new SaveBrand()
                            {
                                Id = Guid.NewGuid(),
                                IdProgProd = item.Id,
                                IdCach = Brand.BrandID,
                                Value = item.Title,
                            };
                            newSaveBrands.Add(SaveBrand);
                        }

                        List<Brand> siteBrands = new List<Brand>();
                        siteBrands = db.Brands.ToList();

                        //Удаление
                        //Списко не совподающих значений с новым спмском
                        List<Brand> BrandsDelete = siteBrands.Where(p => !newBrands.Select(n => n.BrandName)
                                                                                   .Contains(p.BrandName)).ToList();
                        //Списко не совподающих значений с сохранёнными значениями
                        BrandsDelete = BrandsDelete.Concat(siteBrands.Where(p => saveBrands.Where(s => s.IdSiteProd == p.BrandID
                                                                                                   && s.Value == p.BrandName)
                                                                                           .Count() == 0)
                                                                     .ToList())
                                                   .Distinct()
                                                   .ToList();

                        foreach (Brand item in BrandsDelete)
                        {
                            db.Brands.Remove(item);
                        }
                        db.SaveChanges();

                        //Создание
                        siteBrands = db.Brands.ToList();
                        List<Brand> BrandCreate = newBrands.Where(p => !siteBrands.Select(n => n.BrandName)
                                                                                  .Contains(p.BrandName)).ToList();

                        Dictionary<Brand, SaveBrand> CreateBrandsDic = new Dictionary<Brand, SaveBrand>();

                        foreach (Brand item in BrandCreate)
                        {
                            CreateBrandsDic.Add(item,
                                                 newSaveBrands.FirstOrDefault(n => n.IdCach == item.BrandID));
                            db.Brands.Add(item);
                        }
                        db.SaveChanges();

                        foreach (Brand b in CreateBrandsDic.Keys)
                        {
                            CreateBrandsDic[b].IdSiteProd = b.BrandID;
                            saveBrands.Add(CreateBrandsDic[b]);
                        }
                        /*foreach (KeyValuePair<Brand, SaveBrand> itemDic in CreateBrandsDic)
                        {
                            SaveBrand temp = itemDic.Value;
                            temp.IdSiteProd = itemDic.Key.BrandID;
                            saveBrands.Add(temp);
                        }*/
                        siteBrands = db.Brands.ToList();
                        saveBrands = saveBrands.Where(s => siteBrands.Select(p => p.BrandID)
                                                                     .Contains(s.IdSiteProd))
                                               .Distinct()
                                               .ToList();
                        save.Saving(saveBrands, puth2);


                        //подключение бренда к товарам
                        //Загруска списка id'шников продуктов
                        SaveProduct saveProduct = new SaveProduct();
                        saveProd = saveProduct.ReadSave(puth2)
                                              .Distinct()
                                              .ToList();

                        //Загруска списка продуктов имеющие соответствующие свойства
                        ProductForImport productForImport = new ProductForImport();
                        Dictionary<Guid, Guid> ProductsBrandProgId = productForImport.DicBrand();
                        Product product = new Product();
                        product.ConnectBrand(ProductsBrandProgId, saveBrands, saveProd);

                    }
                }


                //Подключение свойств
                ////Удаление значений свойст и связеей без самих свойств (чистка)
                Property PropertyTemp = new Property();
                //PropertyTemp.Сleaning();

                string Name = "Поколение";
                new ImportController().ConnectProperty(Name, pathDataProduct, puth);

                Name = "Позиция";
                new ImportController().ConnectProperty(Name, pathDataProduct, puth);

                ResidueController a = new ResidueController();
                a.Index();

                using (KoreaContext _db = new KoreaContext())
                {
                    CodeSnippet SnippetEnd = _db.CodeSnippets.Single(c => c.Id == snippet.Id);
                    SnippetEnd.FinishedAt = DateTime.UtcNow;
                    SnippetEnd.Info = "Готово";
                    _db.SaveChanges();
                }
            }
            catch
            {
                using (KoreaContext _db = new KoreaContext())
                {
                    CodeSnippet SnippetEnd = _db.CodeSnippets.Single(c => c.Id == snippet.Id);
                    SnippetEnd.FinishedAt = DateTime.UtcNow;
                    SnippetEnd.Info = "Ошибка";
                    _db.SaveChanges();
                }
            }
        }

        ////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////


        public CategoresTree RekordCategory(Guid Id2, string Name, int Weight, int Parent, List<CategoresTree> CategoreTree, int internalID)
        {
            Category categortTemp = new Category()
            {
                CategoryID = internalID,
                Name = Name,
                Description = Name,
                SortOrder = Weight,
                Enabled = true,
                Total_Products_Count = 0,
                ParentCategory = Parent,
                DisplayStyle = "true",
                DisplayChildProducts = false,
                HirecalEnabled = true,
                DisplayBrandsInMenu = true,
                DisplaySubCategoriesInMenu = true,
            };
            categortTemp.UrlPath = UrlGen(Name, Parent, CategoreTree);
            categortTemp.UrlPath = categortTemp.UrlPath.Unidecode();
            categortTemp.UrlPath = categortTemp.UrlPath.Replace("'", "")
                                                       .Replace(" ", "_")
                                                       .Replace("-", "_");
            CategoresTree exportcategory = new CategoresTree()
            {
                Id = Guid.NewGuid(),
                IdProdram = Id2,
                category = categortTemp,
            };
            exportcategory.category.BriefDescription = "";
            internalID++;

            return (exportcategory);
        }

        public string UrlGen(string Name, int Parent, List<CategoresTree> CategoreTree)
        {
            string url = "";

            url = url + Name;
            if (Parent != 0)
            {
                CategoresTree CategoreTemp = CategoreTree.FirstOrDefault(c => c.category
                                                                               .CategoryID == Parent);
                url = UrlGen(CategoreTemp.category.Name
                             , CategoreTemp.category.ParentCategory
                                                    .GetValueOrDefault()
                             , CategoreTree
                             ) + "_" + url;
            }
            return (url);
        }

        public List<ProductTree> RekordProduct(List<ProductForImport> ProductsProgram, List<ProductTree> ProductsForSite, List<AnalogsForImport> Analogs)
        {
            foreach (ProductForImport product in ProductsProgram)
            {

                Product exportProduct = new Product();
                ProductTree ProductTemp = new ProductTree()
                {
                    Id = Guid.NewGuid(),
                    IdProdram = product.Id
                };
                exportProduct.ArtNo = product.Сode.Trim(' ', ',', '-');

                while (ProductsForSite.Where(e => e.product.ArtNo == exportProduct.ArtNo).Count() != 0)
                {
                    exportProduct.ArtNo += "!SVP!";
                }
                exportProduct.Name = product.Name;
                exportProduct.Ratio = 0;
                exportProduct.Discount = 0;
                exportProduct.Weight = 0;
                exportProduct.Enabled = true;
                exportProduct.Recomended = false;
                exportProduct.New = false;
                exportProduct.Bestseller = false;
                exportProduct.OnSale = false;
                exportProduct.AllowPreOrder = false;
                exportProduct.SortBestseller = 0;
                exportProduct.SortNew = 0;
                exportProduct.SortDiscount = 0;
                exportProduct.UrlPath = exportProduct.ArtNo.Unidecode();
                exportProduct.UrlPath = exportProduct.UrlPath.Replace("'", "")
                                                             .Replace(" ", "_")
                                                             .Replace("-", "_");
                exportProduct.SalesNote = "";
                exportProduct.Unit = "";
                exportProduct.ShippingPrice = 0;
                exportProduct.Multiplicity = 1;
                exportProduct.HasMultiOffer = false;
                exportProduct.SpecialCat = false;
                exportProduct.CategoryEnabled = product.CategoryId != null ? true : false;
                exportProduct.DateAdded = DateTime.Today;
                exportProduct.DateModified = exportProduct.DateAdded;
                exportProduct.BriefDescription = CreateAnalog(product.Сode, Analogs);

                ProductTemp.product = exportProduct;
                ProductsForSite.Add(ProductTemp);
            }
            return (ProductsForSite);
        }

        public string CreateAnalog(string Сode, List<AnalogsForImport> Analogs)
        {
            string url = "";
            List<string> AnalogsTemp = Analogs.Where(a => a.Сode1 == Сode)
                                              .Select(a => a.Сode2)
                                              .ToList();
            AnalogsTemp = AnalogsTemp.Concat(Analogs.Where(a => a.Сode2 == Сode)
                                                   .Select(a => a.Сode1)
                                                   .ToList())
                                     .ToList();
            AnalogsTemp = AnalogsTemp.Distinct()
                                        .ToList();
            if (AnalogsTemp.Count() != 0)
            {
                url = "Аналоги: ";
                foreach (string Analog in AnalogsTemp)
                {
                    string AnalogTemp = Analog.Unidecode();
                    AnalogTemp = AnalogTemp.Replace("'", "")
                                           .Replace(" ", "_")
                                           .Replace("-", "_");
                    url = url + "<a href='products/" + AnalogTemp + "'>" + Analog + "</a> <br />";
                }
            }

            return (url);
        }

        public List<SaveProduct> RekordNewProd(List<ProductTree> ProductsForSite)
        {
            List<SaveProduct> Products = new List<SaveProduct>();
            foreach (ProductTree product in ProductsForSite)
            {
                SaveProduct ProductTemp = new SaveProduct()
                {
                    Id = Guid.NewGuid(),
                    IdCach = product.Id,
                    IdProgProd = product.IdProdram,
                    IdSiteProd = -1
                };
                Products.Add(ProductTemp);
            }
            return (Products);
        }


        ////Создание свойств и их подключение
        public void ConnectProperty(string Name, string pathDataProduct, string puth)
        {
            //Создание свойств
            Property SiteProperty = new Property(); //Наше свойство 
            SiteProperty = SiteProperty.Create(Name);

            

            //Создание значений свойств
            //Загрузка списка сохранённых id'шников
            SavePropertyValue savePropertysValue = new SavePropertyValue();
            List<SavePropertyValue> SavePropertysValue = savePropertysValue.ReadSave(Name, puth)
                                                                           .ToList();
            


            //Загрузка значений из программы
            Dictionary<Guid, string> PrograbBase = new Dictionary<Guid, string>();
            PropertyValue propertyValue = new PropertyValue();
            PrograbBase = propertyValue.LoadValues(Name);

            

            //Запись данных в формат сайта и списка id'шников
            Dictionary<PropertyValue, SavePropertyValue> PropertysValueDic = propertyValue.RecordValues(PrograbBase, SiteProperty);

            ////!@!
            //Dictionary<PropertyValue, SavePropertyValue> TestPPropertysValueDic = PropertysValueDic.Where(p => p.Key
            //                                                                                                    .Value == "Три")
            //                                                                                       .ToDictionary(p => p.Key, p => p.Value);


            //Добавление и удаление значений свойств
            SavePropertysValue = propertyValue.CreateAndDelete(PropertysValueDic, SavePropertysValue, SiteProperty);

            //Сохранение списка сохранённых id'шников
            savePropertysValue.Saving(SavePropertysValue, Name, puth);

            ////!@!
            //SavePropertyValue TestSavePropertyValue = SavePropertysValue.FirstOrDefault(s => TestPPropertysValueDic.Select(t => t.Value.Id)
            //                                                                                                       .Contains(s.Id));


            //подключение поколеней к товарам
            //Загруска списка id'шников продуктов
            List<SaveProduct> saveProd = new SaveProduct().ReadSave(puth)
                                                          .ToList();

            //Загруска списка продуктов имеющие соответствующие свойства
            List<ProductForImport> ProductsProgram = new ProductForImport().ListPropertys(Name)
                                                                           .ToList();

            //Создание пар свойство товар
            List<ProductPropertyValue> NewProductsProperty = new ProductPropertyValue().ProductsProperty(Name, SavePropertysValue, ProductsProgram, saveProd)
                                                                                       .ToList();


            //Добавление и удаление пар товар-свойство
            new ProductPropertyValue().CreateAndDelete(NewProductsProperty, SiteProperty.PropertyID);
        }

    }



}