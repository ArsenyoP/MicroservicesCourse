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
            Id = Random.Shared.Next();
            Name = name;
            Publisher = publisher;
            Cost = cost;
        }
    }
}
