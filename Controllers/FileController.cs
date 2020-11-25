using System.Linq;
using Microsoft.AspNetCore.Mvc;
using file_box.Service;
using file_box.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.AspNetCore.Identity;

namespace file_box.Controllers
{
    public class FileController : Controller
    {
        private readonly ActionlogService _logger;
        private readonly FileService _fileService;
        private SignInManager<IdentityUser> _userManager;

        public FileController(ActionlogService logger, FileService fileService, SignInManager<IdentityUser> userManager)
        {
            // for log user action to database
            _logger = logger;
            // for handle file crud operation
            _fileService = fileService;
            // for get login user
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            // get all files
            var allfiles = _fileService.GetAll();
            // to list
            var model = allfiles.ToList();
            // pass data to view (Index.cshtml)
            return View(model);
        }

        public IActionResult Create()
        {
            // for get request
            // just pass new createfile object to view file (Create.cshtml)
            return View();
        }

        [HttpPost]
        public IActionResult Create(string title, string description, IFormFile file)
        {
            // do other validations on  model as needed
            try {
                if (file != null)
                {
                    // create unique file name
                    var uniqueFileName = GetUniqueFileName(file.FileName);
                    // path to uploaded files folder
                    var uploads = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(),"wwwroot/files");
                    // the file path
                    var filePath = System.IO.Path.Combine(uploads,uniqueFileName);
                    // copy the file from memory to that folder
                    using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                        file.CopyTo(fileStream);
                    //file.CopyTo(new System.IO.FileStream(filePath, System.IO.FileMode.Create));
                    //Save model to db table 
                    _fileService.Add(title, description, "files/" + uniqueFileName);
                    // log user action
                    if(_userManager.IsSignedIn(User))
                        _logger.Add(
                            User.Identity.Name,
                            Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                            "Add new file (title: "+title+")"
                            );
                    else 
                        _logger.Add(
                            "",
                            Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                            "Add new file (title: "+title+")"
                            );
                }
            }
            catch (Exception ) {
                // just pass new createfile object to view file (Create.cshtml)
                return View();
            }
            // just pass new createfile object to view file (Create.cshtml)
            return View();
        }

        public IActionResult Delete(int id)
        {
            try {
                // get the file database object
                File deletedFile = _fileService.Get(id);
                if( deletedFile != null) {
                    // get the file location in the file system
                    var filePath = System.IO.Path.Combine(
                        System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(),"wwwroot/"),
                        deletedFile.Path);
                    var fileInfo = new System.IO.FileInfo(filePath);
                    // delete the file from the file system
                    fileInfo.Delete();
                    // delete the file object from database
                    _fileService.Delete(deletedFile);
                    // log user action
                    if(_userManager.IsSignedIn(User))
                        _logger.Add(
                            User.Identity.Name,
                            Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                            "remove file (title: "+deletedFile.Title+")"
                            );
                    else 
                        _logger.Add(
                            "",
                            Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                            "remove file (title: "+deletedFile.Title+")"
                            );
                }
            }
            catch (Exception ) {}
             // redirect to index page 
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public IActionResult Download(int id)
        {
            try {
                // get the file database object
                File requestedFile = _fileService.Get(id);
                if( requestedFile != null) {
                    // get the file location in the file system
                    var filePath = System.IO.Path.Combine(
                        System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(),"wwwroot/"),
                        requestedFile.Path);
                    // get the file content
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    // log user action
                    if(_userManager.IsSignedIn(User))
                        _logger.Add(
                            User.Identity.Name,
                            Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                            "download file (title: "+requestedFile.Title+")"
                            );
                    else 
                        _logger.Add(
                            "",
                            Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                            "download file (title: "+requestedFile.Title+")"
                            );
                    // return it to the user
                    return File(fileBytes, "APPLICATION/octet-stream", requestedFile.Title+System.IO.Path.GetExtension(filePath));
                }
            }
            catch (Exception ) {}
            // redirect to index page 
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public IActionResult Search(string q)
        {
            //filter by search string
            if (!String.IsNullOrEmpty(q))  
            {  
               var model =  _fileService.GetAll().Where(s => s.Description.Contains(q)).ToList();  
               // pass data to view (Search.cshtml)
                return View(model);
            }
             // redirect to index page 
            return Redirect(Request.Headers["Referer"].ToString());
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = System.IO.Path.GetFileName(fileName);
            return  System.IO.Path.GetFileNameWithoutExtension(fileName)
                      + "_" 
                      + Guid.NewGuid().ToString().Substring(0, 4) 
                    + System.IO.Path.GetExtension(fileName);
        }
    }
}
