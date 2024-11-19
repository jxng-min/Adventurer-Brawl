using Jongmin;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Junyoung
{
    public class PlayerCtrl : MonoBehaviour
    {
        public Rigidbody2D m_rigidbody;

        public float MoveSpeed { get; private set; }
        public float JumpPower { get; private set; }

        public Vector2 m_move_vec = Vector2.zero;
        public bool m_is_jump = false;
     

        private IPlayerState m_stop_state, m_move_state, m_jump_state, m_dead_state, m_clear_state, m_down_state; // 각 상태들의 선언
        private PlayerStateContext m_player_state_context;                                          //상태를 변경할 인터페이스 선언

        private Skill[] m_player_skills = new Skill[3]; 

        private void OnEnable()
        {
            GameEventBus.Subscribe(GameEventType.PLAYING, GameManager.Instance.Playing);
        }

        private void OnDisable()
        {
            GameEventBus.Unsubscribe(GameEventType.PLAYING, GameManager.Instance.Playing);
        }

        private void Start()
        {
            GameEventBus.Publish(GameEventType.PLAYING);
            
            m_rigidbody = GetComponent<Rigidbody2D>();

            m_player_state_context = new PlayerStateContext(this);          //Context에 PlayerCtrl 객체 자신을 전달

            m_stop_state = gameObject.AddComponent<PlayerStopState>();      // context를 통해 변경할 상태 스크립트들을 컴포넌트로 추가
            m_move_state = gameObject.AddComponent<PlayerMoveState>();
            m_jump_state = gameObject.AddComponent<PlayerJumpState>();
            m_dead_state = gameObject.AddComponent<PlayerDeadState>();
            m_clear_state = gameObject.AddComponent<PlayerClearState>();
            m_down_state = gameObject.AddComponent<PlayerDownState>();

            m_player_state_context.Transition(m_stop_state);                // 플레이어의 초기 상태를 정지 상태로 설정

            SetPlayerSkill();

            MoveSpeed = 4.0f;
            JumpPower = 15.0f;
        }

        private void FixedUpdate()
        {
            m_rigidbody.linearVelocity = new Vector2(m_move_vec.x * MoveSpeed, m_rigidbody.linearVelocity.y);
        }

        public void PlayerStop()
        {
            m_player_state_context.Transition(m_stop_state);
        }

        public void PlayerMove()
        {
            m_player_state_context.Transition(m_move_state);
        }

        public void PlayerDown()
        {
            if(GameManager.Instance.m_game_status == "Playing")
            {
                m_player_state_context.Transition(m_down_state);
            }
        }

        public void PlayerJump()
        {
            if(!m_is_jump && GameManager.Instance.m_game_status == "Playing")
            {
                m_player_state_context.Transition(m_jump_state);       
            }
        }

        public void DeadPlayer()
        {
            m_player_state_context.Transition(m_dead_state);
        }

        public void ClearPlayer()
        {
            m_player_state_context.Transition(m_clear_state);
        }

        // 플레이어를 좌측 이동시키는 메소드
        public void PlayerMoveLeftBtnDown() 
        {
            if(GameManager.Instance.m_game_status == "Playing")
            {
                m_move_vec = Vector2.left;
                PlayerMove();
            }

        }

        // 플레이어를 우측 이동시키는 메소드
        public void PlayerMoveRightBtnDown()
        {
            if (GameManager.Instance.m_game_status == "Playing")
            {
                m_move_vec = Vector2.right;
                PlayerMove();
            }

        }

        // 플레이어가 이동 버튼에서 손을 떼었을 때 호출되는 메소드
        public void PlayerMoveBtnUP()
        {
            m_move_vec = Vector2.zero;
            PlayerStop();
        }

        // 캐릭터마다 스킬 전략을 설정하는 메소드
        private void SetPlayerSkill()
        {
            switch(GameManager.Instance.CharacterType)
            {
            case Character.SOCIA:
                m_player_skills[0] = new SociaSkill1();
                m_player_skills[1] = new SociaSkill2();
                m_player_skills[2] = new SociaSkill3();
            break;

            case Character.GOV:
                m_player_skills[0] = new GovSkill1();
                m_player_skills[1] = new GovSkill2();
                m_player_skills[2] = new GovSkill3();
            break;

            case Character.ENVA:
                m_player_skills[0] = new EnvaSkill1();
                m_player_skills[1] = new EnvaSkill2();
                m_player_skills[2] = new EnvaSkill3();
            break;
            }
        }

        public void Skill1()
        {
            m_player_skills[0].Effect();
        }

        public void Skill2()
        {
            m_player_skills[1].Effect();
        }

        public void Skill3()
        {
            m_player_skills[2].Effect();
        }
    }
}


