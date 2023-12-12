﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKit.DAL;
using PesKit.Models;
using PesKit.ViewModels;

namespace PesKit.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Blog> blog = await _context.Blogs.Include(b => b.Author).ToListAsync();
            List<Employee> employees = await _context.Employees.Include(e => e.Position).Take(4).ToListAsync();
            List<Project> projects = await _context.Projects.Include(pi => pi.ProjectImages).ToListAsync();
            List<Product> products = await _context.Products.ToListAsync();


            HomeVM homeVM = new HomeVM { Blogs = blog, Employees = employees, Projects = projects, Products = products };
            return View(homeVM);
        }

        public IActionResult ErrorPage(string error)
        {
            if (error == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(model: error);
        }

        public async Task<IActionResult> About()
        {
            List<Employee> employees = await _context.Employees.Include(e => e.Position).Take(4).ToListAsync();
            return View(employees);
        }
    }
}
