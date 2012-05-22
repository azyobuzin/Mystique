﻿using System;
using System.Threading;
using System.Windows.Media;
using Dulcet.Twitter;
using Inscribe.Configuration;
using Inscribe.Storage;
using Inscribe.ViewModels.PartBlocks.MainBlock;
using Livet;
using Livet.Commands;

namespace Inscribe.ViewModels.PartBlocks.BlockCommon
{
    public class NotificationItemViewModel : ViewModel
    {
        private readonly EventDescription description;
        public EventDescription Description
        {
            get { return this.description; }
        }

        public NotificationItemViewModel(EventDescription description, bool enableClose = true)
        {
            this.description = description;
            this.EnableClose = enableClose;
        }

        public EventKind Kind
        {
            get { return this.description.Kind; }
        }

        public UserViewModel SourceUser
        {
            get { return this.description.SourceUser; }
        }

        public string Target
        {
            get
            {
                switch (this.Kind)
                {
                    case EventKind.Favorite:
                    case EventKind.Retweet:
                    case EventKind.Unfavorite:
                        // Show tweet
                        var status = description.TargetTweet.Status as TwitterStatus;
                        if (status != null && status.RetweetedOriginal != null)
                        {
                            return "@" + status.RetweetedOriginal.User.ScreenName + ": " 
                                + status.RetweetedOriginal.Text;
                        }
                        else
                        {
                            if (description.TargetTweet.Status == null)
                                return "(no reference)";
                            else
                                return "@" + description.TargetTweet.Status.User.ScreenName + ": "
                                    + description.TargetTweet.Status.Text;
                        }
                    case EventKind.Mention:
                    case EventKind.DirectMessage:
                        return description.TargetTweet.Status.Text;
                    case EventKind.Suppressed:
                        return "インビジブルなんとか 発動！！";
                    default:
                        try
                        {
                            // Show user
                            return "@" + description.TargetUser.TwitterUser.ScreenName;
                        }
                        catch(Exception e)
                        {
                            ExceptionStorage.Register(e, ExceptionCategory.InternalError, "通知処理が正しく行われませんでした。");
                            return "(Undefined)";
                        }
                }
            }
        }

        public string CreatedAt
        {
            get { return this.description.CreatedAt.ToLocalTime().ToString(); }
        }

        public bool EnableClose { get; private set; }

        public Brush Background
        {
            get { return Setting.Instance.ColoringProperty.TweetWorkerNotifierBackground.GetBrush(); }
        }

        #region RequireCloseイベント

        public event EventHandler<NotifierViewModelEventArgs> RequireClose;
        private Notificator<NotifierViewModelEventArgs> _RequireCloseEvent;
        public Notificator<NotifierViewModelEventArgs> RequireCloseEvent
        {
            get
            {
                if (_RequireCloseEvent == null) _RequireCloseEvent = new Notificator<NotifierViewModelEventArgs>();
                return _RequireCloseEvent;
            }
            set { _RequireCloseEvent = value; }
        }

        protected void OnRequireClose(NotifierViewModelEventArgs e)
        {
            var threadSafeHandler = Interlocked.CompareExchange(ref RequireClose, null, null);
            if (threadSafeHandler != null) threadSafeHandler(this, e);
            RequireCloseEvent.Raise(e);
        }

        #endregion
       
        #region CloseCommand
        ViewModelCommand _CloseCommand;

        public ViewModelCommand CloseCommand
        {
            get
            {
                if (_CloseCommand == null)
                    _CloseCommand = new ViewModelCommand(Close);
                return _CloseCommand;
            }
        }

        private void Close()
        {
            OnRequireClose(new NotifierViewModelEventArgs(this));
        }
        #endregion
        
        #region ShowUserCommand
        
        ViewModelCommand _ShowUserCommand;

        public ViewModelCommand ShowUserCommand
        {
            get
            {
                if (_ShowUserCommand == null)
                    _ShowUserCommand = new ViewModelCommand(ShowUser);
                return _ShowUserCommand;
            }
        }

        private void ShowUser()
        {
            Core.KernelService.MainWindowViewModel.ColumnOwnerViewModel.CurrentFocusColumn.SelectedTabViewModel
                .AddTopUser(this.SourceUser.TwitterUser.ScreenName);
        }
        
        #endregion
    }

    public class NotifierViewModelEventArgs : EventArgs
    {
        public NotificationItemViewModel NotifierViewModel { get; private set; }

        public NotifierViewModelEventArgs(NotificationItemViewModel notifier)
        {
            this.NotifierViewModel = notifier;
        }
    }
}
