using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Hygenus
{
    public class PoziomContext : DbContext
    {
        public DbSet<Poziom> Poziomy { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=127.0.0.1;Port=5432;Username=postgres;Password=postgres;Database=hygenus");
    }

    [Table("poziom")]
    public class Poziom
    {
        [Key]
        [Required]
        [Column("id")]
        public int id { get; set; }
        [Column("nazwa")]
        public string nazwa { get; set; }
        //public ICollection<Wynik> wynik { get; set; }
    }

    [Table("wynik")]
    public class Wynik
    {
        [Key]
        [Required]
        [Column("id")]
        public int id { get; set; }
        [Column("idPoziomu")]
        public Poziom poziom {get; set;}
        [Column("nick")]
        public string nick { get; set; }
        [Column("czas")]
        public double czas { get; set; }
    }
}
