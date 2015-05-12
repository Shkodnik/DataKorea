using Hangfire;
using Korea.Models;
using Korea.Models.Domain;
using Korea.Models.HighlighterDb;
using Korea.Models.Xml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Korea.Controllers
{
    public class ResidueController : Controller
    {
        [HttpGet]
        public void Index()
        {
            string puth = Server.MapPath("../Content/Log/Log.txt");
            BackgroundJob.Enqueue(() => new ResidueController().Pars(puth));
        }
        


        //
        // GET: /Residue/
        public void Pars(string puth)
        {
            string lines = "";
            try
            {
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://hosting.effetto.ru/XML/Upload/Items.xml");
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                // This example assumes the FTP site uses anonymous logon.
                /////////////////////////////////////////////////////////////////////////

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);

                XElement root = XElement.Load(reader);
                List<string> Num =
                    (from el in root.Elements("Склад").Descendants("Количество")
                     select (string)el).Select(e => e.Trim()
                                                     .Replace("'", "")
                                                     .Replace(" ", "")
                                                     .Replace("-", ""))

                                       .ToList();
                List<string> Art =
                    (from el in root.Elements("Склад").Descendants("Номенклатура")
                     select (string)el).Select(e => e.Trim()
                                                     .Replace("'", "")
                                                     .Replace(" ", "")
                                                     .Replace("-", ""))

                                       .ToList();
                List<string> Cost =
                    (from el in root.Elements("Склад").Descendants("Цена")
                     select (string)el).Select(e => e.Trim()
                                                     .Replace("'", "")
                                                     .Replace(" ", "")
                                                     .Replace("-", ""))

                                       .ToList();



                List<ResidueModel> residues = new List<ResidueModel>();
                for (int i = 0; i < Art.Count(); ++i)
                {
                    ResidueModel residue = new ResidueModel();
                    ResidueModel residueOld = residues.FirstOrDefault(r => r.Article == Art[i]);
                    if (residueOld != null)
                    {
                        residues.FirstOrDefault(r => r.Article == Art[i]).Number += Convert.ToInt32(Num[i].Replace(" ", ""));
                    }
                    else
                    {
                        residue.Article = Art[i];
                        if (Num[i] != "") residue.Number = Convert.ToInt32(Num[i].Replace(" ", ""));
                        if (Cost[i] != "") residue.Cost = Convert.ToDouble(Cost[i].Replace(" ", ""));
                        residues.Add(residue);
                    }
                }




                using (KoreaContext db = new KoreaContext())
                {
                    List<ProductForImport> ProductForImports = db.ProductForImports.Include("Generations").ToList();


                    ProductForImports = ProductForImports.Join(residues,
                                           p => p.Сode,
                                           r => r.Article,
                                           (p, r) =>
                                           p.SetCostTale(
                                           r.Cost,
                                           r.Number)
                                           ).ToList();
                    //foreach (ProductForImport product in ProductForEdit)
                    //{
                    //    db.ProductForImports.Attach(product);
                    //    db.Entry(product).State = EntityState.Modified;
                    //}
                    db.SaveChanges();

                }



                //Заливка прямо в базу
                using (koreaEntities1 db = new koreaEntities1())
                {
                    List<Product> ImportProducts = db.Products.ToList();
                    List<Offer> OffersNew = new List<Offer>();

                    List<string> residuesART = residues.Select(r => r.Article.ToLower()).ToList();
                    List<string> productsART = ImportProducts.Select(i => i.ArtNo.ToLower()).ToList();

                    List<string> aga = residuesART.Where(r => productsART.Contains(r)).ToList();

                    ///Остатки и цена
                    List<Product> ProductsSave = new List<Product>();
                    foreach (ResidueModel List in residues)
                    {
                        Product TempProduct = ImportProducts.FirstOrDefault(p => p.ArtNo.ToLower() == List.Article.ToLower());
                        if (TempProduct != null)
                        {
                            Offer OfferTemp = new Offer()
                            {
                                OfferID = 0,
                                Size = null,
                                Amount = List.Number,
                                Price = List.Cost,
                                SupplyPrice = 0,
                                ColorID = null,
                                SizeID = null,
                                Main = true,
                                ArtNo = TempProduct.ArtNo,
                                ProductID = TempProduct.ProductId
                            };
                            OffersNew.Add(OfferTemp);
                        }
                    }
                    List<Offer> OffersSite = db.Offers.ToList();

                    OffersSite = OffersNew.Join(OffersSite,
                                                n => n.ProductID,
                                                s => s.ProductID,
                                                (n, s) => s.Edit(n))
                                           .ToList();
                    db.SaveChanges();
                    //    new Offer
                    //{
                    //    ProductID = n.ProductID,
                    //    ArtNo = n.ArtNo,
                    //    Main = n.Main,
                    //    SizeID = n.SizeID,
                    //    ColorID = n.ColorID,
                    //    SupplyPrice = n.SupplyPrice,
                    //    Amount = n.Amount,
                    //    Color = n.Color,
                    //    Price = n.Price,
                    //    Product = n.Product,
                    //    ShoppingCarts = n.ShoppingCarts,
                    //    Size = n.Size,

                    //    OfferID = s.OfferID,
                    //}

                    OffersSite = db.Offers.ToList();
                    List<Offer> OffersCreate = OffersNew.Where(n => !OffersSite.Select(s => s.ProductID)
                                                                              .Contains(n.ProductID))
                                                        .ToList();
                    foreach (Offer OfferCreate in OffersCreate)
                    {
                        db.Offers.Add(OfferCreate);
                    }
                    List<Offer> OffersDelete = OffersSite.Where(n => !OffersNew.Select(s => s.ProductID)
                                                                              .Contains(n.ProductID))
                                                         .Select(n => n.Empty())
                                                        .ToList();
                    db.SaveChanges();
                }


                reader.Close();
                response.Close();

                lines = DateTime.Now.ToString() + " Cool";
            }
            catch
            {
                lines = DateTime.Now.ToString() + " Error";
            }

            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(puth))
            {
                lines = sr.ReadToEnd() + "\n" + lines + "\n= = = = = = =\n";
            }

            using (StreamWriter outfile = new StreamWriter(puth))
            {
                outfile.WriteAsync(outfile.NewLine + lines);
            }
        }
	}
}