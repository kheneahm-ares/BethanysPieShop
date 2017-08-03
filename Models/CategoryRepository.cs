using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BethanysPieShop.Models
{
    public class CategoryRepository : ICategoryRepository
    {
        private AppDbContext _appDbContext;
        public CategoryRepository(AppDbContext context)
        {
            _appDbContext = context;
        }


        public IEnumerable<Category> Categories => _appDbContext.Categories;
    }
}
