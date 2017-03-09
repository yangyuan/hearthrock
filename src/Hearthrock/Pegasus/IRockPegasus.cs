

namespace Hearthrock.Pegasus
{
    public interface IRockPegasus
    {
        void SetActive();

        bool TryCloseDialog();

        bool TryCloseQuests();

        void EndTurn();

        void TryFinishEndGame();

        RockPegasusState GetSceneMode();
        RockPegasusGameState GetPegasusGameState();

        void NavigateToHub();

        void NavigateToTournament();

        void NavigateToAdventure();

        void TrySetSceneMode(SceneMgr.Mode mode);

        void SelectPracticeOpponent(int index);
    }
}
