using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using LibrarySystem.Commands;
using LibrarySystem.Models;
using LibrarySystem.Services;

namespace LibrarySystem.ViewModels
{
    public class OverdueRecordsViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        
        public ObservableCollection<BorrowRecord> OverdueRecords { get; set; }

        private BorrowRecord _selectedRecord;
        public BorrowRecord SelectedRecord
        {
            get => _selectedRecord;
            set => SetProperty(ref _selectedRecord, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand ReturnCommand { get; }

        public OverdueRecordsViewModel()
        {
            _dataService = DataService.Instance;
            OverdueRecords = new ObservableCollection<BorrowRecord>(
                _dataService.BorrowRecords.Where(br => !br.IsReturned && br.DueDate < DateTime.Now));

            RefreshCommand = new RelayCommand(_ => Refresh());
            ReturnCommand = new RelayCommand<BorrowRecord>(record => ReturnBook(record), record => record != null);
        }

        private void Refresh()
        {
            OverdueRecords = new ObservableCollection<BorrowRecord>(
                _dataService.BorrowRecords.Where(br => !br.IsReturned && br.DueDate < DateTime.Now));
            OnPropertyChanged(nameof(OverdueRecords));
        }

        private void ReturnBook(BorrowRecord record)
        {
            if (record == null) return;

            record.IsReturned = true;
            record.ReturnDate = DateTime.Now;
            record.IsOverdue = true;
            record.UpdatedAt = DateTime.Now;

            var book = _dataService.Books.FirstOrDefault(b => b.Id == record.BookId);
            if (book != null)
            {
                book.Stock++;
            }

            _dataService.SaveAll();
            Refresh();
        }
    }
}
