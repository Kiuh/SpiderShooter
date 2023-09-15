using System;

namespace SpiderShooter.OpeningScene
{
    public interface IView
    {
        public event Action OnJoinLobby;
        public event Action OnCreateLobby;
        public event Action OnPlayRandomLobby;

        public string CodeToJoinLobby { get; }
        public string ServerNameForCreating { get; }
        public string PlayerName { get; }

        public void ShowErrorDialog(string message);
    }
}
