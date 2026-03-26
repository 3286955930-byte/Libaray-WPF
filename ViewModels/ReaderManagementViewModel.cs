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
    public class ReaderManagementViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        
        public ObservableCollection<Reader> Readers { get; set; }

        private Reader _selectedReader;
        public Reader SelectedReader
        {
            get => _selectedReader;
            set => SetProperty(ref _selectedReader, value);
        }

        private Reader _editingReader;
        public Reader EditingReader
        {
            get => _editingReader;
            set => SetProperty(ref _editingReader, value);
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

        public string[] GenderOptions { get; } = new string[] { "男", "女" };

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ReaderManagementViewModel()
        {
            _dataService = DataService.Instance;
            Readers = _dataService.Readers;

            AddCommand = new RelayCommand(_ => OpenAddDialog());
            EditCommand = new RelayCommand<Reader>(reader => OpenEditDialog(reader), reader => reader != null);
            DeleteCommand = new RelayCommand<Reader>(reader => DeleteReader(reader), reader => reader != null);
            SearchCommand = new RelayCommand(_ => Search());
            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        private void OpenAddDialog()
        {
            DialogTitle = "添加读者";
            EditingReader = new Reader();
            IsDialogOpen = true;
        }

        private void OpenEditDialog(Reader reader)
        {
            DialogTitle = "编辑读者";
            EditingReader = reader.Clone();
            IsDialogOpen = true;
        }

        private void DeleteReader(Reader reader)
        {
            var result = MessageBox.Show($"确定要删除读者 \"{reader.Name}\" 吗？", "确认删除", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                _dataService.Readers.Remove(reader);
                _dataService.SaveReaders();
                MessageBox.Show("删除成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Readers = _dataService.Readers;
            }
            else
            {
                var filtered = new ObservableCollection<Reader>(
                    _dataService.Readers.Where(r => 
                        r.Name.Contains(SearchText) || 
                        r.StudentId.Contains(SearchText) ||
                        r.Phone.Contains(SearchText)));
                Readers = filtered;
            }
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(EditingReader.Name))
            {
                MessageBox.Show("请输入姓名！", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(EditingReader.StudentId))
            {
                MessageBox.Show("请输入学号！", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (EditingReader.Id > 0)
            {
                var existing = _dataService.Readers.FirstOrDefault(r => r.Id == EditingReader.Id);
                if (existing != null)
                {
                    existing.Name = EditingReader.Name;
                    existing.StudentId = EditingReader.StudentId;
                    existing.Phone = EditingReader.Phone;
                    existing.Gender = EditingReader.Gender;
                    existing.UpdatedAt = DateTime.Now;
                }
            }
            else
            {
                EditingReader.Id = _dataService.GetNextReaderId();
                EditingReader.CreatedAt = DateTime.Now;
                EditingReader.UpdatedAt = DateTime.Now;
                _dataService.Readers.Add(EditingReader);
            }
            
            _dataService.SaveReaders();
            IsDialogOpen = false;
            MessageBox.Show(EditingReader.Id > 0 ? "修改成功！" : "添加成功！", "成功", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Cancel()
        {
            IsDialogOpen = false;
        }
    }
}
