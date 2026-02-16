namespace CorporateHelpdesk.Models
{
    public class SystemSettings
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; } = "Corporate Helpdesk";
        public bool EnableComments { get; set; } = true;
        public string DefaultPriority { get; set; } = "Średni";
        public int MaxTicketsPerUser { get; set; } = 10;
    }
}