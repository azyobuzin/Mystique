﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace Vanille
{
    // Krile Archive Signature Extension
    public class KASignature
    {
        public static void Signature(Stream input, Stream output, string privateKey)
        {
            using (var sha = new SHA256Cng())
            using (var rsa = new RSACryptoServiceProvider())
            {
                // Compute hash
                var buffer = ReadAllBytes(input).ToArray();
                var hash = sha.ComputeHash(buffer);
                // RSA Initialize
                rsa.FromXmlString(privateKey);
                // format
                var formatter = new RSAPKCS1SignatureFormatter(rsa);
                formatter.SetHashAlgorithm("SHA256");
                var signature = formatter.CreateSignature(hash);
                string magic = "KSA:" + signature.Length + ":";
                var magicbytes = Encoding.UTF8.GetBytes(magic);
                if (magicbytes.Length > 64)
                    throw new Exception("Magic bits too long.");
                output.Write(magicbytes, 0, magicbytes.Length);
                var padding = new byte[64 - magicbytes.Length];
                output.Write(padding, 0, padding.Length);
                output.Write(signature, 0, signature.Length);
                MessageBox.Show(signature.Length.ToString());
                output.Write(buffer, 0, buffer.Length);
            }
        }

        public static bool Verify(Stream input, Stream output, string publicKey)
        {
            // ファイルから情報の抽出
            byte[] buffer = new byte[64];
            if (input.Read(buffer, 0, 64) < 64)
                throw new Exception("File is corrupted.(KSIG: Magic Not Found)");
            var magicstr = Encoding.UTF8.GetString(buffer).Split(new[] { ":" }, StringSplitOptions.None);
            if(magicstr.Length < 2)
                throw new Exception("File is corrupted.(KSIG: Invalid Magic Length)");
            if (magicstr[0] != "KSA")
                throw new Exception("File is corrupted.(KSIG: Invalid Magic String)");
            var siglen = int.Parse(magicstr[1]);
            var sigbuf = new byte[siglen];
            if (input.Read(sigbuf, 0, siglen) < siglen)
                throw new Exception("File is corrupted.(KSIG: Invalid Signature)");
            var data = ReadAllBytes(input).ToArray();

            using (var sha = new SHA256Cng())
            using (var rsa = new RSACryptoServiceProvider())
            {
                // Compute hash
                var hash = sha.ComputeHash(data);
                // RSA Initialize
                rsa.FromXmlString(publicKey);
                // deformat
                var deformatter = new RSAPKCS1SignatureDeformatter(rsa);
                deformatter.SetHashAlgorithm("SHA256");
                if (!deformatter.VerifySignature(hash, sigbuf))
                    return false;
            }
            output.Write(data, 0, data.Length);
            return true;
        }

        public static void CreateKeys(out string publicKey, out string privateKey)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);
            }
        }

        private static MemoryStream ReadAllBytes(Stream stream)
        {
            var ms = new MemoryStream();
            int bytes = 0;
            byte[] temp = new byte[4096];
            while ((bytes = stream.Read(temp, 0, temp.Length)) > 0)
                ms.Write(temp, 0, bytes);
            return ms;
        }
    }
}
