namespace Doctor_CLinic_API.DTO
{
    public class CreatePatientDTO
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsFirstVisit { get; set; } = true;
    }
}
