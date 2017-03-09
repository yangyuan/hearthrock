

namespace Hearthrock.Pegasus
{
    public interface IRockPegasus
    {
        void SetActive();

        bool TryCloseDialog();

        bool TryCloseQuests();

        void EndTurn();

        void TryFinishEndGame();

        RockPegasusSceneState GetPegasusSceneState();

        RockPegasusSubsceneState GetPegasusSubsceneState(RockPegasusSceneState sceneState);

        RockPegasusGameState GetPegasusGameState();

        void NavigateToHub();

        void NavigateToTournament();

        void NavigateToAdventure();

        void TrySetSceneMode(SceneMgr.Mode mode);

        void SelectPracticeOpponent(int index);
        void PlayPractice();
        void PlayTournament();

        long GetSelectedDeckID();

        void ConfigTournament(bool ranked, bool wild);

        void ChoosePracticeMode(bool expert);
        void ChooseDeck(int index);
    }
}
