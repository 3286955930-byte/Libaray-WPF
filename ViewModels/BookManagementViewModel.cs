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
    public class BookManagementViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        
        private ObservableCollection<Book> _books;
        public ObservableCollection<Book> Books
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }

        private Book _selectedBook;
        public Book SelectedBook
        {
            get => _selectedBook;
            set => SetProperty(ref _selectedBook, value);
        }

        private Book _editingBook;
        public Book EditingBook
        {
            get => _editingBook;
            set => SetProperty(ref _editingBook, value);
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        private bool _isDialogOpen;
        public bool IsDialogOpen
        {
            get => _isDialogOpen;
            set => SetProperty(ref _isDialogOpen, value);
        }

        private string _dialogTitle;
        public string DialogTitle
        {
            get => _dialogTitle;
            set => SetProperty(ref _dialogTitle, value);
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public BookManagementViewModel()
        {
            _dataService = DataService.Instance;
            Books = _dataService.Books;

            AddCommand = new RelayCommand(_ => OpenAddDialog());
            EditCommand = new RelayCommand<Book>(book => OpenEditDialog(book), book => book != null);
            DeleteCommand = new RelayCommand<Book>(book => DeleteBook(book), book => book != null);
            SearchCommand = new RelayCommand(_ => Search());
            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        private void OpenAddDialog()
        {
            DialogTitle = "添加图书";
            EditingBook = new Book
            {
                PublishDate = DateTime.Now
            };
            IsDialogOpen = true;
        }

        private void OpenEditDialog(Book book)
        {
            if (book == null) return;
            DialogTitle = "编辑图书";
            EditingBook = book.Clone();
            IsDialogOpen = true;
        }

        private void DeleteBook(Book book)
        {
            if (book == null) return;

            var result = MessageBox.Show($"确定要删除图书《{book.Title}》吗？", "确认删除", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _dataService.Books.Remove(book);
                _dataService.SaveBooks();
                MessageBox.Show("删除成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Books = _dataService.Books;
            }
            else
            {
                var filtered = new ObservableCollection<Book>(
                    _dataService.Books.Where(b => 
                        b.Title.Contains(SearchText) || 
                        b.Author.Contains(SearchText) || 
                        b.ISBN.Contains(SearchText) ||
                        b.Category.Contains(SearchText)));
                Books = filtered;
            }
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(EditingBook.Title))
            {
                MessageBox.Show("请输入书名！", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(EditingBook.Author))
            {
                MessageBox.Show("请输入作者！", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (EditingBook.Id > 0)
            {
                var existing = _dataService.Books.FirstOrDefault(b => b.Id == EditingBook.Id);
                if (existing != null)
                {
                    existing.Title = EditingBook.Title;
                    existing.Author = EditingBook.Author;
                    existing.ISBN = EditingBook.ISBN;
                    existing.Category = EditingBook.Category;
                    existing.Stock = EditingBook.Stock;
                    existing.PublishDate = EditingBook.PublishDate;
                    existing.UpdatedAt = DateTime.Now;
                }
            }
            else
            {
                EditingBook.Id = _dataService.GetNextBookId();
                EditingBook.CreatedAt = DateTime.Now;
                EditingBook.UpdatedAt = DateTime.Now;
                _dataService.Books.Add(EditingBook);
            }

            _dataService.SaveBooks();
            IsDialogOpen = false;
            MessageBox.Show(EditingBook.Id > 0 ? "修改成功！" : "添加成功！", "提示", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Cancel()
        {
            IsDialogOpen = false;
        }
    }
}
