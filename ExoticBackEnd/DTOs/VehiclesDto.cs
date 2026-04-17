namespace ExoticBackend.DTOs
{
    public class VehicleDto
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Category { get; set; }
        public string Drivetrain { get; set; }
        public string Transmission { get; set; }
        public string EngineType { get; set; }
        public string Fuel {  get; set; }
        public int Status { get; set; }

        public int? Year { get; set; }
        public decimal PricePerDay { get; set; }
        public int Times_Rented { get; set; }
        public List<VehicleImageDto> Images { get; set; } = new();
    }
}
