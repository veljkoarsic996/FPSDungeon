﻿//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;

namespace InfimaGames.LowPolyShooterPack.Interface
{
    /// <summary>
    /// CanvasAlpha. Changes the canvas alpha based on certain things that can happen in-game.
    /// </summary>
    public class CanvasAlpha : Element
    {
        #region FIELDS SERIALIZED
        

        [Tooltip("Canvas group to update the alpha for.")]
        [SerializeField]
        private CanvasGroup canvasGroup;

        [Tooltip("Speed of interpolation.")]
        [Range(0.0f, 25.0f)]
        [SerializeField]
        private float interpolationSpeed = 12.0f;
        
        [Tooltip("Alpha of the canvasGroup while the cursor is unlocked (pause menu is open).")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float cursorUnlockedAlpha = 0.6f;
        
        #endregion
        
        #region METHODS
        
        /// <summary>
        /// Tick.
        /// </summary>
        protected override void Tick()
        {
            //Base.
            base.Tick();

            //Check References.
            if (canvasGroup == null)
            {
                //ReferenceError.
                Log.ReferenceError(this, gameObject);
                
                //Return.
                return;
            }

            //Update Alpha.
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, characterBehaviour.IsCursorLocked() ? 1.0f : cursorUnlockedAlpha, Time.deltaTime * interpolationSpeed);
        }
        
        #endregion
    }
}