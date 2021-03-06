﻿using System;
using System.Net;
using Checkout;
using Checkout.ApiServices.Charges.ResponseModels;
using FluentAssertions;
using NUnit.Framework;

namespace Tests
{
    public class BaseServiceTests
    {
        protected ApiClient CheckoutClient;

        [SetUp]
        public void Init()
        {
            // Configure this to switch between Sandbox and Live
            CheckoutConfiguration configuration = new CheckoutConfiguration()
            {
                SecretKey = Environment.GetEnvironmentVariable("CKO_SECRET_KEY"),
                PublicKey = Environment.GetEnvironmentVariable("CKO_PUBLIC_KEY"),
                DebugMode = true
            };

            CheckoutClient = new ApiClient(configuration);
        }

        #region Protected methods

        /// <summary>
        ///     Creates a new charge with default card and new track id and asserts that is not declined
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        protected Charge CreateChargeWithNewTrackId()
        {
            return CreateChargeWithNewTrackId(out string cardNumber);
        }

        /// <summary>
        ///     Creates a new charge with default card and new track id and asserts that is not declined
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        protected Charge CreateChargeWithNewTrackId(out string cardNumber)
        {
            var cardCreateModel = TestHelper.GetCardChargeCreateModel(TestHelper.RandomData.Email);
            cardCreateModel.TrackId = "TRF" + Guid.NewGuid();
            var chargeResponse = CheckoutClient.ChargeService.ChargeWithCard(cardCreateModel);

            chargeResponse.Should().NotBeNull();
            chargeResponse.HttpStatusCode.Should().Be(HttpStatusCode.OK);

            if (CheckoutClient.CheckoutConfiguration.DebugMode && chargeResponse.Model.ResponseCode != "10000")
            {
                Console.WriteLine(string.Format("\n** Charge status is not 'Approved' **\n** Charge status is '{0}' **", chargeResponse.Model.Status.ToUpper()));
                Console.WriteLine(string.Format("\n** Advanced Info ** {0}", chargeResponse.Model.ResponseAdvancedInfo));
            };
            chargeResponse.Model.Status.Should().NotBe("Declined", "CreateChargeWithNewTrackId(out string cardNumber) must create an authorized charge, you may re-run this test");

            cardNumber = cardCreateModel.Card.Number;
            return chargeResponse.Model;
        }

        /// <summary>
        ///     Creates a new charge with provided card and new track id and asserts that is not declined
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <param name="cvv"></param>
        /// <param name="expirityMonth"></param>
        /// <param name="expirityYear"></param>
        /// <returns></returns>
        protected Charge CreateChargeWithNewTrackId(string cardNumber, string cvv, string expirityMonth, string expirityYear)
        {
            var cardCreateModel = TestHelper.GetCardChargeCreateModel(TestHelper.RandomData.Email);
            cardCreateModel.TrackId = "TRF" + Guid.NewGuid();
            cardCreateModel.Card.Number = cardNumber;
            cardCreateModel.Card.Cvv = cvv;
            cardCreateModel.Card.ExpiryMonth = expirityMonth;
            cardCreateModel.Card.ExpiryYear = expirityYear;
            var chargeResponse = CheckoutClient.ChargeService.ChargeWithCard(cardCreateModel);

            chargeResponse.Should().NotBeNull();
            chargeResponse.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            chargeResponse.Model.Status.Should().NotBe("Declined");

            return chargeResponse.Model;
        }

        #endregion
    }
}
