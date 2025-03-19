using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Distillery{
    public int DistilleryID { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public DateTime? EstablishedDate { get; set; }
    public string Description { get; set; }
    public bool Deleted { get; set; }
    }
}