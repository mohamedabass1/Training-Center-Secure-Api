namespace TrainingCenter.Domain.Entities
{
    public class StudentProfile
    {

        // Primary Key + Foreign Key
        // ------------------------------------------------------
        // 🔹 Primary Key of StudentProfiles table
        // 🔹 Foreign Key linked to Students table
        //
        // This creates a One-to-One relationship.
        // One student can have only one profile.
        public int StudentId { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Bio { get; set; }
        public string? LinkedInUrl { get; set; }


        // Each profile belongs to one student.
        public Student Student { get; set; } = null!;

    }

}
