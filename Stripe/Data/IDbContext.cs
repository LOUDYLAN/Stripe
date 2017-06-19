﻿using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stripe.Models;

namespace Stripe.Data
{
    public interface IDbContext<TUser> where TUser : class
    {
        DbSet<TUser> Users { get; set; }

        DbSet<Subscription> Subscriptions { get; set; }

        DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

        DbSet<Invoice> Invoices { get; set; }

        DbSet<CreditCard> CreditCards { get; set; }

        Task<int> SaveChangesAsync();

//        DbEntityEntry<T> Entry<T>(T entity) where T : class;
    }
}
