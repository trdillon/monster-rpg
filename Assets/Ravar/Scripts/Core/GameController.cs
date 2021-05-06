using System.Collections;
using Itsdits.Ravar.Battle;
using Itsdits.Ravar.Character;
using Itsdits.Ravar.Core.Signal;
using Itsdits.Ravar.Monster;
using Itsdits.Ravar.Monster.Condition;
using Itsdits.Ravar.Util;
using UnityEngine;

namespace Itsdits.Ravar.Core
{
    /// <summary>
    /// Controller class for game flow management.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        [Tooltip("GameObject that holds the PlayerController component.")]
        [SerializeField] private PlayerController _playerController;

        private BattlerController _battler;
        private GameState _state;
        private GameState _prevState;

        private void Awake()
        {
            ConditionDB.Init();
        }

        private void Start()
        {
            GameSignals.GAME_PAUSE.AddListener(OnPause);
            GameSignals.GAME_RESUME.AddListener(OnResume);
            GameSignals.GAME_QUIT.AddListener(OnQuit);
            GameSignals.PORTAL_ENTER.AddListener(OnPortalEnter);
            GameSignals.PORTAL_EXIT.AddListener(OnPortalExit);
            GameSignals.DIALOG_OPEN.AddListener(OnDialogOpen);
            GameSignals.DIALOG_CLOSE.AddListener(OnDialogClose);
            GameSignals.BATTLE_LOS.AddListener(OnBattlerEncounter);
            GameSignals.BATTLE_OPEN.AddListener(OnBattleOpen);
            GameSignals.WILD_ENCOUNTER.AddListener(OnWildEncounter);
            GameSignals.PARTY_OPEN.AddListener(OnPartyOpen);
            GameSignals.PARTY_CLOSE.AddListener(OnPartyClose);
        }
        
        private void OnDestroy()
        {
            GameSignals.GAME_PAUSE.RemoveListener(OnPause);
            GameSignals.GAME_RESUME.RemoveListener(OnResume);
            GameSignals.GAME_QUIT.RemoveListener(OnQuit);
            GameSignals.PORTAL_ENTER.RemoveListener(OnPortalEnter);
            GameSignals.PORTAL_EXIT.RemoveListener(OnPortalExit);
            GameSignals.DIALOG_OPEN.RemoveListener(OnDialogOpen);
            GameSignals.DIALOG_CLOSE.RemoveListener(OnDialogClose);
            GameSignals.BATTLE_LOS.RemoveListener(OnBattlerEncounter);
            GameSignals.BATTLE_OPEN.RemoveListener(OnBattleOpen);
            GameSignals.WILD_ENCOUNTER.RemoveListener(OnWildEncounter);
            GameSignals.PARTY_OPEN.RemoveListener(OnPartyOpen);
            GameSignals.PARTY_CLOSE.RemoveListener(OnPartyClose);
        }

        private void Update()
        {
            if (_state == GameState.World)
            {
                _playerController.HandleUpdate();
            }
        }
        
        private void OnBattlerEncounter(BattlerEncounter encounter)
        {
            _state = GameState.Cutscene;
            StartCoroutine(encounter.Battler.TriggerEncounter(_playerController));
        }

        private void OnBattleOpen(BattlerEncounter encounter)
        {
            _state = GameState.Battle;
            StartCoroutine(ShowBattle(encounter.Battler));
        }
        
        private void OnBattleFinish()
        {
            _state = GameState.World;
        }

        private void OnWildEncounter(WildEncounter encounter)
        {
            _state = GameState.Battle;
            StartCoroutine(ShowEncounter(encounter.Monster));
        }
        
        private IEnumerator ShowBattle(BattlerController battler)
        {
            // We need to wait until the Game.Battle scene is loaded before we can dispatch the signal, otherwise
            // the BattleController won't be enabled to listen for it.
            yield return SceneLoader.Instance.LoadSceneNoUnload("Game.Battle", true);
            yield return YieldHelper.END_OF_FRAME;
            GameSignals.BATTLE_START.Dispatch(new BattleItem(_playerController, battler));
        }

        private IEnumerator ShowEncounter(MonsterObj monster)
        {
            yield return SceneLoader.Instance.LoadSceneNoUnload("Game.Battle", true);
            yield return YieldHelper.END_OF_FRAME;
            GameSignals.ENCOUNTER_START.Dispatch(new EncounterItem(monster, _playerController));
        }

        private void EndBattle(BattleResult result, bool isCharBattle)
        {
            _state = GameState.World;
            if (_battler != null && result == BattleResult.Won)
            {
                _battler.SetBattlerState(BattlerState.Defeated);
                _battler = null;
            }
            else if (_battler != null && result == BattleResult.Lost)
            {
                //TODO - handle a loss
            }
            else
            {
                //TODO - handle error
            }
        }

        private void OnPause(bool pause)
        {
            _prevState = _state;
            _state = GameState.Pause;
            Time.timeScale = 0;
            StartCoroutine(SceneLoader.Instance.LoadSceneNoUnload("UI.Popup.Pause", true));
        }
        
        private void OnResume(bool resume)
        {
            _state = _prevState;
            _prevState = GameState.Menu;
            Time.timeScale = 1;
            StartCoroutine(SceneLoader.Instance.UnloadScene("UI.Popup.Pause", true));
        }

        private void OnQuit(bool quit)
        {
            _state = GameState.Menu;
            StartCoroutine(SceneLoader.Instance.UnloadWorldScenes());
            StartCoroutine(SceneLoader.Instance.LoadScene("UI.Menu.Main"));
        }

        private void OnPortalEnter(bool entered)
        {
            _prevState = _state;
            _state = GameState.Cutscene;
        }

        private void OnPortalExit(bool exited)
        {
            _state = _prevState;
            _prevState = GameState.Cutscene;
        }
        
        private void OnDialogOpen(DialogItem dialog)
        {
            _state = GameState.Dialog;
            StartCoroutine(ShowDialog(dialog));
        }

        private void OnDialogClose(string speakerName)
        {
            _state = GameState.World;
            StartCoroutine(SceneLoader.Instance.UnloadScene("UI.Popup.Dialog", true));
        }

        private IEnumerator ShowDialog(DialogItem dialog)
        {
            // We need to wait until the UI.Popup.Dialog scene is loaded before we can dispatch the signal, otherwise
            // the DialogController won't be enabled to listen for it.
            yield return SceneLoader.Instance.LoadSceneNoUnload("UI.Popup.Dialog", true);
            yield return YieldHelper.END_OF_FRAME;
            GameSignals.DIALOG_SHOW.Dispatch(dialog);
        }

        private void OnPartyOpen(bool opened)
        {
            _prevState = _state;
            _state = GameState.Menu;
            StartCoroutine(ShowParty());
        }

        private void OnPartyClose(bool closed)
        {
            _state = _prevState;
            _prevState = GameState.Menu;
            StartCoroutine(SceneLoader.Instance.UnloadScene("UI.Popup.Party", true));
        }

        private IEnumerator ShowParty()
        {
            StartCoroutine(SceneLoader.Instance.LoadSceneNoUnload("UI.Popup.Party", true));
            yield return YieldHelper.END_OF_FRAME;
            GameSignals.PARTY_SHOW.Dispatch(new PartyItem(_playerController.Party));
        }
    }
}