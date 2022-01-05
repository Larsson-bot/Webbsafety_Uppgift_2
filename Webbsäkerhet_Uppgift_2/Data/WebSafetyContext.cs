using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Webbsäkerhet_Uppgift_2.Models;

    public class WebSafetyContext : DbContext
    {
        public WebSafetyContext (DbContextOptions<WebSafetyContext> options)
            : base(options)
        {
        }

        public DbSet<Comment> Comment { get; set; }

        public DbSet<FileFromUser> FileFromUser { get; set; }
    }
