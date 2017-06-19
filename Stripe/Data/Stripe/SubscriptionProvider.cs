﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stripe;
using Stripe.Models;

namespace Stripe.Data
{
    /// <summary>
    /// Implementation for subscription management with Stripe
    /// </summary>
    public class SubscriptionProvider : ISubscriptionProvider
    {
        private readonly StripeSubscriptionService _subscriptionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionProvider"/> class.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        public SubscriptionProvider(string apiKey)
        {
            this._subscriptionService = new StripeSubscriptionService(apiKey);
        }

        /// <summary>
        /// Subscribes the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="planId">The plan identifier.</param>
        /// <param name="trialInDays">The trial in days.</param>
        /// <param name="taxPercent">The tax percent.</param>
        public string SubscribeUser(ApplicationUser user, string planId, int trialInDays = 0, decimal taxPercent = 0)
        {
            var result = this._subscriptionService.Update(user.StripeCustomerId, planId,
                new StripeSubscriptionUpdateOptions
                {
                    PlanId = planId,
                    TaxPercent = taxPercent,
                    TrialEnd = DateTime.UtcNow.AddDays(trialInDays)
                });

            return result.Id;
        }

        /// <summary>
        /// Subscribes the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="planId">The plan identifier.</param>
        /// <param name="trialEnds">The trial ends.</param>
        /// <param name="taxPercent">The tax percent.</param>
        public string SubscribeUser(ApplicationUser user, string planId, DateTime? trialEnds, decimal taxPercent = 0)
        {
            var result = this._subscriptionService.Update(user.StripeCustomerId, planId,
                new StripeSubscriptionUpdateOptions
                {
                    PlanId = planId,
                    TaxPercent = taxPercent,
                    TrialEnd = trialEnds
                });

            return result.Id;
        }

        /// <summary>
        /// Gets the User's subscriptions asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<List<Subscription>> UserSubscriptionsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ends the subscription.
        /// </summary>
        /// <param name="userStripeId">The user stripe identifier.</param>
        /// <param name="subStripeId">The sub stripe identifier.</param>
        /// <param name="cancelAtPeriodEnd">if set to <c>true</c> [cancel at period end].</param>
        /// <returns>
        /// The date when the subscription will be cancelled
        /// </returns>
        public DateTime EndSubscription(string userStripeId, string subStripeId, bool cancelAtPeriodEnd = false)
        {
            var subscription = this._subscriptionService.Cancel(userStripeId, subStripeId, cancelAtPeriodEnd);

            return cancelAtPeriodEnd ? subscription.EndedAt.Value : DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the subscription. (Change subscription plan)
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="subStripeId">The sub stripe identifier.</param>
        /// <param name="newPlanId">The new plan identifier.</param>
        /// <param name="proRate">if set to <c>true</c> [pro rate].</param>
        /// <returns></returns>
        public bool UpdateSubscription(string customerId, string subStripeId, string newPlanId, bool proRate)
        {
            var result = true;
            try
            {
                var currentSubscription = this._subscriptionService.Get(customerId, subStripeId);

                var myUpdatedSubscription = new StripeSubscriptionUpdateOptions
                {
                    PlanId = newPlanId,
                    Prorate = proRate
                };

                if (currentSubscription.TrialEnd != null && currentSubscription.TrialEnd > DateTime.UtcNow)
                {
                    myUpdatedSubscription.TrialEnd = currentSubscription.TrialEnd; // Keep the same trial window as initially created.
                }

                _subscriptionService.Update(customerId, subStripeId, myUpdatedSubscription);
            }
            catch (Exception ex)
            {
                // TODO: Log
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Updates the subscription tax.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="subStripeId">The sub stripe identifier.</param>
        /// <param name="taxPercent">The tax percent.</param>
        /// <returns></returns>
        public bool UpdateSubscriptionTax(string customerId, string subStripeId, decimal taxPercent = 0)
        {
            var result = true;
            try
            {
                var myUpdatedSubscription = new StripeSubscriptionUpdateOptions
                {
                    TaxPercent = taxPercent
                };
                _subscriptionService.Update(customerId, subStripeId, myUpdatedSubscription);
            }
            catch (Exception ex)
            {
                // TODO: log
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Subscribes the user natural month.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="planId">The plan identifier.</param>
        /// <param name="billingAnchorCycle">The billing anchor cycle.</param>
        /// <param name="taxPercent">The tax percent.</param>
        /// <returns></returns>
        public object SubscribeUserNaturalMonth(ApplicationUser user, string planId, DateTime? billingAnchorCycle, decimal taxPercent)
        {
            StripeSubscription stripeSubscription = _subscriptionService.Create
                (user.StripeCustomerId, planId, new StripeSubscriptionCreateOptions
                {
                    BillingCycleAnchor = billingAnchorCycle,
                    TaxPercent = taxPercent
                });

            return stripeSubscription;
        }
    }
}
