using Assets.Scripts.Gameplay.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Core.Card
{
    public abstract class CardComponent
    {
        protected CardController _card;
        public void SetContext(CardController card)
        {
            _card = card;
            card._awake += ManualAwake;
            card._start += ManualStart;
            card._update += ManualUpdate;
            card._onDestroy += ManualOnDestroy;
        }

        public virtual void ManualAwake() { }

        public virtual void ManualStart() { }

        public virtual void ManualUpdate() { }

        public virtual void ManualOnDestroy() { }
    }
}