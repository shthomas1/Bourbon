public class Bourbon
{
    public int BourbonID { get; set; }
    public string Name { get; set; }
    public int DistilleryID { get; set; }
    public int? MashBillID { get; set; }
    public int? TypeID { get; set; }
    public decimal Proof { get; set; }
    public string FlavorNotes { get; set; }
    public int? Age { get; set; }
    public string PhotoUrl { get; set; }
    public bool Deleted { get; set; }
}
