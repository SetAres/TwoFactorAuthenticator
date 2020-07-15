using Authenticator;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using TwoFactorAuthenticator.Commands;
using TwoFactorAuthenticator.Data;
using TwoFactorAuthenticator.Models;
using TwoFactorAuthenticator.View;

namespace TwoFactorAuthenticator.ViewModel
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Properties
        private Account selectedAccount;
        public Account SelectedAccount
        {
            get { return selectedAccount; }
            set
            {
                selectedAccount = value;

                if (selectedAccount != null)
                {
                    Clipboard.SetText(selectedAccount.Code);
                    IsRemoveButtonEnabled = true;
                }
                else
                    IsRemoveButtonEnabled = false;

                OnPropertyChanged();
            }
        }


        private ObservableCollection<Account> accounts;
        public ObservableCollection<Account> Accounts
        {
            get { return accounts; }
            set
            {
                accounts = value;
                OnPropertyChanged();
            }
        }

        private ICollectionView accountFilter;
        public ICollectionView AccountFilter
        {
            get { return accountFilter; }
            set
            {
                accountFilter = value;
                OnPropertyChanged();
            }
        }

        private string accountFilterText;
        public string AccountFilterText
        {
            get => accountFilterText;
            set
            {
                if (value != accountFilterText)
                {
                    accountFilterText = value;
                    OnPropertyChanged();
                }

                accountFilter = CollectionViewSource.GetDefaultView(Accounts);
                accountFilter.Filter = o =>
                {
                    if (((Account)o).Email.ToLower().Contains(value.ToLower()) || ((Account)o).Title.ToLower().Contains(value.ToLower()))
                        return true;
                    else
                        return false;
                };
            }
        }
        #endregion

        #region Commands
        private bool IsAddNewButtonEnabled = true;
        private RelayCommand addNew;
        public RelayCommand AddNew
        {
            get
            {
                return addNew ?? (addNew = new RelayCommand((obj) =>
                {
                    IsAddNewButtonEnabled = false;

                    // Instantiate the dialog box
                    AddingNewAccount dlg = new AddingNewAccount();

                    // Open the dialog box modally
                    dlg.ShowDialog();



                    // Здесь присутствует проблема передачи данных из дочернего окна
                    if (dlg.DialogResult == true)
                    {
                        AccountSettings accountInfo = new AccountSettings(dlg.AccountTextBox.Text, dlg.TitleTextBox.Text, dlg.SecretKeyTextBox.Text);
                        accounts.Add(new Account(accountInfo));

                        using (var db = new ApplicationDbContext())
                        {
                            db.Accounts.Add(accountInfo);
                            db.SaveChanges();
                        }
                    }

                    AddNew.Invalidate();
                    IsAddNewButtonEnabled = true;
                },
                CanExecute => IsAddNewButtonEnabled));
            }
        }

        private bool IsRemoveButtonEnabled = false;
        private RelayCommand remove;
        public RelayCommand Remove
        {
            get
            {
                return remove ?? (remove = new RelayCommand((obj) =>
                {
                    IsRemoveButtonEnabled = false;
                    using (var db = new ApplicationDbContext())
                    {
                        AccountSettings accountInfoToRemove = db.Accounts.ToList().Find(x=>x.Title == selectedAccount.Title);
                        db.Accounts.Remove(accountInfoToRemove);
                        db.SaveChanges();
                    }

                    accounts.Remove(selectedAccount);

                    selectedAccount = null;
                    Remove.Invalidate();
                },
                CanExecute => IsRemoveButtonEnabled));
            }
        }
        #endregion

        public ApplicationViewModel()
        {
            using (var db = new ApplicationDbContext())
            {
                //db.Database.Migrate();
                //db.Accounts.Add(new AccountSettings("Sample", "email@mail.com", "GBROGKINERGIENOE"));
                //db.SaveChanges();

                accounts = new ObservableCollection<Account>();
                foreach (var accountInfo in db.Accounts.ToList())
                    accounts.Add(new Account(accountInfo));
            }
        }
    }
}
