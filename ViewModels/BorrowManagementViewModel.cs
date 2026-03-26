using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using LibrarySystem.Commands;
using LibrarySystem.Models;
using LibrarySystem.Services;

namespace LibrarySystem.ViewModels
{
    public class BorrowManagementViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        
        public ObservableCollection<BorrowRecord> BorrowRecords { get; set; }
        public ObservableCollection<Book> AvailableBooks { get; set; }
        public ObservableCollection<Reader> AvailableReaders { get; set; }

        private BorrowRecord _selectedRecord;
        public BorrowRecord SelectedRecord
        {
            get => _selectedRecord;
            set => SetProperty(ref _selectedRecord, value);
        }

        private Book _selectedBook;
        public Book SelectedBook
        {
            get => _selectedBook;
            set => SetProperty(ref _selectedBook, value);
        }

        private Reader _selectedReader;
        public Reader SelectedReader
        {
            get => _selectedReader;
            set => SetProperty(ref _selectedReader, value);
        }

        private int _borrowDays = 30;
        public int BorrowDays
        {
            get => _borrowDays;
            set => SetProperty(ref _borrowDays, value);
        }

        private bool _isBorrowDialogOpen;
        public bool IsBorrowDialogOpen
        {
            get => _isBorrowDialogOpen;
            set => SetProperty(ref _isBorrowDialogOpen, value);
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public ICommand BorrowCommand { get; }
        public ICommand ReturnCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand SaveBorrowCommand { get; }
        public ICommand CancelBorrowCommand { get; }

        public BorrowManagementViewModel()
        {
            _dataService = DataService.Instance;
            BorrowRecords = _dataService.BorrowRecords;
            AvailableBooks = new ObservableCollection<Book>(_dataService.Books.Where(b => b.Stock > 0));
            AvailableReaders = _dataService.Readers;

            BorrowCommand = new RelayCommand(_ => OpenBorrowDialog());
            ReturnCommand = new RelayCommand<BorrowRecord>(record => ReturnBook(record), record => record != null && !record.IsReturned);
            SearchCommand = new RelayCommand(_ => Search());
            SaveBorrowCommand = new RelayCommand(_ => SaveBorrow());
            CancelBorrowCommand = new RelayCommand(_ => CancelBorrow());
        }

        private void OpenBorrowDialog()
        {
            SelectedBook = null;
            SelectedReader = null;
            BorrowDays = 30;
            IsBorrowDialogOpen = true;
        }

        private void SaveBorrow()
        {
            if (SelectedBook == null)
            {
                MessageBox.Show("请选择要借阅的图书！", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedReader == null)
            {
                MessageBox.Show("请选择读者！", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedBook.Stock <= 0)
            {
                MessageBox.Show("该图书库存不足！", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var record = new BorrowRecord
            {
                Id = _dataService.GetNextBorrowRecordId(),
                BookId = SelectedBook.Id,
                ReaderId = SelectedReader.Id,
                BookTitle = SelectedBook.Title,
                ReaderName = SelectedReader.Name,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(BorrowDays),
                IsReturned = false,
                IsOverdue = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            SelectedBook.Stock--;
            _dataService.BorrowRecords.Add(record);
            _dataService.SaveAll();

            IsBorrowDialogOpen = false;
            AvailableBooks = new ObservableCollection<Book>(_dataService.Books.Where(b => b.Stock > 0));
            OnPropertyChanged(nameof(AvailableBooks));
            MessageBox.Show("借书成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ReturnBook(BorrowRecord record)
        {
            if (record == null) return;

            var result = MessageBox.Show($"确定要归还《{record.BookTitle}》吗？", "确认归还", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                record.IsReturned = true;
                record.ReturnDate = DateTime.Now;
                record.IsOverdue = DateTime.Now > record.DueDate;
                record.UpdatedAt = DateTime.Now;

                var book = _dataService.Books.FirstOrDefault(b => b.Id == record.BookId);
                if (book != null)
                {
                    book.Stock++;
                }

                _dataService.SaveAll();
                AvailableBooks = new ObservableCollection<Book>(_dataService.Books.Where(b => b.Stock > 0));
                OnPropertyChanged(nameof(AvailableBooks));
                MessageBox.Show("还书成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CancelBorrow()
        {
            IsBorrowDialogOpen = false;
        }

        private void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                BorrowRecords = _dataService.BorrowRecords;
            }
            else
            {
                var filtered = new ObservableCollection<BorrowRecord>(
                    _dataService.BorrowRecords.Where(br => 
                        br.BookTitle.Contains(SearchText) || 
                        br.ReaderName.Contains(SearchText)));
                BorrowRecords = filtered;
            }
        }
    }
}
