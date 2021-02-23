using BillManagerApi.Models;
using BillManagerApi.Repositories.Entities;
using BillManagerApi.Repositories.Interfaces;
using BillManagerApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillManagerApi.Services
{
    public class BillService : IBillService
    {
        private readonly IDBContext _repositoryContext;
        private readonly ILogger _logger;

        public BillService(IDBContext repositoryContext, ILogger<BillService> logger)
        {
            _repositoryContext = repositoryContext;
            _logger = logger;
        }

        public Task<BillModel> GetItem(int id)
        {
            Bill bill = _repositoryContext.Bill.Find(id);
            BillModel billModel = new BillModel();
            if (bill != null)
            {
                billModel = new BillModel
                {
                    BillId = bill.BillId,
                    ExpenseDescription = bill.ExpenseDescription,
                    Amount = bill.Amount,
                    Friends = GetFriendShareBills(bill).Result
                };
            }
            return Task.FromResult(billModel);
        }

        public Task<IEnumerable<BillModel>> GetList()
        {
            IList<BillModel> billList = new List<BillModel>();
            _repositoryContext
                    .Bill
                    .ForEachAsync(bill =>
                    {
                        billList.Add(new BillModel
                        {
                            BillId = bill.BillId,
                            ExpenseDescription = bill.ExpenseDescription,
                            Amount = bill.Amount,
                            Friends = GetFriendShareBills(bill).Result
                        });
                    });
            return Task.FromResult(billList.AsEnumerable());
        }

        public Task<int?> PostItem(BillModel item)
        {
            try
            {
                if (string.IsNullOrEmpty(item.ExpenseDescription))
                {
                    throw new ArgumentException("Bill expense description is empty");
                }

                Bill billEntity = new Bill
                {
                    ExpenseDescription = item.ExpenseDescription,
                    Amount = item.Amount,
                    DateCreated = DateTime.Now,
                    CreatedById = "User"
                };
                _repositoryContext.Bill.Add(billEntity);
                if (item.Friends != null)
                {
                    item.Friends.ToList().ForEach(friend =>
                    {
                        Friend friendEntity = _repositoryContext.Friend.Find(friend.FriendId);
                        if (friendEntity != null)
                        {
                            BillShareFriend bsf = new BillShareFriend() { Bill = billEntity, BillId = billEntity.BillId, Friend = friendEntity, FriendId = friend.FriendId };
                            _repositoryContext.BillShareFriend.Add(bsf);
                        }
                    });
                }
                _repositoryContext.DatabaseContext.SaveChanges();
                item.BillId = billEntity.BillId;
                return Task.FromResult(item.BillId as int?);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"BillController Error: PostItem()", item);
                return Task.FromResult(null as int?);
            }
        }

        public Task<bool> PostItems(IEnumerable<BillModel> items)
        {
            try
            {
                items.ToList().ForEach(item =>
                {
                    if (string.IsNullOrEmpty(item.ExpenseDescription))
                    {
                        throw new ArgumentException("Bill expense description is empty");
                    }

                    Bill billEntity = new Bill
                    {
                        ExpenseDescription = item.ExpenseDescription,
                        Amount = item.Amount,
                        DateCreated = DateTime.Now,
                        CreatedById = "User"
                    };
                    _repositoryContext.Bill.Add(billEntity);
                    if (item.Friends != null)
                    {
                        item.Friends.ToList().ForEach(friend =>
                        {
                            Friend friendEntity = _repositoryContext.Friend.Find(friend.FriendId);
                            if (friendEntity != null)
                            {
                                BillShareFriend bsf = new BillShareFriend() { Bill = billEntity, BillId = billEntity.BillId, Friend = friendEntity, FriendId = friend.FriendId };
                                _repositoryContext.BillShareFriend.Add(bsf);
                            }
                        });
                    }
                });
                _repositoryContext.DatabaseContext.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"BillController Error: PostItems()", items);
                return Task.FromResult(false);
            }
        }

        public Task<bool> PutItem(BillModel item)
        {
            try
            {
                Bill existingBill = _repositoryContext.Bill.Find(item.BillId);
                if (existingBill != null)
                {
                    existingBill.ExpenseDescription = item.ExpenseDescription;
                    existingBill.Amount = item.Amount;
                    existingBill.DateModified = DateTime.Now;
                    _repositoryContext.Bill.Update(existingBill).Context.SaveChanges();
                    return Task.FromResult(true);
                }
                else
                {
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"BillController Error: PutItem()", item);
                return Task.FromResult(false);
            }
        }

        public Task<bool> PutItems(IEnumerable<BillModel> items)
        {
            try
            {
                items.ToList().ForEach(item =>
                {
                    Bill existingBill = _repositoryContext.Bill.Find(item.BillId);
                    if (existingBill != null)
                    {
                        existingBill.ExpenseDescription = item.ExpenseDescription;
                        existingBill.Amount = item.Amount;
                        existingBill.DateModified = DateTime.Now;
                        _repositoryContext.Bill.Update(existingBill);
                    }
                });
                _repositoryContext.DatabaseContext.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"BillController Error: PutItems()", items);
                return Task.FromResult(false);
            }
        }

        public Task<bool> DeleteItem(int id)
        {
            try
            {
                Bill Bill = _repositoryContext.Bill.Where(s => s.BillId == id).FirstOrDefault();
                if (Bill == null)
                {
                    throw new KeyNotFoundException($" Bill with Id={id} not found to delete ");
                }

                _repositoryContext.Bill.Remove(Bill);
                _repositoryContext.BillShareFriend.RemoveRange(_repositoryContext.BillShareFriend.Where(bsf => bsf.BillId == Bill.BillId).ToArray());
                _repositoryContext.DatabaseContext.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"BillController Error: DeleteItem({id})");
                return Task.FromResult(false);
            }
        }

        public Task<bool> DeleteItems(IEnumerable<int> idList)
        {
            try
            {
                List<Bill> Bills = _repositoryContext.Bill.Where(s => idList.Contains(s.BillId)).ToList<Bill>();
                if (Bills == null)
                {
                    throw new KeyNotFoundException($" Bills with Ids [{ string.Join(", ", idList) }] not found to delete ");
                }

                Bills.ForEach(Bill =>
                {
                    _repositoryContext.Bill.Remove(Bill);
                    _repositoryContext.BillShareFriend.RemoveRange(_repositoryContext.BillShareFriend.Where(bsf => bsf.BillId == Bill.BillId).ToArray());
                });
                _repositoryContext.DatabaseContext.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"BillController Error: DeleteItems({string.Join(",", idList)})");
                return Task.FromResult(false);
            }
        }

        public Task<bool> LinkItem(int searchId, int linkId)
        {
            try
            {
                BillShareFriend existingLink = _repositoryContext.BillShareFriend.Where(bsf => bsf.FriendId == searchId && bsf.BillId == linkId).FirstOrDefault();
                Friend friendExists = _repositoryContext.Friend.Where(b => b.FriendId == linkId).FirstOrDefault();
                Bill billExists = _repositoryContext.Bill.Where(b => b.BillId == searchId).FirstOrDefault();
                if (existingLink == null && friendExists != null && billExists != null)
                {
                    _repositoryContext
                        .BillShareFriend
                        .Add(
                            new BillShareFriend
                            {
                                BillId = billExists.BillId,
                                Bill = billExists,
                                FriendId = friendExists.FriendId,
                                Friend = friendExists
                            }
                        )
                        .Context.SaveChanges();
                    return Task.FromResult(true);
                }
                else if (existingLink != null)
                {
                    return Task.FromResult(true);
                }
                else
                {
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"BillController Error: LinkItem()", searchId, linkId);
                return Task.FromResult(false);
            }
        }
        public Task<bool> UnLinkItem(int searchId, int linkId)
        {
            try
            {
                BillShareFriend existingLink = _repositoryContext.BillShareFriend.Where(bsf => bsf.FriendId == linkId && bsf.BillId == searchId).FirstOrDefault();
                if (existingLink != null)
                {
                    _repositoryContext.BillShareFriend.Remove(existingLink).Context.SaveChanges();
                    return Task.FromResult(true);
                }
                else
                {
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"BillController Error: UnLinkItem()", searchId, linkId);
                return Task.FromResult(false);
            }
        }
        public IActionResult GetIActionResult(bool result)
        {
            return result ? new OkResult() as IActionResult : new UnprocessableEntityResult() as IActionResult;
        }

        public Task<List<FriendShareBillModel>> GetFriendShareBills(Bill bill)
        {
            return Task.Run(() =>
            {
                List<FriendShareBillModel> list = new List<FriendShareBillModel>();
                List<FriendModel> friends = _repositoryContext
                                                .BillShareFriend
                                                .Where(bsf => bsf.BillId == bill.BillId)
                                                .Include(f => f.Friend)
                                                .Select(f => new FriendModel { FriendId = f.Friend.FriendId, FirstName = f.Friend.FirstName, LastName = f.Friend.LastName })
                                                .ToList();
                float billShare = bill.Amount / friends.Count;
                friends.ForEach(friend => list.Add(new FriendShareBillModel { FriendId = friend.FriendId, FullName = $"{friend.FirstName} {friend.LastName}", AmountShare = billShare }));
                return list;
            });
        }

    }
}
