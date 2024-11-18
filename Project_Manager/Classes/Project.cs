using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Manager.Classes
{
    public class Project
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate {  get; set; }
        public DateTime? EndDate { get; set; }
        public ProjectStatus Status {  get; set; }

        public enum ProjectStatus
        {
            Active,
            OnHold,
            Finished
        }

        public Project(string name, string desctiption, DateTime start, ProjectStatus status, DateTime? endDate = null)
        {
            Name = name;
            Description = desctiption;
            StartDate = start;
            Status = status;
            EndDate = endDate ?? DateTime.MinValue;
        }

    }
}
