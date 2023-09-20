using SpiderShooter.Common;
using System;

namespace SpiderShooter.LobbyScene.Player
{
    public interface IView
    {
        public void SetPlayerName(string name);
        public void SetLocalPlayer(bool value);

        public void SetDeletePlayerTriggerMode(VisualElementMode visualElementMode);
        public void SetDeletePlayerTriggerCallBack(Action action);

        public void SetLeavePlayerTriggerMode(VisualElementMode visualElementMode);
        public void SetLeavePlayerTriggerCallBack(Action action);

        public void SetReadyStateSelectionCallBack(Action<bool> action);
        public void SetReadyStateSelectionMode(VisualElementMode visualElementMode);
        public void SetReadyState(bool value);

        public void SetTeamColorSelectionCallBack(Action<TeamColor> action);
        public void SetTeamColorSelectionMode(VisualElementMode visualElementMode);
        public void SetTeamColor(TeamColor color);
    }
}
