using HealthTrackerProcessor.Class;
using HealthTrackerProcessor.Models;
using HealthTrackerProcessorCore.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace HealthTrackerProcessor.Repositories
{
    public class ProjectRepository : BaseRepository<Project>
    {
        private readonly ILogger _logger;
        private readonly ConfigContext _context;

        public ProjectRepository(ConfigContext context, ILogger<ProjectRepository> logger) : base(context, logger)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            string sErr = "";
            try
            {
                _logger = logger;
                _context = context;
            }
            catch
            {
                goto Get_Out;
            }

        Get_Out:

            if (sErr.Length > 0)
            {
                _logger.LogError(sErr);
            }
        }
        public Project Get(string name, string userID)
        {
            Project result = null;
            List<Project> project = new List<Project>();
            string sErr = "";
            try
            {
                if (String.IsNullOrWhiteSpace(name))
                {
                    sErr += "Name cannot be null!";
                    goto Get_Out;
                }
                if (String.IsNullOrEmpty(userID))
                {
                    sErr += "User info cannot be null!";
                    goto Get_Out;
                }
                project = _context.Porjects.Where(x => x.ID == userID && x.ifDeleted == false).ToList();
                result = project.FirstOrDefault(x => x.Name.ToUpper() == name.ToUpper());

            }
            catch (Exception ex)
            {

                sErr += "\r\n" + ex.Message + "\r\n" + ex.StackTrace;
                goto Get_Out;
            }
        Get_Out:

            if (sErr.Length > 0)
            {
                _logger.LogError(sErr);
            }

            return result;
        }

        public List<Project> GetAll()
        {
            List<Project> projects = new List<Project>();
            string sErr = "";
            try
            {
                projects = _context.Porjects.Where(x => x.ifDeleted == false).ToList();
            }
            catch (Exception ex)
            {

                sErr += "\r\n" + ex.Message + "\r\n" + ex.StackTrace;
                goto Get_Out;
            }
        Get_Out:

            if (sErr.Length > 0)
            {
                _logger.LogError(sErr);
            }

            return projects;
        }

        public List<Project> GetAllDeleted()
        {
            List<Project> projects = new List<Project>();
            string sErr = "";
            try
            {
                projects = _context.Porjects.Where(x => x.ifDeleted == true).ToList();
            }
            catch (Exception ex)
            {

                sErr += "\r\n" + ex.Message + "\r\n" + ex.StackTrace;
                goto Get_Out;
            }
        Get_Out:

            if (sErr.Length > 0)
            {
                _logger.LogError(sErr);
            }

            return projects;
        }

        public List<Project> GetNonDeletedByUserID(string userID)
        {
            List<Project> projects = new List<Project>();
            string sErr = "";
            try
            {
                projects = _context.Porjects.Where(x => x.UserID == userID && x.ifDeleted == false).ToList();
            }
            catch (Exception ex)
            {

                sErr += "\r\n" + ex.Message + "\r\n" + ex.StackTrace;
                goto Get_Out;
            }
        Get_Out:

            if (sErr.Length > 0)
            {
                _logger.LogError(sErr);
            }

            return projects;
        }

        public List<Project> GetDeletedByUserID(string userID)
        {
            List<Project> projects = new List<Project>();
            string sErr = "";
            try
            {
                projects = _context.Porjects.Where(x => x.UserID == userID && x.ifDeleted == true).ToList();
            }
            catch (Exception ex)
            {

                sErr += "\r\n" + ex.Message + "\r\n" + ex.StackTrace;
                goto Get_Out;
            }
        Get_Out:

            if (sErr.Length > 0)
            {
                _logger.LogError(sErr);
            }

            return projects;
        }

        public string Add(Project project)
        {
            string sErr = "";
            try
            {
                if (String.IsNullOrWhiteSpace(project.Name))
                {
                    sErr += "Name cannot be null!";
                    goto Get_Out;
                }

                _context.Porjects.Add(project);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {

                sErr += "\r\n" + ex.Message + "\r\n" + ex.StackTrace;
                goto Get_Out;
            }
        Get_Out:

            if (sErr.Length > 0)
            {
                _logger.LogError(sErr);
            }

            return sErr;
        }
        public string Delete(Project project)
        {
            string sErr = "";
            try
            {
                if (String.IsNullOrWhiteSpace(project.Name))
                {
                    sErr += "Name cannot be null!";
                    goto Get_Out;
                }
                project.ifDeleted = true;
                _context.Porjects.Update(project);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                sErr += "\r\n" + ex.Message + "\r\n" + ex.StackTrace;
                goto Get_Out;
            }
        Get_Out:

            if (sErr.Length > 0)
            {
                _logger.LogError(sErr);
            }

            return sErr;
        }
         public string Update(Project project)
        {
            string sErr = "";
            try
            {
                if (String.IsNullOrWhiteSpace(project.Name))
                {
                    sErr += "Name cannot be null!";
                    goto Get_Out;
                }
                _context.Porjects.Update(project);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                sErr += "\r\n" + ex.Message + "\r\n" + ex.StackTrace;
                goto Get_Out;
            }
        Get_Out:

            if (sErr.Length > 0)
            {
                _logger.LogError(sErr);
            }

            return sErr;
        }
        

    }
}
