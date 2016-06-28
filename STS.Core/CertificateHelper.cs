using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace STS.Core
{
    public static class CertificateHelper
    {

        public static X509Certificate2 GetCertificate(StoreName name, StoreLocation location, X509FindType findType, object findValue)
        {
            X509Store store = new X509Store(name, location);
            X509Certificate2Collection found = null;
            store.Open(OpenFlags.ReadOnly);

            try
            {
                found = store.Certificates.Find(findType, findValue, true);

                if (found.Count == 0)
                    throw new Exception(string.Format("No certificate was found matching the specified criteria. Type: {0} Value: {1}",findType,findValue));


                if (found.Count > 1 && findType == X509FindType.FindBySubjectName)
                {
                    X509Certificate2Collection foundBuffer = new X509Certificate2Collection();
                    foreach (var item in found)
                    {
                        if (item.Subject == string.Format("CN={0}",findValue) )
                        {
                            foundBuffer.Add(item);
                        }
                    }
                    found = foundBuffer;
                }

                if (found.Count > 1)
                    throw new ArgumentException("There are more than one certificate matching the specified criteria.");

                return new X509Certificate2(found[0]);
            }
            finally
            {
                if (found != null)
                {
                    foreach (X509Certificate2 cert in found)
                    {
                        cert.Reset();
                    }
                }

                store.Close();
            }
        }


        public static X509Certificate2 GetCertificate(Assembly assembly, string resourceName, string password = null)
        {
            if (assembly != null)
            {
                var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, (int)stream.Length);
                    X509Certificate2 certificate = null;
                    if (password != null)
                    {
                        certificate = new X509Certificate2(data, password);
                    }
                    else
                    {
                        certificate = new X509Certificate2(data);
                    }
                    return certificate;
                }
            }
            return null;
        }

        public static X509Certificate2 GetCertificate(string assemblyName, string resourceName, string password=null)
        {
            if (assemblyName == null) return null;

            var assembly = FindAssembly(assemblyName);

            if (assembly == null) return null;

            return GetCertificate(assembly,resourceName, password);
        }


        /// <summary>
        /// Sign data using a certificate. Warning: the certificate must be reloaded!
        /// </summary>
        /// <param name="cert"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SignData(X509Certificate2 cert, string data)
        {
            using (var csp = (RSACryptoServiceProvider)cert.PrivateKey)
            {
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    var toSign = Encoding.UTF8.GetBytes(data);
                    var dataHash = sha1.ComputeHash(toSign);
                    return Convert.ToBase64String(csp.SignHash(dataHash, CryptoConfig.MapNameToOID("SHA1")));
                }
            }
        }

        /// <summary>
        /// Verify sign data using a certificate. Warning: the certificate must be reloaded!
        /// </summary>
        /// <param name="cert"></param>
        /// <param name="data"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        public static bool VerifySingedData(X509Certificate2 cert, string data, string signature)
        {
            var singatureData = Convert.FromBase64String(signature);
            using (RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key)
            {
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    var toSign = Encoding.UTF8.GetBytes(data);
                    var dataHash = sha1.ComputeHash(toSign);
                    return csp.VerifyHash(dataHash, CryptoConfig.MapNameToOID("SHA1"), singatureData);
                }
            }
        }

        private static Assembly FindAssembly(string name)
        {
            if (name == null) return null;
            var aList = AppDomain.CurrentDomain.GetAssemblies();
            var assembly = aList.FirstOrDefault(a => a.FullName == name);
            return assembly;
        }
    }


}
