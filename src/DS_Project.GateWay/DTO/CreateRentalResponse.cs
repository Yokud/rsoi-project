namespace DS_Project.GateWay.DTO
{
    public class CreateRentalResponse
    {
        public Guid RentalUid { get; set; }

        public string Status { get; set; }

        public DateOnly DateFrom { get; set; }

        public DateOnly DateTo { get; set; }

        public Guid CarUid { get; set; }

        public PaymentInfo Payment { get; set; }
    }
}
