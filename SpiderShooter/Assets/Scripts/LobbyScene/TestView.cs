using SpiderShooter.Common;
using System;
using UnityEngine;

namespace SpiderShooter.LobbyScene
{
    [AddComponentMenu("LobbyScene.TestView")]
    internal class TestView : MonoBehaviour, IView
    {
        public event Action OnHostPlay;
        public event Action OnHostQuit;

        public void SetClientMode()
        {
            throw new NotImplementedException();
        }

        public void SetHostMode()
        {
            throw new NotImplementedException();
        }

        public void SetLobbyCode(string code)
        {
            throw new NotImplementedException();
        }

        public void SetLobbyMode(LobbyMode lobbyMode)
        {
            throw new NotImplementedException();
        }

        public void SetLobbyName(string name)
        {
            throw new NotImplementedException();
        }

        public void SetPlayTriggerMode(VisualElementMode visualElementMode)
        {
            throw new NotImplementedException();
        }

        public void SetQuitTriggerMode(VisualElementMode visualElementMode)
        {
            throw new NotImplementedException();
        }
    }
}
