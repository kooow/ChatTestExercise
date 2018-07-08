using grapetest2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace grapetest2.Data.Repositories
{

  /// <summary>
  /// 
  /// </summary>
  public class CustomerConversationRepository
  {

    /// <summary>
    /// 
    /// </summary>
    protected readonly ApplicationDbContext _context;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public CustomerConversationRepository(ApplicationDbContext context)
    {
      this._context = context;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public List<CustomerConversation> GetCustomerConversations()
    {
      return this._context.CustomerConversations.OrderByDescending(cc => cc.LastModified).ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="customerConversation"></param>
    public CustomerConversation Add(CustomerConversation customerConversation)
    {
      EntityEntry<CustomerConversation> newCustomerConversation = this._context.CustomerConversations.Add(customerConversation);

      this._context.SaveChanges();
      return newCustomerConversation.Entity;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public CustomerConversationMessage AddMessage(CustomerConversationMessage message)
    {
      EntityEntry<CustomerConversationMessage> newMessage = this._context.CustomerConversationMessages.Add(message);
      this._context.SaveChanges();
      return newMessage.Entity;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="customerConversation"></param>
    public void Update(CustomerConversation customerConversation)
    {
      this._context.CustomerConversations.Update(customerConversation);
      this._context.SaveChanges();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public void UpdateMessage(CustomerConversationMessage message)
    {
      this._context.CustomerConversationMessages.Update(message);
      this._context.SaveChanges();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connectionId"></param>
    /// <returns></returns>
    public CustomerConversation GetCustomerConversationByConnectionId(string connectionId)
    {
      return this._context.CustomerConversations.FirstOrDefault(cc => cc.ConnectionId == connectionId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lastResfreshDateTime"></param>
    /// <returns></returns>
    public List<CustomerConversation> GetModifiedCustomerConversations(DateTime lastResfreshDateTime)
    {
      List<CustomerConversation> list = this._context.CustomerConversations
          .Where(cc => cc.LastModified > lastResfreshDateTime)
          .OrderBy(cc => cc.LastModified).ToList();
      return list;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connectionId"></param>
    /// <returns></returns>
    public List<CustomerConversationMessage> GetMessagesByConnectionId(string connectionId)
    {
      List<CustomerConversationMessage> messages =
           (from ccm in this._context.CustomerConversationMessages
            from cc in this._context.CustomerConversations
            where cc.ConnectionId == connectionId
            where ccm.CustomerConversationId == cc.Id
            orderby ccm.Created ascending
            select ccm).ToList();

      return messages;
    }

  }
}
