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
    public class FriendService : IFriendService
    {
        private readonly IDBContext _repositoryContext;
        private readonly ILogger _logger;

        public FriendService(IDBContext repositoryContext, ILogger<FriendService> logger)
        {
            _repositoryContext = repositoryContext;
            _logger = logger;
        }

        public Task<FriendModel> GetItem(int id)
        {
            Friend friend = _repositoryContext.Friend.Find(id);
            FriendModel friendModel = new FriendModel();
            if (friend != null)
            {
                friendModel = new FriendModel
                {
                    FriendId = friend.FriendId,
                    FirstName = friend.FirstName,
                    LastName = friend.LastName,
                    Bills = GetBillExpenses(friend).Result
                };
            }
            return Task.FromResult(friendModel);
        }

        public Task<IEnumerable<FriendModel>> GetList()
        {
            IList<FriendModel> friendList = new List<FriendModel>();
            _repositoryContext
                    .Friend
                    .ToList()
                    .ForEach(friend =>
                    {
                        friendList.Add(new FriendModel
                        {
                            FriendId = friend.FriendId,
                            FirstName = friend.FirstName,
                            LastName = friend.LastName,
                            Bills = GetBillExpenses(friend).Result
                        });
                    });
            return Task.FromResult(friendList.AsEnumerable());
        }

        public Task<int?> PostItem(FriendModel item)
        {
            try
            {
                if (string.IsNullOrEmpty(item.FirstName))
                {
                    throw new ArgumentException("Friend firstname is empty");
                }

                if (string.IsNullOrEmpty(item.LastName))
                {
                    throw new ArgumentException("Friend lastname is empty");
                }

                Friend friendEntity = new Friend
                {
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    DateCreated = DateTime.Now,
                    CreatedById = "User"
                };
                _repositoryContext.Friend.Add(friendEntity).Context.SaveChanges();
                item.FriendId = friendEntity.FriendId;
                return Task.FromResult(item.FriendId as int?);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"FriendController Error: PostItem()", item);
                return Task.FromResult(null as int?);
            }
        }

        public Task<bool> PostItems(IEnumerable<FriendModel> items)
        {
            try
            {
                items.ToList().ForEach(item =>
                {
                    if (string.IsNullOrEmpty(item.FirstName))
                    {
                        throw new ArgumentException("Friend firstname is empty");
                    }

                    if (string.IsNullOrEmpty(item.LastName))
                    {
                        throw new ArgumentException("Friend lastname is empty");
                    }

                    Friend friendEntity = new Friend
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        DateCreated = DateTime.Now,
                        CreatedById = "User"
                    };
                    _repositoryContext.Friend.Add(friendEntity);
                });
                _repositoryContext.DatabaseContext.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"FriendController Error: PostItems()", items);
                return Task.FromResult(false);
            }
        }

        public Task<bool> PutItem(FriendModel item)
        {
            try
            {
                Friend existingFriend = _repositoryContext.Friend.Find(item.FriendId);
                if (existingFriend != null)
                {
                    existingFriend.FirstName = item.FirstName;
                    existingFriend.LastName = item.LastName;
                    existingFriend.DateModified = DateTime.Now;
                    _repositoryContext.Friend.Update(existingFriend).Context.SaveChanges();
                    return Task.FromResult(true);
                }
                else
                {
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"FriendController Error: PutItem()", item);
                return Task.FromResult(false);
            }
        }

        public Task<bool> PutItems(IEnumerable<FriendModel> items)
        {
            try
            {
                items.ToList().ForEach(item =>
                {
                    Friend existingFriend = _repositoryContext.Friend.Find(item.FriendId);
                    if (existingFriend != null)
                    {
                        existingFriend.FirstName = item.FirstName;
                        existingFriend.LastName = item.LastName;
                        existingFriend.DateModified = DateTime.Now;
                        _repositoryContext.Friend.Update(existingFriend);
                    }
                });
                _repositoryContext.DatabaseContext.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"FriendController Error: PutItems()", items);
                return Task.FromResult(false);
            }
        }

        public Task<bool> DeleteItem(int id)
        {
            try
            {
                Friend friend = _repositoryContext.Friend.Where(s => s.FriendId == id).FirstOrDefault();
                if (friend == null)
                {
                    throw new KeyNotFoundException($" Friend with Id={id} not found to delete ");
                }

                _repositoryContext.Friend.Remove(friend);
                _repositoryContext.BillShareFriend.RemoveRange(_repositoryContext.BillShareFriend.Where(bsf => bsf.FriendId == friend.FriendId).ToArray());
                _repositoryContext.DatabaseContext.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"FriendController Error: DeleteItem({id})");
                return Task.FromResult(false);
            }
        }

        public Task<bool> DeleteItems(IEnumerable<int> idList)
        {
            try
            {
                List<Friend> friends = _repositoryContext.Friend.Where(s => idList.Contains(s.FriendId)).ToList<Friend>();
                if (friends == null)
                {
                    throw new KeyNotFoundException($" Friends with Ids [{ string.Join(", ", idList) }] not found to delete ");
                }

                friends.ForEach(friend =>
                {
                    _repositoryContext.Friend.Remove(friend);
                    _repositoryContext.BillShareFriend.RemoveRange(_repositoryContext.BillShareFriend.Where(bsf => bsf.FriendId == friend.FriendId).ToArray());
                });
                _repositoryContext.DatabaseContext.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"FriendController Error: DeleteItems({string.Join(",", idList)})");
                return Task.FromResult(false);
            }
        }

        public Task<bool> LinkItem(int searchId, int linkId)
        {
            try
            {
                BillShareFriend existingLink = _repositoryContext.BillShareFriend.Where(bsf => bsf.FriendId == searchId && bsf.BillId == linkId).FirstOrDefault();
                Friend friendExists = _repositoryContext.Friend.Where(b => b.FriendId == searchId).FirstOrDefault();
                Bill billExists = _repositoryContext.Bill.Where(b => b.BillId == linkId).FirstOrDefault();
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
                _logger.LogError(ex, $"FriendController Error: LinkItem()", searchId, linkId);
                return Task.FromResult(false);
            }
        }

        public Task<bool> UnLinkItem(int searchId, int linkId)
        {
            try
            {
                BillShareFriend existingLink = _repositoryContext.BillShareFriend.Where(bsf => bsf.FriendId == searchId && bsf.BillId == linkId).FirstOrDefault();
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
                _logger.LogError(ex, $"FriendController Error: UnLinkItem()", searchId, linkId);
                return Task.FromResult(false);
            }
        }

        private async Task<List<string>> GetBillExpenses(Friend friend)
        {
            return await _repositoryContext
                            .DatabaseContext
                            .Entry(friend)
                            .Collection(bsf => bsf.BillShareFriends)
                            .Query()
                            .Select(b => b.Bill.ExpenseDescription)
                            .ToListAsync<string>();
        }

        public IActionResult GetIActionResult(bool result)
        {
            return result ? new OkResult() as IActionResult : new UnprocessableEntityResult() as IActionResult;
        }
    }
}