namespace Board
{
    public class Tenant
    {
        public string name { get; set; }
        public string title { get; set; }
        public string hdoj { get; set; }
    }

    public class BoardOptions
    {
        public Tenant[] Tenants { get; set; }

        public string[] Password { get; set; }

        public bool AffiliationLogo { get; set; }
    }
}
