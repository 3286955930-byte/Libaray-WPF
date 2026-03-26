using System;

namespace LibrarySystem.Models
{
    public class BorrowRecord
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int ReaderId { get; set; }
        public string BookTitle { get; set; }
        public string ReaderName { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned { get; set; }
        public bool IsOverdue { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public BorrowRecord()
        {
            BookTitle = string.Empty;
            ReaderName = string.Empty;
        }

        public int OverdueDays
        {
            get
            {
                if (IsReturned || DateTime.Now <= DueDate)
                    return 0;
                return (int)(DateTime.Now - DueDate).TotalDays;
            }
        }
    }
}
