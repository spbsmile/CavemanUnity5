﻿using System;
using System.Collections;
using System.Collections.Generic;
using Caveman.Bonuses;
using Caveman.CustomAnimation;
using Caveman.Network;
using Caveman.Setting;
using Caveman.Specification;
using Caveman.Utils;
using Caveman.Weapons;
using UnityEngine;
using Random = System.Random;

namespace Caveman.Players
{
    public class PlayerModelBase : MonoBehaviour
    {
        public Action<Vector2> Death;
        public Action RespawnGUIDisabled; 
        public Func<WeaponSpecification.Types, ObjectPool<WeaponModelBase>> ChangedWeaponsPool;

        public Player player;
        public string Id;
        [HideInInspector] public BonusBase bonusBase;
        [HideInInspector] public SpriteRenderer spriteRenderer;
        [HideInInspector] public bool firstRespawn = true;
        
        protected Action ChangedWeapons;
        protected Vector2 delta;
        protected Animator animator;
        protected Vector2 target;
        protected Random r;
        protected ServerConnection serverConnection;
        protected bool multiplayer;
        protected WeaponSpecification.Types typeWeapon;
        protected internal bool invulnerability;
        protected PlayerAnimation playerAnimation;
        //todo переделать под массив
        protected List<PlayerModelBase> players;

        private PlayerPool poolPlayers;
        private ObjectPool<WeaponModelBase> poolWeapons;

        protected virtual void Start()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            playerAnimation = new PlayerAnimation(animator);
        }

        public void Init(Player player, Random random, PlayerPool pool, ServerConnection serverConnection)
        {
            this.serverConnection = serverConnection;
            if (serverConnection != null) multiplayer = true;
            name = player.name;
            transform.GetChild(0).GetComponent<TextMesh>().text = name;
            this.player = player;
            poolPlayers = pool;
            players = new List<PlayerModelBase>();
            players.AddRange(poolPlayers.GetCurrentPlayers());
            poolPlayers.AddedPlayer += @base => players.Add(@base);
            poolPlayers.RemovePlayer += @base => players.Remove(@base);
            r = random;
        }

        public virtual void PickupBonus(BonusBase bonus)
        {
            bonus.Effect(this);
        }

        public virtual void Die()
        {
            Death(transform.position);
            poolPlayers.Store(this);
        }

        public virtual void PickupWeapon(WeaponModelBase weaponModel)
        {
            if (poolWeapons == null || weaponModel.Specification.Type != typeWeapon)
            {
                poolWeapons = ChangedWeaponsPool(weaponModel.Specification.Type);
                typeWeapon = weaponModel.Specification.Type;
                if (ChangedWeapons != null)
                {
                    ChangedWeapons();    
                }
                else
                {
                    print("ChangedWeapons null" + name);
                }
            }
            playerAnimation.Pickup();
            weaponModel.Take();
        }

        public virtual void Throw(Vector2 aim)
        {
            playerAnimation.Throw();
            poolWeapons.New().SetMotion(player, transform.position, aim);
        }

        public virtual IEnumerator Respawn(Vector2 point)
        {
            yield return new WaitForSeconds(Settings.PlayerTimeRespawn);
            Birth(point);
            StopMove();
            if (RespawnGUIDisabled != null) RespawnGUIDisabled();
        }

        public void Birth(Vector2 position)
        {
            poolPlayers.New(Id).transform.position = position;
            invulnerability = true;
            StartCoroutine(ProggressInvulnerability(Settings.PlayerTimeInvulnerability));
        }

        private IEnumerator ProggressInvulnerability(float playerTimeInvulnerability)
        {
            var startTime = Time.time;
            var render = spriteRenderer ? spriteRenderer : GetComponent<SpriteRenderer>();
            while (Time.time  < startTime + playerTimeInvulnerability)
            {
                render.enabled = false;
                yield return new WaitForSeconds(0.1f);
                render.enabled = true;
                yield return new WaitForSeconds(0.1f);
            }
            render.enabled = true;
            invulnerability = false;
        }

        /// <summary>
        /// linear motion
        /// </summary>
        protected virtual void Move()
        {
            transform.position = new Vector3(transform.position.x + delta.x*Time.deltaTime,
                transform.position.y + delta.y*Time.deltaTime);
            playerAnimation.SetMoving(delta.y < 0, delta.x > 0);
        }

        /// <summary>
        /// set delta - direction of motion
        /// </summary>
        /// <param name="target"></param>
        public void SetMove(Vector2 target)
        {
            this.target = target;
            delta = UnityExtensions.CalculateDelta(transform.position, target, Settings.PlayerSpeed);
        }

        public void StopMove()
        {
            delta = Vector2.zero;
            playerAnimation.IsMoving_B = false;
            playerAnimation.IsMoving_F = false;
        }
    }
}


