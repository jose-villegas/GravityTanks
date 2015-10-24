﻿using GTCore.Player;

using UnityEngine;

namespace GTCore.General
{
    public class StickToPlayer : MonoBehaviour
    {
        private StickToPlanet _stickToPlanet;
        public Transform Target;

        private void Awake()
        {
            _stickToPlanet = Target.gameObject.GetComponent<StickToPlanet>();
            // disable hieracy transform, use local transform
            transform.SetParent(transform.parent.transform, false);
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            transform.position = Target.position;
            transform.up = _stickToPlanet.PlanetCurrentNormal;
        }
    }
}