using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using WebPush.Util;

namespace WebPush.Test
{
    [TestClass]
    public class VapidHelperTest
    {
        private const string VALID_AUDIENCE = "http://example.com";
        private const string VALID_SUBJECT = "http://example.com/example";
        private const string VALID_SUBJECT_MAILTO = "mailto:example@example.com";

        [TestMethod]
        public void TestGenerateVapidKeys()
        {
            VapidDetails keys = VapidHelper.GenerateVapidKeys();
            byte[] publicKey = UrlBase64.Decode(keys.PublicKey);
            byte[] privateKey = UrlBase64.Decode(keys.PrivateKey);

            Assert.AreEqual(32, privateKey.Length);
            Assert.AreEqual(65, publicKey.Length);
        }

        [TestMethod]
        public void TestGenerateVapidKeysNoCache()
        {
            VapidDetails keys1 = VapidHelper.GenerateVapidKeys();
            VapidDetails keys2 = VapidHelper.GenerateVapidKeys();

            Assert.AreNotEqual(keys1.PublicKey, keys2.PublicKey);
            Assert.AreNotEqual(keys1.PrivateKey, keys2.PrivateKey);
        }

        [TestMethod]
        public void TestGetVapidHeaders()
        {
            string publicKey = UrlBase64.Encode(new byte[65]);
            string privatekey = UrlBase64.Encode(new byte[32]);
            Dictionary<string, string> headers = VapidHelper.GetVapidHeaders(VALID_AUDIENCE, VALID_SUBJECT, publicKey, privatekey);

            Assert.IsTrue(headers.ContainsKey("Authorization"));
            Assert.IsTrue(headers.ContainsKey("Crypto-Key"));
        }

        [TestMethod]
        public void TestGetVapidHeadersAudienceNotAUrl()
        {
            string publicKey = UrlBase64.Encode(new byte[65]);
            string privatekey = UrlBase64.Encode(new byte[32]);

            Assert.ThrowsException<ArgumentException>(
               delegate
               {
                   VapidHelper.GetVapidHeaders("invalid audience", VALID_SUBJECT, publicKey, privatekey);
               });
        }

        [TestMethod]
        public void TestGetVapidHeadersInvalidPrivateKey()
        {
            string publicKey = UrlBase64.Encode(new byte[65]);
            string privatekey = UrlBase64.Encode(new byte[1]);

            Assert.ThrowsException<ArgumentException>(
               delegate
               {
                   VapidHelper.GetVapidHeaders(VALID_AUDIENCE, VALID_SUBJECT, publicKey, privatekey);
               });
        }

        [TestMethod]
        public void TestGetVapidHeadersInvalidPublicKey()
        {
            string publicKey = UrlBase64.Encode(new byte[1]);
            string privatekey = UrlBase64.Encode(new byte[32]);

            Assert.ThrowsException<ArgumentException>(
               delegate
               {
                   VapidHelper.GetVapidHeaders(VALID_AUDIENCE, VALID_SUBJECT, publicKey, privatekey);
               });
        }

        [TestMethod]
        public void TestGetVapidHeadersSubjectNotAUrlOrMailTo()
        {
            string publicKey = UrlBase64.Encode(new byte[65]);
            string privatekey = UrlBase64.Encode(new byte[32]);

            Assert.ThrowsException<ArgumentException>(
                delegate
                {
                    VapidHelper.GetVapidHeaders(VALID_AUDIENCE, "invalid subject", publicKey, privatekey);
                });
        }

        [TestMethod]
        public void TestGetVapidHeadersWithMailToSubject()
        {
            string publicKey = UrlBase64.Encode(new byte[65]);
            string privatekey = UrlBase64.Encode(new byte[32]);
            Dictionary<string, string> headers = VapidHelper.GetVapidHeaders(VALID_AUDIENCE, VALID_SUBJECT_MAILTO, publicKey,
                privatekey);

            Assert.IsTrue(headers.ContainsKey("Authorization"));
            Assert.IsTrue(headers.ContainsKey("Crypto-Key"));
        }
    }
}