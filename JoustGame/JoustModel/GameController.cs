﻿//-----------------------------------------------------------
//  File:   GameController.cs
//  Desc:   Holds the GameController class
//----------------------------------------------------------- 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;

namespace JoustModel
{
    //-----------------------------------------------------------
    //  Desc:   Controls the game entirely
    //----------------------------------------------------------- 
    public class GameController
    {
        // Reference of the world that must be changed
        public World WorldRef { get; set; }

        // Constructor
        public GameController()
        {
            WorldRef = World.Instance;
            WorldRef.SpawnPoints = new List<Point[]>();
        }

        // Parses all entities in World and activates their Update() method.
        public void Update(object sender, EventArgs e)
        {
            try
            {
                //Trace.WriteLine("WorldObject Count: " + WorldRef.objects.Count);
                // Update everything 50 times per second (subject to change)
                foreach (WorldObject worldObject in WorldRef.objects)
                {
                    Entity entity = worldObject as Entity;                  
                    if (entity != null)
                    {
                        entity.Update();
                        World.Instance.TrackTime();
                    }

                }
            }
            catch (InvalidOperationException)
            {
                return;
            } 
        }

        // Calculates the number of enemies for each stage
        public void CalculateNumEnemies(ref int numBuzzards, ref int numPterodactyls)
        {
            int stage = WorldRef.player.stage;
            numBuzzards = stage + 3;
            if (stage >= 5)
            {
                numPterodactyls = (stage - 4) + (stage / 2);
            }
        }

        // Spawns the entities based on num of buzzards and Pterodactyls
        public void SpawnEnemies(int numBuzzards, int numPterodactyls)
        {
            for (int i = 0; i < numBuzzards; i++)
            {
                Buzzard b = new Buzzard();
                b.coords = new Point(500, 500);
            }
        }

        // Loads and starts a game based on the contents of the specified GameSave file "filename".
        public void Load(string filename)
        {
            string loadedLine = System.IO.File.ReadAllText(string.Format(@"../../Saves/GameSaves/{0}.txt", filename));
            string[] savedObjects = loadedLine.Split(':');
            foreach (string savedObj in savedObjects)
            {
                if (savedObj != "\r\n" && savedObj.Length > 0)
                {
                    string type = savedObj.Substring(0, savedObj.IndexOf(",")); // FIX
                    WorldObject obj = CreateWorldObj(type);
                    obj.Deserialize(savedObj);
                }
            }
        }

        // Load a custom stage
        public void StageLoad(string filename) {
            string loadedLine = System.IO.File.ReadAllText(string.Format(@"../../Saves/Custom Stages/{0}", filename));
            string[] savedObjects = loadedLine.Split(':');
            foreach (string savedObj in savedObjects) {
                if (savedObj != "\r\n" && savedObj.Length > 0) {
                    string type = savedObj.Substring(0, savedObj.IndexOf(","));
                    WorldObject obj = CreateWorldObj(type);
                    obj.Deserialize(savedObj);
                }
            }
        }
        // Saves the current game state to a file whose name is based off the current time
        public string Save()
        {
            string filename = DateTime.Now.ToString("yyyy-MM-dd-H-mm-ss");
            string line2save = "";
            foreach (WorldObject obj in WorldRef.objects)
            {
                line2save += obj.Serialize() + ':';
            }

            string path = string.Format(@"../../Saves/GameSaves/{0}.txt", filename);
            System.IO.File.WriteAllText(path, line2save); 
            return line2save;
        }

        // gets spawn points based on how many are in the current game
        public void GetSpawnPoints() {
            WorldRef.SpawnPoints = new List<Point[]>();
            foreach (WorldObject obj in WorldRef.objects) {
                Trace.WriteLine("obj = " + obj.ToString());
                if (obj is Respawn) {
                    Trace.WriteLine("Respawn Detected at (" + obj.coords.x + ", " + obj.coords.y + ")");
                    Respawn respwn = obj as Respawn;
                    WorldRef.SpawnPoints.Add(new Point[] { new Point(respwn.coords.x, respwn.coords.y), new Point(respwn.coords.x + 100, respwn.coords.y + 15) });
                }
            }
        }

        // Creates and returns a world object based on the type passed in.
        public WorldObject CreateWorldObj(string type)
        {
            switch (type)
            {
                case "Ostrich":
                    return new Ostrich();
                case "Base":
                    return new Base();
                case "Buzzard":
                    return new Buzzard();
                case "Egg":
                    return new Egg();
                case "Platform":
                    return new Platform();
                case "Long Platform 1":
                    Platform pltfm1 = new Platform();
                    pltfm1.SetType("long", 1);
                    return pltfm1;
                case "Long Platform 2":
                    Platform pltfm2 = new Platform();
                    pltfm2.SetType("long", 2);
                    return pltfm2;
                case "Long Platform 3":
                    Platform pltfm3 = new Platform();
                    pltfm3.SetType("long", 3);
                    return pltfm3;
                case "Short Platform 1":
                    Platform pltfms1 = new Platform();
                    pltfms1.SetType("short", 1);
                    return pltfms1;
                case "Short Platform 2":
                    Platform pltfms2 = new Platform();
                    pltfms2.SetType("short", 2);
                    return pltfms2;
                case "Short Platform 3":
                    Platform pltfms3 = new Platform();
                    pltfms3.SetType("short", 3);
                    return pltfms3;
                case "Short Platform 4":
                    Platform pltfms4 = new Platform();
                    pltfms4.SetType("short", 4);
                    return pltfms4;
                case "Pterodactyl":
                    return new Pterodactyl();
                case "Respawn":
                    return new Respawn();
                default:
                    return null;
            }
        }
    }
}
