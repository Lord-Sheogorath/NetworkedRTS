﻿using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace RTS_System
{
    public class Mod_ResourceManager : MonoBehaviour
    {
        public class ResourceCache
        {
            public int ResourceValue { get; private set; } = 0;
            
            public Action<int> OnValueChanged = null;

            private TextMeshProUGUI resourceText = null;

            public ResourceCache() { }
            public ResourceCache(TextMeshProUGUI text, string resourceName)
            {
                resourceText = text;

                if (resourceText)
                {
                    OnValueChanged += (int val) =>
                    {
                        resourceText.text = resourceName + ": " + val;
                    };
                }
            }

            /// <summary>
            /// Will add an amount to the currently stored value
            /// </summary>
            /// <param name="amount">The amount to add</param>
            /// <returns>Whether or not the amount was successfully added</returns>
            public bool IncreaseValue(int amount)
            {
                int temp = ResourceValue;

                temp += amount;

                if (temp < 0)
                {
                    return false;
                }

                ResourceValue = temp;

                OnValueChanged?.Invoke(ResourceValue);

                return true;
            }
        }

        public static Mod_ResourceManager Instance = null;

        [Tooltip("The prefab to display resources with, should have an Image and a TextMeshProUGUI")] public GameObject ResourcePrefab = null;
        [Tooltip("The canvas object that will manage all of the ResourcePrefabs spawned")] public GameObject ResourceOwner = null;

        [Tooltip("All of the resources in the game")] public List<Mod_Resource> Resources = new List<Mod_Resource>();

        private GameObject resourcePrefab = null;
        private Image resourceImage = null;
        private TextMeshProUGUI resourceText = null;

        private Dictionary<Mod_Resource, ResourceCache> resourceCaches = new Dictionary<Mod_Resource, ResourceCache>();
        
        // Start is called before the first frame update
        void Start()
        {
            // Create singleton
            if (!Instance) Instance = this;

            // If this is the singleton then set up resources
            if (Instance.Equals(this))
            {
                ResourceCache cache = null;

                // Loop through all the resources in the game and spawn prefabs for them
                foreach (Mod_Resource resource in Resources)
                {
                    if (resourceCaches.ContainsKey(resource))
                    {
                        DebugManager.WarningMessage($"Resource of name '{resource.ResourceName}' already exists!");
                    }
                    // Add the resource to the dictionary if it isn't there already
                    else
                    {
                        cache = new ResourceCache();

                        // Instantiate prefab and get the Text and Image components from it
                        if (ResourcePrefab && ResourceOwner)
                        {
                            resourcePrefab = Instantiate(ResourcePrefab, ResourceOwner.transform);
                            resourcePrefab.name = $"Resource ({resource.ResourceName})";

                            resourceText = resourcePrefab.GetComponentInChildren<TextMeshProUGUI>();
                            resourceImage = resourcePrefab.GetComponentInChildren<Image>();

                            if (resourceText)
                            {
                                cache = new ResourceCache(resourceText, resource.ResourceName);
                            }
                            if (resourceImage) resourceImage.sprite = resource.ResourceIcon;
                        }

                        // Set the initial value of the cache
                        cache.IncreaseValue(Mathf.Abs(resource.ResourceStartCount));

                        // Add resource cache to dictionary
                        resourceCaches.Add(resource, cache);
                    }
                }

                resourceImage = null;
                resourceText = null;
                resourcePrefab = null;
            }
        }

        /// <summary>
        /// Adds an amount of resources to a resource type
        /// </summary>
        /// <param name="resource">The type of resource to add to</param>
        /// <param name="amount">The amount to add</param>
        /// <returns>Whether the amount was successfully added</returns>
        bool AddResources(Mod_Resource resource, int amount)
        {
            if (!resource) return false;

            if (resourceCaches.ContainsKey(resource))
            {
                return resourceCaches[resource].IncreaseValue(amount);
            }

            return false;
        }
    }
}