using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Web.UI.WebControls;

namespace Backend.Model
{
    public class Progress
    {
        public Progress()
        {
            Tasks = new List<Task>();
        }


        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        public int Order { get; set; }

        public int OwnerId { get; set; }
        public int ProjectId { get; set; }

        public virtual Project Project { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }

        public static object GetProgerssList(int projectId)
        {
            using (var context = new BackendContext())
            {
                return (from progress in context.Progresses
                    where progress.ProjectId == projectId
                    select new
                    {
                        id = progress.Id,
                        name = progress.Name,
                        order = progress.Order
                    }).OrderBy(p => p.order).ToArray();
            }
        }
    }
}