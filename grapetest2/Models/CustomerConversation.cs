using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace grapetest2.Models
{

  /// <summary>
  /// 
  /// </summary>
  public class CustomerConversation
  {

    public CustomerConversation()
    {
    }

    public CustomerConversation(string connectionId)
    {
      this.ConnectionId = String.Copy(connectionId);
      this.Created = DateTime.Now;
      this.LastModified = DateTime.Now;
      this.Unanswered = true;
      this.Closed = false;
    }

    public int Id { get; set; }

    public string ConnectionId { get; set; }

    public bool Unanswered { get; set; }

    public DateTime Created { get; set; }

    public DateTime LastModified { get; set; }

    public bool Closed { get; set; }

  }
}
