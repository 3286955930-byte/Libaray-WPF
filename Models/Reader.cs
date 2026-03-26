using System;

namespace LibrarySystem.Models
{
    public class Reader
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StudentId { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Reader()
        {
            Name = string.Empty;
            StudentId = string.Empty;
            Phone = string.Empty;
            Gender = "男";
        }

        public Reader Clone()
        {
            return new Reader
            {
                Id = this.Id,
                Name = this.Name,
                StudentId = this.StudentId,
                Phone = this.Phone,
                Gender = this.Gender,
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt
            };
        }
    }
}
