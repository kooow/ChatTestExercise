using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace grapetest2.Models
{
  /// <summary>
  /// 
  /// </summary>
  public class CustomerConversationMessage
  {

    public CustomerConversationMessage() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="customerConversationId"></param>
    /// <param name="content"></param>
    public CustomerConversationMessage(int customerConversationId, string content)
    {
      this.CustomerConversationId = customerConversationId;
      this.Content = String.Copy(content);
      this.Created = DateTime.Now;
    }

    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int CustomerConversationId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public CustomerConversation CustomerConversation { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool FromCustomer { get; set; }

  }

}
