using BethanysPieShop.Models;
using BethanysPieShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BethanysPieShop.Controllers
{
    public class PieDataController : Controller
    {
        private IPieRepository _pieRepository;
        private ICategoryRepository _categoryRepository;

        public PieDataController(IPieRepository pieRepo, ICategoryRepository catRepo)
        {
            _pieRepository = pieRepo;
            _categoryRepository = catRepo;
        }

        [Route("api/[controller]/{category}")]
        public IEnumerable<PieViewModel> LoadMorePies(string category)
        {
            string currentCategory = string.Empty;

            IEnumerable<Pie> dbPies = null;

                if (!category.Equals("All"))
                {

                    currentCategory = _categoryRepository.Categories.FirstOrDefault(c => c.CategoryName == category).CategoryName;

                    int categoryId = _categoryRepository.Categories.FirstOrDefault(c => c.CategoryName == category).CategoryId;

                    dbPies = _pieRepository.Pies.Where(p => p.Category.CategoryId == categoryId)
                       .OrderBy(p => p.PieId);
                }
                else
                {
                
                    dbPies = _pieRepository.Pies.OrderBy(p => p.PieId).Take(10);
                }
            

            List<PieViewModel> pies = new List<PieViewModel>();

            foreach (var dbPie in dbPies)
            {
                //convert the pies returned from the database to pieview models
                pies.Add(MapDbPieToPieViewModel(dbPie));
            }
            return pies;
        }

        private PieViewModel MapDbPieToPieViewModel(Pie dbPie)
        {
            return new PieViewModel()
            {
                PieId = dbPie.PieId,
                Name = dbPie.Name,
                Price = dbPie.Price,
                ShortDescription = dbPie.ShortDescription,
                ImageThumbnailUrl = dbPie.ImageThumbnailUrl
            };
        }
    }
}
