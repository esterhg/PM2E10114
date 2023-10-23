using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PM2E10114.Models
{
    public class Sitios
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [MaxLength(50)]
        public byte[] ImagenBytes { get; set; }

        [MaxLength(50)]
        public double Latitud { get; set; }
        [MaxLength(50)]
        public double Longitud { get; set; }
        [MaxLength(100)]
        public string Descripcion { get; set; }
    }
}
