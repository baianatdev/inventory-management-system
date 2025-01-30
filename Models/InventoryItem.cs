namespace InventorySystem.Models{
    public class InventoryItem{
        public int Id{get; set;}
        public string Name{get; set;}
        public string Description{get; set;}
        public int Quantity{get; set;}
        public decimal Price{get; set;}
        public string ImageUrl{get; set;} // for storing item image url
    }
}
