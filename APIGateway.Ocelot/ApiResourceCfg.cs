namespace APIGateway.Ocelot
{
    public class ApiResourceCfg
    {
        public string AuthenticationScheme { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public string Audience { get; set; }
    }
}