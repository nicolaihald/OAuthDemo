using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OAuthDemo.Models
{

    public class Client
    {
        private static readonly Dictionary<string, Client> Clients;

        static Client()
        {
            Clients = new Dictionary<string, Client>();
            Clients.Add("client1", new Client { AllowedOrigin = "*", Name = "Demo Client 1",ApplicationType = ApplicationTypes.JavaScript,Id = "1", Active = true,Secret = "TopGun1"});
            Clients.Add("client2", new Client { AllowedOrigin = "*", Name = "Demo Client 2",ApplicationType = ApplicationTypes.JavaScript,Id = "2", Active = true,Secret = "TopGun2"});
        }

        public string Id { get; set; }
        [Required]
        public string Secret { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public ApplicationTypes ApplicationType { get; set; }
        public bool Active { get; set; }
        public int RefreshTokenLifeTime { get; set; }
        [MaxLength(100)]
        public string AllowedOrigin { get; set; }

        public static Client FindClient(string clientId)
        {
            Client client;
            Clients.TryGetValue(clientId, out client);
            return client;

        }
    }

    public enum ApplicationTypes
    {
        JavaScript = 0,
        NativeConfidential = 1
    };
}