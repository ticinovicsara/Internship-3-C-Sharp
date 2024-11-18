using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Manager.Classes
{
    public class ProjectTask
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public TaskStatus Status { get; set; }
        public int EstimatedDuration { get; set; }
        public Project Project { get; set; }

        public enum TaskStatus
        {
            Active,
            Finished,
            OnHold
        }

        public ProjectTask(string name, string description, DateTime? deadline, TaskStatus status, int duration, Project projectName) 
        {
            Name = name;
            Description = description;
            Deadline = deadline;
            Status = status;
            EstimatedDuration = duration;
            Project = projectName;
        }
    }
}
