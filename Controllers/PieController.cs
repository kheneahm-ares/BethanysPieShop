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
    public class PieController : Controller
    {
        private IPieRepository _pieRepository;
        private ICategoryRepository _categoryRepository;


        //constructor dependency injection
        public PieController(IPieRepository pieRepository, ICategoryRepository categoryRepository)
        {
            _pieRepository = pieRepository;
            _categoryRepository = categoryRepository;
        }

        public ViewResult List(string category)
        {
            IEnumerable<Pie> pies;
            string currentCategory = string.Empty;

            //if no category is listed then grab all pies
            if (string.IsNullOrEmpty(category))
            {
                pies = _pieRepository.Pies.OrderBy(p => p.PieId);
                currentCategory = "All";
            }
            else
            {
                    
                    currentCategory = _categoryRepository.Categories.FirstOrDefault(c => c.CategoryName == category).CategoryName;

                    int categoryId = _categoryRepository.Categories.FirstOrDefault(c => c.CategoryName == category).CategoryId;

                    pies = _pieRepository.Pies.Where(p => p.Category.CategoryId == categoryId)
                       .OrderBy(p => p.PieId);

                
            }

            ViewBag.CurrentCategory = currentCategory;


            return View(new PiesListViewModel
            {
                Pies = pies,
                CurrentCategory = currentCategory
            });
        }
        public IActionResult Details(int id)
        {
            var pie = _pieRepository.GetPieById(id);
            if (pie == null)
                return NotFound();

            return View(pie);
        }
    }
}
