using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
//using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MotorSQL.Data
{
    public class ApplicationDbContext: DbContext
    {
        //private readonly string _connectionString;
        public ApplicationDbContext( DbContextOptions <ApplicationDbContext> options): base(options)
        {
            //_connectionString = configuration.GetConnectionString("ConSQL");
        }

       
    }
}
