using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class MashBill{
    public int MashBillID { get; set; }
    public string Name { get; set; }
    public decimal CornPercentage { get; set; }
    public decimal RyePercentage { get; set; }
    public decimal BarleyPercentage { get; set; }
    public decimal? WheatPercentage { get; set; }
    public bool Deleted { get; set; }
    }
}