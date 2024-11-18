using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Serialization;
using Project_Manager.Classes;


namespace Project_Manager
{
    class Program
    {
        static void Main()
        {
            Project project1 = new Project("project1", "first project", DateTime.Now, Project.ProjectStatus.Active);
            ProjectTask task11 = new ProjectTask("task1", "task 1 for project 1", DateTime.Now.AddDays(2), ProjectTask.TaskStatus.Active, 120, project1);
            ProjectTask task21 = new ProjectTask("task2", "task 2 for project 1", DateTime.Now.AddDays(5), ProjectTask.TaskStatus.OnHold, 90, project1);

            Project project2 = new Project("project2", "second project", DateTime.Now.AddMonths(-1), Project.ProjectStatus.Finished);
            ProjectTask task12 = new ProjectTask("task1", "task 1 for project 2", DateTime.Now.AddDays(-2), ProjectTask.TaskStatus.Finished, 60, project2);
            ProjectTask task22 = new ProjectTask("task2", "task 2 for project 2", DateTime.Now.AddDays(10), ProjectTask.TaskStatus.OnHold, 90, project2);

            Dictionary<Project, List<ProjectTask>> ProjectList = new Dictionary<Project, List<ProjectTask>>();

            ProjectList[project1] = new List<ProjectTask>() { task11, task21};
            ProjectList[project2] = new List<ProjectTask> { task12, task22};

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
                        PrintAllProjects(ProjectList);
                        break;
                    case "2":
                        AddNewProject(ProjectList);
                        break;
                    case "3":
                        DeleteProject(ProjectList);
                        break;
                    case "4":
                        ViewTasks7Days(ProjectList);
                        break;
                    case "5":
                        ViewFilteredProjects(ProjectList);
                        break;
                    case "7":
                        //ManageTask(ProjectList);
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("neispravan unos, unesite ponovno\n");
                        break;
                }

            }
        }


        static void PrintAllProjects(Dictionary<Project, List<ProjectTask>> ProjectList)
        {
            Console.Clear();
            Console.WriteLine("Izbornik: Svi projekti:\n");

            foreach (var project in ProjectList)
            {
                Console.WriteLine($"\nProjekt: {project.Key.Name} - {project.Key.Status}");
                
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

        static void AddNewProject(Dictionary<Project, List<ProjectTask>> ProjectList)
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

                bool project_exists = ProjectList.Keys.Any(p => p.Name.Equals(new_name, StringComparison.OrdinalIgnoreCase));

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

            DateTime? end_date = null;

            if(status == Project.ProjectStatus.Finished)
            {
                while (true)
                {
                    Console.WriteLine("Unesite datum zavrsetka projekta, format(dd.mm.yyyy):\n");
                    string input = Console.ReadLine();

                    if (string.IsNullOrEmpty(input) || !DateTime.TryParseExact(input, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsed_date))
                    {
                        Console.Clear();
                        Console.WriteLine("Neispravan unos datuma, unesite ponovno\n");
                        continue;
                    }

                    end_date = parsed_date;
                    break;
                }
            }

            Project newProject = new Project(new_name, description, DateTime.Now, status, end_date);
            ProjectList.Add(newProject, new List<ProjectTask>());

            Console.Clear();
            Console.WriteLine($"Projekt {new_name} uspjesno dodan\n");

            while (true)
            {
                Console.WriteLine("\nZelite li dodati zadatak? (yes/no): \n");
                string confirm = Console.ReadLine();

                if(confirm == "yes")
                {
                    Console.Clear();
                    AddTaskToProject(ProjectList, newProject);
                    continue;
                }
                else if (confirm == "no")
                {
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Neispravan unos, pokusajte ponovno.\n");
                }
            }

            Console.WriteLine("\nPritisnite bilo sto za povratak...\n");
            Console.ReadKey();
            Console.Clear();
            return;
        }

        static void DeleteProject(Dictionary<Project, List<ProjectTask>> ProjectList)
        {
            Console.Clear();
            Console.WriteLine("Izbornik: Brisanje projekta:\n");
            Console.WriteLine("Unesite '0' za povratak ili bilo sto drugo za nastavak");
            string choice = Console.ReadLine();

            if (choice == "0")
            {
                Console.Clear();
                return;
            }

            string new_name = "";
            while (true)
            {
                Console.WriteLine("Unesite naziv projekta koji zelite obrisati: ");
                new_name = Console.ReadLine();

                if (String.IsNullOrEmpty(new_name))
                {
                    Console.Clear();
                    Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
                    continue;
                }

                var projectToDelete = ProjectList.Keys.FirstOrDefault(p => p.Name.Equals(new_name, StringComparison.OrdinalIgnoreCase));

                if (projectToDelete == null)
                {
                    Console.Clear();
                    Console.WriteLine("Projekt s tim imenom ne postoji, pokusajte ponovno\n");
                }
                else
                {
                    break;
                }
            }

            while (true)
            {
                Console.WriteLine($"\nZelite li stvarno obrisati {new_name} projekt? (yes/no)\n");
                string confirm = Console.ReadLine().Trim().ToLower();

                if (confirm == "yes")
                {
                    var to_delete = ProjectList.Keys.FirstOrDefault(p => p.Name.Equals(new_name, StringComparison.OrdinalIgnoreCase));

                    ProjectList.Remove(to_delete);

                    Console.Clear();
                    Console.WriteLine($"Uspjesno brisanje projekta {to_delete.Name} i njegovih zadataka\n");
                    break;
                }
                else if (confirm == "no")
                {
                    Console.Clear();
                    Console.WriteLine("Brisanje projekta otkazano\n");
                    return;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Neispravan unos, unesite ponovno\n");
                }
            }

            Console.WriteLine("\nPritisnite bilo sto za povratak...");
            Console.ReadKey();
            Console.Clear();
            return;
        }

        static void ViewTasks7Days(Dictionary<Project, List<ProjectTask>> ProjectList)
        {
            Console.Clear();
            Console.WriteLine("Izbornik: Prikaz svih zadataka ciji je rok za 7 dana\n");

            DateTime date7days = DateTime.Now.AddDays(7);

            bool taskFound = false;

            foreach (var project in ProjectList)
            {
                Console.WriteLine($"\nProjekt: {project.Key.Name} - Status: {project.Key.Status}");

                foreach(var task in project.Value)
                {
                    if(task.Deadline.HasValue && task.Deadline.Value <= date7days && task.Deadline.Value >= DateTime.Now)
                    {
                        Console.WriteLine($"\t\tZadatak: {task.Name} - Rok: {task.Deadline?.ToString("dd.MM.yyyy")} - Status: {task.Status} - Ocekivano trajanje: {task.EstimatedDuration} minuta");
                        taskFound = true;
                    }
                }
            }

            if (!taskFound)
            {
                Console.WriteLine("\nNema zadataka ciji je rok za izvrsavanje za 7 dana\n");
            }

            Console.WriteLine("\nPritisnite bilo sto za povratak...");
            Console.ReadKey();
            Console.Clear();
            return;
        }


        static void ViewFilteredProjects(Dictionary<Project, List<ProjectTask>> ProjectList)
        {
            Console.Clear();

            string choice;
            while (true)
            {
                Console.WriteLine("Izbornik: Prikaz filtriranih projekata\n");
                Console.WriteLine("1 - Aktivni");
                Console.WriteLine("2 - Na cekanju");
                Console.WriteLine("3 - Zavrseni");
                Console.WriteLine("\nUnesite '0' za povratak ili bilo sto drugo za nastavak\n");

                choice = Console.ReadLine();

                if (choice == "0")
                {
                    Console.Clear();
                    return;
                }
                else if (choice == "1" || choice == "2" || choice == "3")
                {
                    Console.Clear();
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Neispravan unos, unesite ponovno\n");
                }
            }

            Project.ProjectStatus selectedStatus;

            if (choice == "1")
                selectedStatus = Project.ProjectStatus.Active;
            else if (choice == "2")
                selectedStatus = Project.ProjectStatus.OnHold;
            else
                selectedStatus = Project.ProjectStatus.Finished;

            var filteredProjects = ProjectList.Where(p => p.Key.Status == selectedStatus).ToList();

            if (filteredProjects.Count > 0)
            {
                Console.WriteLine($"Projekti sa statusom: {selectedStatus}\n");
                foreach (var project in filteredProjects)
                {
                    Console.WriteLine($"{project.Key.Name}");
                }
            }
            else
            {
                Console.WriteLine("Nema projekata s tim statusom\n");
            }

            Console.WriteLine("\nPritisnite bilo sto za povratak...");
            Console.ReadKey();
            Console.Clear();
            return;
        }


        static void AddTaskToProject(Dictionary<Project, List<ProjectTask>> ProjectList, Project new_project)
        {
            Console.Clear();
            Console.WriteLine("Dodaj novi zadatak:\n");
            Console.WriteLine("Unesite '0' za povratak ili bilo sto drugo za nastavak\n");
            string choice = Console.ReadLine();

            if (choice == "0")
            {
                Console.Clear();
                return;
            }

            string taskName = "";
            while (true)
            {
                Console.WriteLine("Unesite naziv zadatka");
                taskName = Console.ReadLine();

                bool task_exists = ProjectList[new_project].Any(task => task.Name == taskName);

                if (String.IsNullOrEmpty(taskName))
                {
                    Console.Clear();
                    Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
                    continue;
                }
                else if (task_exists)
                {
                    Console.Clear();
                    Console.WriteLine("Zadatak vec postoji, pokusajte ponovno\n");
                    continue;
                }
                else
                {
                    break;
                }
            }

            ProjectTask.TaskStatus status = ProjectTask.TaskStatus.Active;
            string status_input;
            while (true)
            {
                Console.WriteLine("Unesite status projekta (Active, OnHold, Finished)");
                status_input = Console.ReadLine();

                if (Enum.TryParse(status_input, out status) && Enum.IsDefined(typeof(ProjectTask.TaskStatus), status))
                {
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Neispravan unos, unesite ponovno\n");
                    continue;
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

            DateTime? end_date = null;

            if (status == ProjectTask.TaskStatus.Finished)
            {
                while (true)
                {
                    Console.WriteLine("Unesite datum zavrsetka zadatka, format(dd.mm.yyyy):\n");
                    string input = Console.ReadLine();

                    if (string.IsNullOrEmpty(input) || !DateTime.TryParseExact(input, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsed_date))
                    {
                        Console.Clear();
                        Console.WriteLine("Neispravan unos datuma, unesite ponovno\n");
                        continue;
                    }

                    end_date = parsed_date;
                    break;
                }
            }

            int estimatedDuration = 0;
            while (true)
            {
                Console.WriteLine("Unesite procijenjeno vrijeme trajanja zadatka u minutama: ");
                string duration_input = Console.ReadLine();

                if(int.TryParse(duration_input, out estimatedDuration) && estimatedDuration > 0)
                {
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Neispravan unos ,unesite ponovno\n");
                }
            }

            DateTime? deadline = null;
            while (true)
            {
                Console.WriteLine("Unesite rok zadatka (format dd.MM.yyyy) ili enter - bez roka");

                string input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                {
                    break;
                }
                else if(DateTime.TryParseExact(input, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDeadline))
                {
                    deadline = parsedDeadline;
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Neispravan unos, unesite ponovno\n");
                }
            }

            ProjectTask newTask = new ProjectTask(taskName, description, deadline, status, estimatedDuration, new_project);

            ProjectList[new_project].Add(newTask);

            Console.Clear();
            Console.WriteLine($"Zadatak {taskName} uspjesno dodan u projekt {new_project}");

            Console.WriteLine("\nPritisnite bilo sto za povratak...");
            Console.ReadKey();
            Console.Clear();
            return;
        }
        

    }
}


