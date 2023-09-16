using SpiderShooter.Common;
using System;

namespace SpiderShooter.LobbyScene
{
    public interface IView
    {
        public event Action OnHostPlay;
        public event Action OnHostQuit;

        public void SetPlayTriggerMode(VisualElementMode visualElementMode);
        public void SetQuitTriggerMode(VisualElementMode visualElementMode);

        public void SetClientMode();
        public void SetHostMode();
        public void SetLobbyCode(string code);

        public void SetLobbyName(string name);
        public void SetLobbyMode(LobbyMode lobbyMode);
    }
}
