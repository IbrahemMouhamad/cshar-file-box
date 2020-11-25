using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using file_box.Models;
using Microsoft.AspNetCore.Mvc;

namespace file_box.Service
{
    public class ActionlogService
    {
        private readonly file_boxContext _context;

        public ActionlogService(file_boxContext context)
        {
            _context = context;
        }

        public void Add(string user, string remote, string message)
        {
            Actionlog newLog = new Actionlog();
            newLog.User = user;
            newLog.UserRemote = remote;
            newLog.Message = message;
            newLog.Time = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _context.Add(newLog);
            _context.SaveChanges();
        }

        public IEnumerable<Actionlog> GetAll()
        {
            return _context.Actionlog;
        }
    }
}