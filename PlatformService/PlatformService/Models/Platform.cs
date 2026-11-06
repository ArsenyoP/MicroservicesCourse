namespace PlatformService.Models
{
    public class Platform
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Publisher { get; set; }
        public string Cost { get; set; }

        private Platform()
        {

        }
        public Platform(string name, string publisher, string cost)
        {
            Name = name;
            Publisher = publisher;
            Cost = cost;
        }
    }
}
