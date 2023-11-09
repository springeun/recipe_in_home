using Microsoft.AspNetCore.Mvc;
using recipe_in_home.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace recipe_in_home.Controllers
{
    public class PostController : Controller
    {
        private List<Post> list;
        private readonly Csharp_Post_services postService;
        private readonly IWebHostEnvironment _env;

        public PostController(IWebHostEnvironment env)
        {
            _env = env;
            string connString = "Server=" + "127.0.0.1" +
                                ";Database=" + "recipe_members" +
                                ";port=" + "3306" +
                                ";user=" + "root" +
                                ";password=" + "0125";
            postService = new Csharp_Post_services(connString);
        }

        public IActionResult Index()
        {
            list = postService.Getpost();
            return View(list);
        }

        public IActionResult Details(int id)
        {
            var post = postService.SelectPost(id);
            return View(post);
        }
        public IActionResult Create()
        {
            return View();
        }

        public ActionResult Createpost(IFormCollection form)
        {
            var member_id = form["member_id"].ToString();
            var title = form["Title"].ToString();
            var content = form["Content"].ToString();
            var imageDataFile = form.Files["ImageData"];

            byte[] imageData = null; // 기본값으로 null을 설정
            string fileName = null;
            if (imageDataFile != null && imageDataFile.Length > 0)
            {
                imageData = ReadImageAsByteArray(imageDataFile);

                var imagePath = Path.Combine(_env.WebRootPath, "images");

                var fileExtension = Path.GetExtension(imageDataFile.FileName);
                fileName = Guid.NewGuid().ToString().Replace("-", "") + fileExtension;
                var filePath = Path.Combine(imagePath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageDataFile.CopyTo(stream);
                }
            }

            int result = postService.InsertPost(member_id, title, content, fileName, imageData);

            TempData["result"] = result;
            return View();
        }
        public IActionResult Edit(int id)
        {
            var post = postService.GetPostById(id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }
        public IActionResult EditPost(int Postid, IFormCollection form)
        {
            //var Postid = form["Postid"].ToString();
            var member_id = form["member_id"].ToString();
            var title = form["Title"].ToString();
            var content = form["Content"].ToString();
            var imageDataFile = form.Files["ImageData"];

            byte[] imageData = null; // 기본값으로 null을 설정
            string fileName = null;

            if (imageDataFile != null && imageDataFile.Length > 0)
            {
                // 이미지 파일이 있는 경우에만 처리
                imageData = ReadImageAsByteArray(imageDataFile);

                var imagePath = Path.Combine(_env.WebRootPath, "images");

                var fileExtension = Path.GetExtension(imageDataFile.FileName);
                fileName = Guid.NewGuid().ToString().Replace("-", "") + fileExtension;
                var filePath = Path.Combine(imagePath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageDataFile.CopyTo(stream);
                }
            }

            int result = postService.UpdatePost(Postid, member_id, title, content, fileName);
            TempData["result"] = result;

            if (result == 1)
            {
                Console.WriteLine("수정 성공");
            }
            else
            {
                Console.WriteLine("수정 실패");
            }

            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id)
        {
            var Post = postService.GetPostById(id);
            if (Post == null)
            {
                return NotFound();
            }
            return View(Post);
        }
        public IActionResult DeleteConfirmed(int id)
        {
            var Post = postService.GetPostById(id);
            if (Post == null)
            {
                return NotFound();
            }

            postService.DeletePost(id);

            return RedirectToAction("Index");
        }
        private string SaveImageAndGetPath(IFormFile imageDataFile)
        {
            var uploads = Path.Combine(_env.WebRootPath, "images");
            var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(imageDataFile.FileName);
            var imagePath = Path.Combine(uploads, fileName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                imageDataFile.CopyTo(fileStream);
            }

            return imagePath;
        }

        private byte[] ReadImageAsByteArray(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                return stream.ToArray();
            }
        }

    }
}





