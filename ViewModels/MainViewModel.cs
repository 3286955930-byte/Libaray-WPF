using System.Windows.Input;
using LibrarySystem.Commands;
using LibrarySystem.Views;

namespace LibrarySystem.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public ICommand NavigateToBooksCommand { get; }
        public ICommand NavigateToReadersCommand { get; }
        public ICommand NavigateToBorrowsCommand { get; }
        public ICommand NavigateToOverdueCommand { get; }

        public MainViewModel()
        {
            NavigateToBooksCommand = new RelayCommand(_ => NavigateToBooks());
            NavigateToReadersCommand = new RelayCommand(_ => NavigateToReaders());
            NavigateToBorrowsCommand = new RelayCommand(_ => NavigateToBorrows());
            NavigateToOverdueCommand = new RelayCommand(_ => NavigateToOverdue());

            NavigateToBooks();
        }

        private void NavigateToBooks()
        {
            CurrentView = new BookManagementView();
        }

        private void NavigateToReaders()
        {
            CurrentView = new ReaderManagementView();
        }

        private void NavigateToBorrows()
        {
            CurrentView = new BorrowManagementView();
        }

        private void NavigateToOverdue()
        {
            CurrentView = new OverdueRecordsView();
        }
    }
}
