using Backend.Model;
using System.Linq;


namespace Backend.Biz
{
    public class ProjectBiz
    {
//        public static object AddNew(object json)
//        {
//            var jsonData = Helper.Decode(json);
//
//            using (var context = new BackendContext())
//            {
//                var name = jsonData["project_name"];
//                var description = jsonData["project_description"];
//
//                context.Projects.Add(new Project()
//                {
//                    Name = name ,
//                    Description = description
//                    //todo projectUser and owner
//                });
//
//                context.SaveChanges();
//
//                //todo
//                return true;
//            }
//        }
//
//        public static object DeleteProject(object json)
//        {
//            var jsonData = Helper.Decode(json);
//
//            using (var context = new BackendContext())
//            {
//                int id = int.Parse(jsonData["project_id"]) ;
//                //todo user_id permisson
//
//                var project = context.Projects.FirstOrDefault(s => s.Id == id);
//                context.Projects.Remove(project);
//                context.SaveChanges();
//
//                return true;
//            }
//        }
//
//        public static object ChangeOwner(object json)
//        {
//            var jsonData = Helper.Decode(json);
//
//            using (var context = new BackendContext())
//            {
//                int id = int.Parse(jsonData["project_id"]) ;
//                int owner_id_from = int.Parse(jsonData["owner_id_from"]);
//                int owner_id_to = int.Parse(jsonData["owner_id_to"]);
//
//                //todo  permisson
//
//                var project = context.Projects.FirstOrDefault(s => s.Id == id);
//                //project.OwnerId = owner_id_to;
//                context.SaveChanges();
//
//                return new
//                {
//                    project_id = id,
//                    owner_id = owner_id_to
//                };
//            }
//        }
//
//        public static object GetInfo(object json)
//        {
//            var jsonData = Helper.Decode(json);
//
//            using (var context = new BackendContext())
//            {
//                int id = int.Parse(jsonData["project_id"]) ;
//                var response = from project in context.Projects
//                    where project.Id == id
//                    select new
//                    {
//                        project_id = project.Id,
//                        project_name = project.Name,
//                        project_discription = project.Description
//                    };
//
//                return response;
//            }
//        }
//





    }
}