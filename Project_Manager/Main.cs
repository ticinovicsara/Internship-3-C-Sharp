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
            ProjectTask task11 = new ProjectTask("task11", "task 1 for project 1", DateTime.Now.AddDays(2), ProjectTask.TaskStatus.Active, 120, project1);
            ProjectTask task12 = new ProjectTask("task12", "task 2 for project 1", DateTime.Now.AddDays(5), ProjectTask.TaskStatus.OnHold, 90, project1);
            ProjectTask task13 = new ProjectTask("task13", "task 3 for project 2", DateTime.Now.AddMonths(-2), ProjectTask.TaskStatus.Finished, 60, project1);

            Project project2 = new Project("project2", "second project", DateTime.Now.AddMonths(-1), Project.ProjectStatus.OnHold);
            ProjectTask task21 = new ProjectTask("task21", "task 1 for project 2", DateTime.Now.AddDays(20), ProjectTask.TaskStatus.Active, 120, project2);
            ProjectTask task22 = new ProjectTask("task22", "task 2 for project 2", DateTime.Now.AddDays(-15), ProjectTask.TaskStatus.Finished, 60, project2);
            ProjectTask task23 = new ProjectTask("task23", "task 3 for project 2", DateTime.Now.AddDays(7), ProjectTask.TaskStatus.OnHold, 90, project2);

            Project project3 = new Project("project2", "second project", DateTime.Now.AddMonths(-1), Project.ProjectStatus.Finished);
            ProjectTask task31 = new ProjectTask("task31", "task 1 for project 3", DateTime.Now.AddDays(-20), ProjectTask.TaskStatus.Finished, 120, project3);
            ProjectTask task32 = new ProjectTask("task32", "task 2 for project 3", DateTime.Now.AddDays(-15), ProjectTask.TaskStatus.Finished, 60, project3);
            ProjectTask task33 = new ProjectTask("task33", "task 3 for project 3", DateTime.Now.AddDays(-70), ProjectTask.TaskStatus.Finished, 90, project3);

            Dictionary<Project, List<ProjectTask>> ProjectList = new Dictionary<Project, List<ProjectTask>>();

            ProjectList[project1] = new List<ProjectTask>() { task11, task12, task13};
            ProjectList[project2] = new List<ProjectTask> { task21, task22, task23};
            ProjectList[project3] = new List<ProjectTask> { task31, task32, task33 };


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
                    case "6":
                        ManageProject(ProjectList);
                        break;
                    case "7":
                        ManageTask(ProjectList);
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
                    Console.WriteLine($"\tZadatak: {task.Name} - Status: {task.Status} - Rok: {task.Deadline?.ToString("dd-MM-yyyy")} - Ocekivano trajanje: {task.EstimatedDuration} minutes");
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
            Console.WriteLine("Izbornik: Brisanje projekta\n");
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
                Console.WriteLine("Unesite status zadatka (Active, OnHold, Finished)");
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

            if(status != ProjectTask.TaskStatus.Finished)
            {
                while (true)
                {
                    Console.WriteLine("Unesite rok zadatka (format dd.MM.yyyy) ili enter - bez roka");

                    string input = Console.ReadLine();

                    if (string.IsNullOrEmpty(input))
                    {
                        break;
                    }
                    else if (DateTime.TryParseExact(input, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDeadline))
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
            }
            

            ProjectTask newTask = new ProjectTask(taskName, description, deadline, status, estimatedDuration, new_project);

            ProjectList[new_project].Add(newTask);

            Console.Clear();
            Console.WriteLine($"Zadatak {taskName} uspjesno dodan u projekt {new_project.Name}");

            Console.WriteLine("\nPritisnite bilo sto za povratak...");
            Console.ReadKey();
            Console.Clear();
            return;
        }


        static void ManageProject(Dictionary<Project, List<ProjectTask>> ProjectList)
        {
            Console.Clear();
            Console.WriteLine("Izbornik: Upravljanje projektom\n");

            string name = "";
            Project findProject = null;

            while (true)
            {
                Console.WriteLine("Unesite naziv projekta: \n");
                name = Console.ReadLine();

                if (String.IsNullOrEmpty(name))
                {
                    Console.Clear();
                    Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
                    continue;
                }

                findProject = ProjectList.Keys.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                if (findProject == null)
                {
                    Console.Clear();
                    Console.WriteLine("Projekt s tim imenom ne postoji, pokusajte ponovno\n");
                }
                else
                {
                    break;
                }
            }

            if(findProject != null)
            {
                Console.Clear();
                List<ProjectTask> tasksForProject = ProjectList[findProject];
                Console.WriteLine($"Odabrani projekt: {findProject.Name}\n");
            }
            
         
            while (true)
            {
                Console.WriteLine("\n1 -  Ispis svih zadataka unutar projekta");
                Console.WriteLine("2 -  Prikaz detalja projekta");
                Console.WriteLine("3 -  Uredjivanje statusa projekta");
                Console.WriteLine("4 -  Dodavanje zadatka");
                Console.WriteLine("5 -  Brisanje zadatka");
                Console.WriteLine("6 -  Prikaz ukupno ocekivanog vremena za zavrsetak projekta");
                Console.WriteLine("\n0 -  povratak\n");

                string choice = Console.ReadLine();

                if (choice == "0")
                {
                    Console.Clear();
                    return;
                }

                switch (choice)
                {
                    case "1":
                        PrintAllTasks(ProjectList, findProject);
                        break;
                    case "2":
                        ViewProject(ProjectList, findProject);
                        break;
                    case "3":
                        EditStatus(ProjectList, findProject);
                        break;
                    case "4":
                        AddTaskToProject(ProjectList, findProject);
                        break;
                    case "5":
                        DeleteTaskFromProject(ProjectList, findProject);
                        break;
                    case "6":
                        ViewActiveTasksTime(ProjectList, findProject);
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Neispravan unos, unesite ponovno\n");
                        break;
                }

            }

        }

        static void PrintAllTasks(Dictionary<Project, List<ProjectTask>> ProjectList, Project curr_project)
        {
            Console.Clear();
            Console.WriteLine($"Svi zadaci unutar projekta: {curr_project.Name}:\n");

            foreach (var task in ProjectList[curr_project])
            {
                Console.WriteLine($"Zadatak: {task.Name} - Status: {task.Status} - Rok: {task.Deadline?.ToString("dd-MM-yyyy")} - Ocekivano trajanje: {task.EstimatedDuration} minutes");
            }

            Console.WriteLine("\nPritisnite bilo sto za povratak...\n");
            Console.ReadKey();
            Console.Clear();
            return;
        }

        static void ViewProject(Dictionary<Project, List<ProjectTask>> ProjectList, Project curr_project)
        {
            Console.Clear();
            Console.WriteLine($"Prikaz detalja projekta {curr_project.Name}\n");

            foreach (var task in ProjectList[curr_project])
            {
                Console.WriteLine($"\tZadatak: {task.Name} - Status: {task.Status} - Rok: {task.Deadline?.ToString("dd-MM-yyyy")} - Ocekivano trajanje: {task.EstimatedDuration} minutes");
            }

            Console.WriteLine("\nPritisnite bilo sto za povratak...\n");
            Console.ReadKey();
            Console.Clear();
            return;
        }

        static void EditStatus(Dictionary<Project, List<ProjectTask>> ProjectList, Project curr_project)
        {
            Console.Clear();
            Console.WriteLine($"Uredjivanje statusa projekta {curr_project.Name}\n");

            ProjectTask.TaskStatus status = ProjectTask.TaskStatus.Active;
            string status_input;
            while (true)
            {
                Console.WriteLine("Unesite status projekta (Active, OnHold, Finished)");
                status_input = Console.ReadLine();

                if (Enum.TryParse(status_input, out status) && Enum.IsDefined(typeof(ProjectTask.TaskStatus), status))
                {
                    if (curr_project.Status == Project.ProjectStatus.Finished)
                    {
                        Console.WriteLine("Zavrsen projekt ne moze mijenjati status.");
                        Console.WriteLine("\nPritisnite bilo sto za povratak...");
                        Console.ReadKey();
                        Console.Clear();
                        return;
                    }

                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Neispravan unos, unesite ponovno\n");
                    continue;
                }
            }


        }

        static void DeleteTaskFromProject(Dictionary<Project, List<ProjectTask>> ProjectList, Project curr_project)
        {
            Console.Clear();
            Console.WriteLine($"Brisanje zadatka iz projekta {curr_project.Name}\n");


            var validTasks = ProjectList[curr_project].Select(t => t.Name).ToList();

            string name = "";
            while (true)
            {
                Console.WriteLine("Zadaci:");
                foreach (var task in ProjectList[curr_project])
                {
                    Console.WriteLine($"\t{task.Name} - Status: {task.Status} - Rok: {task.Deadline?.ToString("dd-MM-yyyy")} - Ocekivano trajanje: {task.EstimatedDuration} minutes");
                }

                Console.WriteLine("\nUnesite zadatak koji zelite izbrisati\n");
                name = Console.ReadLine();

                if (string.IsNullOrEmpty(name) || !validTasks.Contains(name))
                {
                    Console.Clear();
                    Console.WriteLine("Neispravan unos, unesite ponovno\n");
                    continue;
                }
                else
                {
                    break;
                }
            }

            var to_delete = ProjectList[curr_project].FirstOrDefault(t => t.Name == name);
            if (to_delete != null)
            {
                while (true)
                {
                    Console.WriteLine($"Jeste li sigurni da zelite izbrisati zadatak '{to_delete}' (yes/no)?");
                    string confirmation = Console.ReadLine();

                    if (confirmation.ToLower() == "no")
                    {
                        Console.WriteLine("Brisanje otkazano.");
                        Console.WriteLine("\nPritisnite bilo sto za povratak...");
                        Console.ReadKey();
                        Console.Clear();
                        return;
                    }
                    else if(confirmation.ToLower() == "yes")
                    {
                        break;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Neispravan unos, unesite ponovno\n");
                    }
                }

                ProjectList[curr_project].Remove(to_delete);

                Console.Clear();
                Console.WriteLine($"Zadatak {name} je uspjesno izbrisan iz projekta {curr_project.Name}");
            }

            else
            {
                Console.Clear();
                Console.WriteLine("Doslo je po pogreske\n");
            }

            Console.WriteLine("\nPritisnite bilo sto za povratak...\n");
            Console.ReadKey();
            Console.Clear();
            return;
        }

        static void ViewActiveTasksTime(Dictionary<Project, List<ProjectTask>> ProjectList, Project curr_project)
        {
            Console.Clear();
            Console.WriteLine($"Prikaz ukupnog ocekivanog vremena za aktivne zadatke u projektu {curr_project.Name}");

            int time = 0;

            foreach (var task in ProjectList[curr_project])
            {
                if (task.Status == ProjectTask.TaskStatus.Active)
                {
                    time += task.EstimatedDuration;
                }
            }
            if(time == 0)
            {
                Console.WriteLine("Nema aktivnih zadtaka u ovom projektu");
            }
            else
            {
                Console.WriteLine($"Ukupno vrijeme je: {time / 60} sati i {time % 60} minuta\n");
            }

            Console.WriteLine("\nPritisnite bilo sto za povratak...\n");
            Console.ReadKey();
            Console.Clear();
            return;
        }

        static void ManageTask(Dictionary<Project, List<ProjectTask>> ProjectList)
        {
            Console.Clear();

            string choice;
            while (true)
            {
                Console.WriteLine("Upravljanje pojedinim zadatkom\n");
                Console.WriteLine("1 - prikaz detalja odabranog zadatka");
                Console.WriteLine("2 - uredjivanje status zadatka");
                Console.WriteLine("\n0 - povratak\n");

                choice = Console.ReadLine();

                if (choice == "0")
                {
                    Console.Clear();
                    return;
                }
                else if (choice == "1" || choice == "2")
                {
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("neispravan unos, unesite ponovno\n");
                }
            }

            Console.Clear();
           
            var allTasks = ProjectList.Values.SelectMany(tasks => tasks).ToList();
            var validNames = allTasks.Select(task => task.Name).ToList();

            string taskName = "";
            while (true)
            {
                Console.WriteLine("Zadaci:");
                foreach (var taskList in ProjectList.Values)
                {
                    foreach (var task in taskList)
                    {
                        Console.WriteLine($"\t{task.Name} - Status: {task.Status} - Rok: {task.Deadline?.ToString("dd-MM-yyyy")} - Ocekivano trajanje: {task.EstimatedDuration} minutes");
                    }
                    Console.WriteLine("\n");
                }

                Console.WriteLine("\nUnesite naziv zadatka:\n");
                taskName = Console.ReadLine();

                if(string.IsNullOrEmpty(taskName) || !validNames.Contains(taskName))
                {
                    Console.Clear();
                    Console.WriteLine("Zadatak nije pronadjen, pokusajte ponovno\n");
                    continue;
                }
                break;
            }

            var selectedTask = allTasks.FirstOrDefault(t =>  t.Name.Equals(taskName, StringComparison.OrdinalIgnoreCase));
            
            if (selectedTask.Status == ProjectTask.TaskStatus.Finished)
            {
                Console.Clear();
                Console.WriteLine($"Zadatak '{selectedTask.Name}' je zavrsen i ne moze se mijenjati.");
                Console.WriteLine("\nPritisnite bilo sto za povratak...");
                Console.ReadKey();
                Console.Clear();
                return;
            }


            if (choice == "1")
            {
                Console.Clear();
                Console.WriteLine($"Detalji zadatka '{selectedTask.Name}':\n");
                Console.WriteLine($"\nProjekt: {selectedTask.Project.Name}");
                Console.WriteLine($"\tOpis: {selectedTask.Description}");
                Console.WriteLine($"\tStatus: {selectedTask.Status}");
                Console.WriteLine($"\tRok: {selectedTask.Deadline?.ToString("dd-MM-yyyy")}");
                Console.WriteLine($"\tOcekivano trajanje: {selectedTask.EstimatedDuration} minutes\n");

            }


            else if (choice == "2")
            {
                Console.Clear();
                while (true)
                {
                    Console.WriteLine($"Trenutni status zadatka '{selectedTask.Name}' je '{selectedTask.Status}'\n");
                    Console.WriteLine("Unesite novi status (Active, OnHold, Finished):\n");

                    string status_input = Console.ReadLine();
                    if (Enum.TryParse(status_input, true, out ProjectTask.TaskStatus newStatus))
                    {
                        selectedTask.Status = newStatus;

                        Console.Clear();
                        Console.WriteLine($"\nStatus zadatka '{selectedTask.Name}' je uspjesno promijenjen na {newStatus}\n");
                        break;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
                        continue;
                    }
                }

            }

            Console.WriteLine("\nPritisnite bilo sto za povratak...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}