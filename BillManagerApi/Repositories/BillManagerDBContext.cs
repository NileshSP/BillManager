using BillManagerApi.Repositories.Entities;
using BillManagerApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BillManagerApi.Repositories
{
    public class BillManagerDBContext : DbContext, IDBContext
    {
        public BillManagerDBContext(DbContextOptions<BillManagerDBContext> options)
            : base(options)
        { }

        public DbSet<Friend> Friend { get; set; }
        public DbSet<Bill> Bill { get; set; }
        public DbSet<BillShareFriend> BillShareFriend { get; set; }
        public DbContext DatabaseContext => this;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BillShareFriend>()
                .HasKey(bf => new { bf.BillId, bf.FriendId });

            modelBuilder.Entity<BillShareFriend>()
                .HasOne(b => b.Bill)
                .WithMany(bsf => bsf.BillShareFriends)
                .HasForeignKey(b => b.BillId);

            modelBuilder.Entity<BillShareFriend>()
                .HasOne(f => f.Friend)
                .WithMany(bsf => bsf.BillShareFriends)
                .HasForeignKey(f => f.FriendId);
        }
    }

    public static class ModelBuilderExtensions
    {
        public static Task SeedSampleData(this IDBContext context)
        {
            return Task.Run(async () =>
            {
                if (!context.Bill.Any() && !context.Friend.Any() && !context.BillShareFriend.Any())
                {
                    Random rnd = new Random();

                    context
                        .Friend
                        .AddRange(Enumerable.Range(1, 20)
                            .Select(s => new Friend()
                            {
                                FirstName = $"test{s}",
                                LastName = $"user{s}",
                                CreatedById = "test",
                                DateCreated = DateTime.Now
                            })
                       );
                    await context.DatabaseContext.SaveChangesAsync();

                    context
                        .Bill
                        .AddRange(Enumerable.Range(1, 10)
                            .Select(s => new Bill()
                            {
                                ExpenseDescription = $"Expense {s}",
                                DateCreated = DateTime.Now,
                                Amount = rnd.Next(100, 500),
                            })
                        );
                    await context.DatabaseContext.SaveChangesAsync();

                    context
                        .Bill
                        .ToList()
                        .ForEach(b =>
                        {
                            context
                                .Friend
                                .OrderBy(item => rnd.Next(0, 5))
                                .Take(rnd.Next(2, 6))
                                .ToList()
                                .ForEach(f =>
                                {
                                    context.BillShareFriend.Add(new BillShareFriend { BillId = b.BillId, FriendId = f.FriendId });
                                });
                        });
                    await context.DatabaseContext.SaveChangesAsync();
                }
            });
        }
    }
}
