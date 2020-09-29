using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HealthTrackerProcessor.Class;
using HealthTrackerProcessor.Repositories;
using HealthTrackerProcessorCore.Class;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using static HealthTrackerProcessor.Enum.HealthTrackerProcessorEnum;
using System.Linq;

namespace HealthTrackerProcessor.Controllers
{
    [Route("v1/userprojects/[controller]")]
    [ApiController]
    public class ProjectController : ConfigDbContextController
    {
        private readonly ProjectRepository _projectRepository;
        private readonly UserRepository _userRepository;
        private readonly ILogger _logger;
        public ProjectController(Project project, User user, ILoggerFactory loggerFactory)
        {
            if (project == null) throw new ArgumentNullException(nameof(project), "project cannot be null");
            if (user == null) throw new ArgumentNullException(nameof(user), "user name cannot be null");
            _projectRepository = new ProjectRepository(base._configDbContext, loggerFactory.CreateLogger<ProjectRepository>());
            _userRepository = new UserRepository(base._configDbContext, loggerFactory.CreateLogger<UserRepository>());
        }

        [HttpPost("Create")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<JsonResult> CreateNewPorject([FromBody] ProjectInput input)
        {
            string sErr = "";
            JsonResult response = null;
            string userID = "";
            try
            {
                Project existProject = null;
                if (String.IsNullOrEmpty(input.Name))
                {
                    sErr = "Project name cannot be empty!";
                    goto Get_Out;
                }
                if (String.IsNullOrEmpty(input.Username))
                {
                    sErr = "User name cannot be empty!";
                    goto Get_Out;
                }
                await Task.Run(() =>
                {
                    userID = _userRepository.GetUserID(input.Username);
                });
                if(String.IsNullOrEmpty(userID))
                {
                    sErr = "Cannot find user!";
                    goto Get_Out;
                }
                await Task.Run(() =>
                {
                    existProject = this._projectRepository.Get(input.Name, userID);
                });
                if (existProject != null)
                {
                    sErr += "Project name cannot be duplicated!";
                    goto Get_Out;
                }
                Project project = new Project()
                {
                    UserID = userID,
                    Name = input.Name,
                    ProjectType = input.ProjectType,
                    Description = input.Description,

                };
                await Task.Run(() =>
                {
                    sErr = _projectRepository.Add(project);
                });
                if (sErr != "") goto Get_Out;
                //using (ConfigContext db = new ConfigContext(new DbContextOptions<ConfigContext>()))
                //{
                //    db.Porjects.Add(project);
                //    db.SaveChanges();
                //}
            }
            catch (Exception ex)
            {

                sErr += "\r\n" + ex.Message + "\r\n" + ex.StackTrace;
                goto Get_Out;
            }
        Get_Out:
            if (!String.IsNullOrEmpty(sErr))
            {
                sErr += "\r\nRoutine=" + MethodInfo.GetCurrentMethod().ReflectedType.Name + "." + MethodInfo.GetCurrentMethod().ToString();
                //_logger.LogError(sErr);
                response = Json(new AjaxResponse
                {
                    Succeed = false,
                    Status = "failed",
                    Message = "Create project failed:" + sErr
                });

                return response;
            }

            else
            {
                sErr = "Create Model Success!";
                //_logger.LogInformation(sErr);
                response = Json(new AjaxResponse
                {
                    Succeed = true,
                    Status = "success",
                    Message = "Create project success!"
                });

                return response;
            }

        }

        [HttpGet("GetUserProjects")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<JsonResult> GetAllProjectsbyUserID([FromHeader] string username)
        {
            string sErr = "";
            JsonResult response = null;
            List<Project> allProjects = new List<Project>();
            string userID = "";
            try
            {
                await Task.Run(() =>
                {
                    userID = _userRepository.GetUserID(username);
                });
                if (String.IsNullOrEmpty(userID))
                {
                    sErr += "Cannot find user!";
                    goto Get_Out;
                }
                await Task.Run(() =>
                {
                    allProjects = _projectRepository.GetNonDeletedByUserID(userID);
                });
            }
            catch (Exception ex)
            {

                sErr += ex.Message + "\r\n" + ex.StackTrace;
            }
        Get_Out:
            if (!String.IsNullOrEmpty(sErr))
            {
                response = Json(new AjaxResponse
                {
                    Succeed = false,
                    Status = "failed",
                    Message = "Get projects failed:" + sErr
                });
                return response;
            }
            else
            {
                response = Json(new AjaxResponse
                {
                    Succeed = true,
                    Status = "success",
                    Message = "Get projects success",
                    Data = allProjects.ToArray()
                });
                return response;
            }
        }

        [HttpPost("UpdateProject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<JsonResult> UpdateUserProject([FromHeader] ProjectInput project)
        {
            string sErr = "";
            JsonResult response = null;
            List<Project> allProjects = new List<Project>();
            string userID = "";
            try
            {
                await Task.Run(() =>
                {
                    userID = _userRepository.GetUserID(project.Username);
                });
                if (String.IsNullOrEmpty(userID))
                {
                    sErr += "Cannot find user!";
                    goto Get_Out;
                }
                await Task.Run(() =>
                {
                    allProjects = _projectRepository.GetNonDeletedByUserID(userID);
                });
                var findProject = allProjects.FirstOrDefault(x => x.Name == project.Name);
                findProject.Name = project.UpdatedName;
                findProject.Description = project.UpdateDescription;
                findProject.ProjectType = project.UpdateProjectType;

                await Task.Run(() =>
                {
                    sErr = _projectRepository.Update(findProject);
                });
            }
            catch (Exception ex)
            {

                sErr += ex.Message + "\r\n" + ex.StackTrace;
            }
        Get_Out:
            if (!String.IsNullOrEmpty(sErr))
            {
                response = Json(new AjaxResponse
                {
                    Succeed = false,
                    Status = "failed",
                    Message = "Update projects failed:" + sErr
                });
                return response;
            }
            else
            {
                response = Json(new AjaxResponse
                {
                    Succeed = true,
                    Status = "success",
                    Message = "Update projects success",
                });
                return response;
            }
        }

        [HttpPost("DeleteProject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<JsonResult> DeleteUserProject([FromHeader] ProjectInput project)
        {
            string sErr = "";
            JsonResult response = null;
            List<Project> allProjects = new List<Project>();
            string userID = "";
            try
            {
                await Task.Run(() =>
                {
                    userID = _userRepository.GetUserID(project.Username);
                });
                if (String.IsNullOrEmpty(userID))
                {
                    sErr += "Cannot find user!";
                    goto Get_Out;
                }
                await Task.Run(() =>
                {
                    allProjects = _projectRepository.GetNonDeletedByUserID(userID);
                });
                var findProject = allProjects.FirstOrDefault(x => x.Name == project.Name);

                await Task.Run(() =>
                {
                    sErr = _projectRepository.Delete(findProject);
                });
            }
            catch (Exception ex)
            {

                sErr += ex.Message + "\r\n" + ex.StackTrace;
            }
        Get_Out:
            if (!String.IsNullOrEmpty(sErr))
            {
                response = Json(new AjaxResponse
                {
                    Succeed = false,
                    Status = "failed",
                    Message = "Delete projects failed:" + sErr
                });
                return response;
            }
            else
            {
                response = Json(new AjaxResponse
                {
                    Succeed = true,
                    Status = "success",
                    Message = "Delete projects success",
                });
                return response;
            }
        }


        public class ProjectInput
        {
            public string Username { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public ProjectType ProjectType { get; set; }
            public string UpdatedName { get; set; }
            public string UpdateDescription { get; set; }
            public ProjectType UpdateProjectType { get; set; }
        }
    }

}
