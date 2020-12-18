﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// プレイヤーからの参照
        /// </summary>
        public static PlayerController instance { get; private set; } = null;

        /// <summary>
        /// プレイヤーの状態
        /// </summary>
        public ActionState state { get { return m_state; } set { m_state = value; } }

        public bool isAcceptAttack { get; set; } = true;

        /// <summary>
        /// メインカメラへの参照
        /// </summary>
        public PlayerCamera playerCamera { get; private set; }

        /// <summary>
        /// プレイヤーアニメーター
        /// </summary>
        public Animator animator { get; private set; }

        /// <summary>
        /// 物理
        /// </summary>
        public Rigidbody playerRigidbody { get; private set; }

        /// <summary>
        /// DamageController
        /// </summary>
        public Damage.DamageController damageController { get; private set; }

        public PlayerArmor armor { get; private set; }

        public AttackCommand attackCommand { get { return m_attackCommand; } }


        /// <summary>
        /// プレイヤーの状態
        /// </summary>
        [Header("State")]
        [SerializeField]
        private ActionState m_state = ActionState.None;

        [SerializeField]
        private OriginalPhysics.LandingDetect landingDetect = new OriginalPhysics.LandingDetect();

        [Header("Attack Info")]
        [SerializeField]
        private AttackCommand m_attackCommand = null;

        [Header("Jump Info")]
        [SerializeField]
        private JumpCommand m_jumpCommand = null;

        [Header("Move Info")]
        [SerializeField]
        private MoveCommand m_moveCommand = null;

        private void Awake()
        {
            if (instance)
            {
#if UNITY_EDITOR
                Debug.LogError("Playerが複数存在しています。");
#endif
            }
            instance = this;
        }

        private void OnEnable()
        {
            damageController = GetComponent<Damage.DamageController>();
            if (!damageController)
            {
#if UNITY_EDITOR
                Debug.LogError("DamageControllerが見つかりません。");
#endif
            }

            animator = GetComponentInChildren<Animator>();
            if (!animator)
            {
#if UNITY_EDITOR
                Debug.LogError("Animatorが見つかりません。");
#endif
            }

            playerRigidbody = GetComponent<Rigidbody>();
            if (!playerRigidbody)
            {
#if UNITY_EDITOR
                Debug.LogError("RigidBodyが見つかりません。");
#endif
            }

            armor = GetComponent<PlayerArmor>();
            if (!armor)
            {
#if UNITY_EDITOR
                Debug.LogError("PlayerArmorが見つかりません。");
#endif
            }

            playerCamera = Camera.main.GetComponent<PlayerCamera>();
        }

        // Update is called once per frame
        void Update()
        {
            switch (m_state)
            {
                case ActionState.Stand:
                    {
                        m_moveCommand.Command(this);
                        // Jump();
                        //AttackInput();

                        m_attackCommand.Command(this);

                        m_jumpCommand.Command(this);
                        break;
                    }
                case ActionState.Run:
                    {
                        m_moveCommand.Command(this);
                        m_attackCommand.Command(this);
                        m_jumpCommand.Command(this);
                        break;
                    }
                case ActionState.Airial:
                    {
                        m_moveCommand.Command(this);

                        break;
                    }

                case ActionState.Attack:
                    {
                        m_attackCommand.Command(this);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void FixedUpdate()
        {
            m_moveCommand.FixedUpdate(this);

            // 移動

            JumpDetect();

            switch (state)
            {
                case ActionState.Airial:
                    animator.SetInteger("State", (int)AnimationState.Airial);
                    break;
                case ActionState.Stand:
                    animator.SetInteger("State", (int)AnimationState.Stand);
                    break;
                case ActionState.Run:
                    animator.SetInteger("State", (int)AnimationState.Run);
                    break;
            }


        }

        /// <summary>
        /// 移動処理（コントローラ）
        /// </summary>
        //private void MoveInput()
        //{
        //    float horizontal, vertical;

        //    horizontal = Input.GetAxis(m_horizontal);
        //    vertical = Input.GetAxis(m_vertical);

        //    if (horizontal == 0.0f && vertical == 0.0f)
        //    {
        //        m_force = Vector2.zero;
        //        return;
        //    }

        //    float rad = Mathf.Atan2(vertical, horizontal);
        //    rad += (m_camera.polar.azimath + 90.0f) * Mathf.Deg2Rad;


        //    m_force.x = Mathf.Cos(rad);
        //    m_force.y = Mathf.Sin(rad);
        //}

        private void JumpDetect()
        {
            if (!landingDetect.IsLanding(gameObject))
            {
                state = ActionState.Airial;
            }
            else if (state == ActionState.Airial) 
            {
                state = ActionState.Stand;
            }
        }

        private void Advance(float value)
        {
            Vector3 vec = Vector3.zero;
            vec.x = Mathf.Cos(transform.rotation.eulerAngles.x * Mathf.Deg2Rad) * value;
            vec.z = Mathf.Sin(transform.rotation.eulerAngles.y * Mathf.Deg2Rad) * value;

            playerRigidbody.AddForce(vec);
        }

        public void SetAcceptAttack(int i)
        {
            if (animator != null)
            {
                animator.SetBool("IsAcceptAttack", i != 0);
            }
            isAcceptAttack = i != 0;
        }

        public void SetState(int state)
        {
            m_state = (ActionState)state;
        }

        public void SetAnimationState(in AnimationState state)
        {
            animator.SetInteger("State", (int)state);
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }

    [Flags]
    public enum ActionState
    {
        None = 0,
        Stand = 1 << 0,
        Run = 1 << 1,
        Attack = 1 << 2,
        Airial = 1 << 3
    }

    public enum AnimationState
    {
        Stand = 0,
        Run,
        Airial
    }
}

