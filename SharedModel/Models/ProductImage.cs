using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? Thumb { get; set; }
        public int ProductId { get; set; }

        [NotMapped]
        public byte[]? Buffer { get; set; }

    }
}
