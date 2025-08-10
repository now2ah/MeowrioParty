using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meowrio.Util
{
    public enum ECharacterType
    {
        Mario,
        Luigi,
        Waluigi,
        Birdo
    }

    public class CharacterFactory : MonoBehaviour
    {
        [SerializeField] private List<Transform> _characterPrefabList;

        public Transform GenerateCharacter(ECharacterType characterType)
        {
            Transform character = null;

            switch (characterType)
            {
                case ECharacterType.Mario:
                    character = Instantiate(_characterPrefabList[(int)characterType]);
                    break;
                case ECharacterType.Luigi:
                    character = Instantiate(_characterPrefabList[(int)characterType]);
                    break;
                case ECharacterType.Waluigi:
                    character = Instantiate(_characterPrefabList[(int)characterType]);
                    break;
                case ECharacterType.Birdo:
                    character = Instantiate(_characterPrefabList[(int)characterType]);
                    break;
            }

            return character;
        }
    }
}

