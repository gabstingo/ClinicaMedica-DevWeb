using ClinicaMedica.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicaMedica.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Usuario> Logins { get; set; }

        public DbSet<Paciente> Paciente { get; set; }

        public DbSet<Consulta> Consulta { get; set; }
    }
}
