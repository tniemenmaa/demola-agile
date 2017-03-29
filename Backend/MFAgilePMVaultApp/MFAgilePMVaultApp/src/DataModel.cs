using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFAgilePMVaultApp
{
    public class Product
    {
        public int InternalId { get; set;  }
        public string NameOrTitle { get; set; }
    }

    public class Backlog
    {
        public int InternalId { get; set; }
        public string NameOrTitle { get; set; }
        public string Description { get; set; }
        public int BacklogType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class UserStory
    {
        public int InternalId { get; set; }
        public int UserStoryId { get; set; }
        public string NameOrTitle { get; set; }
        public string Description { get; set; }
        public int UserStoryState { get; set; }
        public int StoryPoints { get; set; }
        public Person ResponsiblePerson { get; set; }
        public List<Task> TaskList { get; set; }
    }

    public class Task
    {
        public int InternalId { get; set; }
        public string NameOrTitle { get; set; }
        public string Description { get; set; }
        public int TaskState { get; set; }
        public int HoursRemaining { get; set; }
        public Person ResponsiblePerson { get; set; }
    }

    public class Feature
    {
        public int InternalId { get; set; }
        public string NameOrTitle { get; set; }
        public string Description { get; set; }
        public List<UserStory> UserStoryList;
    }

    public class Team
    {
        public string NameOrtitle { get; set; }
        public Person TeamLeader { get; set; }
        public List<Person> TeamMembers { get; set; }
        Sprint Sprint { get; set; }
    }

    public class Sprint
    {
        public string NameOrTitle { get; set; } // Actually Sprint name in Metadata
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public  List<UserStory> UserStoryList { get; set; }
    }

    public class Person
    {
        public string PersonName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

}
