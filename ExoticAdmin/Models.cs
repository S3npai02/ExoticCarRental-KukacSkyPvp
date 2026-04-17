using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ExoticAdmin
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string ExteriorColor { get; set; }
        public string Interior { get; set; }
        public int Year { get; set; }
        public int Weight { get; set; }
        public int Doors { get; set; }
        public string WheelStyle { get; set; }
        public string Powertrain { get; set; }
        public string Transmission { get; set; }
        public int Hp { get; set; }
        public int Torque { get; set; }
        public string Acceleration { get; set; }
        public int TopSpeed { get; set; }
        public string Extras { get; set; }
        public string Drive { get; set; }
        public decimal PricePerDay { get; set; }
        public string Description { get; set; }
        public int TimesRented { get; set; }
        public int Status { get; set; }
        public string ImageUrl { get; set; }

        public DateTime? MotExpiry { get; set; }
        public int CurrentMileage { get; set; }
        public string MaintenanceNotes { get; set; }

        public string StatusName => Status == 1 ? "✔️ Elérhető" : Status == 2 ? "🚗 Kiadva" : "🔧 Szervizben";
        public string StatusColor => Status == 1 ? "#4CAF50" : Status == 2 ? "#D4A325" : "#F44336";
        public string DisplayName => $"{Brand} {Model} ({Year})";
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string LicenseNumber { get; set; }
        public int Clearance { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        public bool IsVerified { get; set; }
        public bool IsDriver { get; set; }
        public bool IsBanned { get; set; }
        public string AdminNotes { get; set; }
        public byte[] ProfilePictureData { get; set; }
        public ImageSource ProfileImage
        {
            get
            {
                if (ProfilePictureData == null || ProfilePictureData.Length == 0) return null;
                try
                {
                    using (var ms = new MemoryStream(ProfilePictureData))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = ms;
                        image.EndInit();
                        image.Freeze();
                        return image;
                    }
                }
                catch { return null; }
            }
        }
        public string RoleName
        {
            get
            {
                if (Clearance == 3) return "Admin";
                if (IsDriver) return "Sofőr";
                if (IsVerified && !string.IsNullOrWhiteSpace(LicenseNumber)) return "Felhasználó";
                return "Vendég";
            }
        }

        public string VerifiedText => IsBanned ? "🚫 TILTÓLISTÁN" : (IsVerified ? "✔ Hitelesítve" : "✖ Nincs hitelesítve");
        public string VerifiedColor => IsBanned ? "#FF6B6B" : (IsVerified ? "#4CAF50" : "#F44336");
    }

    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int VehicleId { get; set; }
        public string CustomerName { get; set; }
        public string VehicleName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }

        public string StatusName => Status == 1 ? "Függőben" : Status == 2 ? "Aktív" : "Befejezve";
        public string StatusIcon => Status == 1 ? "⏳" : Status == 2 ? "●" : "✔️";

        public SolidColorBrush StatusColor
        {
            get
            {
                switch (Status)
                {
                    case 1: return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A90E2"));
                    case 2: return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));
                    case 3: return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4B4B"));
                    default: return Brushes.Gray;
                }
            }
        }
    }

    public class ChartItem
    {
        public string Label { get; set; }
        public double Value { get; set; }
        public double Width { get; set; }
        public string DisplayValue => $"{Value:N0} Ft";
    }
}