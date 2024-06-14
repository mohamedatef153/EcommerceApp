using Azure.Core;
using DatabaseAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModelClasses;
using ModelClasses.ViewModels;

namespace webApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }



        public IActionResult Index()
        {
            var products = _context.products.Include(m => m.Category).Include(t => t.ImgUrl).ToList();
            return View(products);
        }
        public IActionResult Details(int id)
        {

            ProductViewModel vm = new ProductViewModel
            {
                Product = _context.products.Include(m => m.Category).FirstOrDefault(p => p.Id == id),


            };
            vm.Product.ImgUrl = _context.images.Where(u => u.ProductId == id).ToList();
            if (vm.Product.ImgUrl != null)
            {
                return View(vm);

            }
            else { return Content("image is null "); }

        }
        public async Task<IActionResult> Update(int id)
        {
            if (id == 0)
            {
                return Content($"{id}");
            }

            ViewBag.Cat = await _context.Categories.ToListAsync();
            ProductViewModel vm = new ProductViewModel
            {
                Product = await _context.products.Include(m => m.Category).FirstOrDefaultAsync(p => p.Id == id),


            };
            vm.Product.ImgUrl = await _context.images.Where(u => u.ProductId == id).ToListAsync();
            return View(vm);



        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Update(ProductViewModel vm)
        {
            try
            {
                ViewBag.Cat = _context.Categories.ToList();

                if (vm.Product.Id == 0)
                {
                    return NotFound("Product ID is zero. Possible form submission issue.");
                }

                var productToEdit = _context.products.FirstOrDefault(u => u.Id == vm.Product.Id);
                if (productToEdit == null)
                {
                    return NotFound($"Product with ID {vm.Product.Id} not found.");
                }

                productToEdit.Name = vm.Product.Name;
                productToEdit.Price = vm.Product.Price;
                productToEdit.Discreption = vm.Product.Discreption;
                productToEdit.CategoryId = vm.Product.CategoryId;

                if (vm.Images != null)
                {
                    foreach (var item in vm.Images)
                    {
                        string tempFileName = item.FileName;

                        if (!tempFileName.Contains("Home"))
                        {
                            string stringFileName = UploadFiles(item);
                            var addressImage = new PImages
                            {
                                ImageUrl = stringFileName,
                                ProductId = vm.Product.Id,
                                ProductName = vm.Product.Name
                            };
                            _context.images.Add(addressImage);
                        }
                        else if (string.IsNullOrEmpty(productToEdit.HomeImgUrl))
                        {
                            string homeImgUrl = UploadFiles(item);
                            productToEdit.HomeImgUrl = homeImgUrl;
                        }
                    }
                }

                _context.products.Update(productToEdit);
                _context.SaveChanges();

                return RedirectToAction("Index", "Product");
            }
            catch (Exception ex)
            {
                // Log the exception (you can use any logging framework or simply write to a file)
                Console.WriteLine(ex);
                return BadRequest("An error occurred while updating the product.");
            }
        }
        public IActionResult Create()
        {
            ViewBag.Cat = _context.Categories.ToList();
            ProductViewModel vm = new ProductViewModel()
            {
                Inventories = new Inventory(),
                PImages = new PImages(),
                /*  CategoriesList = _context.Categories.ToList().Select(u => new SelectListItem
                  {
                      Text = u.Name,
                      Value = u.Id.ToString(),
                  }),*/

            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel vm)
        {

            string homeImageUrl = "";
            if (vm.Images != null)
            {
                foreach (var image in vm.Images)
                {
                    homeImageUrl = image.FileName;
                    if (homeImageUrl.Contains("Home"))
                    {
                        homeImageUrl = UploadFiles(image);
                        break;
                    }
                }
            }
            vm.Product.HomeImgUrl = homeImageUrl;
            await _context.AddAsync(vm.Product);
            await _context.SaveChangesAsync();

            var newProduct = await _context.products.Include(c => c.Category).FirstOrDefaultAsync(u => u.Name == vm.Product.Name);
            vm.Inventories.Name = newProduct.Name;
            vm.Inventories.Category = newProduct.Category.Name;

            await _context.inventories.AddAsync(vm.Inventories);
            await _context.SaveChangesAsync();

            if (vm.Images != null)
            {
                foreach (var image in vm.Images)
                {
                    string tempFileName = image.FileName;
                    if (!tempFileName.Contains("Home"))
                    {
                        string stringFileName = UploadFiles(image);
                        var addressImage = new PImages
                        {
                            ImageUrl = stringFileName,
                            ProductId = newProduct.Id,
                            ProductName = newProduct.Name
                        };
                        await _context.images.AddAsync(addressImage);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Product");


            ViewBag.Cat = await _context.Categories.ToListAsync();
            //  return View(vm);
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            if (id != 0) {
                var productToDelete = _context.products.FirstOrDefault(p => p.Id == id);
                var imagesToDelete = _context.images.Where(p => p.Id == id).Select(u => u.ImageUrl);
                foreach (var image in imagesToDelete)
                {
                    string imageUrl = "Images\\" + image;
                    var toDeleteFromFolder = Path.Combine(_environment.WebRootPath, imageUrl.TrimStart('\\'));
                    DeleteAImage(toDeleteFromFolder);
                }
                if (productToDelete.HomeImgUrl != "")
                {
                    string imageUrl = "Images\\" + productToDelete.HomeImgUrl;
                    var toDeleteFromFolder = Path.Combine(_environment.WebRootPath, imageUrl.TrimStart('\\'));
                    DeleteAImage(toDeleteFromFolder);
                }
                _context.products.Remove(productToDelete);
                _context.SaveChanges();
            }
            else
            {
                return Json(new { success = false, message = "failed to delete the image" });
            }


            return Json(new { success = true, message = "deleted Successfuly" });
        }

        public IActionResult DeleteImg(string Id)
        {
            int routeId = 0;
            if(Id != null)
            {
                if (!Id.Contains("Home"))
                {
                    var ImageToDeleteFromPImage = _context.images.FirstOrDefault(u=>u.ImageUrl==Id);
                    if(ImageToDeleteFromPImage != null)
                    {
                        routeId = ImageToDeleteFromPImage.ProductId;
                        _context.images.Remove(ImageToDeleteFromPImage);
                    }
                }
                else
                {
                    var ImageToDeleteFromProduct = _context.products.FirstOrDefault(u=>u.HomeImgUrl==Id);
                    if (ImageToDeleteFromProduct != null) {
                        ImageToDeleteFromProduct.HomeImgUrl = "";
                        routeId = ImageToDeleteFromProduct.Id;
                        _context.products.Update(ImageToDeleteFromProduct);
                    }
                }
                string ImageUrl = "Images\\" + Id;
                var toDeleteImageFromFolder = Path.Combine(_environment.WebRootPath, ImageUrl);
                DeleteAImage(toDeleteImageFromFolder);
                _context.SaveChanges();
                return Json(new { success = true, message = "Pic was Deleted Successfuly", id = routeId });

            }
            return Json(new { success = false, message = "Faild to delete" });
        }


        private void DeleteAImage(string toDeleteFromFolder)
        {
            if(System.IO.File.Exists(toDeleteFromFolder))
            {
                System.IO.File.Delete(toDeleteFromFolder);
            }
        }

        private string UploadFiles(IFormFile image)
        {
            string fileName = null;
            if (image != null)
            {
                string uploadDirLocation = Path.Combine(_environment.WebRootPath, "Images");
                fileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                string filePath = Path.Combine(uploadDirLocation, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }
            }
            return fileName;
        }
    }
}
