using System.Collections;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using SpaceKomodo.Utilities;
using TMPro;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Views
{
    public class EffectVisualView : MonoBehaviour, IInitializable<(SkillEffect, int)>
    {
        [SerializeField] private TMP_Text _valueText;
        [SerializeField] private ParticleSystem _particleSystem;
        
        public void Initialize((SkillEffect, int) data)
        {
            var (effect, value) = data;
            
            if (_valueText != null)
            {
                var prefix = "";
                switch (effect)
                {
                    case SkillEffect.TargetDamage:
                    case SkillEffect.SelfDamage:
                        prefix = "-";
                        _valueText.color = Color.red;
                        break;
                        
                    case SkillEffect.TargetHeal:
                    case SkillEffect.SelfHeal:
                        prefix = "+";
                        _valueText.color = Color.green;
                        break;
                }
                
                _valueText.text = $"{prefix}{value}";
            }
            
            if (_particleSystem != null)
            {
                _particleSystem.Play();
            }
            
            StartCoroutine(AnimateText());
        }
        
        private IEnumerator AnimateText()
        {
            if (_valueText == null) yield break;
            
            var duration = 1.0f;
            var elapsed = 0f;
            
            var startPos = _valueText.transform.localPosition;
            var endPos = startPos + Vector3.up * 1.0f;
            
            var startColor = _valueText.color;
            var endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
            
            while (elapsed < duration)
            {
                var t = elapsed / duration;
                
                _valueText.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
                _valueText.color = Color.Lerp(startColor, endColor, t);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            _valueText.transform.localPosition = endPos;
            _valueText.color = endColor;
        }
    }
}