namespace DS_Project.Rentals.DTO
{
    public class CreatePaidRentalRequest
    {
        public Guid CarUid { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public Guid PaymentUid { get; set; }
    }
}
