using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace WebPush.NetCore.Test
{
    [TestClass]
    public class WebPushClientTest
    {
        private const string TEST_FCM_ENDPOINT =
            @"https://fcm.googleapis.com/fcm/send/efz_TLX_rLU:APA91bE6U0iybLYvv0F3mf6";

        private const string TEST_GCM_ENDPOINT = @"https://android.googleapis.com/gcm/send/";

        private const string TEST_PRIVATE_KEY = @"on6X5KmLEFIVvPP3cNX9kE0OF6PV9TJQXVbnKU2xEHI";

        private const string TEST_PUBLIC_KEY =
                                  @"BCvKwB2lbVUYMFAaBUygooKheqcEU-GDrVRnu8k33yJCZkNBNqjZj0VdxQ2QIZa4kV5kpX9aAqyBKZHURm6eG1A";

        [TestMethod]
        public void TestGCMAPIKeyInOptions()
        {
            WebPushClient client = new WebPushClient();

            string gcmAPIKey = @"teststring";
            PushSubscription subscription = new PushSubscription(TEST_GCM_ENDPOINT, TEST_PUBLIC_KEY, TEST_PRIVATE_KEY);

            Dictionary<string, object> options = new Dictionary<string, object>();
            options["gcmAPIKey"] = gcmAPIKey;
            HttpRequestMessage message = client.GenerateRequestDetails(subscription, "test payload", options);
            string authorizationHeader = message.Headers.GetValues("Authorization").First();

            Assert.AreEqual("key=" + gcmAPIKey, authorizationHeader);

            // Test previous incorrect casing of gcmAPIKey
            Dictionary<string, object> options2 = new Dictionary<string, object>();
            options2["gcmApiKey"] = gcmAPIKey;
            Assert.ThrowsException<ArgumentException>(delegate
            {
                client.GenerateRequestDetails(subscription, "test payload", options2);
            });
        }

        [TestMethod]
        public void TestSendFCMMessage()
        {
            string p256dh = @"BPez73CdNHyBIFW";
            string auth = @"r_36Ti2Z";
            WebPushClient client = new WebPushClient();
            PushSubscription subscription = new PushSubscription(TEST_FCM_ENDPOINT, p256dh, auth);
            var vapidDetails = new VapidDetails("mailto:example@example.com", TEST_PUBLIC_KEY, TEST_PRIVATE_KEY);
            client.SendNotification(subscription, "Insert here a payload", vapidDetails);
        }

        [TestMethod]
        public void TestSetGCMAPIKey()
        {
            WebPushClient client = new WebPushClient();

            string gcmAPIKey = @"teststring";
            client.SetGCMAPIKey(gcmAPIKey);
            PushSubscription subscription = new PushSubscription(TEST_GCM_ENDPOINT, TEST_PUBLIC_KEY, TEST_PRIVATE_KEY);
            HttpRequestMessage message = client.GenerateRequestDetails(subscription, "test payload");
            string authorizationHeader = message.Headers.GetValues("Authorization").First();

            Assert.AreEqual("key=" + gcmAPIKey, authorizationHeader);
        }

        [TestMethod]
        public void TestSetGCMAPIKeyEmptyString()
        {
            WebPushClient client = new WebPushClient();

            Assert.ThrowsException<ArgumentException>(delegate
           {
               client.SetGCMAPIKey("");
           });
        }

        [TestMethod]
        public void TestSetGCMAPiKeyNonGCMPushService()
        {
            // Ensure that the API key doesn't get added on a service that doesn't accept it.
            WebPushClient client = new WebPushClient();

            string gcmAPIKey = @"teststring";
            client.SetGCMAPIKey(gcmAPIKey);
            PushSubscription subscription = new PushSubscription(TEST_FCM_ENDPOINT, TEST_PUBLIC_KEY, TEST_PRIVATE_KEY);
            HttpRequestMessage message = client.GenerateRequestDetails(subscription, "test payload");

            IEnumerable<string> values;
            Assert.IsFalse(message.Headers.TryGetValues("Authorization", out values));
        }

        [TestMethod]
        public void TestSetGCMAPIKeyNull()
        {
            WebPushClient client = new WebPushClient();

            client.SetGCMAPIKey(@"somestring");
            client.SetGCMAPIKey(null);

            PushSubscription subscription = new PushSubscription(TEST_GCM_ENDPOINT, TEST_PUBLIC_KEY, TEST_PRIVATE_KEY);
            HttpRequestMessage message = client.GenerateRequestDetails(subscription, "test payload");

            IEnumerable<string> values;
            Assert.IsFalse(message.Headers.TryGetValues("Authorization", out values));
        }

        [TestMethod]
        public void TestSetVapidDetails()
        {
            WebPushClient client = new WebPushClient();

            client.SetVapidDetails("mailto:example@example.com", TEST_PUBLIC_KEY, TEST_PRIVATE_KEY);

            PushSubscription subscription = new PushSubscription(TEST_FCM_ENDPOINT, TEST_PUBLIC_KEY, TEST_PRIVATE_KEY);
            HttpRequestMessage message = client.GenerateRequestDetails(subscription, "test payload");
            string authorizationHeader = message.Headers.GetValues("Authorization").First();
            string cryptoHeader = message.Headers.GetValues("Crypto-Key").First();

            Assert.IsTrue(authorizationHeader.StartsWith("WebPush "));
            Assert.IsTrue(cryptoHeader.Contains("p256ecdsa"));
        }
    }
}