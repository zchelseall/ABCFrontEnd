using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.IO;

namespace ABCSolutionsWPF
{
    /// <summary>
    /// Interaction logic for FormSubmit.xaml
    /// </summary>
    public partial class FormSubmit : Window
    {
        public List<Grades> grades;
        public List<Dictionary<string, string>> schools;
        public Dictionary<string, string> dictSchools;
        public BlockChainClient client;

        public FormSubmit()
        {
            InitializeComponent();
            init();
        }
        
        private void init()
        {
            grades = new List<Grades>();
            /*grades.Add(new Grades()
            {
                CourseID = "111111111",
                CourseName = "作弊101",
                Credit = "0",
                Mark = "-50"
            });*/
            dgGrades.ItemsSource = grades;

            dictSchools = new Dictionary<string, string>
            {
                {"Tsinghua University","i94fcba8aa927150a1df622c9c5328036087594f3"},
                {"University of Waterloo","i2404d1fc87e790eb390f8a077554538c78e05c49"}
            };

            this.client = BlockChainClient.GetInstance();
            this.cbSchools.ItemsSource = dictSchools;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*
            string msgtext = "确认提交？";
            string txt = "提交成绩";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBox.Show(msgtext, txt, button);
            MessageBoxResult result = MessageBox.Show(msgtext, txt, button);

            if (result == MessageBoxResult.No) return;
            */

            Credentials cred = new Credentials();
            cred.Name = this.tbName.Text;
            cred.School = this.cbSchools.SelectedValue as string;
            cred.StudID = this.tbStudID.Text;
            cred.Term = this.tbTerm.Text;
            cred.transcript = grades;

            string serialized = JsonConvert.SerializeObject(cred);
            string key;
            var ciphertext = Crypto.Encode(Encoding.UTF8.GetBytes(serialized), out key);
            string guid = Guid.NewGuid().ToString("n");
            FormPrivateKey formPrivateKey = new FormPrivateKey(guid, key);
            formPrivateKey.Show();
            var msg = this.client.sendTranscript(cred.School, guid, ciphertext);
            MessageBox.Show(msg);
        }

        private void CallAPI()
        {

        }

        /*
        private void encrypt(string original)
        {
            try
            {
                using (Aes myAes = Aes.Create())
                {
                    // Encrypt the string to an array of bytes.
                    byte[] encrypted = EncryptStringToBytes_Aes(original,
                    myAes.Key, myAes.IV);
                    // Decrypt the bytes to a string.
                    //string roundtrip = DecryptStringFromBytes_Aes(encrypted,
                    myAes.Key, myAes.IV);
                    //Display the original data and the decrypted data.
                    Console.WriteLine("Original: {0}", original);
                    Console.WriteLine("Round Trip: {0}", roundtrip);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }*/

        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key
                , aesAlg.IV);
                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt
                    , encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(
                        csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
        /*
        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key
, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key
                , aesAlg.IV);
                {
                    // Read the decrypted bytes from the decrypting
                    stream
                    // and place them in a string.
                    plaintext = srDecrypt.ReadToEnd();
                }
            }
        }
    }
return plaintext;
}*/



public class Credentials
        {
            public string Name { get; set; }
            public string School { get; set; }
            public string StudID { get; set; }
            public string Term { get; set; }
            public List<Grades> transcript { get; set; }
        }

        public class Grades
        {
            public string CourseID { get; set; }
            public string CourseName { get; set; }
            public string Credit { get; set; }
            public string Mark { get; set; }
  
            public Grades()
            {
            }
        }

    }
}
