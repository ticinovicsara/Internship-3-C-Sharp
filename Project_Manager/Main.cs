using System;
using System.Collections.Generic;

using Project_Manager.Classes;


namespace Project_Manager
{
    class Program
    {
        static void Main(string[] args)
        {
            Project project1 = new Project("project1", "first project", DateTime.Now, Project.ProjectStatus.Active);
            ProjectTask task11 = new ProjectTask("task1", "task 1 for project 1", DateTime.Now.AddDays(2), ProjectTask.TaskStatus.Active, 120, project1);
            ProjectTask task21 = new ProjectTask("task2", "task 2 for project 1", DateTime.Now.AddDays(5), ProjectTask.TaskStatus.OnHold, 90, project1);

            Project project2 = new Project("project2", "second project", DateTime.Now.AddMonths(-1), Project.ProjectStatus.Finished);
            ProjectTask task12 = new ProjectTask("task1", "task 1 for project 2", DateTime.Now.AddDays(-2), ProjectTask.TaskStatus.Finished, 60, project2);
            ProjectTask task22 = new ProjectTask("task2", "task 2 for project 2", DateTime.Now.AddDays(10), ProjectTask.TaskStatus.OnHold, 90, project2);

            Dictionary<Project, List<ProjectTask>> projectTasks = new Dictionary<Project, List<ProjectTask>>();

            projectTasks[project1] = new List<ProjectTask>() { task11, task21};
            projectTasks[project2] = new List<ProjectTask> { task12, task22};

            Console.Clear();
            while (true)
            {
                Console.WriteLine("Glavni izbornik\n");
                Console.WriteLine("1 - Ispis svih projekata");
                Console.WriteLine("2 - Dodaj novi projekt");
                Console.WriteLine("3 - Brisanje projekta");
                Console.WriteLine("4 - Svi zadaci, rok za 7 dana");
                Console.WriteLine("5 - Prikaz filtriranih projekata");
                Console.WriteLine("6 - Upravljanje projektom");
                Console.WriteLine("7 - Upravljanje zadatkom\n");

                string choice = Console.ReadLine().Trim();

                switch (choice)
                {
                    case "1":
                        PrintAllProjects(projectTasks);
                        break;
                    case "2":
                        AddNewProject(projectTasks);
                        break;
                    case "3":
                        //DeleteProject();
                        break;
                    case "4":
                        //Tasks7Days();
                        break;
                    case "5":
                        //TaskFilter();
                        break;
                    case "7":
                        //ManageTask();
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("neispravan unos, unesite ponovno\n");
                        break;
                }

            }
        }


        static void PrintAllProjects(Dictionary<Project, List<ProjectTask>> projectTasks)
        {
            Console.Clear();
            Console.WriteLine("Izbornik: Svi projekti:\n");

            foreach (var project in projectTasks)
            {
                Console.WriteLine($"Projekt: {project.Key.Name} - {project.Key.Status}");
                
                foreach (var task in project.Value)
                {
                    Console.WriteLine($"\tZadatak: {task.Name} - Status: {task.Status} - Rok: {task.Deadline?.ToString("dd-mm-yyyy")} - Ocekivano trajanje: {task.EstimatedDuration} minutes");
                }
            }

            Console.WriteLine("\nPritisnite bilo sto za povratak...\n");
            Console.ReadKey();
            Console.Clear();
            return;
        }

        static void AddNewProject(Dictionary<Project, List<ProjectTask>> projectTasks)
        {
            Console.Clear();
            Console.WriteLine("Izbornik: Dodaj novi projekt:\n");
            Console.WriteLine("Unesite '0' za povratak ili bilo sto drugo za nastavak");
            string choice = Console.ReadLine();

            if(choice == "0")
            {
                Console.Clear();
                return;
            }

            string new_name = "";
            while (true)
            {
                Console.WriteLine("Unesite naziv novog projekta: ");
                new_name = Console.ReadLine();

                if (String.IsNullOrEmpty(new_name))
                {
                    Console.Clear();
                    Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
                    continue;
                }

                bool project_exists = projectTasks.Keys.Any(p => p.Name.Equals(new_name, StringComparison.OrdinalIgnoreCase));

                if (project_exists)
                {
                    Console.Clear();
                    Console.WriteLine("Projekt s tim imenom vec postoji, pokusajte ponovno\n");
                }
                else
                {
                    break;
                }
            }

            string description;
            while (true)
            {
                Console.WriteLine("Unesite opis novog projekta");
                description = Console.ReadLine();

                if (String.IsNullOrEmpty(description))
                {
                    Console.Clear();
                    Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
                }
                else
                {
                    break;
                }
            }

            Project.ProjectStatus status = Project.ProjectStatus.Active;
            string status_input;
            while (true)
            {
                Console.WriteLine("Unesite status projekta (Active, OnHold, Finished)");
                status_input = Console.ReadLine();

                if (Enum.TryParse(status_input, out status) && Enum.IsDefined(typeof(Project.ProjectStatus), status))
                {
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Neispravan unos, unesite ponovno\n");
                }
            }

            Project newProject = new Project(new_name, description, DateTime.Now, status);
            projectTasks.Add(newProject, new List<ProjectTask>());

            Console.Clear();
            Console.WriteLine($"Projekt {new_name} uspjesno dodan\n");

            Console.WriteLine("\nPritisnite bilo sto za povratak...\n");
            Console.ReadKey();
            Console.Clear();
            return;
        }
    }
}
