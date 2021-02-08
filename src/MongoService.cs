using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Faactory.Extensions.MongoDB
{
    internal class MongoService : IMongoService
    {
        private readonly MongoClient client;
        private readonly X509Certificate certificate;
        private readonly ILogger logger;
        
        public MongoService( ILoggerFactory loggerFactory, IOptions<MongoServiceOptions> optionsAccessor )
        {
            logger = loggerFactory.CreateLogger<MongoService>();

            var options = optionsAccessor.Value;

            var clientSettings = MongoClientSettings.FromConnectionString( options.Connection );

            if ( options.Certificate?.Any() == true )
            {
                certificate = new X509Certificate( options.Certificate );
                clientSettings.UseTls = true;
                clientSettings.AllowInsecureTls = false;
                clientSettings.SslSettings.ServerCertificateValidationCallback = ValidateServerCertficate;
            }

            client = new MongoClient( clientSettings );
        }

        public IMongoClient Client => client;

        private bool ValidateServerCertficate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool certMatch = false; // Assume failure

            switch( sslPolicyErrors )
            {
                case SslPolicyErrors.None:
                {
                    logger.LogDebug( "No validation errors - accepting certificate." );
                    certMatch = true;
                    break;
                }
                case SslPolicyErrors.RemoteCertificateChainErrors:
                {
                    logger.LogDebug( "Failed to validate certificate chain. Most likely a self-signed certificate." );

                    // It is a self-signed certificate, so chain length will be 1.
                    if ( chain.ChainElements.Count == 1 && chain.ChainStatus[0].Status == X509ChainStatusFlags.UntrustedRoot )
                    {
                        // This verifies that the issuer and serial number matches.
                        // You can also use a cryptographic hash, or match the two certificates byte by byte.
                        if ( certificate.Equals( cert ) )
                        {
                            logger.LogDebug( "The certificates match." );
                            certMatch = true;
                        }
                    }
                    break;
                }
                default:
                {
                    logger.LogError( "Name mismatch or remote-cert not available. Rejecting connection." );
                    break;
                }
            }
            
            return ( certMatch );
        }
    }
}
