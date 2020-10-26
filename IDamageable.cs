using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Allows entities to deal and receive damage – and die</summary>
    internal interface IDamageable
    {
        ///<summary>The amount of health remaining before this entity dies</summary>
        int Health { get; }

        ///<summary>The maximum amount of health this entity can possess</summary>
        int MaxHealth { get; }

        ///<summary>Condition used to check if this entity has died</summary>
        bool IsDead { get; }

        ///<summary>Increases this entity's health by an amount</summary>
        void Heal(int amount = 1);

        ///<summary>Decreases this entity's health by an amount until this entity dies</summary>
        void TakeDamage(int amount = 1);

        ///<summary>Kills this entity</summary>
        void Die();
    }
}