using System.Security.Cryptography;
using System.Text;

namespace Hashing
{
    public class Encrypt
    {
       
            #region Metodi

            /// <summary>
            /// Hashing method
            /// </summary>
            /// <param name="data">data to hash such as password</param>
            /// <param name="salt">salt to add to the data associated with user</param>
            /// <returns>the hash of the combination between data and salt</returns>
            public string Hash(string data, string salt)
            {
                SHA256 sha256 = SHA256.Create();
                sha256.ComputeHash(Encoding.UTF8.GetBytes(data + salt));

                return Convert.ToBase64String(sha256.Hash);
            }


            /// <summary>
            /// Random salt generator 
            /// </summary>
            /// <returns>An 8 character string ending with =</returns>
            public string SaltGenerator()// RANDOM SALT GENERATOR
            {
                string salt = "";
                Random random = new Random();
                bool flag;
                int value;

                for (int i = 0; i < 7; i++)
                {
                    do
                    {
                        flag = false;
                        value = random.Next(33, 125);// TABELLA ASCII
                        if (value == 61)
                            flag = true;
                    }
                    while (flag);

                    salt += (char)value;// CONVERTIAMO IL VALORE INTERO IN ASCII
                }

                return salt + "=";// AGGIUNGIAMO ALLA STRINGA = stringadi 8 caratteri
            }

            #endregion
    }
    
}