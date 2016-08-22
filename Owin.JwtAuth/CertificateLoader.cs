using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Owin.JwtAuth
{
    /// <summary>
    /// Loads X509 certificates from the Windows Certificate Store.
    /// </summary>
    public static class CertificateLoader
    {
        /// <summary>
        /// Loads a certificate from the machine-wide "Trusted People" store, identified by the subject name.
        /// </summary>
        public static X509Certificate2 BySubjectName(string subjectName)
        {
            var store = new X509Store(StoreName.TrustedPeople, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            try
            {
                return store.Certificates
                    .Find(X509FindType.FindBySubjectName, subjectName, validOnly: true)
                    .Cast<X509Certificate2>().FirstOrDefault();
            }
            finally
            {
                store.Close();
            }
        }
    }
}