namespace ChatApp.Data.Models
{
    public class Contact
    {
        public long Id { get; set; }
        public string ContactUserId { get; set; }
        public ApplicationUser ContactUser { get; set; }
        public string OwnerUserId { get; set; }
        public ApplicationUser OwnerUser { get; set; }
        //public string? Name { get; set; }
        //public string? Icon {  get; set; }
    }
}
