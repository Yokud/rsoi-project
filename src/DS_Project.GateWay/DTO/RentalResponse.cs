namespace DS_Project.GateWay
{
    public class RentalResponse
    {
        public Guid RentalUid { get; set; }

        public string Status { get; set; }

        public DateOnly DateFrom { get; set; }

        public DateOnly DateTo { get; set; }

        public object Car { get; set; }

        public object Payment { get; set; }
    }
}
