using System.Collections;
using Itsdits.Ravar.Animation;
using Itsdits.Ravar.Character;
using Itsdits.Ravar.Core.Signal;
using Itsdits.Ravar.Monster;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Itsdits.Ravar.Battle
{
    /// <summary>
    /// Controller class that handles the flow and logic of the Battle scene.
    /// </summary>
    public class BattleController : MonoBehaviour
    {
        [Header("Scene Management")]
        [Tooltip("EventSystem for the Battle scene.")]
        [SerializeField] private EventSystem _eventSystem;
        [Tooltip("Camera for the Battle scene.")]
        [SerializeField] private Camera _camera;
        [Tooltip("BattleUIController that handles the UI of the Battle scene.")]
        [SerializeField] private BattleUIController _ui;
        [Tooltip("BattleAnimator that handles the animations in the Battle scene.")]
        [SerializeField] private BattleAnimator _animator;
        
        private BattlerController _battler;
        private PlayerController _player;
        private MonsterParty _battlerParty;
        private MonsterParty _playerParty;
        private MonsterObj _wildMonster;
        
        private BattleState _state;
        private BattleState? _prevState;

        private int _escapeAttempts;
        private bool _isCharBattle;
        private bool _isMonsterDown;
        
        private void Start()
        {
            //DisableBattle();
            //TODO - handle this better
            _player = FindObjectOfType<PlayerController>();
        }

        private void OnEnable()
        {
            GameSignals.BATTLE_START.AddListener(EnableCharBattle);
        }

        private void OnDisable()
        {
            GameSignals.BATTLE_START.RemoveListener(EnableCharBattle);
        }

        private void Update()
        {
            //throw new NotImplementedException();
        }

        private IEnumerator SetupBattle()
        {
            _ui.HideHud();
            if (!_isCharBattle)
            {
                // Setup the BattleMonsters and play the starting animations.
                _animator.PlayerMonster.Setup(_playerParty.GetHealthyMonster());
                _animator.PlayBattleStartAnimation(_animator.PlayerMonster);
                _animator.EnemyMonster.Setup(_wildMonster);
                _animator.PlayBattleStartAnimation(_animator.EnemyMonster);
                
                // Set the moves list in the UI and show some dialog.
                _ui.SetMoveList(_animator.PlayerMonster.Monster.Moves);
                yield return _ui.TypeDialog($"You have encountered an enemy {_animator.EnemyMonster.Monster.Base.Name}!");
            }
            else
            {
                // Show the character sprites and some dialog.
                _animator.ShowCharacterSprites(_player, _battler);
                yield return _ui.TypeDialog($"{_battler.Name} has challenged you to a battle!");

                // Deploy enemy monster.
                //TODO - animate this
                _animator.BattlerImage.gameObject.SetActive(false); 
                _animator.EnemyMonster.gameObject.SetActive(true);
                MonsterObj enemyLeadMonster = _battlerParty.GetHealthyMonster();
                _animator.EnemyMonster.Setup(enemyLeadMonster);
                yield return _ui.TypeDialog($"{_battler.Name} has deployed {enemyLeadMonster.Base.Name} to the battle!");

                // Deploy player monster.
                //TODO - animate this too
                _animator.PlayerImage.gameObject.SetActive(false); 
                _animator.PlayerMonster.gameObject.SetActive(true);
                MonsterObj playerLeadMonster = _playerParty.GetHealthyMonster();
                _animator.PlayerMonster.Setup(playerLeadMonster);
                
                // Set the moves list in the UI and show some dialog.
                _ui.SetMoveList(_animator.PlayerMonster.Monster.Moves);
                yield return _ui.TypeDialog($"You have deployed {playerLeadMonster.Base.Name} to the battle!");
            }

            // Finish setup and pass control to the UI for action selection.
            _escapeAttempts = 0;
            //_partyScreen.Init(); //TODO - make the party scene
            //_ui.ActionSelection(); //TODO - fix input selection
        }

        private void EnableCharBattle(BattlerEncounter battler)
        {
            _eventSystem.enabled = true;
            _camera.enabled = true;
            _battler = battler.Battler;
            _battlerParty = _battler.Party;
            _playerParty = _player.Party;
            _isCharBattle = true;
            StartCoroutine(SetupBattle());
        }

        private void DisableBattle()
        {
            _eventSystem.enabled = false;
            _camera.enabled = false;
        }
    }
}