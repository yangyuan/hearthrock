

namespace Hearthrock.Pegasus
{
    public interface IRockPegasus
    {
        void SetActive();

        bool TryCloseDialog();

        bool TryCloseQuests();

        RockPegasusState GetSceneMode();

        void NavigateToHub();

        void NavigateToTournament();

        void NavigateToAdventure();

        void TrySetSceneMode(SceneMgr.Mode mode);

        void SelectPracticeOpponent(int index);
    }
}
