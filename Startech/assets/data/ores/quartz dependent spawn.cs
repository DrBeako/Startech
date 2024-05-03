using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using Objects = StardewValley.Object;

namespace TestCase
{
    /// <summary>
    /// Sample mod demonstrating the usage of IItemExtensionsApi.
    /// </summary>
    public class TestCase : Mod
    {
        // IItemExtensionsApi instance
        IItemExtensionsApi api;

        /// <summary>
        /// Entry point of the mod.
        /// </summary>
        /// <param name="helper">SMAPI's Mod Helper.</param>
        public override void Entry(IModHelper helper)
        {
            // Subscribe to the GameLoop.GameLaunched event
            this.Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

            // Subscribe to the Player.Warped event
            this.Helper.Events.Player.Warped += this.OnWarped;
        }

        /// <summary>
        /// Handles the GameLaunched event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            if (this.Helper.ModRegistry.GetApi<IItemExtensionsApi>("mistyspring.ItemExtensions") is IItemExtensionsApi api)
            {
                this.api = api;
            }
        }

        /// <summary>
        /// Handles the Warped event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnWarped(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
            // Check if the player has entered the underground mine
            if (e.NewLocation is MineShaft UndergroundMine)
            {
                // Find all quartz in the map
                if (UndergroundMine.objects.Pairs.Where(kvp => kvp.Value.ItemId == Objects.quartzID).ToList() is List<KeyValuePair<Vector2, Objects>> quartzPair)
                {
                    // Loop through all quartz in the map
                    foreach (KeyValuePair<Vector2, Objects> quartz in quartzPair)
                    {
                        // For each quartz, find all points around it
                        Vector2 position = quartz.Key;
                        List<Vector2> points = new List<Vector2>();

                        for (int y = (int)position.Y - 1; y <= (int)position.Y + 1; y++)
                        {
                            for (int x = (int)position.X - 1; x <= (int)position.X + 1; x++)
                            {
                                Vector2 newPosition = new Vector2(x, y);
                                if (x != position.X && y != position.Y)
                                    points.Add(newPosition);
                            }
                        }

                        // Spawn clumps around the quartz
                        foreach (Vector2 point in points)
                        {
                            if (this.api.TrySpawnClump("Lumisteria.GlimmeringClumpsNodes_Clump_Copper", point, UndergroundMine, out string error, true))
                                this.Monitor.Log($"Successfully spawned at {point}.", LogLevel.Warn);
                            else
                                this.Monitor.Log($"Failed to spawn at {point}. Error: {error}", LogLevel.Error);
                        }
                    }
                }
            }
        }
    }
}