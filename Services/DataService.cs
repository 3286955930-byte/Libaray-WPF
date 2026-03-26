using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using LibrarySystem.Models;

namespace LibrarySystem.Services
{
    public class DataService
    {
        private static readonly string DataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private static readonly string BooksFile = Path.Combine(DataFolder, "books.json");
        private static readonly string ReadersFile = Path.Combine(DataFolder, "readers.json");
        private static readonly string BorrowRecordsFile = Path.Combine(DataFolder, "borrowrecords.json");

        private readonly JavaScriptSerializer _serializer;

        public ObservableCollection<Book> Books { get; private set; }
        public ObservableCollection<Reader> Readers { get; private set; }
        public ObservableCollection<BorrowRecord> BorrowRecords { get; private set; }

        private static DataService _instance;
        public static DataService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DataService();
                }
                return _instance;
            }
        }

        private DataService()
        {
            _serializer = new JavaScriptSerializer();
            Books = new ObservableCollection<Book>();
            Readers = new ObservableCollection<Reader>();
            BorrowRecords = new ObservableCollection<BorrowRecord>();
            LoadData();
        }

        private void EnsureDataFolder()
        {
            if (!Directory.Exists(DataFolder))
            {
                Directory.CreateDirectory(DataFolder);
            }
        }

        public void LoadData()
        {
            EnsureDataFolder();

            try
            {
                if (File.Exists(BooksFile))
                {
                    var json = File.ReadAllText(BooksFile);
                    var books = _serializer.Deserialize<ObservableCollection<Book>>(json);
                    if (books != null) Books = books;
                }
                else
                {
                    InitializeSampleBooks();
                }

                if (File.Exists(ReadersFile))
                {
                    var json = File.ReadAllText(ReadersFile);
                    var readers = _serializer.Deserialize<ObservableCollection<Reader>>(json);
                    if (readers != null) Readers = readers;
                }
                else
                {
                    InitializeSampleReaders();
                }

                if (File.Exists(BorrowRecordsFile))
                {
                    var json = File.ReadAllText(BorrowRecordsFile);
                    var records = _serializer.Deserialize<ObservableCollection<BorrowRecord>>(json);
                    if (records != null) BorrowRecords = records;
                }
            }
            catch
            {
                InitializeSampleBooks();
                InitializeSampleReaders();
            }
        }

        private void InitializeSampleBooks()
        {
            Books = new ObservableCollection<Book>
            {
                new Book { Id = 1, Title = "C#高级编程", Author = "Christian Nagel", ISBN = "9787302550831", Category = "计算机", Stock = 10, PublishDate = new DateTime(2020, 1, 1), CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Book { Id = 2, Title = "Java核心技术", Author = "Cay S. Horstmann", ISBN = "9787111627290", Category = "计算机", Stock = 8, PublishDate = new DateTime(2019, 6, 1), CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Book { Id = 3, Title = "数据结构与算法", Author = "严蔚敏", ISBN = "9787302147510", Category = "计算机", Stock = 15, PublishDate = new DateTime(2018, 3, 1), CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Book { Id = 4, Title = "高等数学", Author = "同济大学", ISBN = "9787040396638", Category = "数学", Stock = 20, PublishDate = new DateTime(2017, 8, 1), CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Book { Id = 5, Title = "大学物理", Author = "程守洙", ISBN = "9787040285749", Category = "物理", Stock = 12, PublishDate = new DateTime(2019, 1, 1), CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };
        }

        private void InitializeSampleReaders()
        {
            Readers = new ObservableCollection<Reader>
            {
                new Reader { Id = 1, Name = "张三", StudentId = "2021001", Phone = "13800138001", Gender = "男", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Reader { Id = 2, Name = "李四", StudentId = "2021002", Phone = "13800138002", Gender = "女", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Reader { Id = 3, Name = "王五", StudentId = "2021003", Phone = "13800138003", Gender = "男", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };
        }

        public void SaveBooks()
        {
            EnsureDataFolder();
            var json = _serializer.Serialize(Books);
            File.WriteAllText(BooksFile, json);
        }

        public void SaveReaders()
        {
            EnsureDataFolder();
            var json = _serializer.Serialize(Readers);
            File.WriteAllText(ReadersFile, json);
        }

        public void SaveBorrowRecords()
        {
            EnsureDataFolder();
            var json = _serializer.Serialize(BorrowRecords);
            File.WriteAllText(BorrowRecordsFile, json);
        }

        public void SaveAll()
        {
            SaveBooks();
            SaveReaders();
            SaveBorrowRecords();
        }

        public int GetNextBookId()
        {
            return Books.Count > 0 ? Books.Max(b => b.Id) + 1 : 1;
        }

        public int GetNextReaderId()
        {
            return Readers.Count > 0 ? Readers.Max(r => r.Id) + 1 : 1;
        }

        public int GetNextBorrowRecordId()
        {
            return BorrowRecords.Count > 0 ? BorrowRecords.Max(br => br.Id) + 1 : 1;
        }
    }
}
