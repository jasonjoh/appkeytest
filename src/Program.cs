using System.Security.Cryptography.X509Certificates;
using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

const string tenantId = "YOUR_TENANT_ID";
// Private key for both App A and App B, already set in Azure portal
const string keyFile = "../keys/AppKeyTestCert.pfx";
const string keyPassword = "";

// App A
// Client ID/App ID
const string clientId = "YOUR_APP_A_CLIENT_ID";

// App B (app to try to add a key to)
const string otherAppObjectId = "YOUR_APP_B_OBJECT_ID";

// New key to add
const string newKeyCerFile = "../keys/NewKey.cer";
const string newKeyPfxFile = "../keys/NewKey.pfx";

// Configure app-only auth using App A
var clientCert = new X509Certificate2(keyFile, keyPassword.ToCharArray());
var clientCertCredential = new ClientCertificateCredential(tenantId, clientId, clientCert);

var appContext = new TokenRequestContext(new[] { "https://graph.microsoft.com/.default" });
var appToken = await clientCertCredential.GetTokenAsync(appContext);
Console.WriteLine($"App token: {appToken.Token}");

using var httpClient = new HttpClient();
var graphClient = new GraphServiceClient(new TokenCredentialAuthProvider(clientCertCredential));
var serializer = graphClient.HttpProvider.Serializer;

try
{
    await AddKeyToApp(otherAppObjectId, newKeyCerFile, newKeyPfxFile, graphClient);
    Console.WriteLine("Key added to app successfully with app token");
}
catch (Exception ex)
{
    Console.WriteLine("Error adding key with app token:");
    Console.WriteLine(ex.Message);
}

static async Task AddKeyToApp(string appObjectId, string cerPath, string pfxPath, GraphServiceClient client)
{
    var newKey = new X509Certificate2(cerPath);

    var keyCredential = new Microsoft.Graph.KeyCredential
    {
        Usage = "Verify",
        Type = "AsymmetricX509Cert",
        Key = newKey.GetRawCertData()
    };

    var proof = GenerateProof(pfxPath, appObjectId);

    await client.Applications[appObjectId]
        .AddKey(keyCredential, proof)
        .Request()
        .PostAsync();
}

static string GenerateProof(string pfxPath, string objectId)
{
    var signingCert = new X509Certificate2(pfxPath, keyPassword);

    var audience = "00000002-0000-0000-c000-000000000000";
    var claims = new Dictionary<string, object>()
    {
        { "aud", audience },
        { "iss", objectId }
    };

    var securityTokenDescriptor = new SecurityTokenDescriptor
    {
        Claims = claims,
        NotBefore = DateTime.UtcNow,
        Expires = DateTime.UtcNow.AddMinutes(10),
        SigningCredentials = new X509SigningCredentials(signingCert)
    };

    var handler = new JsonWebTokenHandler();
    var proof = handler.CreateToken(securityTokenDescriptor);
    return proof;
}
