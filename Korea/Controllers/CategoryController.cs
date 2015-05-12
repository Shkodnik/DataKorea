using Korea.Models;
using Korea.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Korea.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        /// <summary>
        /// Get list of categories by parent Id
        /// </summary>
        /// <param name="id">Parent category id or null if root</param>
        public ActionResult Index(Guid? id)
        {
            using (KoreaContext db = new KoreaContext())
            {
                if (id.HasValue)
                    ViewBag.CategoryId = id.Value;
                return View(GetCategoryList());
            }
        }

        /// <summary>List
        /// Entire category store cache
        /// </summary>
        private List<CategoryForImport> categories = null;
        Random rng = new Random();

        /// <summary>
        /// Recurent function for category tree building
        /// </summary>
        /// <param name="parentId">Id of current level</param>
        /// <param name="depth">Depth of current level</param>
        /// <returns>IEnumerable of Category</returns>
        private IEnumerable<CategoryForImport> RecurentBuildCategoriesTree(Guid? parentId, int depth)
        {
            //initiate result store
            List<CategoryForImport> result = new List<CategoryForImport>();

            //calculate visual '=' shift
            string shift = "";
            for (int i = 0; i < depth; i++)
                shift += "=";

            depth++;

            //iterate current childrens
            foreach (CategoryForImport category in categories.Where(c => c.CategoryId == parentId))
            {
                //compose title
                category.Title = shift + " " + category.Title + " (" + category.Weight + ")";

                //store category
                result.Add(category);

                //and all it's children
                int index = result.IndexOf(category);
                result.InsertRange(index + 1, RecurentBuildCategoriesTree(category.Id, depth));
            }
            return result;
        }

        /// <summary>
        /// Fill entire category cache
        /// </summary>
        private void FillCategoryCache()
        {
            using (KoreaContext db = new KoreaContext())
            {
                //store categories to cache
                categories = db.CategoryForImports
                    .OrderBy(c => c.Weight)
                    .ThenBy(c => c.Title)
                    .ToList();

            }
        }

        /// <summary>
        /// Category select list tree building
        /// </summary>
        /// <param name="selectedId">Selected value</param>
        /// <returns></returns>
        private SelectList GetCategorySelectList(Guid? selectedId)
        {
            FillCategoryCache();

            //return select list
            SelectList result = new SelectList(RecurentBuildCategoriesTree(null, 0), "Id", "Title", selectedId);

            return result;
        }

        /// <summary>
        /// Category tree building
        /// </summary>
        /// <returns>IEnumerable of category</returns>
        private IEnumerable<CategoryForImport> GetCategoryList()
        {
            FillCategoryCache();

            return RecurentBuildCategoriesTree(null, 0);
        }

        /// <summary>
        /// Select proper parent category
        /// </summary>
        private void FillCategories()
        {
            //determinate selected category
            string categoryIdString = Request.QueryString["CategoryId"];
            if (!string.IsNullOrWhiteSpace(categoryIdString))
            {
                ViewData["CategoryId"] = GetCategorySelectList(new Guid(categoryIdString));
            }
            else
            {
                ViewData["CategoryId"] = GetCategorySelectList(null);
            }
        }

        /// <summary>
        /// Create category stub
        /// </summary>
        public ActionResult Create()
        {
            FillCategories();
            return View(
                new CategoryForImport()
                {
                    Weight = 0
                }
            );
        }

        /// <summary>
        /// Validate and save category data
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(CategoryForImport category)
        {
            try
            {
                using (KoreaContext db = new KoreaContext())
                {
                    if (null != db.CategoryForImports.FirstOrDefault(c => c.Title == category.Title))
                    {
                        //Console.WriteLine("Ошибка!!");
                        FillCategories();
                        return View();
                    }
                    else
                    {
                        category.Id = Guid.NewGuid();
                        category.ExtId = -1;
                        db.CategoryForImports.Add(category);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                FillCategories();
                return View();
            }
        }

        /// <summary>
        /// Get category for edit
        /// </summary>
        /// <param name="id">Category's Id</param>
        public ActionResult Edit(Guid id)
        {
            FillCategories();
            using (KoreaContext db = new KoreaContext())
            {
                return View(
                    db.CategoryForImports.FirstOrDefault(c => c.Id == id)
                    );
            }
        }

        /// <summary>
        /// Validate and update category
        /// </summary>
        /// <param name="category">Category for update</param>
        [HttpPost]
        public ActionResult Edit(CategoryForImport category)
        {
            try
            {
                using (KoreaContext db = new KoreaContext())
                {
                    if (category.Id != db.CategoryForImports.FirstOrDefault(c => c.Title == category.Title).Id)
                    {
                        //Console.WriteLine("Ошибка!!");
                        FillCategories();
                        return View();
                    }
                    else
                    {
                        CategoryForImport temp = db.CategoryForImports.FirstOrDefault(c => c.Id == category.Id);
                        temp.CategoryId = category.CategoryId;
                        temp.Title = category.Title;
                        temp.Weight = category.Weight;
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                FillCategories();
                return View();
            }
        }

        /// <summary>
        /// Make delete question
        /// </summary>
        /// <param name="id">Category's Id for delete</param>
        public ActionResult Delete(Guid id)
        {
            using (KoreaContext db = new KoreaContext())
            {
                return View(
                    db.CategoryForImports.FirstOrDefault(c => c.Id == id)
                    );
            }
        }

        /// <summary>
        /// Make delete on post action
        /// </summary>
        [HttpPost]
        public ActionResult Delete(FormCollection form)
        {
            try
            {
                if (form["Title"] != "Без категории")
                {
                    Guid id = new Guid(form["Id"]);

                    using (KoreaContext db = new KoreaContext())
                    {
                        new CategoryForImport().DeleteCategory(id);
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    throw new Exception(); 
                }
            }
            catch
            {
                return View();
            }
        }
    }
}
