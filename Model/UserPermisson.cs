using System.ComponentModel.DataAnnotations;
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
        public int Id { get; set; }

        public int UserId { get; set; }
        public int ProjectId { get; set; }
        
        [Required]
        public Permission Permission { get; set; }

        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
    }
}