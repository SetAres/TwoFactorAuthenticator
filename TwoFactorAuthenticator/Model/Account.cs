using Authenticator;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace TwoFactorAuthenticator.Models
{
    public class Account : AccountAuthentication, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Timer timer;
        private AccountSettings accountInfo;

        private int id;
        public int Id
        {
            get { return id; }
            private set
            {
                id = value;
                OnPropertyChanged();
            }
        }

        private string title;
        public string Title
        {
            get { return title; }
            private set
            {
                title = value;
                OnPropertyChanged();
            }
        }

        private string email; // email@mail.com ...
        public string Email
        {
            get { return email; }
            private set
            {
                email = value;
                OnPropertyChanged();
            }
        }

        private int secondsLeft; // seconds left before key update
        public int SecondsLeft
        {
            get { return secondsLeft; }
            private set
            {
                secondsLeft = value;
                OnPropertyChanged();
            }
        }

        private string code; // authenticator code
        public string Code
        {
            get { return code; }
            private set
            {
                code = value;
                OnPropertyChanged();
            }
        }



        public Account(AccountSettings accountInfo) : base(accountInfo.SecretKey)
        {
            this.accountInfo = accountInfo;

            Title = accountInfo.Title;
            Email = accountInfo.Email;

            Code = GetCode();

            timer = new Timer((o) =>
            {
                var currentDateTime = DateTime.Now;
                if (currentDateTime.Second == 0 || currentDateTime.Second == 30)
                {
                    SecondsLeft = 30;
                    Code = GetCode();
                }
                else
                {
                    if (currentDateTime.Second < 30)
                        SecondsLeft = 30 - currentDateTime.Second;
                    else
                        SecondsLeft = 60 - currentDateTime.Second;
                }
            }, null, 0, 1000);
        }
    }
}
