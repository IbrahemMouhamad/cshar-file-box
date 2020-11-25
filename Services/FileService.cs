using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using file_box.Models;
using Microsoft.AspNetCore.Mvc;

namespace file_box.Service
{
    public class FileService
    {
        private readonly file_boxContext _context;

        public FileService(file_boxContext context)
        {
            _context = context;
        }

        public void Add(string title, string desc, string path)
        {
            File newFile = new File();
            newFile.Title = title;
            newFile.Description = desc;
            newFile.Path = path;
            newFile.CreatedAt = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _context.Add(newFile);
            _context.SaveChanges();
        }

        public File Get(int id)
        {
            return _context.File.FirstOrDefault(file => file.Id == id);
        }

        public IEnumerable<File> GetAll()
        {
            return _context.File;
        }

        public void Delete(File file)
        {
            _context.File.Remove(file);
            _context.SaveChanges();
        }
    }
}