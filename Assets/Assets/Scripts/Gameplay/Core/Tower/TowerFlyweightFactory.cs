using Assets.Scripts.Gameplay.Core.Card;
using Assets.Scripts.Managers;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Assets.Scripts.Gameplay.Core.Towers
{
    /// <summary>
    /// The tower flyweight factory creates and stores tower flyweights using the CardDatabase data.
    /// </summary>
    public static class TowerFlyweightFactory
    {
        private static readonly Dictionary<int, TowerFlyweight> _flyweights = new Dictionary<int, TowerFlyweight>();

        public static async Task<TowerFlyweight> GetFlyweight(CardUIData card)
        {
            int cardId = int.Parse(card.CardID);

            if (_flyweights.TryGetValue(cardId, out TowerFlyweight flyweight))
            {
                return flyweight;
            }

            flyweight = await CreateFlyweight(card);
            _flyweights[cardId] = flyweight;
            return flyweight;
        }

        public static bool ModifyFlyweight(int id, string propertyName, object newValue)
        {
            if (_flyweights.TryGetValue(id, out TowerFlyweight flyweight))
            {
                var property = typeof(TowerFlyweight).GetProperty(propertyName);
                if (property != null && property.CanWrite)
                {
                    property.SetValue(flyweight, newValue);

                    var card = new CardUIData();
                    card.CardID = flyweight.CardID.ToString();
                    card.ManaCost = flyweight.ManaCost;
                    card.Level = flyweight.Stars;
                    card.InitialHp = flyweight.InitialHp;
                    card.Damage = flyweight.Damage;
                    card.FireCooldown = flyweight.FireCooldown;
                    card.VerticalCellsRange = flyweight.VerticalCellsRange;
                    card.HorizontalCellsRange = flyweight.HorizontalCellsRange;
                    card.ProjectileSprite = flyweight.ProjectileSprite.name;
                    card.CardIcon = flyweight.CardIcon.name;
                    card.Type = flyweight.TypeIcon.name;
                    EventManager.Instance.Trigger(EventName.CardUpdate, card);

                    return true;
                }
                else
                {
                    Debug.LogError($"{propertyName} Not exists");
                }
            }
            else
            {
                Debug.LogError($"{id} Not Exist.");
            }
            return false;
        }

        private static async Task<TowerFlyweight> CreateFlyweight(CardUIData card)
        {
            int cardId = int.Parse(card.CardID);

            Task<Sprite> towerSpriteTask = LoadSprite($"Assets/Assets/Sprites/Towers/Tower_{card.CardID}.png");
            Task<Sprite> projectileSpriteTask = LoadSprite($"Assets/Assets/Sprites/Projectiles/{card.ProjectileSprite}.png");
            Task<Sprite> cardIconTask = LoadSprite($"Assets/Assets/Sprites/UI/icons/{card.CardIcon}.png");
            Task<Sprite> typeIconTask = LoadSprite($"Assets/Assets/Sprites/UI/{card.Type}.png");

            await Task.WhenAll(towerSpriteTask, cardIconTask, typeIconTask);

            return new TowerFlyweight(
                cardId,
                towerSpriteTask.Result,
                cardIconTask.Result,
                card.ManaCost,
                card.Level,
                typeIconTask.Result,
                card.InitialHp,
                card.Damage,
                card.FireCooldown,
                card.VerticalCellsRange,
                card.HorizontalCellsRange,
                projectileSpriteTask.Result
            );
        }

        public static async Task PreloadFlyweights(List<CardUIData> allCards)
        {
            List<Task<TowerFlyweight>> preloadTasks = new List<Task<TowerFlyweight>>();

            foreach (var card in allCards)
            {
                int cardId = int.Parse(card.CardID);
                if (!_flyweights.ContainsKey(cardId))
                {
                    preloadTasks.Add(CreateFlyweight(card));
                }
            }

            TowerFlyweight[] results = await Task.WhenAll(preloadTasks);

            // Guardar los flyweights en el diccionario
            for (int i = 0; i < allCards.Count; i++)
            {
                int cardId = int.Parse(allCards[i].CardID);
                _flyweights[cardId] = results[i];
            }

            Debug.Log("Flyweight load is complete.");
        }


        private static async Task<Sprite> LoadSprite(string path)
        {
            var handle = Addressables.LoadAssetAsync<Sprite>(path);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }
            else
            {
                Debug.LogError($"Error at load: {path}");
                return null;
            }
        }
    }
}
