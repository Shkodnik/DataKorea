using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Korea.Models.Domain;
using Korea.Models.HighlighterDb;

namespace Korea.Models
{
 
    public class KoreaContext : IdentityDbContext<ApplicationUser>
    {



        public KoreaContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<SupplierForImport> SupplierForImports { get; set; }
        public DbSet<CategoryForImport> CategoryForImports { get; set; }
        public DbSet<ProductForImport> ProductForImports { get; set; }
        public DbSet<AnalogsForImport> AnalogsForImports { get; set; }
        public DbSet<GenerationForImport> GenerationForImports { get; set; }
        public DbSet<BrandForImport> BrandForImports { get; set; }
        public DbSet<ModelForImport> ModelForImports { get; set; }
        public DbSet<PositionForImport> PositionForImports { get; set; }

        public DbSet<CodeSnippet> CodeSnippets { get; set; }

        public static KoreaContext Create()
        {
            return new KoreaContext();
        }
    }
}