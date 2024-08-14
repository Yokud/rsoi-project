namespace DS_Project.Cars.DTO
{
    public enum CarType
    {
        SEDAN,
        SUV,
        MINIVAN,
        ROADSTER
    }

    public class CarResponse
    {
        public Guid CarUid { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string RegistrationNumber { get; set; }

        public int Power { get; set; }

        public int Price { get; set; }

        public string Type { get; set; }

        public bool Available { get; set; }

        public static CarResponse FromCar(Car car)
        {
            return new CarResponse
            {
                CarUid = car.CarUid,
                Brand = car.Brand,
                Model = car.Model,
                RegistrationNumber = car.RegistrationNumber,
                Price = car.Price,
                Type = car.Type,
                Available = car.Availability,
                Power = car.Power ?? 0,
            };
        }
    }
}
