using System;

namespace Lib
{
     public class AppSecrets 
     {
        public string BaseAddress { get; set; }
        public string JwtSigningKey {get;set;}
        public string StripeSecretKey { get; set; }
        public string StripePublishableKey { get; set; }
        public string PriceId { get; set; }
    }
}
