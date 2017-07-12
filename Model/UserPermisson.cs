using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;

namespace Backend.Model
{
    public enum Permission
    {
        Creator,
        Administrator,
        Participant 
    }
    
    public class UserPermisson
    {
        [Key,Column(Order = 0)]
        public int UserId { get; set; }
        
        [Key,Column(Order = 1)]
        public int ProjectId { get; set; }
        
        [Required]
        public Permission Permission { get; set; }

        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
    }
}