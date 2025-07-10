using System;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;


namespace PixelLandsVillage
{
    public class ConfigurableObjectElement : MonoBehaviour
    {

        public Sprite[] Options;

        // Returns the index of the currently selected sprite. The currently selected sprite is the one that matches the sprite in this game object's sprite renderer component.
        public int SelectedOption
        {
            get
            {
                int optionIdx = 0;
                SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
                if  (Options == null || sr == null)
                {
                    return -1;
                }
                foreach (var option in Options)
                {
                    if (option == sr.sprite)
                    {
                        return optionIdx;
                    }
                    optionIdx++;
                }

                // The current sprite doesn't match any of the options so return -1;
                return -1;
            }
        }

        public void UpdateSprite(int selectedOption)
        {
            if (Options == null || Options.Length == 0)
            {
                return;
            }
            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            {
                //Undo.RecordObject(sr, $"Changed Configurable Object {name}");
                sr.sprite = Options[selectedOption];
            }
        }
    }
}
